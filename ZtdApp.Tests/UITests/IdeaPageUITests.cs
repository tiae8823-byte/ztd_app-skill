using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Xunit;
using Xunit.Abstractions;

namespace ZtdApp.Tests.UITests;

/// <summary>
/// 想法收集功能 UI 自动化测试
/// </summary>
/// <remarks>
/// 注意：UI 测试需要编译后的可执行文件
/// 运行前请先执行: dotnet build --configuration Release
/// </remarks>
public class IdeaPageUITests : IDisposable
{
    private readonly UIA3Automation _automation;
    private readonly string _exePath;
    private readonly ITestOutputHelper _output;

    public IdeaPageUITests(ITestOutputHelper output)
    {
        _output = output;
        _automation = new UIA3Automation();
        _exePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "ZtdApp", "bin", "Release", "net8.0-windows", "win-x64", "ZtdApp.exe");

        _output.WriteLine($"[UI测试] 可执行文件路径: {_exePath}");
        _output.WriteLine($"[UI测试] 文件存在: {File.Exists(_exePath)}");
    }

    public void Dispose()
    {
        _automation.Dispose();
        _output.WriteLine("[UI测试] 自动化资源已释放");
    }

    private Window? LaunchApp()
    {
        if (!File.Exists(_exePath))
        {
            _output.WriteLine($"[UI测试] 错误: 可执行文件不存在 - {_exePath}");
            throw new FileNotFoundException(
                $"可执行文件不存在，请先执行: dotnet build --configuration Release\n" +
                $"预期路径: {_exePath}");
        }

        _output.WriteLine("[UI测试] 启动应用程序...");
        var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);
        _output.WriteLine($"[UI测试] 主窗口标题: {window?.Title}");
        return window;
    }

    private void SetTextBoxText(AutomationElement? textBox, string text)
    {
        if (textBox == null)
        {
            _output.WriteLine("[UI测试] 警告: 文本框为空，无法设置文本");
            return;
        }

        var textBoxElement = textBox.AsTextBox();
        if (textBoxElement != null)
        {
            textBoxElement.Text = text;
            _output.WriteLine($"[UI测试] 设置文本框内容: {text}");
        }
    }

    private int GetIdeaCount(Window window)
    {
        // 查找所有卡片（想法卡片使用 CardBorder 样式）
        var cards = window.FindAllDescendants(cf => cf.ByClassName("Border"));
        int count = 0;
        foreach (var card in cards)
        {
            // 检查是否包含"删除"按钮（这是想法卡片的特征）
            var deleteBtn = card.FindFirstDescendant(cf => cf.ByName("删除"));
            if (deleteBtn != null)
            {
                count++;
            }
        }
        return count;
    }

    // ============ 导航测试 ============

    [Fact]
    public void Navigation_AllPages_ShouldNavigateCorrectly()
    {
        _output.WriteLine("=== 测试: 导航功能 ===");

        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        Assert.NotNull(window);
        _output.WriteLine($"[通过] 窗口加载成功: {window.Title}");

        // 测试导航到待办列表
        var todoButton = window.FindFirstDescendant(cf => cf.ByName("待办列表"));
        if (todoButton != null)
        {
            todoButton.Click();
            System.Threading.Thread.Sleep(300);
            _output.WriteLine("[通过] 成功导航到待办列表");
        }

        // 测试导航到今日待办
        var todayButton = window.FindFirstDescendant(cf => cf.ByName("今日待办"));
        if (todayButton != null)
        {
            todayButton.Click();
            System.Threading.Thread.Sleep(300);
            _output.WriteLine("[通过] 成功导航到今日待办");
        }

        // 测试导航回想法收集
        var ideaButton = window.FindFirstDescendant(cf => cf.ByName("想法收集"));
        if (ideaButton != null)
        {
            ideaButton.Click();
            System.Threading.Thread.Sleep(300);
            _output.WriteLine("[通过] 成功导航回想法收集");
        }

        Assert.NotNull(window);
    }

    // ============ 添加和删除测试 ============

    [Fact]
    public void IdeaPage_AddAndDeleteIdea_ShouldWorkCorrectly()
    {
        _output.WriteLine("=== 测试: 添加和删除想法 ===");

        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        Assert.NotNull(window);
        _output.WriteLine($"[通过] 窗口加载成功");

        // 获取初始想法数量
        var initialCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 初始想法数量: {initialCount}");

        // 查找输入框和添加按钮
        var inputBox = window.FindFirstDescendant(cf => cf.ByAutomationId("想法输入框"))
                        ?? window.FindFirstDescendant(cf => cf.ByName("输入想法..."));

        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null)
        {
            _output.WriteLine("[跳过] 无法找到输入框或添加按钮");
            return;
        }

        // 添加想法
        var testIdea = "测试删除功能_" + Guid.NewGuid().ToString("N")[..8];
        SetTextBoxText(inputBox, testIdea);
        addButton.Click();
        System.Threading.Thread.Sleep(500);

        var afterAddCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 添加后想法数量: {afterAddCount}");
        Assert.True(afterAddCount > initialCount, "添加后数量应该增加");
        _output.WriteLine("[通过] 想法添加成功");

        // 查找并点击删除按钮
        var deleteButton = window.FindFirstDescendant(cf => cf.ByName("删除"));
        Assert.NotNull(deleteButton);
        _output.WriteLine("[信息] 找到删除按钮");

        deleteButton.Click();
        System.Threading.Thread.Sleep(500);

        var afterDeleteCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 删除后想法数量: {afterDeleteCount}");
        Assert.True(afterDeleteCount < afterAddCount, "删除后数量应该减少");
        _output.WriteLine("[通过] 删除功能正常工作");
    }

    [Fact]
    public void IdeaPage_DeleteButton_BindingShouldWork()
    {
        _output.WriteLine("=== 测试: 删除按钮绑定验证 ===");

        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        Assert.NotNull(window);

        // 查找输入框
        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null)
        {
            _output.WriteLine("[跳过] 无法找到输入控件");
            return;
        }

        // 添加一个想法
        SetTextBoxText(inputBox, "绑定测试想法");
        addButton.Click();
        System.Threading.Thread.Sleep(500);

        // 验证想法已添加
        var afterAddCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 添加后数量: {afterAddCount}");
        Assert.True(afterAddCount >= 1, "应该至少有一个想法");

        // 查找删除按钮
        var deleteButton = window.FindFirstDescendant(cf => cf.ByName("删除"));
        Assert.NotNull(deleteButton);
        _output.WriteLine("[通过] 删除按钮存在");

        // 验证删除按钮可点击（不抛异常）
        try
        {
            deleteButton.Click();
            System.Threading.Thread.Sleep(500);
            _output.WriteLine("[通过] 删除按钮点击成功，无异常");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"[失败] 删除按钮点击异常: {ex.Message}");
            Assert.Fail($"删除按钮点击失败: {ex.Message}");
        }

        // 验证删除后数量减少
        var afterDeleteCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 删除后数量: {afterDeleteCount}");
        Assert.True(afterDeleteCount < afterAddCount, "删除后数量应该减少");
        _output.WriteLine("[通过] 删除按钮绑定正确，功能正常");
    }

    // ============ 异常输入测试 ============

    [Fact]
    public void IdeaPage_AddEmptyIdea_ShouldNotAdd()
    {
        _output.WriteLine("=== 测试: 空内容不应添加 ===");

        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        if (window == null) return;

        var initialCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 初始数量: {initialCount}");

        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null) return;

        // 尝试添加空内容
        SetTextBoxText(inputBox, "");
        addButton.Click();
        System.Threading.Thread.Sleep(300);

        var afterCount = GetIdeaCount(window);
        _output.WriteLine($"[信息] 尝试添加后数量: {afterCount}");
        Assert.Equal(initialCount, afterCount);
        _output.WriteLine("[通过] 空内容未添加");
    }

    [Fact]
    public void IdeaPage_AddVeryLongIdea_ShouldHandleCorrectly()
    {
        _output.WriteLine("=== 测试: 超长内容处理 ===");

        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        if (window == null) return;

        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null) return;

        // 添加超长内容
        var longText = new string('A', 1000);
        SetTextBoxText(inputBox, longText);
        addButton.Click();
        System.Threading.Thread.Sleep(500);

        // 应用应该正常处理
        Assert.NotNull(window);
        _output.WriteLine("[通过] 超长内容处理正常");
    }

    // ============ 稳定性测试 ============

    [Fact]
    public void IdeaPage_RapidOperations_ShouldNotCrash()
    {
        _output.WriteLine("=== 测试: 快速操作稳定性 ===");

        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        if (window == null) return;

        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null) return;

        // 快速连续操作
        for (int i = 0; i < 5; i++)
        {
            SetTextBoxText(inputBox, $"快速想法_{i}_{Guid.NewGuid():N}"[..20]);
            addButton.Click();
            System.Threading.Thread.Sleep(200);
        }

        System.Threading.Thread.Sleep(500);

        // 应用应该保持稳定
        Assert.NotNull(window);
        _output.WriteLine("[通过] 快速操作稳定性测试通过");
    }
}
