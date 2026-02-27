namespace ZtdApp.Models;

/// <summary>
/// 笔记模型
/// </summary>
public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }  // 分类标签
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public long UpdatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    // WPF 数据绑定显示属性
    public string CreatedAtDisplay => FormatTime(CreatedAt);

    private static string FormatTime(long timestamp)
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        return date.ToString("MM-dd HH:mm");
    }
}
