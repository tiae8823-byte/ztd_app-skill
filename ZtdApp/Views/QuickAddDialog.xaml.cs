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

        // 窗口加载时聚焦输入框
        Loaded += (s, e) =>
        {
            ContentTextBox.Focus();
            ContentTextBox.SelectAll();
        };
    }

    /// <summary>
    /// 键盘事件处理
    /// </summary>
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Esc 关闭窗口
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
        }
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
