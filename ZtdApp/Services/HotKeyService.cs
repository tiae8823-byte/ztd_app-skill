using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ZtdApp.Services;

/// <summary>
/// 全局热键服务，封装 Win32 RegisterHotKey API
/// </summary>
public class HotKeyService : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;
    private const uint MOD_NOREPEAT = 0x4000;

    private readonly Dictionary<int, Action> _hotKeyActions = new();
    private IntPtr _windowHandle;
    private HwndSource? _source;
    private int _hotKeyId = 9000;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// 初始化热键服务，关联到指定窗口
    /// </summary>
    public void Initialize(Window window)
    {
        _windowHandle = new WindowInteropHelper(window).Handle;
        _source = HwndSource.FromHwnd(_windowHandle);
        _source?.AddHook(HwndHook);
    }

    /// <summary>
    /// 注册全局热键
    /// </summary>
    /// <param name="modifiers">修饰键（如 MOD_CONTROL | MOD_ALT）</param>
    /// <param name="key">虚拟键码</param>
    /// <param name="action">触发动作</param>
    /// <returns>热键ID，失败返回-1</returns>
    public int RegisterHotKey(uint modifiers, uint key, Action action)
    {
        var id = _hotKeyId++;

        if (!RegisterHotKey(_windowHandle, id, modifiers | MOD_NOREPEAT, key))
        {
            return -1;
        }

        _hotKeyActions[id] = action;
        return id;
    }

    /// <summary>
    /// 注册 Ctrl+Alt+字母 热键
    /// </summary>
    public int RegisterCtrlAltHotKey(char letter, Action action)
    {
        var keyCode = (uint)(letter >= 'a' && letter <= 'z' ? letter - 'a' + 'A' : letter);
        return RegisterHotKey(MOD_CONTROL | MOD_ALT, keyCode, action);
    }

    /// <summary>
    /// 注销指定热键
    /// </summary>
    public void UnregisterHotKey(int id)
    {
        if (_hotKeyActions.ContainsKey(id))
        {
            UnregisterHotKey(_windowHandle, id);
            _hotKeyActions.Remove(id);
        }
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY)
        {
            var id = wParam.ToInt32();
            if (_hotKeyActions.TryGetValue(id, out var action))
            {
                action?.Invoke();
                handled = true;
            }
        }

        return IntPtr.Zero;
    }

    public void Dispose()
    {
        foreach (var id in _hotKeyActions.Keys.ToList())
        {
            UnregisterHotKey(id);
        }

        _source?.RemoveHook(HwndHook);
        _source?.Dispose();
    }
}
