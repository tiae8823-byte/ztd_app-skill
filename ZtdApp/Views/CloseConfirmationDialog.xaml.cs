using System.Windows;

namespace ZtdApp.Views;

/// <summary>
/// 关闭确认对话框，询问用户是最小化到托盘还是直接退出
/// </summary>
public partial class CloseConfirmationDialog : Window
{
    /// <summary>
    /// 用户选择的结果
    /// </summary>
    public enum CloseAction
    {
        MinimizeToTray,
        ExitApplication
    }

    /// <summary>
    /// 用户选择的行为
    /// </summary>
    public CloseAction SelectedAction { get; private set; }

    /// <summary>
    /// 是否记住选择
    /// </summary>
    public bool RememberChoice => RememberChoiceCheckBox?.IsChecked ?? false;

    public CloseConfirmationDialog()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 最小化到托盘按钮点击
    /// </summary>
    private void MinimizeToTrayButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = CloseAction.MinimizeToTray;
        DialogResult = true;
        Close();
    }

    /// <summary>
    /// 直接退出按钮点击
    /// </summary>
    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = CloseAction.ExitApplication;
        DialogResult = true;
        Close();
    }
}
