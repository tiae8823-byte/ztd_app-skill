using Microsoft.Data.Sqlite;
using Xunit;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.Tests;

public class IdeaTests : IDisposable
{
    private readonly SharedMemoryDatabase3 _db;
    private readonly IdeaRepository _repository;
    private readonly IdeaManager _manager;

    public IdeaTests()
    {
        _db = new SharedMemoryDatabase3("IdeaTests");
        _db.Initialize();

        _repository = new IdeaRepository(_db);
        _manager = new IdeaManager(_repository);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Fact]
    public void CreateIdea_ShouldAddToDatabase()
    {
        var content = "测试想法";
        var idea = _manager.Create(content);

        Assert.NotNull(idea);
        Assert.Equal(content, idea.Content);
        Assert.True(idea.CreatedAt > 0);
    }

    [Fact]
    public void Create_MultipleIdeas_AreReturnedInOrder()
    {
        _manager.Create("想法1");
        System.Threading.Thread.Sleep(10);
        _manager.Create("想法2");
        System.Threading.Thread.Sleep(10);
        _manager.Create("想法3");

        var ideas = _manager.GetAll();

        Assert.Equal(3, ideas.Count);
        Assert.Equal("想法3", ideas[0].Content);
        Assert.Equal("想法2", ideas[1].Content);
        Assert.Equal("想法1", ideas[2].Content);
    }

    [Fact]
    public void GetAllIdeas_ShouldReturnAllIdeas()
    {
        _manager.Create("想法1");
        _manager.Create("想法2");

        var ideas = _manager.GetAll();

        Assert.NotNull(ideas);
        Assert.True(ideas.Count >= 2);
        Assert.Contains(ideas, i => i.Content == "想法1");
        Assert.Contains(ideas, i => i.Content == "想法2");
    }

    [Fact]
    public void DeleteIdea_ShouldRemoveFromDatabase()
    {
        var idea = _manager.Create("待删除的想法");
        _manager.Delete(idea.Id);

        var ideas = _manager.GetAll();
        Assert.DoesNotContain(ideas, i => i.Id == idea.Id);
    }

    [Fact]
    public void Create_EmptyContent_StillCreatesIdea()
    {
        var idea = _manager.Create("");

        Assert.NotNull(idea);
        Assert.Equal("", idea.Content);
    }

    [Fact]
    public void Create_WhitespaceContent_CreatesIdea()
    {
        var idea = _manager.Create("   ");

        Assert.NotNull(idea);
        Assert.Equal("   ", idea.Content);
    }

    [Fact]
    public void Create_VeryLongContent_SavesCorrectly()
    {
        var longContent = new string('A', 10000);
        _manager.Create(longContent);
        var ideas = _manager.GetAll();

        Assert.Equal(longContent, ideas[0].Content);
    }

    [Fact]
    public void GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var ideas = _manager.GetAll();
        Assert.Empty(ideas);
    }

    [Fact]
    public void Delete_NonExistentId_DoesNotThrow()
    {
        var exception = Record.Exception(() => _manager.Delete("non-existent-id"));
        Assert.Null(exception);
    }

    [Fact]
    public void CreateMany_Ideas_LoopTerminatesCorrectly()
    {
        const int count = 1000;

        for (int i = 0; i < count; i++)
        {
            _manager.Create($"想法 {i}");
        }

        var ideas = _manager.GetAll();
        Assert.Equal(count, ideas.Count);
    }

    [Fact]
    public void GetAll_LargeList_ReturnsWithoutInfiniteLoop()
    {
        const int count = 500;
        for (int i = 0; i < count; i++)
        {
            _manager.Create($"想法 {i}");
        }

        for (int i = 0; i < 10; i++)
        {
            var ideas = _manager.GetAll();
            Assert.Equal(count, ideas.Count);
        }
    }

    [Fact]
    public void Idea_CreatedAt_IsTimestamp()
    {
        var beforeTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var idea = new Idea { Content = "测试" };
        var afterTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Assert.InRange(idea.CreatedAt, beforeTime, afterTime);
    }

    [Fact]
    public void Idea_Id_IsUnique()
    {
        var idea1 = new Idea { Content = "想法1" };
        var idea2 = new Idea { Content = "想法2" };

        Assert.NotEqual(idea1.Id, idea2.Id);
        Assert.Matches("^[a-f0-9-]{36}$", idea1.Id);
    }
}

public class SharedMemoryDatabase3 : DatabaseService, IDisposable
{
    private readonly string _dbName;
    private SqliteConnection? _maintainConnection;

    public SharedMemoryDatabase3(string dbName)
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
