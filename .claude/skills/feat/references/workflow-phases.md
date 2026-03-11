# 各阶段详细操作指南

4 阶段完整操作流程。每个阶段的独特内容在此，通用定义（分级标准、GWT 模板、原则）见 SKILL.md。

---

## 阶段 0：进度恢复

**检查**：读取 `.claude/feat-progress.json`

- 存在未完成 → 提示恢复（显示功能名、状态、测试意图、最后记录）
- 不存在 → 读取 PRD.md，显示功能清单，用户选择

详见 [progress-management.md](progress-management.md)

---

## 阶段 1：Design

### 步骤 1.1：Explore - 读现有代码

**必须先理解再设计**。读取与功能相关的现有文件：
- 相关的 ViewModel、Model、Service、Repository
- 现有页面 XAML 的布局模式
- BrandColors.xaml 中可复用的样式

### 步骤 1.2：设计 UI + 复杂度评估

```markdown
🎨 布局设计: [功能名称]

布局: [垂直/水平/网格]
组件: [Button x2, TextBox x1, Card x1]
复用样式: [CardBorder, PageTitleTextBlock, ...]

草图:
┌─────────────────────────┐
│  [标题]                 │
│  [内容区域]             │
│  [操作按钮]             │
└─────────────────────────┘

🧪 复杂度评估:
- 级别: [CRITICAL / NON-CRITICAL]
- 理由: [为什么这样分类]
```

### 步骤 1.3：测试意图（用户确认）

使用 SKILL.md 中的 Given-When-Then 模板生成测试意图草稿，等待用户确认。

**AI 角色限制**：
- ✅ 生成测试意图草稿供用户审查
- ✅ 根据确认的意图生成测试代码
- ❌ 自己修改测试意图
- ❌ 跳过用户确认直接实现

### 步骤 1.4：生成测试代码（TDD 红灯）

用户确认测试意图后，**立即**生成测试代码：

```csharp
// 测试方法命名: Test_[场景名]
[Fact]
public void Test_场景名()
{
    // Given: [中文注释，来自测试意图]
    // When: [中文注释]
    // Then: [中文注释]
}
```

**运行测试**：
```bash
dotnet test ZtdApp.Tests --filter "FullyQualifiedName~[测试类名]"
```

此时应全部失败 🔴（代码未实现）。这是正确的 TDD 红灯状态。

### 步骤 1.5：保存进度

```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "design",
    "level": "CRITICAL",
    "testIntent": [...],
    "testsCreated": ["XxxTests.cs"]
  }
}
```

---

## 阶段 2：Build

### 步骤 2.1：按需调研

**不强制调研**。仅在以下情况搜索：
- 使用不熟悉的 API 或框架特性
- 尝试 2 次仍未解决的问题
- 需要参考其他项目的实现模式

调研时告知用户参考来源：
```markdown
实现方案参考:
- GitHub: [项目链接]
- 官方文档: [文档链接]
```

### 步骤 2.2：编码实现

**参考测试意图逐步实现**：
- 意图1 → 实现对应方法
- 意图2 → 实现对应方法
- 目标：让所有测试通过

**UI 实现规则**：
- 必须使用 BrandColors.xaml 共享样式
- 禁止内联 FontSize/Padding/Color
- 新样式先在 BrandColors.xaml 定义再引用
- 按钮高度控制：Margin + Padding + FontSize + FontWeight + VerticalContentAlignment，禁止固定 Height

### 步骤 2.3：保存进度

更新 status 为 `build`，记录已创建的文件列表。

---

## 阶段 3：Verify

### 步骤 3.1：编译检查

```bash
dotnet build ZtdApp --configuration Release
```

编译失败 → 返回 Build 修复。

### 步骤 3.2：运行自动测试（TDD 绿灯）

```bash
dotnet test ZtdApp.Tests --filter "FullyQualifiedName~[测试类名]"
```

此时应全部通过 🟢。失败 → 返回 Build 修复。

### 步骤 3.3：人工验收

测试全部通过后，提示用户运行应用进行验收：

```markdown
✅ 自动测试全部通过

请运行应用验收：
ZtdApp/bin/Release/net10.0-windows/win-x64/ZtdApp.exe

验收清单（基于测试意图）：
- [ ] 场景1: [描述]
- [ ] 场景2: [描述]
- [ ] 场景3: [描述]

请确认验收结果。
```

等待用户回复验收结果。不通过 → 返回 Build 修复。

### 步骤 3.4：UI 一致性检查

使用 [design-guide.md](../design-guide.md) 中的 UI 一致性检查清单。

### 问题处理

- **阻塞性问题**：返回 Build 修复，修复后重新 Verify
- **非阻塞性问题**：记录到 `.claude/known-issues.json`，继续 Commit

---

## 阶段 4：Commit

### 步骤 4.1：更新 PRD.md

```markdown
- [x] 功能名称 ✅ commit: [hash]
```

### 步骤 4.2：提交代码

```bash
git add <具体文件列表>
git commit -m "feat: [功能名称]"
```

### 步骤 4.3：推送到远程

```bash
git push origin <当前分支>
```

### 步骤 4.4：记录经验（Learn）

如果开发过程中遇到了坑或发现了新的模式：
- 记录到 `.claude/known-issues.json`（技术问题）
- 更新项目记忆（反复出现的模式）

### 步骤 4.5：清理进度

```bash
rm .claude/feat-progress.json
```

---

## 测试代码规范

### 测试类组织

```csharp
// 文件: ZtdApp.Tests/[功能名]Tests.cs
public class [功能名]Tests
{
    [Fact]
    public void Test_场景1() { /* Given-When-Then */ }

    [Fact]
    public void Test_场景2() { /* Given-When-Then */ }
}
```

### 数据库测试

使用 `SharedMemoryDatabase` 变体（SQLite 内存模式），每个测试文件独立命名。详见 [../project-config.md](../project-config.md) 测试约定。

### UI 测试

- 需 `[Collection("UITests")]` 防止并行
- 需先 Release 构建
- 使用 FlaUI.UIA3 交互

### 测试深度参考

| 级别 | 场景数量 | 覆盖范围 |
|------|---------|---------|
| CRITICAL | 5-8 个场景 | 核心路径 + 边界条件 + 异常处理 |
| NON-CRITICAL | 2-4 个场景 | 核心路径 + 关键边界 |
