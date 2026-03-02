using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class TodayViewModel : ObservableObject
{
    private readonly TaskManager _taskManager;
    private readonly TomatoService _tomatoService;

    [DllImport("user32.dll")]
    private static extern bool LockWorkStation();

    private const int MaxTodayTasks = 3;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    [ObservableProperty]
    private int _todayTaskCount;

    [ObservableProperty]
    private int _maxTasks = MaxTodayTasks;

    // 自定义时长对话框
    [ObservableProperty]
    private bool _isDialogVisible;

    [ObservableProperty]
    private string _currentTaskIdForDialog = string.Empty;

    [ObservableProperty]
    private int _selectedPresetMinutes = 25;

    [ObservableProperty]
    private string _customMinutesInput = string.Empty;

    [ObservableProperty]
    private string _customSecondsInput = string.Empty;

    public ObservableCollection<TodayTaskItem> TodayTasks { get; } = new();

    public bool CanAddMore => TodayTaskCount < MaxTodayTasks;

    public TodayViewModel(TaskManager taskManager, TomatoService tomatoService)
    {
        _taskManager = taskManager;
        _tomatoService = tomatoService;

        // 订阅番茄钟事件
        _tomatoService.TimerTick += OnTimerTick;
        _tomatoService.TimerCompleted += OnTimerCompleted;

        LoadTasks();
    }

    // 无参构造函数用于设计时
    public TodayViewModel()
    {
        _taskManager = null!;
        _tomatoService = null!;
    }

    /// <summary>
    /// 从待办列表加入今日待办（带替换确认）
    /// </summary>
    public bool TryAddTaskFromTodo(TodoTask task)
    {
        if (TodayTaskCount >= MaxTodayTasks)
        {
            // 满了，提示用户是否替换
            var result = MessageBox.Show(
                $"今日待办已满（{MaxTodayTasks}个），是否要替换最后一个任务？",
                "替换确认",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return false;

            // 移除最后一个任务（退回待办）
            if (TodayTasks.Count > 0)
            {
                var lastTask = TodayTasks[^1];
                _taskManager.MoveToStatus(lastTask.Task.Id, TodoTaskStatus.Todo);
                TodayTasks.Remove(lastTask);
            }
        }

        // 添加新任务
        _taskManager.MoveToStatus(task.Id, TodoTaskStatus.Today);
        LoadTasks();
        return true;
    }

    [RelayCommand]
    private void CompleteTask(string id)
    {
        // 先停止该任务的番茄钟
        var taskItem = TodayTasks.FirstOrDefault(t => t.Task.Id == id);
        if (taskItem != null && taskItem.IsTomatoRunning)
        {
            _tomatoService.Complete();
        }

        _taskManager.Complete(id);
        LoadTasks();
    }

    [RelayCommand]
    private void MoveBackToTodo(string id)
    {
        // 停止番茄钟
        var taskItem = TodayTasks.FirstOrDefault(t => t.Task.Id == id);
        if (taskItem != null && taskItem.IsTomatoRunning)
        {
            _tomatoService.Abandon();
        }

        _taskManager.MoveToStatus(id, TodoTaskStatus.Todo);
        LoadTasks();
    }

    [RelayCommand]
    private void StartTomato(string id)
    {
        var taskItem = TodayTasks.FirstOrDefault(t => t.Task.Id == id);
        if (taskItem == null) return;

        if (taskItem.IsTomatoPaused)
        {
            // 继续计时
            _tomatoService.Resume();
            taskItem.IsTomatoRunning = true;
            taskItem.IsTomatoPaused = false;
        }
        else
        {
            // 显示自定义时长对话框
            CurrentTaskIdForDialog = id;
            SelectedPresetMinutes = 25;
            CustomMinutesInput = string.Empty;
            IsDialogVisible = true;
        }
    }

    [RelayCommand]
    private void SelectPreset(string minutesStr)
    {
        if (int.TryParse(minutesStr, out var minutes))
        {
            SelectedPresetMinutes = minutes;
        }
        CustomMinutesInput = string.Empty;
    }

    [RelayCommand]
    private void ConfirmStartTomato()
    {
        int totalSeconds;

        if (!string.IsNullOrWhiteSpace(CustomMinutesInput) || !string.IsNullOrWhiteSpace(CustomSecondsInput))
        {
            // 自定义输入模式：分钟 + 秒
            int.TryParse(CustomMinutesInput, out var customMinutes);
            int.TryParse(CustomSecondsInput, out var customSeconds);
            totalSeconds = customMinutes * 60 + customSeconds;
        }
        else
        {
            // 预设模式
            totalSeconds = SelectedPresetMinutes * 60;
        }

        if (totalSeconds <= 0 || totalSeconds > 120 * 60)
        {
            MessageBox.Show("请输入有效时长（最长 120 分钟）", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // 关闭对话框
        IsDialogVisible = false;

        var taskItem = TodayTasks.FirstOrDefault(t => t.Task.Id == CurrentTaskIdForDialog);
        if (taskItem == null) return;

        // 如果有其他任务在计时，先停止
        foreach (var item in TodayTasks)
        {
            if (item.IsTomatoRunning && item.Task.Id != CurrentTaskIdForDialog)
            {
                item.IsTomatoRunning = false;
                item.IsTomatoPaused = false;
            }
        }

        // 开始新计时（传秒）
        _tomatoService.Start(CurrentTaskIdForDialog, totalSeconds);
        taskItem.IsTomatoRunning = true;
        taskItem.IsTomatoPaused = false;
        var displayMinutes = totalSeconds / 60;
        var displaySeconds = totalSeconds % 60;
        taskItem.TimerDisplay = $"{displayMinutes:D2}:{displaySeconds:D2}";
    }

    [RelayCommand]
    private void CancelDialog()
    {
        IsDialogVisible = false;
    }

    [RelayCommand]
    private void PauseTomato(string id)
    {
        var taskItem = TodayTasks.FirstOrDefault(t => t.Task.Id == id);
        if (taskItem == null || !taskItem.IsTomatoRunning) return;

        _tomatoService.Pause();
        taskItem.IsTomatoRunning = false;
        taskItem.IsTomatoPaused = true;
    }

    [RelayCommand]
    private void AbandonTomato(string id)
    {
        var taskItem = TodayTasks.FirstOrDefault(t => t.Task.Id == id);
        if (taskItem == null) return;

        _tomatoService.Abandon();
        taskItem.IsTomatoRunning = false;
        taskItem.IsTomatoPaused = false;
        taskItem.TimerDisplay = "25:00";
    }

    public void LoadTasks()
    {
        if (_taskManager == null) return;

        var tasks = _taskManager.GetByStatus(TodoTaskStatus.Today);

        // 保留正在计时的任务状态，只更新列表差异
        var existingIds = TodayTasks.Select(t => t.Task.Id).ToHashSet();
        var newIds = tasks.Select(t => t.Id).ToHashSet();

        // 移除已不存在的任务
        foreach (var item in TodayTasks.Where(t => !newIds.Contains(t.Task.Id)).ToList())
            TodayTasks.Remove(item);

        // 添加新任务（保留已有任务不动，避免重置番茄钟状态）
        foreach (var task in tasks)
        {
            if (!existingIds.Contains(task.Id))
                TodayTasks.Add(new TodayTaskItem(task));
        }

        TodayTaskCount = TodayTasks.Count;
        OnPropertyChanged(nameof(CanAddMore));
    }

    private void OnTimerTick(int elapsedSeconds)
    {
        // 在 UI 线程更新
        Application.Current.Dispatcher.Invoke(() =>
        {
            var runningTask = TodayTasks.FirstOrDefault(t => t.IsTomatoRunning || t.IsTomatoPaused);
            if (runningTask != null && _tomatoService.CurrentTomato != null)
            {
                runningTask.TimerDisplay = _tomatoService.CurrentTomato.TimerDisplay;
            }
        });
    }

    private void OnTimerCompleted()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var completedTask = TodayTasks.FirstOrDefault(t => t.IsTomatoRunning);
            if (completedTask != null)
            {
                completedTask.IsTomatoRunning = false;
                completedTask.TimerDisplay = "00:00";
            }

            // 系统级锁屏
            LockWorkStation();
        });
    }

    public void Dispose()
    {
    }
}

/// <summary>
/// 今日待办任务项（包装类，包含番茄钟状态）
/// </summary>
public partial class TodayTaskItem : ObservableObject
{
    public TodoTask Task { get; }

    [ObservableProperty]
    private bool _isTomatoRunning;

    [ObservableProperty]
    private bool _isTomatoPaused;

    [ObservableProperty]
    private string _timerDisplay = "25:00";

    public TodayTaskItem(TodoTask task)
    {
        Task = task;
    }
}
