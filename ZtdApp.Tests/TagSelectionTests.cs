using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// 想法转待办时选择标签的功能测试
/// </summary>
public class TagSelectionTests : IDisposable
{
    private readonly SharedMemoryDatabase2 _db;
    private readonly IdeaManager _ideaManager;
    private readonly TaskManager _taskManager;
    private readonly NoteManager _noteManager;
    private readonly IdeaViewModel _viewModel;

    public TagSelectionTests()
    {
        _db = new SharedMemoryDatabase2("TagSelectionTests");
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

    // ── Idea 模型属性测试 ──────────────────────────────

    [Fact]
    public void Idea_IsShortTime_DefaultFalse()
    {
        var idea = new Idea { Content = "测试" };
        Assert.False(idea.IsShortTime);
        Assert.False(idea.IsLongTime);
        Assert.Null(idea.SelectedTimeTag);
    }

    [Fact]
    public void Idea_SetIsShortTime_SelectedTimeTagUpdates()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsShortTime = true;
        Assert.Equal("<30分钟", idea.SelectedTimeTag);
    }

    [Fact]
    public void Idea_SetIsLongTime_SelectedTimeTagUpdates()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsLongTime = true;
        Assert.Equal(">30分钟", idea.SelectedTimeTag);
    }

    [Fact]
    public void Idea_TimeTag_MutualExclusion_ShortToLong()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsShortTime = true;
        idea.IsLongTime = true;

        Assert.False(idea.IsShortTime);
        Assert.True(idea.IsLongTime);
        Assert.Equal(">30分钟", idea.SelectedTimeTag);
    }

    [Fact]
    public void Idea_TimeTag_MutualExclusion_LongToShort()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsLongTime = true;
        idea.IsShortTime = true;

        Assert.True(idea.IsShortTime);
        Assert.False(idea.IsLongTime);
        Assert.Equal("<30分钟", idea.SelectedTimeTag);
    }

    [Fact]
    public void Idea_IsShowingActions_TrueWhenExpandedAndNotSelectingTags()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsExpanded = true;
        Assert.True(idea.IsShowingActions);
    }

    [Fact]
    public void Idea_IsShowingActions_FalseWhenSelectingTags()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsExpanded = true;
        idea.IsSelectingTags = true;
        Assert.False(idea.IsShowingActions);
    }

    [Fact]
    public void Idea_ResetTagSelection_ClearsAllState()
    {
        var idea = new Idea { Content = "测试" };
        idea.IsSelectingTags = true;
        idea.IsShortTime = true;
        idea.SelectedCategoryTag = "工作";

        idea.ResetTagSelection();

        Assert.False(idea.IsSelectingTags);
        Assert.False(idea.IsShortTime);
        Assert.False(idea.IsLongTime);
        Assert.Null(idea.SelectedTimeTag);
        Assert.Null(idea.SelectedCategoryTag);
    }

    // ── ViewModel 命令测试 ────────────────────────────

    [Fact]
    public void StartTagSelection_SetsIsSelectingTagsTrue()
    {
        _viewModel.InputContent = "测试想法";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];

        _viewModel.StartTagSelectionCommand.Execute(idea.Id);

        Assert.True(idea.IsSelectingTags);
    }

    [Fact]
    public void StartTagSelection_ResetsExistingTagState()
    {
        _viewModel.InputContent = "测试想法";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];
        idea.IsShortTime = true;

        _viewModel.StartTagSelectionCommand.Execute(idea.Id);

        Assert.True(idea.IsSelectingTags);
        Assert.False(idea.IsShortTime);
    }

    [Fact]
    public void CancelTagSelection_ResetsSelectingState()
    {
        _viewModel.InputContent = "测试想法";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];
        _viewModel.StartTagSelectionCommand.Execute(idea.Id);
        idea.IsShortTime = true;

        _viewModel.CancelTagSelectionCommand.Execute(idea.Id);

        Assert.False(idea.IsSelectingTags);
        Assert.False(idea.IsShortTime);
    }

    [Fact]
    public void ConfirmTagSelection_WithTimeTag_CreatesTaskWithTags()
    {
        _viewModel.InputContent = "有标签的任务";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];
        _viewModel.StartTagSelectionCommand.Execute(idea.Id);
        idea.IsShortTime = true;
        idea.SelectedCategoryTag = "工作";

        _viewModel.ConfirmTagSelectionCommand.Execute(idea.Id);

        Assert.Empty(_viewModel.Ideas);
        var tasks = _taskManager.GetByStatus(TodoTaskStatus.Todo);
        Assert.Single(tasks);
        Assert.Equal("有标签的任务", tasks[0].Content);
        Assert.Equal("<30分钟", tasks[0].TimeTag);
        Assert.Equal("工作", tasks[0].CategoryTag);
    }

    [Fact]
    public void ConfirmTagSelection_WithLongTimeTag_CreatesTaskCorrectly()
    {
        _viewModel.InputContent = "大于30分钟任务";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];
        _viewModel.StartTagSelectionCommand.Execute(idea.Id);
        idea.IsLongTime = true;

        _viewModel.ConfirmTagSelectionCommand.Execute(idea.Id);

        var tasks = _taskManager.GetByStatus(TodoTaskStatus.Todo);
        Assert.Single(tasks);
        Assert.Equal(">30分钟", tasks[0].TimeTag);
        Assert.Null(tasks[0].CategoryTag);
    }

    [Fact]
    public void ConfirmTagSelection_WithoutTimeTag_DoesNotCreateTask()
    {
        _viewModel.InputContent = "未选时间标签";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];
        _viewModel.StartTagSelectionCommand.Execute(idea.Id);
        // 不设置时间标签

        _viewModel.ConfirmTagSelectionCommand.Execute(idea.Id);

        // 想法未删除，任务未创建
        Assert.Single(_viewModel.Ideas);
        Assert.Empty(_taskManager.GetByStatus(TodoTaskStatus.Todo));
    }

    [Fact]
    public void ConfirmTagSelection_WithCategoryOnly_NoTimeTag_DoesNotCreateTask()
    {
        _viewModel.InputContent = "只有分类标签";
        _viewModel.AddIdeaCommand.Execute(null);
        var idea = _viewModel.Ideas[0];
        _viewModel.StartTagSelectionCommand.Execute(idea.Id);
        idea.SelectedCategoryTag = "学习";

        _viewModel.ConfirmTagSelectionCommand.Execute(idea.Id);

        Assert.Single(_viewModel.Ideas);
        Assert.Empty(_taskManager.GetByStatus(TodoTaskStatus.Todo));
    }

    [Fact]
    public void StartTagSelection_WithInvalidId_DoesNotCrash()
    {
        var ex = Record.Exception(() => _viewModel.StartTagSelectionCommand.Execute("invalid-id"));
        Assert.Null(ex);
    }

    [Fact]
    public void CancelTagSelection_WithInvalidId_DoesNotCrash()
    {
        var ex = Record.Exception(() => _viewModel.CancelTagSelectionCommand.Execute("invalid-id"));
        Assert.Null(ex);
    }

    [Fact]
    public void ConfirmTagSelection_WithInvalidId_DoesNotCrash()
    {
        var ex = Record.Exception(() => _viewModel.ConfirmTagSelectionCommand.Execute("invalid-id"));
        Assert.Null(ex);
    }

    [Fact]
    public void NewCommands_ShouldExist()
    {
        Assert.NotNull(_viewModel.StartTagSelectionCommand);
        Assert.NotNull(_viewModel.CancelTagSelectionCommand);
        Assert.NotNull(_viewModel.ConfirmTagSelectionCommand);
    }
}
