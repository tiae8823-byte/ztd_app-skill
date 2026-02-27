using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 初始化数据库
        var dbService = Services.GetRequiredService<DatabaseService>();
        dbService.Initialize();

        // 创建主窗口
        var viewModel = Services.GetRequiredService<IdeaViewModel>();
        var mainWindow = new MainWindow(viewModel);

        // 设置窗口属性
        mainWindow.Title = "ZTD - 想法收集";
        mainWindow.Width = 500;
        mainWindow.Height = 700;
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        mainWindow.Show();
    }
}
