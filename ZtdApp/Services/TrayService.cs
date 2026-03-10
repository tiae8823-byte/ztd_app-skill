using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace ZtdApp.Services;

/// <summary>
/// 系统托盘服务，管理托盘图标和上下文菜单
/// </summary>
public class TrayService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    private Window? _mainWindow;
    private HotKeyService? _hotKeyService;
    private AppConfigService? _configService;
    private bool _isDisposed;

    /// <summary>
    /// 托盘图标是否可见
    /// </summary>
    public bool IsIconVisible => _notifyIcon?.Visible ?? false;

    /// <summary>
    /// 初始化托盘服务
    /// </summary>
    public void Initialize(Window mainWindow, HotKeyService hotKeyService, AppConfigService? configService = null)
    {
        _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        _hotKeyService = hotKeyService ?? throw new ArgumentNullException(nameof(hotKeyService));
        _configService = configService;

        CreateNotifyIcon();
    }

    private void CreateNotifyIcon()
    {
        // 创建上下文菜单
        var contextMenu = new ContextMenuStrip();

        var showItem = new ToolStripMenuItem("显示主窗口");
        showItem.Click += (s, e) => ShowMainWindow();

        var exitItem = new ToolStripMenuItem("退出");
        exitItem.Click += (s, e) => ShutdownApplication();

        contextMenu.Items.Add(showItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(exitItem);

        // 创建托盘图标
        _notifyIcon = new NotifyIcon
        {
            Icon = GetApplicationIcon(),
            Text = "ZTD App - 点击显示主窗口",
            Visible = false,
            ContextMenuStrip = contextMenu
        };

        // 双击显示主窗口
        _notifyIcon.DoubleClick += (s, e) => ShowMainWindow();
    }

    /// <summary>
    /// 显示托盘图标
    /// </summary>
    public void ShowIcon()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(1000, "ZTD App", "应用已最小化到托盘，快捷键仍可用", ToolTipIcon.Info);
        }
    }

    /// <summary>
    /// 隐藏托盘图标
    /// </summary>
    public void HideIcon()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
        }
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    public void ShowMainWindow()
    {
        if (_mainWindow == null) return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
            _mainWindow.Topmost = true;
            _mainWindow.Topmost = false;
        });
    }

    /// <summary>
    /// 隐藏主窗口到托盘
    /// </summary>
    public void HideToTray()
    {
        if (_mainWindow == null) return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            _mainWindow.Hide();
            ShowIcon();
        });
    }

    /// <summary>
    /// 完全退出应用
    /// </summary>
    public void ShutdownApplication()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // 清除记住的选择偏好
            _configService?.ClearCloseBehavior();

            // 清理托盘图标
            Dispose();

            // 关闭应用
            Application.Current.Shutdown();
        });
    }

    /// <summary>
    /// 获取应用程序图标
    /// </summary>
    private Icon GetApplicationIcon()
    {
        // 尝试使用应用程序图标
        try
        {
            // 使用系统默认的信息图标作为后备
            return SystemIcons.Application;
        }
        catch
        {
            return SystemIcons.Application;
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _notifyIcon?.Dispose();
        _notifyIcon = null;
        _isDisposed = true;
    }
}
