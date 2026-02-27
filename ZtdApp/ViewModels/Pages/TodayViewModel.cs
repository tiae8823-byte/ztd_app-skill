using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class TodayViewModel : ObservableObject
{
    private readonly TaskManager _taskManager;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    public ObservableCollection<TodoTask> TodayTasks { get; } = new();

    public TodayViewModel(TaskManager taskManager)
    {
        _taskManager = taskManager;
        LoadTodayTasks();
    }

    // 无参构造函数用于设计时
    public TodayViewModel()
    {
        _taskManager = null!;
    }

    [RelayCommand]
    private void AddTask()
    {
        if (string.IsNullOrWhiteSpace(InputContent))
            return;

        _taskManager?.Create(InputContent, TodoTaskStatus.Today);
        InputContent = string.Empty;
        LoadTodayTasks();
    }

    [RelayCommand]
    private void DeleteTask(string id)
    {
        _taskManager?.Delete(id);
        LoadTodayTasks();
    }

    public void LoadTodayTasks()
    {
        TodayTasks.Clear();
        if (_taskManager == null) return;

        foreach (var task in _taskManager.GetByStatus(TodoTaskStatus.Today))
        {
            TodayTasks.Add(task);
        }
    }
}
