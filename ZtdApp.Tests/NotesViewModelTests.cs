using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Services;
using ZtdApp.ViewModels.Pages;

namespace ZtdApp.Tests;

/// <summary>
/// NotesViewModel 功能测试
/// </summary>
public class NotesViewModelTests : IDisposable
{
    private readonly SharedMemoryDatabase7 _db;
    private readonly NoteManager _noteManager;
    private readonly NotesViewModel _viewModel;

    public NotesViewModelTests()
    {
        _db = new SharedMemoryDatabase7("NotesViewModelTests");
        _db.Initialize();

        var noteRepository = new NoteRepository(_db);
        _noteManager = new NoteManager(noteRepository);
        _viewModel = new NotesViewModel(_noteManager);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Fact]
    public void Constructor_ShouldLoadExistingNotes()
    {
        _noteManager.Create("笔记1", "工作");
        _noteManager.Create("笔记2", "学习");

        var newViewModel = new NotesViewModel(_noteManager);

        Assert.Equal(2, newViewModel.Notes.Count);
    }

    [Fact]
    public void AddNote_WithCategory_ShouldAddToList()
    {
        _viewModel.InputContent = "新笔记";
        _viewModel.SelectedCategory = "工作";

        _viewModel.AddNoteCommand.Execute(null);

        Assert.Single(_viewModel.Notes);
        Assert.Equal("新笔记", _viewModel.Notes[0].Content);
        Assert.Equal("工作", _viewModel.Notes[0].Category);
        Assert.Empty(_viewModel.InputContent);
        Assert.Null(_viewModel.SelectedCategory);
    }

    [Fact]
    public void AddNote_WithoutCategory_ShouldAddToList()
    {
        _viewModel.InputContent = "无分类笔记";

        _viewModel.AddNoteCommand.Execute(null);

        Assert.Single(_viewModel.Notes);
        Assert.Equal("无分类笔记", _viewModel.Notes[0].Content);
        Assert.Null(_viewModel.Notes[0].Category);
    }

    [Fact]
    public void AddNote_EmptyContent_ShouldNotAdd()
    {
        _viewModel.InputContent = "   ";

        _viewModel.AddNoteCommand.Execute(null);

        Assert.Empty(_viewModel.Notes);
    }

    [Fact]
    public void OpenEditDialog_ShouldPopulateEditFields()
    {
        var note = _noteManager.Create("原始内容", "工作");
        _viewModel.LoadNotes();

        _viewModel.OpenEditDialogCommand.Execute(note.Id);

        Assert.True(_viewModel.IsEditDialogVisible);
        Assert.Equal(note.Id, _viewModel.EditNoteId);
        Assert.Equal("原始内容", _viewModel.EditContent);
        Assert.Equal("工作", _viewModel.EditCategory);
    }

    [Fact]
    public void SaveEdit_ShouldUpdateNote()
    {
        var note = _noteManager.Create("原始内容", "工作");
        _viewModel.LoadNotes();

        _viewModel.OpenEditDialogCommand.Execute(note.Id);
        _viewModel.EditContent = "修改后的内容";
        _viewModel.EditCategory = "学习";
        _viewModel.SaveEditCommand.Execute(null);

        Assert.False(_viewModel.IsEditDialogVisible);
        Assert.Equal("修改后的内容", _viewModel.Notes[0].Content);
        Assert.Equal("学习", _viewModel.Notes[0].Category);
    }

    [Fact]
    public void SaveEdit_EmptyContent_ShouldNotUpdate()
    {
        var note = _noteManager.Create("原始内容", "工作");
        _viewModel.LoadNotes();

        _viewModel.OpenEditDialogCommand.Execute(note.Id);
        _viewModel.EditContent = "   ";
        _viewModel.SaveEditCommand.Execute(null);

        // 对话框应该仍然可见（因为验证失败）
        Assert.True(_viewModel.IsEditDialogVisible);
        // 原始内容不应该被修改
        Assert.Equal("原始内容", _viewModel.Notes[0].Content);
    }

    [Fact]
    public void CancelEdit_ShouldCloseDialog()
    {
        var note = _noteManager.Create("原始内容", "工作");
        _viewModel.LoadNotes();

        _viewModel.OpenEditDialogCommand.Execute(note.Id);
        _viewModel.EditContent = "修改后的内容";
        _viewModel.CancelEditCommand.Execute(null);

        Assert.False(_viewModel.IsEditDialogVisible);
        // 原始内容不应该被修改
        Assert.Equal("原始内容", _viewModel.Notes[0].Content);
    }

    [Fact]
    public void DeleteNote_ShouldRemoveFromList()
    {
        var note = _noteManager.Create("要删除的笔记", "工作");
        _viewModel.LoadNotes();

        // 注意：DeleteNote 会弹出确认对话框，这里直接调用 Manager 的 Delete
        _noteManager.Delete(note.Id);
        _viewModel.LoadNotes();

        Assert.Empty(_viewModel.Notes);
    }

    [Fact]
    public void LoadNotes_ShouldOrderByCreatedAtDesc()
    {
        _noteManager.Create("笔记1", "工作");
        Thread.Sleep(10); // 确保时间戳不同
        _noteManager.Create("笔记2", "学习");
        Thread.Sleep(10);
        _noteManager.Create("笔记3", "个人");

        _viewModel.LoadNotes();

        Assert.Equal(3, _viewModel.Notes.Count);
        Assert.Equal("笔记3", _viewModel.Notes[0].Content); // 最新的在前
        Assert.Equal("笔记2", _viewModel.Notes[1].Content);
        Assert.Equal("笔记1", _viewModel.Notes[2].Content);
    }
}

/// <summary>
/// 共享内存数据库（用于 NotesViewModel 测试）
/// </summary>
public class SharedMemoryDatabase7 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase7(string dbName)
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

        // 创建 Notes 表
        var command = _maintainConnection.CreateCommand();
        command.CommandText = @"
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
        _maintainConnection?.Close();
        _maintainConnection?.Dispose();
    }
}
