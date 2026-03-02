using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ZtdApp.Models;

namespace ZtdApp.Data;

public class NoteRepository
{
    private readonly DatabaseService _dbService;

    public NoteRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void Add(Note note)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Notes (Id, Content, Category, CreatedAt, UpdatedAt)
            VALUES (@id, @content, @category, @createdAt, @updatedAt)";
        command.Parameters.AddWithValue("@id", note.Id);
        command.Parameters.AddWithValue("@content", note.Content);
        command.Parameters.AddWithValue("@category", note.Category ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", note.CreatedAt);
        command.Parameters.AddWithValue("@updatedAt", note.UpdatedAt);
        command.ExecuteNonQuery();
    }

    public List<Note> GetAll()
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Content, Category, CreatedAt, UpdatedAt FROM Notes ORDER BY CreatedAt DESC";

        var notes = new List<Note>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            notes.Add(new Note
            {
                Id = reader.GetString(0),
                Content = reader.GetString(1),
                Category = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedAt = reader.GetInt64(3),
                UpdatedAt = reader.GetInt64(4)
            });
        }

        return notes;
    }

    public void Update(Note note)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Notes
            SET Content = @content, Category = @category, UpdatedAt = @updatedAt
            WHERE Id = @id";
        command.Parameters.AddWithValue("@id", note.Id);
        command.Parameters.AddWithValue("@content", note.Content);
        command.Parameters.AddWithValue("@category", note.Category ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@updatedAt", note.UpdatedAt);
        command.ExecuteNonQuery();
    }

    public void Delete(string id)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Notes WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }
}
