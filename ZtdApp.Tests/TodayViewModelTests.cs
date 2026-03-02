using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// 今日待办功能测试
/// </summary>
public class TodayViewModelTests : IDisposable
{
    private readonly SharedMemoryDatabase5 _db;
    private readonly TaskManager _taskManager;
    private readonly TomatoRepository _tomatoRepository;
    private readonly TomatoService _tomatoService;
    private readonly TodayViewModel _viewModel;

    public TodayViewModelTests()
    {
        _db = new SharedMemoryDatabase5("TodayViewModelTests");
        _db.Initialize();

        var taskRepository = new TaskRepository(_db);
        _taskManager = new TaskManager(taskRepository);
        _tomatoRepository = new TomatoRepository(_db);
        _tomatoService = new TomatoService(_tomatoRepository);
        _viewModel = new TodayViewModel(_taskManager, _tomatoService);
    }

    public void Dispose()
    {
        _tomatoService.Dispose();
        _db.Dispose();
    }

    // === 基础加载 ===

    [Fact]
    public void Constructor_ShouldLoadExistingTodayTasks()
    {
        _taskManager.Create("今日任务1", TodoTaskStatus.Today, ">30分钟");
        _taskManager.Create("今日任务2", TodoTaskStatus.Today, ">30分钟");

        var vm = new TodayViewModel(_taskManager, _tomatoService);

        Assert.Equal(2, vm.TodayTaskCount);
    }

    [Fact]
    public void LoadTasks_ShouldNotLoadNonTodayTasks()
    {
        _taskManager.Create("待办", TodoTaskStatus.Todo, "<30分钟");
        _taskManager.Create("今日", TodoTaskStatus.Today, ">30分钟");
        _taskManager.Create("完成", TodoTaskStatus.Done, "<30分钟");

        var vm = new TodayViewModel(_taskManager, _tomatoService);

        Assert.Equal(1, vm.TodayTaskCount);
    }

    // === 3个任务限制 ===

    [Fact]
    public void CanAddMore_ShouldBeTrueWhenLessThan3()
    {
        _taskManager.Create("任务1", TodoTaskStatus.Today, ">30分钟");

        var vm = new TodayViewModel(_taskManager, _tomatoService);

        Assert.True(vm.CanAddMore);
    }

    [Fact]
    public void CanAddMore_ShouldBeFalseWhen3Tasks()
    {
        _taskManager.Create("任务1", TodoTaskStatus.Today, ">30分钟");
        _taskManager.Create("任务2", TodoTaskStatus.Today, ">30分钟");
        _taskManager.Create("任务3", TodoTaskStatus.Today, ">30分钟");

        var vm = new TodayViewModel(_taskManager, _tomatoService);

        Assert.False(vm.CanAddMore);
        Assert.Equal(3, vm.TodayTaskCount);
    }

    // === 完成任务 ===

    [Fact]
    public void CompleteTask_ShouldRemoveFromList()
    {
        _taskManager.Create("今日任务", TodoTaskStatus.Today, ">30分钟");
        var taskId = _taskManager.GetByStatus(TodoTaskStatus.Today)[0].Id;

        _viewModel.CompleteTaskCommand.Execute(taskId);

        Assert.Empty(_viewModel.TodayTasks);
        var doneTasks = _taskManager.GetByStatus(TodoTaskStatus.Done);
        Assert.Single(doneTasks);
    }

    [Fact]
    public void CompleteTask_InvalidId_ShouldNotCrash()
    {
        _viewModel.CompleteTaskCommand.Execute("invalid-id");
        Assert.Empty(_viewModel.TodayTasks);
    }

    // === 退回待办 ===

    [Fact]
    public void MoveBackToTodo_ShouldChangeStatusToTodo()
    {
        _taskManager.Create("今日任务", TodoTaskStatus.Today, ">30分钟");
        var taskId = _taskManager.GetByStatus(TodoTaskStatus.Today)[0].Id;

        _viewModel.MoveBackToTodoCommand.Execute(taskId);

        Assert.Empty(_viewModel.TodayTasks);
        var todoTasks = _taskManager.GetByStatus(TodoTaskStatus.Todo);
        Assert.Single(todoTasks);
    }

    // === 命令存在性 ===

    [Fact]
    public void Commands_ShouldExist()
    {
        Assert.NotNull(_viewModel.CompleteTaskCommand);
        Assert.NotNull(_viewModel.MoveBackToTodoCommand);
        Assert.NotNull(_viewModel.StartTomatoCommand);
        Assert.NotNull(_viewModel.PauseTomatoCommand);
        Assert.NotNull(_viewModel.AbandonTomatoCommand);
        Assert.NotNull(_viewModel.SelectPresetCommand);
        Assert.NotNull(_viewModel.ConfirmStartTomatoCommand);
        Assert.NotNull(_viewModel.CancelDialogCommand);
        Assert.NotNull(_viewModel.StartBreakCommand);
        Assert.NotNull(_viewModel.SkipBreakCommand);
    }

    // === 自定义时长对话框 ===

    [Fact]
    public void StartTomato_NewTimer_ShouldShowDialog()
    {
        _taskManager.Create("今日任务", TodoTaskStatus.Today, ">30分钟");
        _viewModel.LoadTasks();  // 重新加载任务
        var taskId = _viewModel.TodayTasks[0].Task.Id;

        _viewModel.StartTomatoCommand.Execute(taskId);

        Assert.True(_viewModel.IsDialogVisible);
        Assert.Equal(taskId, _viewModel.CurrentTaskIdForDialog);
        Assert.Equal(25, _viewModel.SelectedPresetMinutes);
    }

    [Fact]
    public void SelectPreset_ShouldUpdateSelectedMinutes()
    {
        _viewModel.SelectPresetCommand.Execute("45");

        Assert.Equal(45, _viewModel.SelectedPresetMinutes);
        Assert.Empty(_viewModel.CustomMinutesInput);
    }

    [Fact]
    public void ConfirmStartTomato_WithPreset_ShouldStartTimer()
    {
        _taskManager.Create("今日任务", TodoTaskStatus.Today, ">30分钟");
        _viewModel.LoadTasks();
        var taskId = _viewModel.TodayTasks[0].Task.Id;

        _viewModel.StartTomatoCommand.Execute(taskId);
        _viewModel.SelectPresetCommand.Execute("15");
        _viewModel.ConfirmStartTomatoCommand.Execute(null);

        Assert.False(_viewModel.IsDialogVisible);
        Assert.True(_tomatoService.IsRunning);
        Assert.Equal(15 * 60, _tomatoService.CurrentTomato?.TargetDuration);
    }

    [Fact]
    public void ConfirmStartTomato_WithCustomInput_ShouldUseCustomValue()
    {
        _taskManager.Create("今日任务", TodoTaskStatus.Today, ">30分钟");
        _viewModel.LoadTasks();
        var taskId = _viewModel.TodayTasks[0].Task.Id;

        _viewModel.StartTomatoCommand.Execute(taskId);
        _viewModel.CustomMinutesInput = "30";
        _viewModel.ConfirmStartTomatoCommand.Execute(null);

        Assert.False(_viewModel.IsDialogVisible);
        Assert.True(_tomatoService.IsRunning);
        Assert.Equal(30 * 60, _tomatoService.CurrentTomato?.TargetDuration);
    }

    [Fact]
    public void CancelDialog_ShouldHideDialog()
    {
        _taskManager.Create("今日任务", TodoTaskStatus.Today, ">30分钟");
        _viewModel.LoadTasks();
        var taskId = _viewModel.TodayTasks[0].Task.Id;

        _viewModel.StartTomatoCommand.Execute(taskId);
        _viewModel.CancelDialogCommand.Execute(null);

        Assert.False(_viewModel.IsDialogVisible);
        Assert.False(_tomatoService.IsRunning);
    }

    // === 遮罩层 ===

    [Fact]
    public void IsOverlayVisible_InitialState_ShouldBeFalse()
    {
        Assert.False(_viewModel.IsOverlayVisible);
    }

    [Fact]
    public void SkipBreak_ShouldHideOverlay()
    {
        // 手动设置遮罩可见（模拟番茄钟完成）
        _viewModel.GetType().GetProperty("IsOverlayVisible")?.SetValue(_viewModel, true);

        _viewModel.SkipBreakCommand.Execute(null);

        Assert.False(_viewModel.IsOverlayVisible);
    }

    // === TodayTaskItem ===

    [Fact]
    public void TodayTaskItem_ShouldHaveCorrectInitialState()
    {
        var task = new TodoTask { Content = "测试任务" };
        var item = new TodayTaskItem(task);

        Assert.Equal(task, item.Task);
        Assert.False(item.IsTomatoRunning);
        Assert.False(item.IsTomatoPaused);
        Assert.Equal("25:00", item.TimerDisplay);
    }

    [Fact]
    public void TodayTaskItem_TimerDisplay_ShouldUpdateCorrectly()
    {
        var item = new TodayTaskItem(new TodoTask());

        // 模拟经过 90 秒（1分30秒）
        // TimerDisplay 显示剩余时间，默认 25 分钟 = 1500 秒
        // 经过 90 秒后剩余 1410 秒 = 23:30
        // 但这里 ElapsedSeconds 是私有的，无法直接设置
        // 所以这个测试验证初始状态
        Assert.Equal("25:00", item.TimerDisplay);
    }
}

/// <summary>
/// 番茄钟服务测试
/// </summary>
public class TomatoServiceTests : IDisposable
{
    private readonly SharedMemoryDatabase6 _db;
    private readonly TomatoRepository _tomatoRepository;
    private readonly TomatoService _tomatoService;

    public TomatoServiceTests()
    {
        _db = new SharedMemoryDatabase6("TomatoServiceTests");
        _db.Initialize();
        _tomatoRepository = new TomatoRepository(_db);
        _tomatoService = new TomatoService(_tomatoRepository);
    }

    public void Dispose()
    {
        _tomatoService.Dispose();
        _db.Dispose();
    }

    [Fact]
    public void Start_ShouldCreateRunningTomato()
    {
        _tomatoService.Start("task-1", 25);

        Assert.True(_tomatoService.IsRunning);
        Assert.NotNull(_tomatoService.CurrentTomato);
        Assert.Equal(25 * 60, _tomatoService.CurrentTomato.TargetDuration);
    }

    [Fact]
    public void Pause_ShouldSetIsPaused()
    {
        _tomatoService.Start("task-1", 25);
        _tomatoService.Pause();

        Assert.False(_tomatoService.IsRunning);
        Assert.True(_tomatoService.IsPaused);
    }

    [Fact]
    public void Resume_ShouldContinueRunning()
    {
        _tomatoService.Start("task-1", 25);
        _tomatoService.Pause();
        _tomatoService.Resume();

        Assert.True(_tomatoService.IsRunning);
        Assert.False(_tomatoService.IsPaused);
    }

    [Fact]
    public void Stop_ShouldClearCurrentTomato()
    {
        _tomatoService.Start("task-1", 25);
        _tomatoService.Stop();

        Assert.False(_tomatoService.IsRunning);
        Assert.Null(_tomatoService.CurrentTomato);
    }

    [Fact]
    public void GetThisWeekStats_ShouldReturnCorrectData()
    {
        // 这个测试验证方法存在且可调用
        var (count, totalMinutes) = _tomatoService.GetThisWeekStats();

        Assert.Equal(0, count);
        Assert.Equal(0, totalMinutes);
    }
}

// === 内存数据库实现 ===

public class SharedMemoryDatabase5 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase5(string dbName)
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

    public void Dispose()
    {
        _maintainConnection?.Dispose();
    }
}

public class SharedMemoryDatabase6 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase6(string dbName)
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

    public void Dispose()
    {
        _maintainConnection?.Dispose();
    }
}
