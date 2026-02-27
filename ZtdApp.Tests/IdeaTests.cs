using Microsoft.Data.Sqlite;
using ZtdApp.Data;
using ZtdApp.Models;
using ZtdApp.Services;

namespace ZtdApp.Tests;

public class IdeaTests : IDisposable
{
    private readonly DatabaseService _dbService;
    private readonly IdeaRepository _repository;
    private readonly IdeaManager _manager;

    public IdeaTests()
    {
        // 使用内存数据库进行测试
        _dbService = new InMemoryDatabaseService();
        _dbService.Initialize();
        _repository = new IdeaRepository(_dbService);
        _manager = new IdeaManager(_repository);
    }

    public void Dispose()
    {
        _dbService.Dispose();
    }

    // ============ 核心逻辑测试 ============

    [Fact]
    public void CreateIdea_ShouldAddToDatabase()
    {
        // Arrange
        var content = "测试想法";

        // Act
        var idea = _manager.Create(content);

        // Assert
        Assert.NotNull(idea);
        Assert.Equal(content, idea.Content);
        Assert.True(idea.CreatedAt > 0);
    }

    [Fact]
    public void Create_MultipleIdeas_AreReturnedInOrder()
    {
        // Arrange & Act
        _manager.Create("想法1");
        _manager.Create("想法2");
        _manager.Create("想法3");

        var ideas = _manager.GetAll();

        // Assert - 应该按创建时间倒序返回
        Assert.Equal(3, ideas.Count);
        Assert.Equal("想法3", ideas[0].Content);
        Assert.Equal("想法2", ideas[1].Content);
        Assert.Equal("想法1", ideas[2].Content);
    }

    [Fact]
    public void GetAllIdeas_ShouldReturnAllIdeas()
    {
        // Arrange
        _manager.Create("想法1");
        _manager.Create("想法2");

        // Act
        var ideas = _manager.GetAll();

        // Assert
        Assert.NotNull(ideas);
        Assert.True(ideas.Count >= 2);
        Assert.Contains(ideas, i => i.Content == "想法1");
        Assert.Contains(ideas, i => i.Content == "想法2");
    }

    [Fact]
    public void DeleteIdea_ShouldRemoveFromDatabase()
    {
        // Arrange
        var idea = _manager.Create("待删除的想法");

        // Act
        _manager.Delete(idea.Id);

        // Assert
        var ideas = _manager.GetAll();
        Assert.DoesNotContain(ideas, i => i.Id == idea.Id);
    }

    // ============ 边界条件测试 ============

    [Fact]
    public void Create_EmptyContent_StillCreatesIdea()
    {
        // Act
        var idea = _manager.Create("");

        // Assert
        Assert.NotNull(idea);
        Assert.Equal("", idea.Content);
    }

    [Fact]
    public void Create_WhitespaceContent_CreatesIdea()
    {
        // Act
        var idea = _manager.Create("   ");

        // Assert
        Assert.NotNull(idea);
        Assert.Equal("   ", idea.Content);
    }

    [Fact]
    public void Create_VeryLongContent_SavesCorrectly()
    {
        // Arrange
        var longContent = new string('A', 10000);

        // Act
        var idea = _manager.Create(longContent);
        var ideas = _manager.GetAll();

        // Assert
        Assert.Equal(longContent, ideas[0].Content);
    }

    [Fact]
    public void GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var ideas = _manager.GetAll();

        // Assert
        Assert.Empty(ideas);
    }

    [Fact]
    public void Delete_NonExistentId_DoesNotThrow()
    {
        // Act & Assert - 不应该抛出异常
        var exception = Record.Exception(() => _manager.Delete("non-existent-id"));
        Assert.Null(exception);
    }

    // ============ 死循环防御测试 ============

    [Fact]
    public void CreateMany_Ideas_LoopTerminatesCorrectly()
    {
        // Arrange - 创建大量想法
        const int count = 1000;

        // Act - 如果有死循环，这个会卡住
        for (int i = 0; i < count; i++)
        {
            _manager.Create($"想法 {i}");
        }

        var ideas = _manager.GetAll();

        // Assert - 确保循环正常结束
        Assert.Equal(count, ideas.Count);
    }

    [Fact]
    public void GetAll_LargeList_ReturnsWithoutInfiniteLoop()
    {
        // Arrange - 先创建大量数据
        const int count = 500;
        for (int i = 0; i < count; i++)
        {
            _manager.Create($"想法 {i}");
        }

        // Act - 多次调用 GetAll，确保没有死循环
        for (int i = 0; i < 10; i++)
        {
            var ideas = _manager.GetAll();
            Assert.Equal(count, ideas.Count);
        }
    }

    // ============ 日志功能测试 ============

    [Fact]
    public void Idea_CreatedAt_IsTimestamp()
    {
        // Arrange
        var beforeTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Act
        var idea = new Idea { Content = "测试" };

        // Assert
        var afterTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Assert.InRange(idea.CreatedAt, beforeTime, afterTime);
    }

    [Fact]
    public void Idea_Id_IsUnique()
    {
        // Arrange & Act
        var idea1 = new Idea { Content = "想法1" };
        var idea2 = new Idea { Content = "想法2" };

        // Assert
        Assert.NotEqual(idea1.Id, idea2.Id);
        Assert.Matches("^[a-f0-9-]{36}$", idea1.Id);
    }
}

// ============ 内存数据库辅助类 ============

public class InMemoryDatabaseService : DatabaseService, IDisposable
{
    private SqliteConnection? _connection;

    public override SqliteConnection CreateConnection()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        return _connection;
    }

    public new void Dispose()
    {
        _connection?.Dispose();
    }
}
