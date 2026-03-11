using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ZtdApp.Data;
using ZtdApp.Services;
using ZtdApp.ViewModels;
using ZtdApp.ViewModels.Pages;
using ZtdApp.Views;
using Application = System.Windows.Application;

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
        services.AddSingleton<IdeaRepository>();
        services.AddSingleton<TaskRepository>();
        services.AddSingleton<NoteRepository>();
        services.AddSingleton<TomatoRepository>();

        // 服务层
        services.AddSingleton<IdeaManager>();
        services.AddSingleton<TaskManager>();
        services.AddSingleton<NoteManager>();
        services.AddSingleton<TomatoService>();
        services.AddSingleton<HotKeyService>();
        services.AddSingleton<TrayService>();
        services.AddSingleton<AppConfigService>();

        // ViewModel 层
        services.AddSingleton<IdeaViewModel>();
        services.AddSingleton<TodoViewModel>();
        services.AddSingleton<TodayViewModel>();
        services.AddSingleton<NotesViewModel>();
        services.AddSingleton<WeeklyReviewViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<QuickAddViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 初始化数据库
        var dbService = Services.GetRequiredService<DatabaseService>();
        dbService.Initialize();

        // 每次启动重置关闭行为偏好（确保显示关闭确认对话框）
        var configService = Services.GetRequiredService<AppConfigService>();
        configService.ClearCloseBehavior();

        // 创建主窗口
        var viewModel = Services.GetRequiredService<MainWindowViewModel>();
        var hotKeyService = Services.GetRequiredService<HotKeyService>();
        var trayService = Services.GetRequiredService<TrayService>();
        var mainWindow = new MainWindow(viewModel, hotKeyService, Services, trayService, configService);

        mainWindow.Show();
    }
}
