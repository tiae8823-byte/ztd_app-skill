---
name: feat
description: Implement single feature using Build-Test-Commit cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能

采用 **Build-Test-Commit** 循环实现**单个**功能。

## 核心理念

**小步快跑**：
- 一次只实现一个功能
- 每个功能完成后立即测试验证
- 验证通过后立即 commit
- PRD 是活的，可以随时加功能
- **进度自动保存**：随时中断，下次可以继续

**Build-Test-Commit 循环**：
```
写代码 → 写测试 → 编译+手动测试 → Commit
   ↓         ↓          ↓            ↓
 修改      修改        修复        回滚
   ↓         ↓          ↓            ↓
保存进度  保存进度    保存进度   清除进度
   ↓         ↓          ↓            ↓
feat-progress.json (各阶段自动更新)
```

## 可用 MCP 工具

| MCP 工具 | 用途 |
|----------|------|
| `mcp__context7__resolve-library-id` + `mcp__context7__query-docs` | 查询框架 API 用法 |
| `mcp__github__search_code` | 搜索参考实现 |
| `mcp__sequential-thinking__sequentialthinking` | 复杂功能设计 |

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
  │                       保存进度 (status: build)
  │                            │
  │                            ▼
  │                       Build-Test-Commit
  │                            │
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
状态: verify（已完成: build, test）
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

### 3. 实现单个功能

**确认要实现的功能**：
```
🔨 开始实现: [功能名称]

涉及内容:
- [后端部分]
- [前端部分]

开始？
```

**用户确认后，保存进度**：
```json
// .claude/feat-progress.json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "build",
    "createdAt": "2026-02-27T14:30:00Z",
    "description": "开始实现功能",
    "filesCreated": [],
    "testsCreated": []
  }
}
```

**Build - 编写代码**：
- 按需创建文件
- 实现功能逻辑
- 实现前端 UI（使用 PRD 中确定的组件库）

**Build 完成后，更新进度**：
```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "test",
    "createdAt": "2026-02-27T14:30:00Z",
    "filesCreated": ["file1.cs", "file2.xaml", ...],
    "lastCheckpoint": "Build阶段完成，已创建文件"
  }
}
```

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
    "filesCreated": ["file1.cs", "file2.xaml", ...],
    "testsCreated": ["Test1.cs", "Test2.cs"],
    "lastCheckpoint": "Test阶段完成，已编写测试用例"
  }
}
```

**Verify - 编译并自动化测试**：
```bash
# 编译生成可执行版本
dotnet build --configuration Release

# 运行单元测试
dotnet test

# 运行 UI 自动化测试（FlaUI）
dotnet test --filter "FullyQualifiedName~UITests"
```

**UI 自动化测试要点**：
- 正常流程：功能按预期工作
- 异常输入：空值、超长文本、特殊字符
- 边界情况：零个、一个、最大数量
- 错误处理：网络失败、数据库错误
- 数据持久化：重启后数据保留
- 界面响应：加载状态、错误提示
- 兼容性：已有功能未被破坏

**FlaUI 集成**：
```xml
<!-- ZtdApp.Tests/ZtdApp.Tests.csproj -->
<PackageReference Include="FlaUI.UIA3" Version="4.0.0" />
```

**UI 测试示例**：
```csharp
[Fact]
public void IdeaPage_AddIdea_ShouldAppearInList()
{
    using var app = FlaUI.Application.Launch("bin/Release/net8.0-windows/ZtdApp.exe");
    using var automation = new UIA3Automation();

    var window = app.GetMainWindow(automation);
    var ideaInput = window.FindFirstDescendant(cf => cf.ByName("想法输入框"));
    var addButton = window.FindFirstDescendant(cf => cf.ByName("添加按钮"));

    ideaInput.Text = "测试想法";
    addButton.Click();

    var ideaList = window.FindFirstDescendant(cf => cf.ByName("想法列表"));
    Assert.Contains("测试想法", ideaList.AsListBox().Items.Select(i => i.Name));
}
```

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

### 4. 更新 PRD.md

标记功能完成：

```markdown
### MVP 功能（必须实现）

- [ ] 用户注册
- [x] 用户登录 ✅ commit: a1b2c3d
- [ ] 发布文章
```

### 5. 询问下一步

```
✅ [功能名称] 完成

版本信息已记录到: .claude/version-history.log

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

    现在做置顶还是继续当前功能？
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
🔨 实现功能: [功能名称]

## 设计
[简要说明实现思路]

## Build
✅ 创建: [文件列表]

## Test
✅ 测试: [数量] 个用例

## Verify
✅ 编译成功: dotnet build --configuration Release
✅ 单元测试通过: dotnet test
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

AI: 🔨 开始实现: 用户登录

    涉及内容:
    - 后端: 登录 API、JWT 生成
    - 前端: 登录表单、状态管理

    开始？

用户: 开始

AI: [Build → Test → Verify → Commit → Compact]

    ✅ 用户登录 完成

    编译: dotnet build --configuration Release
    测试: dotnet test
    手动测试: 功能验证正常
    Commit: feat: 用户登录
    版本记录: .claude/version-history.log

    更新 PRD:
    - [x] 用户登录 ✅ commit: abc123

    建议运行 /compact 清理上下文

    继续下一个？

用户: 文章得能删除

AI: 好，加到 PRD:
    - [ ] 删除文章

    先做删除还是继续注册？

用户: 先做删除

AI: 🔨 开始实现: 删除文章
    ...

---

用户: /feat

AI: 📋 当前功能清单：

    MVP 功能：
    1. [ ] 用户注册
    2. [x] 用户登录 ✅
    3. [ ] 发布文章
    4. [ ] 删除文章 ← 新增

    已完成：1 个

    做哪个？
```

### 进度恢复流程

```
用户: /feat

AI: 📋 检测到未完成的功能：

    当前功能: 想法收集
    状态: verify（已完成: build, test）
    最后记录: Test阶段完成，等待编译验证
    创建时间: 2026-02-27 14:30

    是否继续？
    - Y: 继续开发
    - N: 重新开始

用户: Y

AI: 🔨 继续实现: 想法收集

    当前进度: verify（已创建代码和测试）
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
    "status": "test",              // build | test | verify | commit | completed
    "createdAt": "2026-02-27T14:30:00Z",
    "description": "开始实现功能",
    "filesCreated": [
      "ZtdApp/ZtdApp.csproj",
      "ZtdApp/Models/Idea.cs",
      "ZtdApp/Data/DatabaseService.cs"
    ],
    "testsCreated": [
      "ZtdApp.Tests/IdeaTests.cs"
    ],
    "lastCheckpoint": "Test阶段完成，等待编译验证",
    "updatedAt": "2026-02-27T15:20:00Z"
  }
}
```

### Status 值说明

| Status | 说明 | 已完成阶段 |
|--------|------|-----------|
| `build` | 正在编写代码 | - |
| `test` | 已完成代码，正在写测试 | build |
| `verify` | 已完成测试，正在编译验证 | build, test |
| `commit` | 已完成验证，准备提交 | build, test, verify |
| `completed` | 功能完成 | 全部 |

### 进度保存时机

| 阶段 | 操作 | 文件变更 |
|------|------|---------|
| 开始功能 | 创建进度文件 | status: build |
| Build 完成 | 更新进度 | status: test, filesCreated |
| Test 完成 | 更新进度 | status: verify, testsCreated |
| Verify 完成 | 更新进度 | status: commit |
| Commit 完成 | 删除进度文件 | 清除 |

### 进度恢复流程

1. **检测进度**：`/feat` 执行时读取 `feat-progress.json`
2. **提示用户**：显示当前功能、状态、最后记录
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
- 测试要覆盖核心逻辑，不追求完美
- **每次功能完成后必须编译生成可执行版本**，确保代码可运行
- **手动测试验证功能正常**，不要跳过
- **每个阶段完成后立即更新进度文件**，支持中断恢复
- commit 信息简洁清晰
- 提交后**删除进度文件**，运行 `/compact` 清理上下文
- 随时可以加新功能到 PRD
- 遇到问题及时讨论，不要硬撑
