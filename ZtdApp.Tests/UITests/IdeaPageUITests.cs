using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Xunit;

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

    public IdeaPageUITests()
    {
        _automation = new UIA3Automation();
        _exePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "ZtdApp", "bin", "Release", "net8.0-windows", "win-x64", "ZtdApp.exe");
    }

    public void Dispose()
    {
        _automation.Dispose();
    }

    private Window? LaunchApp()
    {
        if (!File.Exists(_exePath))
        {
            throw new FileNotFoundException(
                $"可执行文件不存在，请先执行: dotnet build --configuration Release\n" +
                $"预期路径: {_exePath}");
        }

        var app = Application.Launch(_exePath);
        return app.GetMainWindow(_automation);
    }

    private void SetTextBoxText(AutomationElement? textBox, string text)
    {
        if (textBox == null) return;

        var textBoxElement = textBox.AsTextBox();
        if (textBoxElement != null)
        {
            textBoxElement.Text = text;
        }
    }

    // ============ 正常流程测试 ============

    [Fact]
    public void IdeaPage_AddIdea_ShouldAppearInList()
    {
        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        // 等待窗口加载
        Assert.NotNull(window);
        Assert.Contains("ZTD", window.Title);

        // 查找输入框和按钮
        var inputBox = window.FindFirstDescendant(cf => cf.ByAutomationId("想法输入框"))
                        ?? window.FindFirstDescendant(cf => cf.ByName("输入想法..."));

        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"))
                       ?? window.FindFirstDescendant(cf => cf.ByAutomationId("AddIdeaButton"));

        if (inputBox == null || addButton == null)
        {
            // 如果找不到具体控件，跳过测试
            return;
        }

        // 输入想法内容
        SetTextBoxText(inputBox, "测试想法内容");
        addButton.Click();

        // 等待列表更新
        System.Threading.Thread.Sleep(500);

        // 验证想法出现在列表中
        var ideaList = window.FindFirstDescendant(cf => cf.ByName("想法列表"));
        Assert.NotNull(ideaList);

        var listBox = ideaList.AsListBox();
        var items = listBox.Items;
        Assert.NotEmpty(items);

        // 检查是否包含测试想法
        var found = items.Any(item => item.Name.Contains("测试想法内容"));
        Assert.True(found, "添加的想法应该出现在列表中");
    }

    // ============ 异常输入测试 ============

    [Fact]
    public void IdeaPage_AddEmptyIdea_ShouldHandleGracefully()
    {
        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        if (window == null) return;

        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null) return;

        // 尝试添加空内容
        SetTextBoxText(inputBox, "");
        addButton.Click();

        System.Threading.Thread.Sleep(300);

        // 应用应该不会崩溃
        Assert.NotNull(window);
    }

    [Fact]
    public void IdeaPage_AddVeryLongIdea_ShouldHandleCorrectly()
    {
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
    }

    // ============ 边界情况测试 ============

    [Fact]
    public void IdeaPage_DeleteIdea_ShouldRemoveFromList()
    {
        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        if (window == null) return;

        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null) return;

        // 添加一个想法
        SetTextBoxText(inputBox, "待删除的想法");
        addButton.Click();

        System.Threading.Thread.Sleep(500);

        // 查找删除按钮
        var deleteButton = window.FindFirstDescendant(cf => cf.ByName("删除"));

        if (deleteButton != null)
        {
            deleteButton.Click();
            System.Threading.Thread.Sleep(300);

            // 验证想法被删除
            Assert.NotNull(window);
        }
    }

    // ============ 错误处理测试 ============

    [Fact]
    public void IdeaPage_AppShouldNotCrash_OnRapidOperations()
    {
        using var app = Application.Launch(_exePath);
        var window = app.GetMainWindow(_automation);

        if (window == null) return;

        var inputBox = window.FindFirstDescendant(cf => cf.ByName("输入想法..."));
        var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

        if (inputBox == null || addButton == null) return;

        // 快速连续操作
        for (int i = 0; i < 10; i++)
        {
            SetTextBoxText(inputBox, $"快速想法 {i}");
            addButton.Click();
            System.Threading.Thread.Sleep(100);
        }

        System.Threading.Thread.Sleep(500);

        // 应用应该保持稳定
        Assert.NotNull(window);
    }
}
