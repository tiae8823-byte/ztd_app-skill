using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ZtdApp.Services;
using ZtdApp.ViewModels;

namespace ZtdApp.Views;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; }
    private readonly HotKeyService _hotKeyService;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(MainWindowViewModel viewModel, HotKeyService hotKeyService, IServiceProvider serviceProvider)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        _hotKeyService = hotKeyService;
        _serviceProvider = serviceProvider;
        InitializeComponent();

        // 窗口加载时注册热键
        Loaded += MainWindow_Loaded;
        // 窗口关闭时清理热键
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 初始化热键服务
        _hotKeyService.Initialize(this);

        // 注册 Ctrl+Alt+I 全局热键
        var hotKeyId = _hotKeyService.RegisterCtrlAltHotKey('I', ShowQuickAddDialog);

        if (hotKeyId == -1)
        {
            System.Diagnostics.Debug.WriteLine("注册全局热键失败，可能已被其他程序占用");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"全局热键 Ctrl+Alt+I 注册成功，ID: {hotKeyId}");
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _hotKeyService.Dispose();
    }

    /// <summary>
    /// 显示快速添加对话框
    /// </summary>
    private void ShowQuickAddDialog()
    {
        // 在主线程上执行
        Dispatcher.Invoke(() =>
        {
            // 每次创建新的 ViewModel 实例（因为对话框关闭后 ViewModel 可能被清理）
            var quickAddVm = _serviceProvider.GetRequiredService<QuickAddViewModel>();
            QuickAddDialog.ShowDialog(quickAddVm);
        });
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // 根据 CurrentPage 类型决定执行哪个命令
            if (ViewModel.CurrentPage is ViewModels.Pages.IdeaViewModel ideaVM)
            {
                ideaVM.AddIdeaCommand.Execute(null);
            }
            else if (ViewModel.CurrentPage is ViewModels.Pages.NotesViewModel notesVM)
            {
                notesVM.AddNoteCommand.Execute(null);
            }
            e.Handled = true;
        }
    }
}
