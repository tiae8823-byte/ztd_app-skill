using System;
using System.IO;
using System.Text.Json;

namespace ZtdApp.Services;

/// <summary>
/// 应用配置服务，保存用户偏好设置
/// </summary>
public class AppConfigService
{
    private readonly string _configPath;
    private AppConfig? _config;

    public AppConfigService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "ZtdApp");
        Directory.CreateDirectory(appFolder);
        _configPath = Path.Combine(appFolder, "app-config.json");
    }

    /// <summary>
    /// 用户关闭行为偏好
    /// </summary>
    public enum CloseBehavior
    {
        Ask,           // 每次询问
        MinimizeToTray, // 最小化到托盘
        ExitApplication // 直接退出
    }

    /// <summary>
    /// 应用配置
    /// </summary>
    public class AppConfig
    {
        public CloseBehavior CloseBehavior { get; set; } = CloseBehavior.Ask;
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    public AppConfig Load()
    {
        if (_config != null)
            return _config;

        if (!File.Exists(_configPath))
        {
            _config = new AppConfig();
            return _config;
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
        catch
        {
            _config = new AppConfig();
        }

        return _config;
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    public void Save(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_configPath, json);
        _config = config;
    }

    /// <summary>
    /// 清除关闭行为偏好（恢复为询问）
    /// </summary>
    public void ClearCloseBehavior()
    {
        var config = Load();
        config.CloseBehavior = CloseBehavior.Ask;
        Save(config);
    }

    /// <summary>
    /// 获取关闭行为
    /// </summary>
    public CloseBehavior GetCloseBehavior()
    {
        return Load().CloseBehavior;
    }

    /// <summary>
    /// 设置关闭行为
    /// </summary>
    public void SetCloseBehavior(CloseBehavior behavior)
    {
        var config = Load();
        config.CloseBehavior = behavior;
        Save(config);
    }
}
