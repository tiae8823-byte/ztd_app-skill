using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// TodoViewModel 功能测试
/// </summary>
public class TodoViewModelTests : IDisposable
{
    private readonly SharedMemoryDatabase4 _db;
    private readonly TaskManager _taskManager;
    private readonly TodoViewModel _viewModel;

    public TodoViewModelTests()
    {
        _db = new SharedMemoryDatabase4("TodoViewModelTests");
        _db.Initialize();

        var taskRepository = new TaskRepository(_db);
        _taskManager = new TaskManager(taskRepository);
        _viewModel = new TodoViewModel(_taskManager);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    // === 基础加载 ===

    [Fact]
    public void Constructor_ShouldLoadExistingTodos()
    {
        _taskManager.Create("短任务", TodoTaskStatus.Todo, "<30分钟", "工作");
        _taskManager.Create("长任务", TodoTaskStatus.Todo, ">30分钟", "学习");

        var vm = new TodoViewModel(_taskManager);

        Assert.Equal(1, vm.ShortTimeTasks.Count);
        Assert.Equal(1, vm.LongTimeTasks.Count);
    }

    [Fact]
    public void LoadTodos_ShouldNotLoadNonTodoTasks()
    {
        _taskManager.Create("待办", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("今日", TodoTaskStatus.Today, ">30分钟");
        _taskManager.Create("完成", TodoTaskStatus.Done, "<30分钟");

        var vm = new TodoViewModel(_taskManager);

        Assert.Equal(1, vm.ShortTimeTasks.Count);
        Assert.Empty(vm.LongTimeTasks);
    }

    [Fact]
    public void LoadTodos_ShouldGroupByTimeTag()
    {
        _taskManager.Create("短1", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("短2", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("长1", TodoTaskStatus.Todo, ">30分钟");

        var vm = new TodoViewModel(_taskManager);

        Assert.Equal(2, vm.ShortTimeTasks.Count);
        Assert.Equal(1, vm.LongTimeTasks.Count);
        Assert.Equal(2, vm.ShortTimeCount);
        Assert.Equal(1, vm.LongTimeCount);
    }

    // === 筛选 ===

    [Fact]
    public void ToggleTimeFilter_ShouldFilterTasks()
    {
        _taskManager.Create("短任务", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("长任务", TodoTaskStatus.Todo, ">30分钟");

        var vm = new TodoViewModel(_taskManager);

        vm.ToggleTimeFilterCommand.Execute("<30分钟");
        Assert.Equal(1, vm.ShortTimeTasks.Count);
        Assert.Empty(vm.LongTimeTasks);

        // 再次点击取消筛选
        vm.ToggleTimeFilterCommand.Execute("<30分钟");
        Assert.Equal(1, vm.ShortTimeTasks.Count);
        Assert.Equal(1, vm.LongTimeTasks.Count);
    }

    [Fact]
    public void ToggleCategoryFilter_ShouldFilterTasks()
    {
        _taskManager.Create("工作短", TodoTaskStatus.Todo, "<30分钟", "工作");
        _taskManager.Create("学习短", TodoTaskStatus.Todo, "<30分钟", "学习");
        _taskManager.Create("工作长", TodoTaskStatus.Todo, ">30分钟", "工作");

        var vm = new TodoViewModel(_taskManager);

        vm.ToggleCategoryFilterCommand.Execute("工作");
        Assert.Equal(1, vm.ShortTimeTasks.Count);
        Assert.Equal(1, vm.LongTimeTasks.Count);
        Assert.Equal("工作短", vm.ShortTimeTasks[0].Content);

        // 取消筛选
        vm.ToggleCategoryFilterCommand.Execute("工作");
        Assert.Equal(2, vm.ShortTimeTasks.Count);
    }

    [Fact]
    public void CombinedFilters_ShouldWork()
    {
        _taskManager.Create("工作短", TodoTaskStatus.Todo, "<30分钟", "工作");
        _taskManager.Create("学习短", TodoTaskStatus.Todo, "<30分钟", "学习");
        _taskManager.Create("工作长", TodoTaskStatus.Todo, ">30分钟", "工作");

        var vm = new TodoViewModel(_taskManager);

        // 筛选时间+分类
        vm.ToggleTimeFilterCommand.Execute("<30分钟");
        vm.ToggleCategoryFilterCommand.Execute("工作");

        Assert.Equal(1, vm.ShortTimeTasks.Count);
        Assert.Empty(vm.LongTimeTasks);
        Assert.Equal("工作短", vm.ShortTimeTasks[0].Content);
    }

    // === 删除 ===

    [Fact]
    public void DeleteTask_ShouldRemoveFromList()
    {
        _taskManager.Create("待删除", TodoTaskStatus.Todo, "<30分钟");
        var vm = new TodoViewModel(_taskManager);
        var taskId = vm.ShortTimeTasks[0].Id;

        vm.DeleteTaskCommand.Execute(taskId);

        Assert.Empty(vm.ShortTimeTasks);
    }

    [Fact]
    public void DeleteTask_InvalidId_ShouldNotCrash()
    {
        _viewModel.DeleteTaskCommand.Execute("invalid-id");
        Assert.Empty(_viewModel.ShortTimeTasks);
    }

    // === 加入今日待办 ===

    [Fact]
    public void MoveToToday_ShouldRemoveFromTodoList()
    {
        _taskManager.Create("长任务", TodoTaskStatus.Todo, ">30分钟");
        var vm = new TodoViewModel(_taskManager);
        var taskId = vm.LongTimeTasks[0].Id;

        vm.MoveToTodayCommand.Execute(taskId);

        Assert.Empty(vm.LongTimeTasks);
        var todayTasks = _taskManager.GetByStatus(TodoTaskStatus.Today);
        Assert.Single(todayTasks);
        Assert.Equal("长任务", todayTasks[0].Content);
    }

    // === 批量完成 ===

    [Fact]
    public void CheckedCount_ShouldUpdateWhenTaskChecked()
    {
        _taskManager.Create("任务1", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("任务2", TodoTaskStatus.Todo, "<30分钟");
        var vm = new TodoViewModel(_taskManager);

        Assert.Equal(0, vm.CheckedCount);
        Assert.False(vm.HasCheckedTasks);

        vm.ShortTimeTasks[0].IsChecked = true;

        Assert.Equal(1, vm.CheckedCount);
        Assert.True(vm.HasCheckedTasks);
    }

    [Fact]
    public void BatchComplete_ShouldCompleteCheckedTasks()
    {
        _taskManager.Create("任务1", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("任务2", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("任务3", TodoTaskStatus.Todo, "<30分钟");
        var vm = new TodoViewModel(_taskManager);

        vm.ShortTimeTasks[0].IsChecked = true;
        vm.ShortTimeTasks[1].IsChecked = true;

        vm.BatchCompleteCommand.Execute(null);

        Assert.Equal(1, vm.ShortTimeTasks.Count);
        var doneTasks = _taskManager.GetByStatus(TodoTaskStatus.Done);
        Assert.Equal(2, doneTasks.Count);
    }

    [Fact]
    public void BatchComplete_NoChecked_ShouldDoNothing()
    {
        _taskManager.Create("任务1", TodoTaskStatus.Todo, "<30分钟");
        var vm = new TodoViewModel(_taskManager);

        vm.BatchCompleteCommand.Execute(null);

        Assert.Equal(1, vm.ShortTimeTasks.Count);
    }

    // === 显示属性 ===

    [Fact]
    public void HasAnyTasks_ShouldReflectState()
    {
        Assert.False(_viewModel.HasAnyTasks);

        _taskManager.Create("任务", TodoTaskStatus.Todo, "<30分钟");
        _viewModel.LoadTodos();

        Assert.True(_viewModel.HasAnyTasks);
    }

    [Fact]
    public void BatchCompleteText_ShouldShowCount()
    {
        _taskManager.Create("任务1", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("任务2", TodoTaskStatus.Todo, "<30分钟");
        var vm = new TodoViewModel(_taskManager);

        vm.ShortTimeTasks[0].IsChecked = true;
        Assert.Equal("批量完成 (1个)", vm.BatchCompleteText);

        vm.ShortTimeTasks[1].IsChecked = true;
        Assert.Equal("批量完成 (2个)", vm.BatchCompleteText);
    }

    // === 命令存在性 ===

    [Fact]
    public void Commands_ShouldExist()
    {
        Assert.NotNull(_viewModel.ToggleTimeFilterCommand);
        Assert.NotNull(_viewModel.ToggleCategoryFilterCommand);
        Assert.NotNull(_viewModel.DeleteTaskCommand);
        Assert.NotNull(_viewModel.MoveToTodayCommand);
        Assert.NotNull(_viewModel.BatchCompleteCommand);
    }

    // === Task.IsChecked 属性 ===

    [Fact]
    public void TodoTask_IsChecked_ShouldNotify()
    {
        var task = new TodoTask();
        var notified = false;
        task.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TodoTask.IsChecked))
                notified = true;
        };

        task.IsChecked = true;

        Assert.True(notified);
        Assert.True(task.IsChecked);
    }

    [Fact]
    public void TodoTask_CategoryTagDisplay_ShouldReturnEmptyForNull()
    {
        var task = new TodoTask { CategoryTag = null };
        Assert.Equal("", task.CategoryTagDisplay);

        task.CategoryTag = "工作";
        Assert.Equal("工作", task.CategoryTagDisplay);
    }
}

public class SharedMemoryDatabase4 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase4(string dbName)
    {
        _dbName = dbName;
    }

    public override SqliteConnection CreateConnection()
    {
        return new SqliteConnection($"Data Source={_dbName};Mode=Memory;Cache=Shared");
    }

    public void Initialize()
    {
        _maintainConnection = CreateConnection();
        _maintainConnection.Open();

        var command = _maintainConnection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Ideas (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                CreatedAt INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Tasks (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                Status INTEGER NOT NULL,
                TimeTag TEXT,
                CategoryTag TEXT,
                CreatedAt INTEGER NOT NULL,
                CompletedAt INTEGER
            );

            CREATE TABLE IF NOT EXISTS Notes (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                Category TEXT,
                CreatedAt INTEGER NOT NULL,
                UpdatedAt INTEGER NOT NULL
            )";
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _maintainConnection?.Dispose();
    }
}
