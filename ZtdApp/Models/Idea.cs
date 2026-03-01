using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZtdApp.Models;

public class Idea : INotifyPropertyChanged
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; } = string.Empty;
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsShowingActions));
            }
        }
    }

    private bool _isSelectingTags;
    public bool IsSelectingTags
    {
        get => _isSelectingTags;
        set
        {
            if (_isSelectingTags != value)
            {
                _isSelectingTags = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsShowingActions));
            }
        }
    }

    // 操作按钮行：展开且未处于标签选择模式时显示
    public bool IsShowingActions => IsExpanded && !IsSelectingTags;

    // 时间标签（互斥，必选一个）
    private bool _isShortTime;
    public bool IsShortTime
    {
        get => _isShortTime;
        set
        {
            if (_isShortTime != value)
            {
                _isShortTime = value;
                if (value) { _isLongTime = false; OnPropertyChanged(nameof(IsLongTime)); }
                OnPropertyChanged();
            }
        }
    }

    private bool _isLongTime;
    public bool IsLongTime
    {
        get => _isLongTime;
        set
        {
            if (_isLongTime != value)
            {
                _isLongTime = value;
                if (value) { _isShortTime = false; OnPropertyChanged(nameof(IsShortTime)); }
                OnPropertyChanged();
            }
        }
    }

    public string? SelectedTimeTag => IsShortTime ? "<30分钟" : IsLongTime ? ">30分钟" : null;

    // 分类标签（可选）
    public string? SelectedCategoryTag { get; set; }

    public void ResetTagSelection()
    {
        IsSelectingTags = false;
        _isShortTime = false;
        _isLongTime = false;
        OnPropertyChanged(nameof(IsShortTime));
        OnPropertyChanged(nameof(IsLongTime));
        SelectedCategoryTag = null;
    }

    // WPF 数据绑定显示属性
    public string CreatedAtDisplay => FormatTime(CreatedAt);

    private static string FormatTime(long timestamp)
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        return date.ToString("MM-dd HH:mm");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
