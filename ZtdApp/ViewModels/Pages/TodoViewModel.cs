using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class TodoViewModel : ObservableObject
{
    private readonly TaskManager _taskManager;

    [ObservableProperty]
    private string _inputContent = string.Empty;

    public ObservableCollection<TodoTask> Todos { get; } = new();

    public TodoViewModel(TaskManager taskManager)
    {
        _taskManager = taskManager;
        LoadTodos();
    }

    // 无参构造函数用于设计时
    public TodoViewModel()
    {
        _taskManager = null!;
    }

    [RelayCommand]
    private void AddTodo()
    {
        if (string.IsNullOrWhiteSpace(InputContent))
            return;

        _taskManager?.Create(InputContent, TodoTaskStatus.Todo);
        InputContent = string.Empty;
        LoadTodos();
    }

    [RelayCommand]
    private void DeleteTodo(string id)
    {
        _taskManager?.Delete(id);
        LoadTodos();
    }

    public void LoadTodos()
    {
        Todos.Clear();
        if (_taskManager == null) return;

        foreach (var task in _taskManager.GetByStatus(TodoTaskStatus.Todo))
        {
            Todos.Add(task);
        }
    }
}
