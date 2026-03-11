using System.ComponentModel;

namespace ZtdApp.Models;

public class TestCard : INotifyPropertyChanged
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
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
    }

    public string CreatedAtDisplay => FormatTime(CreatedAt);

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
