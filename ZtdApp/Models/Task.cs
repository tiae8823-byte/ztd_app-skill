using System.ComponentModel;

namespace ZtdApp.Models;

/// <summary>
/// 任务状态
/// </summary>
public enum TodoTaskStatus
{
    Todo,    // 待办列表
    Today,   // 今日待办
    Done     // 已完成
}

/// <summary>
/// 任务模型
/// </summary>
public class TodoTask : INotifyPropertyChanged
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public TodoTaskStatus Status { get; set; } = TodoTaskStatus.Todo;
    public string? TimeTag { get; set; }  // <30分钟 / >30分钟
    public string? CategoryTag { get; set; }  // 工作 / 个人 / 学习 / 运动
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public long? CompletedAt { get; set; }

    // UI 绑定状态
    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
    }

    // 卡片展开状态（用于点击展开操作按钮）
    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
    }

    // WPF 数据绑定显示属性
    public string CreatedAtDisplay => FormatTime(CreatedAt);
    public string CompletedAtDisplay => CompletedAt.HasValue ? FormatTime(CompletedAt.Value) : "";
    public string CategoryTagDisplay => CategoryTag ?? "";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string FormatTime(long timestamp)
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        return date.ToString("MM-dd HH:mm");
    }
}
