namespace ZtdApp.Models;

public class Idea
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    // WPF 数据绑定显示属性
    public string CreatedAtDisplay => FormatTime(CreatedAt);

    private static string FormatTime(long timestamp)
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        return date.ToString("MM-dd HH:mm");
    }
}
