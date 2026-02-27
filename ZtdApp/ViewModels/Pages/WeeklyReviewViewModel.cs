using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class WeeklyReviewViewModel : ObservableObject
{
    private readonly TaskManager _taskManager;

    [ObservableProperty]
    private int _completedTasksCount = 0;

    [ObservableProperty]
    private string _totalPomodoroTime = "0h 0m";

    [ObservableProperty]
    private int _ideaCount = 0;

    [ObservableProperty]
    private int _noteCount = 0;

    public WeeklyReviewViewModel(TaskManager taskManager)
    {
        _taskManager = taskManager;
        LoadReviewData();
    }

    // 无参构造函数用于设计时
    public WeeklyReviewViewModel()
    {
        _taskManager = null!;
    }

    [RelayCommand]
    private void RefreshData()
    {
        LoadReviewData();
    }

    private void LoadReviewData()
    {
        if (_taskManager == null) return;

        // 获取已完成任务数
        CompletedTasksCount = _taskManager.GetByStatus(TodoTaskStatus.Done).Count;

        // 番茄钟时间（简化版本）
        TotalPomodoroTime = "0h 0m";
    }
}
