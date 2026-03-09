using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZtdApp.Services;

namespace ZtdApp.ViewModels;

/// <summary>
/// 快速添加想法对话框的 ViewModel
/// </summary>
public partial class QuickAddViewModel : ObservableObject
{
    private readonly IdeaManager _ideaManager;

    [ObservableProperty]
    private string _content = string.Empty;

    public QuickAddViewModel(IdeaManager ideaManager)
    {
        _ideaManager = ideaManager;
    }

    /// <summary>
    /// 添加想法命令
    /// </summary>
    [RelayCommand]
    private void AddIdea()
    {
        if (string.IsNullOrWhiteSpace(Content))
            return;

        _ideaManager.Create(Content);
        Content = string.Empty;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 清空输入命令
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        Content = string.Empty;
    }

    /// <summary>
    /// 请求关闭对话框的事件
    /// </summary>
    public event EventHandler? RequestClose;
}
