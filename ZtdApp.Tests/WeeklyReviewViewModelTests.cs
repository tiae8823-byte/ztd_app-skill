using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// 每周回顾功能测试
/// </summary>
public class WeeklyReviewViewModelTests : IDisposable
{
    private readonly SharedMemoryDatabase8 _db;
    private readonly TaskManager _taskManager;
    private readonly TomatoService _tomatoService;
    private readonly NoteManager _noteManager;
    private readonly WeeklyReviewViewModel _viewModel;

    public WeeklyReviewViewModelTests()
    {
        _db = new SharedMemoryDatabase8("WeeklyReviewTests");
        _db.Initialize();

        var taskRepository = new TaskRepository(_db);
        var tomatoRepository = new TomatoRepository(_db);
        var noteRepository = new NoteRepository(_db);

        _taskManager = new TaskManager(taskRepository);
        _tomatoService = new TomatoService(tomatoRepository);
        _noteManager = new NoteManager(noteRepository);

        _viewModel = new WeeklyReviewViewModel(_taskManager, _tomatoService, _noteManager);
    }

    public void Dispose()
    {
        _tomatoService.Dispose();
        _db.Dispose();
    }

    // === 基础加载 ===

    [Fact]
    public void Constructor_ShouldLoadInitialData()
    {
        var vm = new WeeklyReviewViewModel(_taskManager, _tomatoService, _noteManager);

        Assert.Equal(0, vm.CompletedTasksCount);
        Assert.Equal("0h 0m / 0次", vm.TotalPomodoroTime);
        Assert.Equal(0, vm.NoteCount);
    }

    [Fact]
    public void LoadReviewData_ShouldCountThisWeekCompletedTasks()
    {
        // 创建本周完成的任务
        var task1 = _taskManager.Create("本周任务1", TodoTaskStatus.Todo);
        _taskManager.Complete(task1.Id);

        var task2 = _taskManager.Create("本周任务2", TodoTaskStatus.Todo);
        _taskManager.Complete(task2.Id);

        _viewModel.RefreshDataCommand.Execute(null);

        Assert.Equal(2, _viewModel.CompletedTasksCount);
        Assert.Equal(2, _viewModel.CompletedTasks.Count);
    }

    [Fact]
    public void LoadReviewData_ShouldCalculatePomodoroStats()
    {
        // 创建番茄钟记录（25分钟 + 15分钟 = 40分钟，2次）
        var tomato1 = new Tomato
        {
            Duration = 25 * 60,
            TargetDuration = 25 * 60,
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            CompletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        _db.AddTomato(tomato1);

        var tomato2 = new Tomato
        {
            Duration = 15 * 60,
            TargetDuration = 15 * 60,
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            CompletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        _db.AddTomato(tomato2);

        _viewModel.RefreshDataCommand.Execute(null);

        Assert.Equal("0h 40m / 2次", _viewModel.TotalPomodoroTime);
    }

    [Fact]
    public void LoadReviewData_ShouldCountThisWeekNotes()
    {
        _noteManager.Create("本周笔记1");
        _noteManager.Create("本周笔记2");
        _noteManager.Create("本周笔记3");

        _viewModel.RefreshDataCommand.Execute(null);

        Assert.Equal(3, _viewModel.NoteCount);
    }

    // === 任务列表展开/收起 ===

    [Fact]
    public void ToggleTaskList_ShouldChangeExpandedState()
    {
        Assert.False(_viewModel.IsTaskListExpanded);

        _viewModel.ToggleTaskListCommand.Execute(null);
        Assert.True(_viewModel.IsTaskListExpanded);

        _viewModel.ToggleTaskListCommand.Execute(null);
        Assert.False(_viewModel.IsTaskListExpanded);
    }

    [Fact]
    public void CompletedTasks_ShouldContainTaskDetails()
    {
        var task = _taskManager.Create("测试任务", TodoTaskStatus.Todo);
        _taskManager.Complete(task.Id);

        _viewModel.RefreshDataCommand.Execute(null);

        Assert.Single(_viewModel.CompletedTasks);
        Assert.Equal("测试任务", _viewModel.CompletedTasks[0].Content);
        Assert.NotNull(_viewModel.CompletedTasks[0].CompletedAt);
    }

    // === 刷新功能 ===

    [Fact]
    public void RefreshData_ShouldReloadAllStats()
    {
        // 初始状态
        Assert.Equal(0, _viewModel.CompletedTasksCount);

        // 添加数据
        var task = _taskManager.Create("新任务", TodoTaskStatus.Todo);
        _taskManager.Complete(task.Id);
        _noteManager.Create("新笔记");

        // 刷新
        _viewModel.RefreshDataCommand.Execute(null);

        Assert.Equal(1, _viewModel.CompletedTasksCount);
        Assert.Equal(1, _viewModel.NoteCount);
    }

    // === 删除完成任务 ===

    [Fact]
    public void DeleteCompletedTask_ShouldRemoveFromListAndUpdateCount()
    {
        var task = _taskManager.Create("要删除的任务", TodoTaskStatus.Todo);
        _taskManager.Complete(task.Id);
        _viewModel.RefreshDataCommand.Execute(null);
        Assert.Equal(1, _viewModel.CompletedTasksCount);

        _viewModel.DeleteCompletedTaskCommand.Execute(task.Id);

        Assert.Equal(0, _viewModel.CompletedTasksCount);
        Assert.Empty(_viewModel.CompletedTasks);
    }

    // === 命令存在性 ===

    [Fact]
    public void Commands_ShouldExist()
    {
        Assert.NotNull(_viewModel.RefreshDataCommand);
        Assert.NotNull(_viewModel.ToggleTaskListCommand);
        Assert.NotNull(_viewModel.DeleteCompletedTaskCommand);
    }
}

// === 内存数据库实现 ===

public class SharedMemoryDatabase8 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase8(string dbName)
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
            );

            CREATE TABLE IF NOT EXISTS Tomatoes (
                Id TEXT PRIMARY KEY,
                TaskId TEXT,
                Duration INTEGER NOT NULL,
                TargetDuration INTEGER NOT NULL,
                StartTime INTEGER NOT NULL,
                CompletedAt INTEGER
            )";
        command.ExecuteNonQuery();
    }

    public void AddTomato(Tomato tomato)
    {
        using var connection = CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Tomatoes (Id, TaskId, Duration, TargetDuration, StartTime, CompletedAt)
            VALUES (@id, @taskId, @duration, @targetDuration, @startTime, @completedAt)";
        command.Parameters.AddWithValue("@id", tomato.Id);
        command.Parameters.AddWithValue("@taskId", tomato.TaskId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@duration", tomato.Duration);
        command.Parameters.AddWithValue("@targetDuration", tomato.TargetDuration);
        command.Parameters.AddWithValue("@startTime", tomato.StartTime);
        command.Parameters.AddWithValue("@completedAt", tomato.CompletedAt ?? (object)DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _maintainConnection?.Dispose();
    }
}
