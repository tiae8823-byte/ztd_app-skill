using System.ComponentModel;
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
    private readonly TrayService _trayService;
    private readonly AppConfigService _configService;
    private static bool _isQuickAddDialogOpen = false; // 防止快捷键堆叠

    public MainWindow(MainWindowViewModel viewModel, HotKeyService hotKeyService, IServiceProvider serviceProvider, TrayService trayService, AppConfigService configService)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        _hotKeyService = hotKeyService;
        _serviceProvider = serviceProvider;
        _trayService = trayService;
        _configService = configService;
        InitializeComponent();

        // 窗口加载时注册热键和托盘
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

        // 初始化托盘服务（传入配置服务）
        _trayService.Initialize(this, _hotKeyService, _configService);
    }

    /// <summary>
    /// 重写关闭事件，处理关闭确认对话框
    /// </summary>
    protected override void OnClosing(CancelEventArgs e)
    {
        // 如果托盘图标已经可见（已经在托盘模式），只隐藏主窗口，不退出应用
        if (_trayService.IsIconVisible)
        {
            e.Cancel = true;  // 取消关闭操作
            Hide();          // 只隐藏主窗口，保持托盘图标可见
            return;
        }

        // 获取保存的用户偏好
        var closeBehavior = _configService.GetCloseBehavior();

        // 如果用户已设置偏好，直接执行对应操作
        if (closeBehavior != AppConfigService.CloseBehavior.Ask)
        {
            e.Cancel = true;
            switch (closeBehavior)
            {
                case AppConfigService.CloseBehavior.MinimizeToTray:
                    _trayService.HideToTray();
                    break;
                case AppConfigService.CloseBehavior.ExitApplication:
                    e.Cancel = false;
                    _trayService.Dispose();
                    base.OnClosing(e);
                    break;
            }
            return;
        }

        // 取消默认关闭行为
        e.Cancel = true;

        // 显示关闭确认对话框
        var dialog = new CloseConfirmationDialog
        {
            Owner = this
        };

        var result = dialog.ShowDialog();

        if (result == true)
        {
            // 如果用户勾选了"记住选择"，保存偏好
            if (dialog.RememberChoice)
            {
                var behavior = dialog.SelectedAction switch
                {
                    CloseConfirmationDialog.CloseAction.MinimizeToTray => AppConfigService.CloseBehavior.MinimizeToTray,
                    CloseConfirmationDialog.CloseAction.ExitApplication => AppConfigService.CloseBehavior.ExitApplication,
                    _ => AppConfigService.CloseBehavior.Ask
                };
                _configService.SetCloseBehavior(behavior);
            }

            switch (dialog.SelectedAction)
            {
                case CloseConfirmationDialog.CloseAction.MinimizeToTray:
                    // 最小化到托盘
                    _trayService.HideToTray();
                    break;

                case CloseConfirmationDialog.CloseAction.ExitApplication:
                    // 完全退出应用
                    e.Cancel = false; // 允许关闭
                    _trayService.Dispose();
                    base.OnClosing(e);
                    break;
            }
        }
        // 如果对话框被取消（result == false），则不做任何事，保持窗口打开
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
        // 检查对话框是否已在显示，防止堆叠
        if (_isQuickAddDialogOpen)
        {
            System.Diagnostics.Debug.WriteLine("快速添加对话框已在显示，忽略快捷键");
            return;
        }

        // 在主线程上执行
        Dispatcher.Invoke(() =>
        {
            // 标记对话框为已打开
            _isQuickAddDialogOpen = true;

            try
            {
                // 创建新的 ViewModel 实例（因为对话框关闭后 ViewModel 可能被清理）
                var quickAddVm = _serviceProvider.GetRequiredService<QuickAddViewModel>();

                // 监听对话框关闭事件
                quickAddVm.RequestClose += OnQuickAddDialogClosed;

                QuickAddDialog.ShowDialog(quickAddVm);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"显示快速添加对话框时出错: {ex.Message}");
                _isQuickAddDialogOpen = false;
            }
        });
    }

    /// <summary>
    /// 快速添加对话框关闭后的处理
    /// </summary>
    private void OnQuickAddDialogClosed(object? sender, EventArgs e)
    {
        _isQuickAddDialogOpen = false;
        System.Diagnostics.Debug.WriteLine("快速添加对话框已关闭，可以响应下次快捷键");
    }

    private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
