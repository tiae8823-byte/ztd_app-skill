using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ZtdApp.Models;

namespace ZtdApp.Data;

public class TaskRepository
{
    private readonly DatabaseService _dbService;

    public TaskRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void Add(TodoTask task)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Tasks (Id, Content, Status, TimeTag, CategoryTag, CreatedAt, CompletedAt)
            VALUES (@id, @content, @status, @timeTag, @categoryTag, @createdAt, @completedAt)";
        command.Parameters.AddWithValue("@id", task.Id);
        command.Parameters.AddWithValue("@content", task.Content);
        command.Parameters.AddWithValue("@status", (int)task.Status);
        command.Parameters.AddWithValue("@timeTag", task.TimeTag ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@categoryTag", task.CategoryTag ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", task.CreatedAt);
        command.Parameters.AddWithValue("@completedAt", task.CompletedAt ?? (object)DBNull.Value);
        command.ExecuteNonQuery();
    }

    public List<TodoTask> GetByStatus(TodoTaskStatus status)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Content, Status, TimeTag, CategoryTag, CreatedAt, CompletedAt FROM Tasks WHERE Status = @status ORDER BY CreatedAt DESC";
        command.Parameters.AddWithValue("@status", (int)status);

        var tasks = new List<TodoTask>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tasks.Add(new TodoTask
            {
                Id = reader.GetString(0),
                Content = reader.GetString(1),
                Status = (TodoTaskStatus)reader.GetInt32(2),
                TimeTag = reader.IsDBNull(3) ? null : reader.GetString(3),
                CategoryTag = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = reader.GetInt64(5),
                CompletedAt = reader.IsDBNull(6) ? null : reader.GetInt64(6)
            });
        }

        return tasks;
    }

    public void Delete(string id)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Tasks WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    public void UpdateStatus(string id, TodoTaskStatus status)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Tasks SET Status = @status WHERE Id = @id";
        command.Parameters.AddWithValue("@status", (int)status);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    public void Complete(string id)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Tasks SET Status = @status, CompletedAt = @completedAt WHERE Id = @id";
        command.Parameters.AddWithValue("@status", (int)TodoTaskStatus.Done);
        command.Parameters.AddWithValue("@completedAt", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }
}
