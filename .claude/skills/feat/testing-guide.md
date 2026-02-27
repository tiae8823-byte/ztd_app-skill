# 测试策略参考

> 本文档提供详细的测试策略和方法，供 Test 和 Verify 阶段参考使用。

## 单元测试

**测试框架**：xUnit

**测试范围**：
- 核心逻辑的单元测试
- 边界条件测试
- 死循环防御测试（如递归调用限制、循环次数上限）
- 日志功能测试（验证日志正确写入、格式正确）
- 不追求高覆盖率，但要覆盖核心场景

---

## UI 自动化测试

**测试框架**：FlaUI.UIA3

**项目配置**：
```xml
<!-- ZtdApp.Tests/ZtdApp.Tests.csproj -->
<PackageReference Include="FlaUI.UIA3" Version="4.0.0" />
```

**测试要点**：
- 正常流程：功能按预期工作
- 异常输入：空值、超长文本、特殊字符
- 边界情况：零个、一个、最大数量
- 错误处理：数据库错误
- 数据持久化：重启后数据保留
- 界面响应：加载状态、错误提示

**UI 测试示例**：
```csharp
[Fact]
public void MainWindow_AddIdea_ShouldAppearInList()
{
    using var app = FlaUI.Application.Launch("bin/Release/net8.0-windows/ZtdApp.exe");
    using var automation = new UIA3Automation();

    var window = app.GetMainWindow(automation);
    var ideaInput = window.FindFirstDescendant(cf => cf.ByName("IdeaInput"));
    var addButton = window.FindFirstDescendant(cf => cf.ByName("添加想法"));

    ideaInput.Text = "测试想法";
    addButton.Click();

    var ideaList = window.FindFirstDescendant(cf => cf.ByAutomationId("PART_IdeasList"));
    Assert.Contains("测试想法", ideaList.AsListBox().Items.Select(i => i.Name));
}
```

---

## 集成测试（增量）

**测试策略**：每增加一个功能后，进行一次增量集成测试，验证新功能与已有功能的协作正常。

**集成测试要点**：

| 测试项 | 测试内容 | 预期结果 |
|--------|---------|---------|
| **1. 功能间数据流转** | 测试功能之间的数据传递<br>例如：想法→待办→今日待办 | 数据正确流转 |
| **2. 跨功能导航** | 在不同功能页面间切换操作 | 无崩溃，状态保持 |
| **3. 数据一致性** | 同一数据在不同地方显示一致 | 无矛盾显示 |
| **4. 数据库状态** | 验证所有 CRUD 操作正确持久化 | 重启后数据保留 |
| **5. 端到端流程** | 从想法添加到归档的完整路径 | 流程顺畅 |
| **6. 性能测试** | 100+ 条数据下的表现 | 响应流畅，无明显卡顿 |
| **7. 兼容性回归** | 确保新功能没破坏已有功能 | 已有功能正常工作 |
| **8. UI 一致性** | 新功能 UI 与已有页面风格一致 | 颜色、间距、圆角一致 |

**集成测试检查清单**：

```
- [ ] 启动程序，导航到所有功能页面
- [ ] 验证功能间的数据流转（想法→待办/笔记等）
- [ ] 测试跨功能操作（从想法页面跳转到其他功能）
- [ ] 添加大量数据（100+ 条）测试性能
- [ ] 关闭程序重启，验证数据持久化
- [ ] 验证已有功能未被破坏
- [ ] 完成端到端用户场景测试
- [ ] 检查新功能 UI 风格一致性
```

**集成测试流程**：
```
功能 N 完成
    ↓
Integration 增量集成测试（功能 N + 功能 1..N-1）
    ↓
测试通过 → Commit
测试失败 → 修复 → 重新测试 → Commit
```

---

## 手动测试清单

| 测试项 | 测试步骤 | 预期结果 |
|--------|---------|---------|
| **1. 启动测试** | 双击运行 `ZtdApp.exe` | 窗口正常显示，标题正确 |
| **2. 基础功能** | 执行功能主要操作 | 按预期工作 |
| **3. UI 样式** | 检查按钮、输入框等 | 符合品牌样式 |
| **4. 交互反馈** | 悬停、点击、聚焦 | 有正确的视觉反馈 |
| **5. 异常输入** | 空值、超长、特殊字符 | 正确处理 |

---

## Verify 阶段编译命令

```bash
# 编译生成可执行版本
dotnet build --configuration Release

# 运行单元测试
dotnet test

# 运行 UI 自动化测试
dotnet test --filter "FullyQualifiedName~UITests"

# 启动程序进行手动测试
ZtdApp\bin\Release\net8.0-windows\ZtdApp.exe
```
