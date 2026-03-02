using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ZtdApp.Models;

namespace ZtdApp.Data;

public class TomatoRepository
{
    private readonly DatabaseService _dbService;

    public TomatoRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void Add(Tomato tomato)
    {
        using var connection = _dbService.CreateConnection();
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

    public void Update(Tomato tomato)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Tomatoes
            SET Duration = @duration, CompletedAt = @completedAt
            WHERE Id = @id";
        command.Parameters.AddWithValue("@duration", tomato.Duration);
        command.Parameters.AddWithValue("@completedAt", tomato.CompletedAt ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", tomato.Id);
        command.ExecuteNonQuery();
    }

    public List<Tomato> GetByTaskId(string taskId)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, TaskId, Duration, TargetDuration, StartTime, CompletedAt FROM Tomatoes WHERE TaskId = @taskId ORDER BY StartTime DESC";
        command.Parameters.AddWithValue("@taskId", taskId);

        var tomatoes = new List<Tomato>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tomatoes.Add(new Tomato
            {
                Id = reader.GetString(0),
                TaskId = reader.IsDBNull(1) ? null : reader.GetString(1),
                Duration = reader.GetInt32(2),
                TargetDuration = reader.GetInt32(3),
                StartTime = reader.GetInt64(4),
                CompletedAt = reader.IsDBNull(5) ? null : reader.GetInt64(5)
            });
        }

        return tomatoes;
    }

    /// <summary>
    /// 获取本周完成的番茄钟记录
    /// </summary>
    public List<Tomato> GetThisWeek()
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        // 计算本周开始时间（周一 00:00:00）
        var now = DateTime.Now;
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
        if (now.DayOfWeek == DayOfWeek.Sunday)
        {
            weekStart = now.Date.AddDays(-6);
        }
        var weekStartTimestamp = ((DateTimeOffset)weekStart).ToUnixTimeMilliseconds();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, TaskId, Duration, TargetDuration, StartTime, CompletedAt
            FROM Tomatoes
            WHERE CompletedAt >= @weekStart AND Duration >= 10
            ORDER BY StartTime DESC";
        command.Parameters.AddWithValue("@weekStart", weekStartTimestamp);

        var tomatoes = new List<Tomato>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tomatoes.Add(new Tomato
            {
                Id = reader.GetString(0),
                TaskId = reader.IsDBNull(1) ? null : reader.GetString(1),
                Duration = reader.GetInt32(2),
                TargetDuration = reader.GetInt32(3),
                StartTime = reader.GetInt64(4),
                CompletedAt = reader.IsDBNull(5) ? null : reader.GetInt64(5)
            });
        }

        return tomatoes;
    }
}
