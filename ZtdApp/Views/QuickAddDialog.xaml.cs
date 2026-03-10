using System.Windows;
using System.Windows.Input;
using ZtdApp.ViewModels;

namespace ZtdApp.Views;

/// <summary>
/// 快速添加想法对话框
/// </summary>
public partial class QuickAddDialog : Window
{
    private readonly QuickAddViewModel _viewModel;

    public QuickAddDialog(QuickAddViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // 订阅关闭请求事件
        _viewModel.RequestClose += (s, e) => Close();

        // 记录日志
        System.Diagnostics.Debug.WriteLine("QuickAddDialog 已初始化");
    }

    /// <summary>
    /// 设置窗口位置：屏幕下 1/3 处
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // 获取屏幕尺寸
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;

        // 设置位置：水平居中，垂直方向在屏幕下 1/3 处（66% 位置）
        Left = (screenWidth - ActualWidth) / 2;
        Top = screenHeight * 0.66;

        // 聚焦输入框
        ContentTextBox.Focus();
    }

    /// <summary>
    /// 键盘事件处理
    /// </summary>
    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        // Esc 关闭窗口
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
        }
        // Tab 清空内容
        else if (e.Key == Key.Tab)
        {
            ContentTextBox.Text = string.Empty;
            _viewModel.Content = string.Empty;
            e.Handled = true;
        }
    }

    /// <summary>
    /// 文本框键盘事件处理
    /// </summary>
    private void ContentTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        // Enter 添加想法（不包含 Shift+Enter）
        if (e.Key == Key.Enter &&
            !e.KeyboardDevice.IsKeyDown(Key.LeftShift) &&
            !e.KeyboardDevice.IsKeyDown(Key.RightShift))
        {
            _viewModel.AddIdeaCommand.Execute(null);
            e.Handled = true;
        }
    }

    /// <summary>
    /// 文本变化时控制 placeholder 显示
    /// </summary>
    private void ContentTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        PlaceholderText.Visibility = string.IsNullOrEmpty(ContentTextBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// 快速添加对话框关闭后的处理
    /// </summary>
    private void OnQuickAddDialogClosed(object? sender, EventArgs e)
    {
        // 从 MainWindow 移除事件监听
        // 这里不需要做特殊处理，因为每次都是新的 ViewModel 实例
    }

    /// <summary>
    /// 静态方法：显示对话框
    /// </summary>
    public static void ShowDialog(QuickAddViewModel viewModel)
    {
        var dialog = new QuickAddDialog(viewModel);
        dialog.ShowDialog();
    }
}
