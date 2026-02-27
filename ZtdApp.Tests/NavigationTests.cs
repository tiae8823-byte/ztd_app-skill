using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;
using ZtdApp.ViewModels;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// MainWindowViewModel 导航功能测试
/// </summary>
public class NavigationTests : IDisposable
{
    private readonly SharedMemoryDatabase _db;
    private readonly MainWindowViewModel _viewModel;

    public NavigationTests()
    {
        // 使用共享内存数据库
        _db = new SharedMemoryDatabase("NavigationTests");
        _db.Initialize();

        var ideaRepo = new IdeaRepository(_db);
        var taskRepo = new TaskRepository(_db);
        var noteRepo = new NoteRepository(_db);

        var ideaManager = new IdeaManager(ideaRepo);
        var taskManager = new TaskManager(taskRepo);
        var noteManager = new NoteManager(noteRepo);

        var ideaViewModel = new IdeaViewModel(ideaManager, taskManager, noteManager);
        var todoViewModel = new TodoViewModel(taskManager);
        var todayViewModel = new TodayViewModel(taskManager);
        var notesViewModel = new NotesViewModel(noteManager);
        var weeklyReviewViewModel = new WeeklyReviewViewModel(taskManager);

        _viewModel = new MainWindowViewModel(
            ideaViewModel,
            todoViewModel,
            todayViewModel,
            notesViewModel,
            weeklyReviewViewModel);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    // ============ 核心导航测试 ============

    [Fact]
    public void Constructor_ShouldSetDefaultPageToIdeas()
    {
        Assert.NotNull(_viewModel.CurrentPage);
        Assert.Equal("想法收集", _viewModel.CurrentTitle);
    }

    [Fact]
    public void NavigateToIdeas_ShouldUpdateCurrentPageAndTitle()
    {
        _viewModel.NavigateToTodosCommand.Execute(null);
        _viewModel.NavigateToIdeasCommand.Execute(null);
        Assert.Equal("想法收集", _viewModel.CurrentTitle);
    }

    [Fact]
    public void NavigateToTodos_ShouldUpdateCurrentPageAndTitle()
    {
        _viewModel.NavigateToTodosCommand.Execute(null);
        Assert.Equal("待办列表", _viewModel.CurrentTitle);
    }

    [Fact]
    public void NavigateToToday_ShouldUpdateCurrentPageAndTitle()
    {
        _viewModel.NavigateToTodayCommand.Execute(null);
        Assert.Equal("今日待办", _viewModel.CurrentTitle);
    }

    [Fact]
    public void NavigateToNotes_ShouldUpdateCurrentPageAndTitle()
    {
        _viewModel.NavigateToNotesCommand.Execute(null);
        Assert.Equal("笔记库", _viewModel.CurrentTitle);
    }

    [Fact]
    public void NavigateToReview_ShouldUpdateCurrentPageAndTitle()
    {
        _viewModel.NavigateToReviewCommand.Execute(null);
        Assert.Equal("每周回顾", _viewModel.CurrentTitle);
    }

    [Fact]
    public void Navigate_MultiplePagesInSequence_ShouldUpdateCorrectly()
    {
        _viewModel.NavigateToTodosCommand.Execute(null);
        Assert.Equal("待办列表", _viewModel.CurrentTitle);

        _viewModel.NavigateToTodayCommand.Execute(null);
        Assert.Equal("今日待办", _viewModel.CurrentTitle);

        _viewModel.NavigateToNotesCommand.Execute(null);
        Assert.Equal("笔记库", _viewModel.CurrentTitle);

        _viewModel.NavigateToReviewCommand.Execute(null);
        Assert.Equal("每周回顾", _viewModel.CurrentTitle);

        _viewModel.NavigateToIdeasCommand.Execute(null);
        Assert.Equal("想法收集", _viewModel.CurrentTitle);
    }

    [Fact]
    public void AllNavigationCommands_ShouldExist()
    {
        Assert.NotNull(_viewModel.NavigateToIdeasCommand);
        Assert.NotNull(_viewModel.NavigateToTodosCommand);
        Assert.NotNull(_viewModel.NavigateToTodayCommand);
        Assert.NotNull(_viewModel.NavigateToNotesCommand);
        Assert.NotNull(_viewModel.NavigateToReviewCommand);
    }

    [Fact]
    public void ViewModels_ShouldBeInitialized()
    {
        Assert.NotNull(_viewModel.IdeaViewModel);
        Assert.NotNull(_viewModel.TodoViewModel);
        Assert.NotNull(_viewModel.TodayViewModel);
        Assert.NotNull(_viewModel.NotesViewModel);
        Assert.NotNull(_viewModel.WeeklyReviewViewModel);
    }
}

/// <summary>
/// 共享内存数据库 - 使用 Cache=Shared 模式
/// </summary>
public class SharedMemoryDatabase : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase(string dbName)
    {
        _dbName = dbName;
    }

    public override SqliteConnection CreateConnection()
    {
        return new SqliteConnection($"Data Source={_dbName};Mode=Memory;Cache=Shared");
    }

    public new void Initialize()
    {
        // 保持一个连接打开以维持内存数据库
        _maintainConnection = CreateConnection();
        _maintainConnection.Open();

        // 创建 Ideas 表
        var command = _maintainConnection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Ideas (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                CreatedAt INTEGER NOT NULL
            )";
        command.ExecuteNonQuery();

        // 创建 Tasks 表
        var taskCommand = _maintainConnection.CreateCommand();
        taskCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Tasks (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                Status INTEGER NOT NULL,
                TimeTag TEXT,
                CategoryTag TEXT,
                CreatedAt INTEGER NOT NULL,
                CompletedAt INTEGER
            )";
        taskCommand.ExecuteNonQuery();

        // 创建 Notes 表
        var noteCommand = _maintainConnection.CreateCommand();
        noteCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Notes (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                Category TEXT,
                CreatedAt INTEGER NOT NULL,
                UpdatedAt INTEGER NOT NULL
            )";
        noteCommand.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _maintainConnection?.Dispose();
    }
}
