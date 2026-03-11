using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.ViewModels.Pages;

public partial class TodoViewModel : ObservableObject
{
    private readonly TaskManager _taskManager;

    // 筛选状态
    [ObservableProperty]
    private string? _selectedTimeFilter;

    [ObservableProperty]
    private string? _selectedCategoryFilter;

    // 分组任务列表
    public ObservableCollection<TodoTask> ShortTimeTasks { get; } = new();
    public ObservableCollection<TodoTask> LongTimeTasks { get; } = new();

    // 批量操作
    [ObservableProperty]
    private int _checkedCount;

    [ObservableProperty]
    private bool _hasCheckedTasks;

    public string BatchCompleteText => $"批量完成 ({CheckedCount}个)";

    // 分组计数
    public int ShortTimeCount => ShortTimeTasks.Count;
    public int LongTimeCount => LongTimeTasks.Count;

    // 分组可见性
    public bool HasShortTimeTasks => ShortTimeTasks.Count > 0;
    public bool HasLongTimeTasks => LongTimeTasks.Count > 0;
    public bool HasAnyTasks => ShortTimeTasks.Count > 0 || LongTimeTasks.Count > 0;

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

    partial void OnSelectedTimeFilterChanged(string? value)
    {
        LoadTodos();
    }

    partial void OnSelectedCategoryFilterChanged(string? value)
    {
        LoadTodos();
    }

    partial void OnCheckedCountChanged(int value)
    {
        OnPropertyChanged(nameof(BatchCompleteText));
    }

    [RelayCommand]
    private void ToggleTimeFilter(string timeTag)
    {
        SelectedTimeFilter = SelectedTimeFilter == timeTag ? null : timeTag;
    }

    [RelayCommand]
    private void ToggleCategoryFilter(string category)
    {
        SelectedCategoryFilter = SelectedCategoryFilter == category ? null : category;
    }

    [RelayCommand]
    private void DeleteTask(string id)
    {
        _taskManager?.Delete(id);
        LoadTodos();
    }

    [RelayCommand]
    private void MoveToToday(string id)
    {
        _taskManager?.MoveToStatus(id, TodoTaskStatus.Today);
        LoadTodos();
    }

    [RelayCommand]
    private void CompleteTask(string id)
    {
        _taskManager?.Complete(id);
        LoadTodos();
    }

    [RelayCommand]
    private void BatchComplete()
    {
        if (_taskManager == null) return;

        var checkedIds = ShortTimeTasks
            .Where(t => t.IsChecked)
            .Select(t => t.Id)
            .ToList();

        foreach (var id in checkedIds)
        {
            _taskManager.Complete(id);
        }

        LoadTodos();
    }

    [RelayCommand]
    private void ToggleTaskExpand(TodoTask task)
    {
        if (task == null)
        {
            System.Diagnostics.Debug.WriteLine("ToggleTaskExpand: task is null");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"ToggleTaskExpand: {task.Id}, IsExpanded: {task.IsExpanded}");

        try
        {
            // 互斥展开：收起其他所有卡片
            foreach (var t in ShortTimeTasks)
            {
                if (t != task && t.IsExpanded)
                    t.IsExpanded = false;
            }
            foreach (var t in LongTimeTasks)
            {
                if (t != task && t.IsExpanded)
                    t.IsExpanded = false;
            }

            // 切换当前卡片状态
            task.IsExpanded = !task.IsExpanded;

            System.Diagnostics.Debug.WriteLine($"ToggleTaskExpand: After toggle, IsExpanded: {task.IsExpanded}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ToggleTaskExpand Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
        }
    }

    public void LoadTodos()
    {
        // 取消旧任务的事件监听
        foreach (var task in ShortTimeTasks)
            task.PropertyChanged -= OnTaskPropertyChanged;
        foreach (var task in LongTimeTasks)
            task.PropertyChanged -= OnTaskPropertyChanged;

        ShortTimeTasks.Clear();
        LongTimeTasks.Clear();

        if (_taskManager == null) return;

        var allTodos = _taskManager.GetByStatus(TodoTaskStatus.Todo);

        // 应用分类筛选
        if (SelectedCategoryFilter != null)
        {
            allTodos = allTodos.Where(t => t.CategoryTag == SelectedCategoryFilter).ToList();
        }

        // 分组
        foreach (var task in allTodos)
        {
            if (task.TimeTag == "<30分钟")
            {
                if (SelectedTimeFilter == null || SelectedTimeFilter == "<30分钟")
                {
                    task.PropertyChanged += OnTaskPropertyChanged;
                    ShortTimeTasks.Add(task);
                }
            }
            else if (task.TimeTag == ">30分钟")
            {
                if (SelectedTimeFilter == null || SelectedTimeFilter == ">30分钟")
                {
                    task.PropertyChanged += OnTaskPropertyChanged;
                    LongTimeTasks.Add(task);
                }
            }
        }

        UpdateCheckedCount();
        OnPropertyChanged(nameof(ShortTimeCount));
        OnPropertyChanged(nameof(LongTimeCount));
        OnPropertyChanged(nameof(HasShortTimeTasks));
        OnPropertyChanged(nameof(HasLongTimeTasks));
        OnPropertyChanged(nameof(HasAnyTasks));
    }

    private void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TodoTask.IsChecked))
        {
            UpdateCheckedCount();
        }
    }

    private void UpdateCheckedCount()
    {
        CheckedCount = ShortTimeTasks.Count(t => t.IsChecked);
        HasCheckedTasks = CheckedCount > 0;
    }
}
