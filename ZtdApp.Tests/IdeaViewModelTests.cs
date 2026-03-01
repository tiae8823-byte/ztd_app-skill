using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// IdeaViewModel 功能测试
/// </summary>
public class IdeaViewModelTests : IDisposable
{
    private readonly SharedMemoryDatabase2 _db;
    private readonly IdeaManager _ideaManager;
    private readonly TaskManager _taskManager;
    private readonly NoteManager _noteManager;
    private readonly IdeaViewModel _viewModel;

    public IdeaViewModelTests()
    {
        _db = new SharedMemoryDatabase2("IdeaViewModelTests");
        _db.Initialize();

        var ideaRepository = new IdeaRepository(_db);
        var taskRepository = new TaskRepository(_db);
        var noteRepository = new NoteRepository(_db);

        _ideaManager = new IdeaManager(ideaRepository);
        _taskManager = new TaskManager(taskRepository);
        _noteManager = new NoteManager(noteRepository);

        _viewModel = new IdeaViewModel(_ideaManager, _taskManager, _noteManager);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Fact]
    public void Constructor_ShouldLoadExistingIdeas()
    {
        _ideaManager.Create("已有想法1");
        _ideaManager.Create("已有想法2");

        var newViewModel = new IdeaViewModel(_ideaManager, _taskManager, _noteManager);

        Assert.Equal(2, newViewModel.Ideas.Count);
    }

    [Fact]
    public void AddIdea_ShouldAddToList()
    {
        _viewModel.InputContent = "测试想法";
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Single(_viewModel.Ideas);
        Assert.Equal("测试想法", _viewModel.Ideas[0].Content);
    }

    [Fact]
    public void AddIdea_ShouldClearInputAfterAdd()
    {
        _viewModel.InputContent = "测试想法";
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Equal(string.Empty, _viewModel.InputContent);
    }

    [Fact]
    public void DeleteIdea_ShouldRemoveFromList()
    {
        _ideaManager.Create("待删除的想法");
        var viewModel = new IdeaViewModel(_ideaManager, _taskManager, _noteManager);
        var ideaId = viewModel.Ideas[0].Id;

        viewModel.DeleteIdeaCommand.Execute(ideaId);

        Assert.Empty(viewModel.Ideas);
    }

    [Fact]
    public void AddIdea_EmptyContent_ShouldNotAdd()
    {
        _viewModel.InputContent = "";
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Empty(_viewModel.Ideas);
    }

    [Fact]
    public void AddIdea_WhitespaceContent_ShouldNotAdd()
    {
        _viewModel.InputContent = "   ";
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Empty(_viewModel.Ideas);
    }

    [Fact]
    public void AddIdea_NullContent_ShouldNotAdd()
    {
        _viewModel.InputContent = null!;
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Empty(_viewModel.Ideas);
    }

    [Fact]
    public void AddIdea_VeryLongContent_ShouldWork()
    {
        var longContent = new string('A', 5000);
        _viewModel.InputContent = longContent;
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Single(_viewModel.Ideas);
        Assert.Equal(longContent, _viewModel.Ideas[0].Content);
    }

    [Fact]
    public void Commands_ShouldExist()
    {
        Assert.NotNull(_viewModel.AddIdeaCommand);
        Assert.NotNull(_viewModel.DeleteIdeaCommand);
        Assert.NotNull(_viewModel.ToggleExpandCommand);
        Assert.NotNull(_viewModel.StartTagSelectionCommand);
        Assert.NotNull(_viewModel.CancelTagSelectionCommand);
        Assert.NotNull(_viewModel.ConfirmTagSelectionCommand);
        Assert.NotNull(_viewModel.ConvertToNoteCommand);
        Assert.NotNull(_viewModel.QuickCompleteCommand);
    }

    [Fact]
    public void ToggleExpand_ShouldToggleIsExpanded()
    {
        _viewModel.InputContent = "测试想法";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];

        Assert.False(idea.IsExpanded);

        _viewModel.ToggleExpandCommand.Execute(idea);
        Assert.True(idea.IsExpanded);

        _viewModel.ToggleExpandCommand.Execute(idea);
        Assert.False(idea.IsExpanded);
    }

    [Fact]
    public void ConvertToTodo_ShouldCreateTaskAndRemoveIdea()
    {
        _viewModel.InputContent = "转为待办";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];

        _viewModel.StartTagSelectionCommand.Execute(idea.Id);
        idea.IsShortTime = true;
        _viewModel.ConfirmTagSelectionCommand.Execute(idea.Id);

        Assert.Empty(_viewModel.Ideas);
        var tasks = _taskManager.GetByStatus(TodoTaskStatus.Todo);
        Assert.Single(tasks);
        Assert.Equal("转为待办", tasks[0].Content);
        Assert.Equal("<30分钟", tasks[0].TimeTag);
    }

    [Fact]
    public void ConvertToNote_ShouldCreateNoteAndRemoveIdea()
    {
        _viewModel.InputContent = "转为笔记";
        _viewModel.AddIdeaCommand.Execute(null);
        var ideaId = _viewModel.Ideas[0].Id;

        _viewModel.ConvertToNoteCommand.Execute(ideaId);

        Assert.Empty(_viewModel.Ideas);
        var notes = _noteManager.GetAll();
        Assert.Single(notes);
        Assert.Equal("转为笔记", notes[0].Content);
    }

    [Fact]
    public void QuickComplete_ShouldCreateCompletedTaskAndRemoveIdea()
    {
        _viewModel.InputContent = "快速完成";
        _viewModel.AddIdeaCommand.Execute(null);
        var ideaId = _viewModel.Ideas[0].Id;

        _viewModel.QuickCompleteCommand.Execute(ideaId);

        Assert.Empty(_viewModel.Ideas);
        var completedTasks = _taskManager.GetByStatus(TodoTaskStatus.Done);
        Assert.Single(completedTasks);
        Assert.Equal("快速完成", completedTasks[0].Content);
    }

    [Fact]
    public void ConvertToTodo_WithInvalidId_ShouldNotCrash()
    {
        _viewModel.StartTagSelectionCommand.Execute("invalid-id");
        _viewModel.ConfirmTagSelectionCommand.Execute("invalid-id");
        Assert.Empty(_viewModel.Ideas);
    }

    [Fact]
    public void ConvertToNote_WithInvalidId_ShouldNotCrash()
    {
        _viewModel.ConvertToNoteCommand.Execute("invalid-id");
        Assert.Empty(_viewModel.Ideas);
    }

    [Fact]
    public void QuickComplete_WithInvalidId_ShouldNotCrash()
    {
        _viewModel.QuickCompleteCommand.Execute("invalid-id");
        Assert.Empty(_viewModel.Ideas);
    }

    [Fact]
    public void Ideas_ShouldBeObservableCollection()
    {
        Assert.IsType<System.Collections.ObjectModel.ObservableCollection<Idea>>(_viewModel.Ideas);
    }

    [Fact]
    public void AddMultipleIdeas_ShouldUpdateListCorrectly()
    {
        _viewModel.InputContent = "想法1";
        _viewModel.AddIdeaCommand.Execute(null);

        _viewModel.InputContent = "想法2";
        _viewModel.AddIdeaCommand.Execute(null);

        _viewModel.InputContent = "想法3";
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Equal(3, _viewModel.Ideas.Count);
    }

    [Fact]
    public void Ideas_ShouldBeInReverseChronologicalOrder()
    {
        _viewModel.InputContent = "第一个";
        _viewModel.AddIdeaCommand.Execute(null);
        System.Threading.Thread.Sleep(10);

        _viewModel.InputContent = "第二个";
        _viewModel.AddIdeaCommand.Execute(null);
        System.Threading.Thread.Sleep(10);

        _viewModel.InputContent = "第三个";
        _viewModel.AddIdeaCommand.Execute(null);

        Assert.Equal("第三个", _viewModel.Ideas[0].Content);
        Assert.Equal("第二个", _viewModel.Ideas[1].Content);
        Assert.Equal("第一个", _viewModel.Ideas[2].Content);
    }
}

public class SharedMemoryDatabase2 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase2(string dbName)
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
