---
name: feat
description: Implement single feature using Design-Build-Test-Commit cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能

采用 **Design-Build-Test-Commit** 循环实现**单个**功能，确保 UI 风格一致。

## 核心理念

**小步快跑**：
- 一次只实现一个功能
- 每个功能完成后立即测试验证
- 验证通过后立即 commit
- PRD 是活的，可以随时加功能
- **进度自动保存**：随时中断，下次可以继续
- **UI 风格一致**：每个功能都遵循统一的设计规范

**Design-Build-Test-Commit 循环**：
```
UI设计(自动) → 写代码 → 写测试 → 编译+手动测试 → Commit
      ↓             ↓         ↓            ↓            ↓
    修改         修改      修改        修复        回滚
    ↓             ↓         ↓          ↓            ↓
  保存进度      保存进度  保存进度   保存进度   清除进度
    ↓             ↓         ↓          ↓            ↓
  feat-progress.json (各阶段自动更新)
```

> **说明**: Design 阶段由 AI 自动判断组件和样式，用户只需确认 Y/N

## 可用 MCP 工具

| MCP 工具 | 用途 |
|----------|------|
| `mcp__context7__resolve-library-id` + `mcp__context7__query-docs` | 查询框架 API 用法 |
| `mcp__github__search_code` | 搜索参考实现 |
| `mcp__sequential-thinking__sequentialthinking` | 复杂功能设计 |

## 设计规范参考（无需自己设计 UI）

### 推荐设计系统

| 设计系统 | 适用场景 | 参考链接 |
|---------|---------|---------|
| **Apple Human Interface Guidelines** | iOS/macOS 风格，简洁优雅 | https://developer.apple.com/design/human-interface-guidelines/ |
| **Google Material Design** | Android/Android 风格，色彩丰富 | https://m3.material.io/ |
| **Anthropic 品牌风格** | 当前项目已配置，直接使用 | `ZtdApp/Styles/BrandColors.xaml` |

### Anthropic 品牌样式（当前项目）

#### 颜色系统
| 资源键 | 颜色值 | 用途 |
|--------|--------|------|
| `BrandDark` | `#141413` | 主文本、深色背景 |
| `BrandLight` | `#faf9f5` | 浅色背景、深色文字 |
| `BrandMidGray` | `#b0aea5` | 次要文本、说明 |
| `BrandLightGray` | `#e8e6dc` | 卡片背景、边框 |
| `BrandOrange` | `#d97757` | 主按钮、主强调色 |
| `BrandBlue` | `#6a9bcc` | 次要按钮、信息提示 |
| `BrandGreen` | `#788c5d` | 成功状态、完成标记 |

#### 内置样式
| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `MainWindowStyle` | Window | 主窗口背景和前景色 |
| `Button` | Button | 主按钮（橙色，圆角） |
| `SecondaryButton` | Button | 次要按钮（蓝色） |
| `DeleteButton` | Button | 删除按钮（灰色，悬停变红） |
| `TextBox` | TextBox | 输入框（聚焦变橙色） |
| `TextBlock` | TextBlock | 文本基础样式 |
| `HeadingTextBlock` | TextBlock | 标题（24pt，粗体） |
| `SubheadingTextBlock` | TextBlock | 副标题（灰色） |
| `CardBorder` | Border | 卡片容器（圆角背景） |

#### 字号系统
| 资源键 | 大小 | 用途 |
|--------|------|------|
| `FontSizeSmall` | 12pt | 辅助文本、时间戳 |
| `FontSizeMedium` | 14pt | 正文、按钮文字 |
| `FontSizeLarge` | 18pt | 小标题 |
| `FontSizeTitle` | 24pt | 页面标题 |
| `FontSizeHeading` | 28pt | 大标题 |

### 通用设计原则（适用于所有风格）

#### 间距系统
| 元素 | 标准间距 | 紧凑间距 | 宽松间距 |
|------|---------|---------|---------|
| 组件内边距 | 12-16px | 8-10px | 20-24px |
| 组件间距 | 8-12px | 4-6px | 16-20px |
| 页面边距 | 20-24px | 12-16px | 32-40px |

#### 圆角系统
| 元素 | 圆角半径 |
|------|---------|
| 按钮 | 6-8px |
| 输入框 | 6-8px |
| 卡片 | 8-12px |
| 弹窗 | 12-16px |

#### 阴影系统
| 深度 | 用途 |
|------|------|
| 浅阴影 | 卡片、浮层 |
| 中阴影 | 下拉菜单 |
| 深阴影 | 模态弹窗 |

## 与 PRD 的联动

```
/feat 执行
  │
  ├─ 检查 feat-progress.json 是否存在且有未完成功能
  │   │
  │   ├─ 存在 → 提示恢复进度
  │   │         │
  │   │         ├─ Y: 继续开发（跳到对应阶段）
  │   │         └─ N: 重新开始（清除进度）
  │   │
  │   └─ 不存在 → 读取 PRD.md
  │               │
  │               ├── PRD 不存在 → 提示先运行 /prd
  │               │
  │               └── PRD 存在 → 显示功能清单
  │                            │
  │                            ▼
  │                       用户选择一个功能
  │                            │
  │                            ▼
  │                       Design 阶段 → Build → Test → Verify → Commit
  │                            │
  │                       ├─ Design → 保存进度 (status: build, 记录设计)
  │                       ├─ Build → 保存进度 (status: test, 记录文件)
  │                       ├─ Test → 保存进度 (status: verify, 记录测试)
  │                       ├─ Verify → 保存进度 (status: commit)
  │                       ├─ Commit → PostGitCommand Hook 记录版本
  │                       └─ 完成 → 清除进度 → 更新 PRD.md
  │
  └─ 询问是否继续下一个
```

## 执行流程

### 0. 检查进度恢复

```
检查 .claude/feat-progress.json 是否存在

存在且有未完成功能 → 提示恢复进度
```

**恢复提示示例**：
```markdown
📋 检测到未完成的功能：

当前功能: 想法收集
状态: verify（已完成: design, build, test）
设计方案: 垂直布局, Button, TextBox, CardBorder
最后记录: Test阶段完成，等待编译验证
创建时间: 2026-02-27 14:30

是否继续？
- Y: 继续开发
- N: 重新开始
```

---

### 1. 读取 PRD

```
读取 docs/PRD.md

失败 → "未找到 PRD.md，请先运行 /prd 创建需求文档"

成功 → 显示功能清单
```

### 2. 显示功能清单，让用户选择

```markdown
📋 当前功能清单：

MVP 功能：
1. [ ] 用户注册
2. [ ] 用户登录
3. [ ] 发布文章

已完成：
（暂无）

做哪个功能？（输入序号或名称）
```

### 3. Design 阶段 - UI 设计（自动化）

**Design 阶段由 AI 自动完成**，用户只需确认。

**AI 自动判断逻辑**：
- 根据 PRD 中的功能描述，判断需要哪些 UI 组件
- 自动从 `BrandColors.xaml` 选择对应样式
- 生成 UI 布局方案
- 展示给用户确认

**确认提示**：
```markdown
🎨 UI 设计: [功能名称]

自动生成的设计方案:

页面布局: [垂直/水平/网格布局]
使用组件: Button x2, TextBox x2, CardBorder
使用样式: Button (橙色), TextBox (聚焦橙色), CardBorder (圆角)

布局草图:
┌─────────────────────────┐
│  [标题]                 │
│                         │
│  [输入框]               │
│  [按钮]                 │
└─────────────────────────┘

是否符合预期？(Y/N，或提出修改意见)
```

**用户确认后，保存进度**：
```json
// .claude/feat-progress.json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "design",
    "designLayout": "垂直布局",
    "designComponents": ["Button", "TextBox", "CardBorder"],
    "designSketch": "[布局草图]",
    "lastCheckpoint": "UI 设计完成，等待用户确认"
  }
}
```

---

### 4. Build 阶段 - 编写代码

**Build 时必须遵循 Design 阶段确认的样式**：

**Build - 编写代码**：
- 按需创建文件
- 实现功能逻辑
- **实现前端 UI（使用 Design 阶段确认的样式）**

**Build 完成后，更新进度**：
```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "test",
    "createdAt": "2026-02-27T14:30:00Z",
    "filesCreated": ["Views/FeatureView.xaml", "ViewModels/FeatureViewModel.cs", ...],
    "lastCheckpoint": "Build阶段完成，已创建文件"
  }
}
```

---

### 5. Test 阶段 - 编写测试

**Test - 编写测试**：
- 核心逻辑的单元测试
- 边界条件测试
- 死循环防御测试（如递归调用限制、循环次数上限）
- 日志功能测试（验证日志正确写入、格式正确）
- 不追求高覆盖率，但要覆盖核心场景

**Test 完成后，更新进度**：
```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "verify",
    "createdAt": "2026-02-27T14:30:00Z",
    "filesCreated": ["Views/FeatureView.xaml", ...],
    "testsCreated": ["FeatureTests.cs"],
    "lastCheckpoint": "Test阶段完成，已编写测试用例"
  }
}
```

---

### 6. Verify 阶段 - 编译并测试

**Verify - 编译并测试**：
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

**UI 一致性检查**：
```markdown
✅ UI 一致性检查:
- [ ] 按钮颜色符合品牌规范（主按钮橙色，次要按钮蓝色）
- [ ] 输入框聚焦状态为橙色边框
- [ ] 卡片使用 CardBorder 样式
- [ ] 文本使用对应字体大小资源
- [ ] 整体风格与已有页面一致
```

---

### 集成测试（增量）

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

### 单元测试

**测试框架**：xUnit

**测试范围**：
- 核心逻辑的单元测试
- 边界条件测试
- 死循环防御测试
- 日志功能测试

---

### UI 自动化测试

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

### 手动测试清单

| 测试项 | 测试步骤 | 预期结果 |
|--------|---------|---------|
| **1. 启动测试** | 双击运行 `ZtdApp.exe` | 窗口正常显示，标题正确 |
| **2. 基础功能** | 执行功能主要操作 | 按预期工作 |
| **3. UI 样式** | 检查按钮、输入框等 | 符合品牌样式 |
| **4. 交互反馈** | 悬停、点击、聚焦 | 有正确的视觉反馈 |
| **5. 异常输入** | 空值、超长、特殊字符 | 正确处理 |

---

### 问题排查与解决方案搜索

**何时应该搜索**（判断标准）：

| 场景 | 是否搜索 | 理由 |
|------|---------|------|
| 简单的语法错误、拼写错误 | ❌ 不搜索 | 自己能快速定位和修复 |
| 明确的编译错误（缺少引用、类型不匹配） | ❌ 不搜索 | 错误信息已经很明确 |
| 逻辑错误（业务逻辑实现问题） | ❌ 不搜索 | 需要理解业务，搜索无用 |
| **尝试修复 2 次仍未解决的问题** | ✅ 搜索 | 可能是不熟悉的技术问题 |
| **框架/库的 Bug 或限制** | ✅ 搜索 | 社区可能已有解决方案 |
| **UI 渲染问题（对齐、裁剪、样式异常）** | ✅ 搜索 | 通常是框架特性或已知问题 |
| **性能问题、内存泄漏** | ✅ 搜索 | 需要专业的优化方案 |
| **平台特定问题（Windows/Linux/Mac）** | ✅ 搜索 | 需要了解平台特性 |
| **第三方库使用问题** | ✅ 搜索 | 官方文档或社区有答案 |

**核心原则**：
- 能自己快速解决的，不要搜索（浪费时间）
- 尝试 2 次仍未解决的，立即搜索（避免盲目试错）
- 涉及框架/库的问题，优先搜索（站在巨人肩膀上）

---

**搜索策略**（按优先级）：

**1. GitHub Issues 搜索**（最优先）
```
使用 WebSearch 搜索：
site:github.com [技术栈] [问题关键词]

示例：
- site:github.com WPF Button text clipped
- site:github.com dotnet build error CS0246
- site:github.com microsoft-ui-xaml padding issue
```
- ✅ 优势：真实案例、有解决方案、有讨论、可能是官方确认的 Bug
- 🎯 适用：框架 Bug、UI 问题、已知限制、最佳实践
- 💡 技巧：搜索项目的官方仓库（如 dotnet/wpf, microsoft/microsoft-ui-xaml）

**2. 官方文档查询**
```
使用 mcp__context7__query-docs 查询：
- 框架 API 用法
- 配置选项说明
- 官方推荐方案
```
- ✅ 优势：权威、准确、最新
- 🎯 适用：API 使用、配置问题、功能说明
- 💡 技巧：先用 resolve-library-id 确定库 ID，再 query-docs

**3. Stack Overflow 搜索**
```
使用 WebSearch 搜索：
site:stackoverflow.com [问题关键词]

示例：
- site:stackoverflow.com WPF Button content vertical alignment
- site:stackoverflow.com C# async await deadlock
```
- ✅ 优势：问答形式、多种方案对比、有投票排序
- 🎯 适用：通用编程问题、算法实现、设计模式
- 💡 技巧：关注高赞回答和被采纳的答案

**4. 技术博客/文章**
```
使用 WebSearch 搜索：
[问题关键词] solution fix workaround

示例：
- WPF text clipping solution
- dotnet memory leak fix
```
- ✅ 优势：详细教程、实战经验、深度分析
- 🎯 适用：复杂问题、性能优化、架构设计
- 💡 技巧：优先选择近期文章（技术更新快）

---

**搜索关键词技巧**：

| 技巧 | 示例 | 说明 |
|------|------|------|
| 包含技术栈 | `WPF Button`, `dotnet build` | 缩小搜索范围 |
| 使用英文 | `text clipped` 而不是 `文字被遮挡` | 搜索结果更丰富 |
| 包含错误代码 | `CS0246`, `NullReferenceException` | 精确定位问题 |
| 添加动作词 | `fix`, `solution`, `workaround`, `resolve` | 找到解决方案 |
| 使用引号 | `"Button content clipped"` | 精确匹配短语 |
| 排除无关词 | `-android -ios` | 排除其他平台 |

---

**搜索后的处理流程**：

```
1. 阅读多个解决方案（至少 3 个）
   ↓
2. 对比方案的优缺点
   - 是否适用当前场景？
   - 是否有副作用？
   - 社区反馈如何？
   ↓
3. 优先选择
   - 官方推荐方案
   - 高赞/被采纳的方案
   - 最新的方案（技术更新快）
   ↓
4. 理解原理后再应用
   - 不要盲目复制代码
   - 理解为什么这样能解决问题
   - 考虑是否需要调整
   ↓
5. 应用并测试验证
   ↓
6. 记录到 known-issues.json
   - 如果是框架已知问题
   - 记录问题、解决方案、参考链接
```

---

**示例：本次 UI 问题的完整流程**

```
问题：WPF 按钮文字底部被遮挡
  ↓
第 1 次尝试：增加 Height (36 → 40)
  ↓ 失败
第 2 次尝试：增加 Padding (8,6 → 10,10)
  ↓ 更糟糕（遮挡更严重）
  ↓
触发搜索条件：尝试 2 次仍未解决
  ↓
GitHub 搜索：site:github.com WPF Button text clipped
  ↓
找到相关 Issue：microsoft/microsoft-ui-xaml#9543
  - 标题：Button cuts the content at some scaling levels
  - 状态：Open (官方确认的 Bug)
  - 解决方案：设置 Padding="0"
  - 原理：DPI 缩放时 Padding 计算有问题
  ↓
应用方案：Padding="0" + UseLayoutRounding="True"
  ↓
测试验证 → 成功！
  ↓
更新 known-issues.json：
{
  "id": "ui-001",
  "solution": "设置 Padding='0'",
  "reference": "https://github.com/microsoft/microsoft-ui-xaml/issues/9543"
}
  ↓
提交代码
```

---

**注意事项**：

- ⚠️ 不要过度依赖搜索：简单问题自己解决更快
- ⚠️ 不要盲目复制代码：理解原理后再应用
- ⚠️ 不要只看一个方案：对比多个方案选最优
- ⚠️ 注意方案的时效性：优先选择最新的方案
- ⚠️ 记录解决方案：避免下次遇到同样问题

---

### 问题处理流程

**触发时机**：手动测试后发现问题

**问题分类**：

| 问题类型 | 定义 | 处理建议 |
|---------|------|---------|
| **阻塞性问题** | 功能不可用、数据丢失、崩溃 | 必须修复 |
| **非阻塞性问题** | UI 细节、性能优化、视觉不完美 | 可选择处理方式 |

**处理选项**：

```markdown
发现问题后，提供三个选项：

1. **继续修复**
   - 返回 Build 阶段重新修改
   - 适用于：可快速解决的问题

2. **标记为已知问题**
   - 记录到 PRD.md 的"已知问题"章节
   - 继续 Commit，后续迭代解决
   - 适用于：非阻塞性问题、需要更多时间研究的问题

3. **直接提交**
   - 忽略问题，继续 Commit
   - 适用于：极小的视觉瑕疵、不影响使用的细节
```

**已知问题记录格式**（PRD.md）：

```markdown
## 已知问题

### UI 优化
- [ ] 想法操作按钮高度对齐问题（视觉细节，不影响功能）
  - 影响：快速完成按钮高度略有不一致
  - 优先级：低
  - 计划：后续 UI 优化时统一处理

### 性能优化
- [ ] 大量数据加载性能（>1000条时略有卡顿）
  - 影响：数据量大时响应稍慢
  - 优先级：中
  - 计划：实现虚拟化列表
```

**提示用户示例**：

```markdown
⚠️ 发现问题：操作按钮高度略有不一致

问题类型：非阻塞性（UI 细节）
影响范围：视觉效果，不影响功能使用

处理选项：
1. 继续修复 - 尝试其他解决方案
2. 标记为已知问题 - 记录到 PRD.md，后续优化
3. 直接提交 - 忽略此问题

你的选择？(输入 1/2/3)
```

---

**Commit - 提交代码**：
```bash
git add .
git commit -m "feat: [功能名称]"
```

> 提交后，PostGitCommand Hook 会自动记录版本信息到 `.claude/version-history.log`

**Commit 完成后，清除进度**：
```bash
# 删除进度文件
rm .claude/feat-progress.json
```

**Compact - 清理上下文**：
```
/compact  清理历史消息，为下一个功能准备干净的上下文
```

### 7. 更新 PRD.md

标记功能完成：

```markdown
### MVP 功能（必须实现）

- [ ] 用户注册
- [x] 用户登录 ✅ commit: a1b2c3d
- [ ] 发布文章
```

### 8. 询问下一步

```
✅ [功能名称] 完成

版本信息已记录到: .claude/version-history.log

UI 风格: 使用 Anthropic 品牌样式
设计组件: Button, TextBox, CardBorder

建议运行 /compact 清理上下文，为下一个功能准备干净的环境

继续下一个功能？
- 输入序号继续
- 或告诉我新需求
- 或说"结束"退出
```

## 过程中发现新需求

用户可以随时提出新功能：

```
用户: 等等，文章还得能置顶

AI: 好，加到 PRD 里：
    - [ ] 文章置顶

    先做置顶还是继续当前功能？
```

更新 PRD.md：
```markdown
### MVP 功能（必须实现）

- [ ] 用户注册
- [ ] 用户登录
- [ ] 发布文章
- [ ] 文章置顶 ← 新增
```

## 输出格式

```markdown
🎨 实现功能: [功能名称]

## Design
✅ 布局: [垂直/水平/网格]
✅ 组件: [Button x2, TextBox x1, CardBorder]
✅ 样式: [BrandColors.xaml 自动应用]

## Build
✅ 创建: [文件列表]

## Test
✅ 测试: [数量] 个用例

## Verify
✅ 编译成功: dotnet build --configuration Release
✅ 单元测试通过: dotnet test
✅ UI 一致性检查: 通过
✅ 手动测试完成: 功能验证正常

## Commit
✅ feat: [功能名称]
✅ 版本记录: .claude/version-history.log

## 更新 PRD
✅ 已标记完成

---

建议运行 /compact 清理上下文

继续下一个？(输入序号 / 新需求 / 结束)
```

## 对话示例

### 正常流程

```
用户: /feat

AI: 📋 当前功能清单：

    MVP 功能：
    1. [ ] 用户注册
    2. [ ] 用户登录
    3. [ ] 发布文章

    做哪个？

用户: 2

AI: 🎨 UI 设计: 用户登录

    自动生成的设计方案:

    页面布局: 垂直布局
    使用组件: Button x1, TextBox x2, HeadingTextBlock
    使用样式: Button (橙色主按钮), TextBox (聚焦橙色)

    布局草图:
    ┌─────────────────────────┐
    │  用户登录               │
    │  请输入你的账号          │
    │                         │
    │  [用户名输入框]         │
    │  [密码输入框]           │
    │                         │
    │  [登录按钮]             │
    └─────────────────────────┘

    是否符合预期？(Y/N)

用户: 文章得能删除

AI: 好，加到 PRD:
    - [ ] 删除文章

    先做删除还是继续注册？

用户: 先做删除

AI: 🎨 UI 设计: 删除文章
    自动生成设计方案...

    页面布局: 独立确认对话框
    使用组件: Button x2 (确认/取消), TextBlock
    使用样式: Button (橙色), SecondaryButton (蓝色)

    是否符合预期？(Y/N)

用户: Y

AI: [Build → Test → Verify → Commit → Compact]

    ✅ 删除文章 完成

    设计方案: 独立对话框, Button x2, TextBlock
    编译: dotnet build --configuration Release
    测试: dotnet test
    UI 一致性: 通过
    手动测试: 功能验证正常
    Commit: feat: 删除文章
```

### 进度恢复流程

```
用户: /feat

AI: 📋 检测到未完成的功能：

    当前功能: 想法收集
    状态: verify（已完成: design, build, test）
    最后记录: Test阶段完成，等待编译验证
    设计方案: 垂直布局, Button, TextBox, CardBorder
    创建时间: 2026-02-27 14:30

    是否继续？
    - Y: 继续开发
    - N: 重新开始

用户: Y

AI: 🔨 继续实现: 想法收集

    当前进度: verify（已完成设计、代码、测试）
    开始编译验证...

[继续 Verify → Commit 流程]
```

## 进度维护

### 进度文件位置

`.claude/feat-progress.json` - 功能开发进度文件

### 进度文件格式

```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "design",              // design | build | test | verify | commit | completed
    "createdAt": "2026-02-27T14:30:00Z",
    "description": "开始实现功能",
    "designLayout": "垂直布局",
    "designComponents": ["Button", "TextBox", "CardBorder"],
    "designSketch": "[简单的ASCII布局图]",
    "filesCreated": [],
    "testsCreated": [],
    "lastCheckpoint": "UI 设计完成，等待用户确认"
  }
}
```

### Status 值说明

| Status | 说明 | 已完成阶段 |
|--------|------|-----------|
| `design` | 正在进行 UI 设计 | - |
| `build` | 设计完成，正在编写代码 | design |
| `test` | 已完成代码，正在写测试 | design, build |
| `verify` | 已完成测试，正在编译验证 | design, build, test |
| `commit` | 已完成验证，准备提交 | design, build, test, verify |
| `completed` | 功能完成 | 全部 |

### 进度保存时机

| 阶段 | 操作 | 文件变更 |
|------|------|---------|
| 开始功能 | 创建进度文件 | status: design |
| Design 完成 | 更新进度 | status: build, designLayout, designComponents |
| Build 完成 | 更新进度 | status: test, filesCreated |
| Test 完成 | 更新进度 | status: verify, testsCreated |
| Verify 完成 | 更新进度 | status: commit |
| Commit 完成 | 删除进度文件 | 清除 |

### 进度恢复流程

1. **检测进度**：`/feat` 执行时读取 `feat-progress.json`
2. **提示用户**：显示当前功能、状态、最后记录、设计信息
3. **用户选择**：
   - Y: 跳转到对应阶段继续开发
   - N: 删除进度文件，重新开始

### 进度文件管理规则

- ✅ 功能完成 commit 后**必须删除**进度文件
- ✅ 进度文件**不提交**到 git（在 .gitignore 中）
- ✅ 用户选择"重新开始"时**立即清除**进度
- ✅ 每个阶段完成后**立即更新**进度文件

---

## 错误处理

### 验证失败
1. 分析错误原因
2. 修复代码
3. 重新验证
4. 多次失败考虑回滚: `git reset --hard`

### 需求变更
1. 停止当前实现
2. 讨论变更内容
3. 更新 PRD
4. 重新规划

## 注意事项

- **一次只做一个功能**，不要贪多
- **Design 阶段不可跳过**：必须确认 UI 组件和样式
- **使用已有的 BrandColors.xaml 样式**，不要自己定义新颜色
- 测试要覆盖核心逻辑，不追求完美
- **每次功能完成后必须编译生成可执行版本**，确保代码可运行
- **手动测试验证功能正常**，不要跳过
- **UI 一致性检查**：确保新功能与已有页面风格一致
- **每个阶段完成后立即更新进度文件**，支持中断恢复
- commit 信息简洁清晰
- 提交后**删除进度文件**，运行 `/compact` 清理上下文
- 随时可以加新功能到 PRD
- 遇到问题及时讨论，不要硬撑
