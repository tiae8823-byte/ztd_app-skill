using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ZtdApp.Data;
using ZtdApp.Services;
using ZtdApp.ViewModels;
using ZtdApp.Views;

namespace ZtdApp;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // 数据层
        services.AddSingleton<DatabaseService>();
        services.AddScoped<IdeaRepository>();

        // 服务层
        services.AddScoped<IdeaManager>();

        // ViewModel 层
        services.AddScoped<IdeaViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // 初始化数据库
        var dbService = Services.GetRequiredService<DatabaseService>();
        dbService.Initialize();

        // 创建主窗口
        var window = new Window();
        var viewModel = Services.GetRequiredService<IdeaViewModel>();
        var ideaPage = new IdeaPage(viewModel);
        window.Content = ideaPage;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var windowId = WinRT.Interop.WindowId.CreateFromWin32(hwnd);

        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        // 设置窗口大小和位置
        appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 500, Height = 700 });
        appWindow.Move(new Windows.Graphics.PointInt32 { X = 100, Y = 100 });
        appWindow.Title = "ZTD - 想法收集";

        window.Activate();
    }
}
