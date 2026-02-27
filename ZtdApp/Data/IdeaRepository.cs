using Microsoft.Data.Sqlite;
using ZtdApp.Models;

namespace ZtdApp.Data;

public class IdeaRepository
{
    private readonly DatabaseService _dbService;

    public IdeaRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void Add(Idea idea)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Ideas (Id, Content, CreatedAt)
            VALUES (@id, @content, @createdAt)";
        command.Parameters.AddWithValue("@id", idea.Id);
        command.Parameters.AddWithValue("@content", idea.Content);
        command.Parameters.AddWithValue("@createdAt", idea.CreatedAt);
        command.ExecuteNonQuery();
    }

    public List<Idea> GetAll()
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Content, CreatedAt FROM Ideas ORDER BY CreatedAt DESC";

        var ideas = new List<Idea>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ideas.Add(new Idea
            {
                Id = reader.GetString(0),
                Content = reader.GetString(1),
                CreatedAt = reader.GetInt64(2)
            });
        }

        return ideas;
    }

    public void Delete(string id)
    {
        using var connection = _dbService.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Ideas WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }
}
