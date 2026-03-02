using System.Collections.ObjectModel;
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

    private const int MaxTodayTasks = 3;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    [ObservableProperty]
    private int _todayTaskCount;

    [ObservableProperty]
    private int _maxTasks = MaxTodayTasks;

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

        // 如果有其他任务在计时，先停止
        foreach (var item in TodayTasks)
        {
            if (item.IsTomatoRunning && item.Task.Id != id)
            {
                item.IsTomatoRunning = false;
                item.IsTomatoPaused = false;
            }
        }

        if (taskItem.IsTomatoPaused)
        {
            // 继续计时
            _tomatoService.Resume();
            taskItem.IsTomatoRunning = true;
            taskItem.IsTomatoPaused = false;
        }
        else
        {
            // 开始新计时
            _tomatoService.Start(id, 25);  // 默认25分钟
            taskItem.IsTomatoRunning = true;
            taskItem.IsTomatoPaused = false;
            taskItem.TimerDisplay = "25:00";
        }
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
        TodayTasks.Clear();
        if (_taskManager == null) return;

        var tasks = _taskManager.GetByStatus(TodoTaskStatus.Today);
        foreach (var task in tasks)
        {
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
        // 在 UI 线程处理
        Application.Current.Dispatcher.Invoke(() =>
        {
            var completedTask = TodayTasks.FirstOrDefault(t => t.IsTomatoRunning);
            if (completedTask != null)
            {
                completedTask.IsTomatoRunning = false;
                completedTask.TimerDisplay = "00:00";

                // 提示用户
                MessageBox.Show(
                    "番茄钟完成！休息一下吧。",
                    "完成",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        });
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
