using System;
using ZtdApp.Data;
using ZtdApp.Models;

namespace ZtdApp.Services;

/// <summary>
/// 番茄钟服务 - 管理计时器和持久化
/// </summary>
public class TomatoService : IDisposable
{
    private readonly TomatoRepository _repository;
    private System.Timers.Timer? _timer;
    private Tomato? _currentTomato;

    public event Action<int>? TimerTick;  // 每秒触发，传递已过秒数
    public event Action? TimerCompleted;  // 计时完成

    public Tomato? CurrentTomato => _currentTomato;
    public bool IsRunning => _currentTomato?.IsRunning ?? false;
    public bool IsPaused => _currentTomato?.IsPaused ?? false;

    public TomatoService(TomatoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 开始番茄钟计时
    /// </summary>
    /// <param name="taskId">关联的任务ID</param>
    /// <param name="durationSeconds">目标时长（秒）</param>
    public void Start(string? taskId, int durationSeconds = 25 * 60)
    {
        // 停止之前的计时
        Stop();

        _currentTomato = new Tomato
        {
            TaskId = taskId,
            TargetDuration = durationSeconds,
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            IsRunning = true,
            IsPaused = false,
            ElapsedSeconds = 0
        };

        _timer = new System.Timers.Timer(1000);  // 1秒间隔
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    public void Pause()
    {
        if (_currentTomato == null || !_currentTomato.IsRunning) return;

        _currentTomato.IsRunning = false;
        _currentTomato.IsPaused = true;
        _timer?.Stop();
    }

    /// <summary>
    /// 继续计时
    /// </summary>
    public void Resume()
    {
        if (_currentTomato == null || !_currentTomato.IsPaused) return;

        _currentTomato.IsRunning = true;
        _currentTomato.IsPaused = false;
        _timer?.Start();
    }

    /// <summary>
    /// 放弃计时（<10秒不记录）
    /// </summary>
    public void Abandon()
    {
        if (_currentTomato == null) return;

        var elapsed = _currentTomato.ElapsedSeconds;

        // 停止计时器
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        // <10秒不记录
        if (elapsed >= 10)
        {
            _currentTomato.Duration = elapsed;
            _currentTomato.CompletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _repository.Add(_currentTomato);
        }

        _currentTomato = null;
    }

    /// <summary>
    /// 完成计时（手动标记完成）
    /// </summary>
    public void Complete()
    {
        if (_currentTomato == null) return;

        var elapsed = _currentTomato.ElapsedSeconds;

        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        // >=10秒才记录
        if (elapsed >= 10)
        {
            _currentTomato.Duration = elapsed;
            _currentTomato.CompletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _repository.Add(_currentTomato);
        }

        _currentTomato = null;
    }

    /// <summary>
    /// 停止并清理（不保存）
    /// </summary>
    public void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _currentTomato = null;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_currentTomato == null) return;

        _currentTomato.ElapsedSeconds++;

        // 检查是否完成
        if (_currentTomato.ElapsedSeconds >= _currentTomato.TargetDuration)
        {
            _timer?.Stop();
            _currentTomato.Duration = _currentTomato.ElapsedSeconds;
            _currentTomato.CompletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _currentTomato.IsRunning = false;

            // 保存到数据库
            _repository.Add(_currentTomato);

            TimerCompleted?.Invoke();
        }
        else
        {
            TimerTick?.Invoke(_currentTomato.ElapsedSeconds);
        }
    }

    /// <summary>
    /// 获取本周番茄钟统计
    /// </summary>
    public (int count, int totalMinutes) GetThisWeekStats()
    {
        var tomatoes = _repository.GetThisWeek();
        var totalSeconds = 0;
        foreach (var t in tomatoes)
        {
            totalSeconds += t.Duration;
        }
        return (tomatoes.Count, totalSeconds / 60);
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}
