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
    private readonly IdeaManager _manager;
    private readonly IdeaViewModel _viewModel;

    public IdeaViewModelTests()
    {
        _db = new SharedMemoryDatabase2("IdeaViewModelTests");
        _db.Initialize();

        var repository = new IdeaRepository(_db);
        _manager = new IdeaManager(repository);
        _viewModel = new IdeaViewModel(_manager);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Fact]
    public void Constructor_ShouldLoadExistingIdeas()
    {
        _manager.Create("已有想法1");
        _manager.Create("已有想法2");

        var newViewModel = new IdeaViewModel(_manager);

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
        _manager.Create("待删除的想法");
        var viewModel = new IdeaViewModel(_manager);
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
            )";
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _maintainConnection?.Dispose();
    }
}
