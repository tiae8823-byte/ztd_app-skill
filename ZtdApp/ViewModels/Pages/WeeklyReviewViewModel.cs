using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class WeeklyReviewViewModel : ObservableObject
{
    private readonly TaskManager _taskManager;
    private readonly TomatoService _tomatoService;
    private readonly NoteManager _noteManager;

    [ObservableProperty]
    private int _completedTasksCount = 0;

    [ObservableProperty]
    private string _totalPomodoroTime = "0h 0m";

    [ObservableProperty]
    private int _noteCount = 0;

    [ObservableProperty]
    private bool _isTaskListExpanded = false;

    public ObservableCollection<TodoTask> CompletedTasks { get; } = new();

    public WeeklyReviewViewModel(TaskManager taskManager, TomatoService tomatoService, NoteManager noteManager)
    {
        _taskManager = taskManager;
        _tomatoService = tomatoService;
        _noteManager = noteManager;
        LoadReviewData();
    }

    // 无参构造函数用于设计时
    public WeeklyReviewViewModel()
    {
        _taskManager = null!;
        _tomatoService = null!;
        _noteManager = null!;
    }

    [RelayCommand]
    public void RefreshData()
    {
        LoadReviewData();
    }

    [RelayCommand]
    public void ToggleTaskList()
    {
        IsTaskListExpanded = !IsTaskListExpanded;
    }

    [RelayCommand]
    public void DeleteCompletedTask(string id)
    {
        _taskManager.Delete(id);
        LoadReviewData();
    }

    private void LoadReviewData()
    {
        if (_taskManager == null || _tomatoService == null || _noteManager == null) return;

        // 获取本周完成任务
        var tasks = _taskManager.GetThisWeekCompleted();
        CompletedTasksCount = tasks.Count;

        // 加载任务列表
        CompletedTasks.Clear();
        foreach (var task in tasks)
        {
            CompletedTasks.Add(task);
        }

        // 获取番茄钟统计
        var (count, totalMinutes) = _tomatoService.GetThisWeekStats();
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;
        TotalPomodoroTime = $"{hours}h {minutes}m / {count}次";

        // 获取本周笔记数
        NoteCount = _noteManager.GetThisWeekCount();
    }
}
