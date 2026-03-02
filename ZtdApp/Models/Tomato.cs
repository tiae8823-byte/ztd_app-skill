using System.ComponentModel;

namespace ZtdApp.Models;

/// <summary>
/// 番茄钟记录
/// </summary>
public class Tomato : INotifyPropertyChanged
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? TaskId { get; set; }  // 关联任务ID
    public int Duration { get; set; }  // 实际时长（秒），>10秒才记录
    public int TargetDuration { get; set; } = 25 * 60;  // 目标时长（秒），默认25分钟
    public long StartTime { get; set; }
    public long? CompletedAt { get; set; }

    // UI 绑定属性
    private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }
    }

    private bool _isPaused;
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (_isPaused != value)
            {
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }
    }

    private int _elapsedSeconds;
    public int ElapsedSeconds
    {
        get => _elapsedSeconds;
        set
        {
            if (_elapsedSeconds != value)
            {
                _elapsedSeconds = value;
                OnPropertyChanged(nameof(ElapsedSeconds));
                OnPropertyChanged(nameof(TimerDisplay));
            }
        }
    }

    // 显示格式 MM:SS
    public string TimerDisplay
    {
        get
        {
            var remaining = TargetDuration - ElapsedSeconds;
            if (remaining < 0) remaining = 0;
            var minutes = remaining / 60;
            var seconds = remaining % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
