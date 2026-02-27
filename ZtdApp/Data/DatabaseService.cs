using Microsoft.Data.Sqlite;
using System.IO;

namespace ZtdApp.Data;

public class DatabaseService
{
    private readonly string _dbPath;

    public DatabaseService()
    {
        // 数据库文件放在 AppData 目录
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ZtdApp");
        Directory.CreateDirectory(appDataPath);
        _dbPath = Path.Combine(appDataPath, "ztd.db");
    }

    public void Initialize()
    {
        using var connection = CreateConnection();
        connection.Open();

        // 创建 Idea 表
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Ideas (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL,
                CreatedAt INTEGER NOT NULL
            )";
        command.ExecuteNonQuery();
    }

    public virtual SqliteConnection CreateConnection()
    {
        return new SqliteConnection($"Data Source={_dbPath}");
    }
}
