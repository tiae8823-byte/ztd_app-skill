---
name: feat
description: Implement single feature using TDD (Test-Driven Development) cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能（TDD 版）

采用 **Design-Test-Build-Refactor-Verify-Commit** 循环实现**单个**功能。
测试驱动开发：**先写测试，再写实现**。

## 核心理念

**TDD + 小步快跑**：
- 一次只实现一个功能
- **先写失败的测试（Red），再写代码让测试通过（Green），最后重构（Refactor）**
- 验证通过后立即 commit
- PRD 是活的，可以随时加功能
- **进度自动保存**：随时中断，下次可以继续
- **UI 风格一致**：每个功能都遵循统一的设计规范

**Design-Test-Build-Refactor-Verify-Commit 循环**：
```
UI设计 → 写测试(Red) → 写代码(Green) → 重构(Refactor) → 编译+手动测试 → Commit
  ↓          ↓              ↓               ↓                ↓              ↓
 修改       修改          修改           修改            修复          回滚
  ↓          ↓              ↓               ↓                ↓              ↓
保存进度   保存进度      保存进度       保存进度        保存进度      清除进度
  ↓          ↓              ↓               ↓                ↓              ↓
feat-progress.json (各阶段自动更新)
```

> **与传统 DBTVC 的区别**：Test 在 Build 之前。先定义"正确行为是什么"（测试），再实现行为（代码）。

### Red-Green-Refactor 循环

TDD 的核心是在 Test 和 Build 阶段反复执行小循环：

```
┌─────────────────────────────────────┐
│                                     │
│  1. Red    → 写一个失败的测试        │
│  2. Green  → 写最少的代码让它通过    │
│  3. Refactor → 清理代码，保持测试绿  │
│                                     │
│  重复，直到功能完整                   │
│                                     │
└─────────────────────────────────────┘
```

**关键纪律**：
- Red 阶段：测试必须**先运行并失败**，证明测试有效
- Green 阶段：写**最少**的代码让测试通过，不多写
- Refactor 阶段：改结构不改行为，测试必须始终绿色

### 务实 TDD 指南

**不是所有功能都需要严格 Red-Green-Refactor**。MVP 阶段要务实：

| 功能类型 | 推荐方式 | 原因 |
|---------|---------|------|
| 业务逻辑（创建/删除/状态转换） | 严格 TDD | 逻辑复杂，测试驱动设计最有价值 |
| 数据持久化（增删改查） | 严格 TDD | 数据正确性关键 |
| UI 布局和样式 | Design-Build 先行，后补测试 | UI 需要视觉探索，难以先写测试 |
| 快速原型/验证想法 | Build 先行，后补核心测试 | 速度优先 |
| Bug 修复 | TDD 修复（先写重现测试） | 防止回归 |

**需求不清晰时的处理**：

```
Design 阶段 AI 生成测试意图时：

  确定的需求  → 写测试意图 → 正常 TDD
  模糊的需求  → 标记"待确认"，跳过 → Build 过程中想清楚后补测试
  完全不确定  → 整个功能用 Build 先行模式探索

用户回复示例：
"1-3 确认，4 和 5 我没想好，先跳过"
→ AI 基于 1-3 写测试，4 和 5 在 Build 中确认后补充
```

> **核心原则**：TDD 是工具不是教条。严格 TDD 用在值得的地方（业务逻辑），灵活处理不确定的地方（UI 探索、模糊需求）。

## 文件结构

本技能采用**通用流程 + 项目配置**分离架构：

| 文件 | 性质 | 换项目时 | 说明 |
|------|------|---------|------|
| `skill.md` | 通用 | 不改 | 工作流程（本文件） |
| `troubleshooting-guide.md` | 通用 | 不改 | 问题排查方法论 |
| `examples.md` | 通用 | 不改 | 对话示例 |
| **`project-config.md`** | **项目专属** | **由 /prd 生成** | 构建命令、样式系统、技术栈 |
| **`design-guide.md`** | **项目专属** | **由 /prd 生成** | 设计规范和样式清单 |
| `testing-guide.md` | 通用 | 不改 | TDD 测试方法论和策略 |

> **新项目使用**：复制 `.claude/skills/` → 运行 `/prd` → 自动生成项目专属文件 → 运行 `/feat`

## 可用 MCP 工具

| MCP 工具 | 用途 |
|----------|------|
| `mcp__context7__resolve-library-id` + `mcp__context7__query-docs` | 查询框架 API 用法 |
| `mcp__github__search_code` | 搜索参考实现 |
| `mcp__sequential-thinking__sequentialthinking` | 复杂功能设计 |

---

## 与 PRD 的联动

```
/feat 执行
  │
  ├─ 检查 project-config.md 是否存在
  │   │
  │   └─ 不存在 → "未找到项目配置，请先运行 /prd 生成"
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
  │                       Design → Test → Build → Refactor → Verify → Commit
  │                            │
  │                       ├─ Design → 保存进度 (status: test, 记录设计+测试意图)
  │                       ├─ Test → 保存进度 (status: build, 记录测试文件)
  │                       ├─ Build → 保存进度 (status: refactor, 记录实现文件)
  │                       ├─ Refactor → 保存进度 (status: verify)
  │                       ├─ Verify → 保存进度 (status: commit)
  │                       ├─ Commit → PostGitCommand Hook 记录版本
  │                       └─ 完成 → 清除进度 → 更新 PRD.md
  │
  └─ 询问是否继续下一个
```

---

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
状态: build（已完成: design, test）
设计方案: 垂直布局, Button, TextBox, CardBorder
测试文件: IdeaTests.cs (8 个测试用例, 全部 Red)
最后记录: 测试编写完成，等待实现代码
创建时间: 2026-03-01 14:30

是否继续？
- Y: 继续开发
- N: 重新开始
```

---

### 1. 读取 PRD 和项目配置

```
读取 docs/PRD.md + project-config.md

PRD 不存在 → "未找到 PRD.md，请先运行 /prd 创建需求文档"
project-config.md 不存在 → "未找到项目配置，请先运行 /prd 生成"

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

---

### 3. Design 阶段 - 布局设计 + 测试意图

**做两件事**：布局设计 + 定义测试意图（测试意图是 Test 阶段的输入）。

**流程**：
1. 读取 [project-config.md](project-config.md) 和 [design-guide.md](design-guide.md) 了解样式系统
2. AI 根据 PRD 自动生成布局方案（什么组件放哪里）
3. **匹配共享样式**：为每个组件指定设计系统中的命名样式
4. **定义测试意图**：列出 5-8 条需要测试的核心场景（比传统方式更详细，因为下一步就要写测试）
5. 展示布局草图 + 测试意图给用户确认
6. 用户确认后保存进度到 feat-progress.json

**共享样式优先原则**：
- **必须优先使用已有共享样式**，禁止内联写死样式属性
- 如果现有样式不满足需求，先在样式文件中新增命名样式，再引用
- 如果发现重复出现的 UI 模式（2+ 处），应提升为共享样式

**确认提示**：
```markdown
🎨 布局设计: [功能名称]

布局: [垂直/水平/网格]
组件: Button x2, TextBox x1, Card

┌─────────────────────────┐
│  [标题]                 │
│                         │
│  [输入框]               │
│  [按钮]                 │
└─────────────────────────┘

🧪 测试意图（Test 阶段将基于此编写测试）:
1. 输入内容后点击按钮，数据应持久化到数据库
2. 输入为空时点击按钮，不应创建条目
3. 创建成功后，列表应立即显示新条目
4. 超长内容（500+ 字符）应正常保存和显示
5. 特殊字符（emoji、换行）应正常处理
6. 删除后，列表应移除该条目，数据库同步清除

是否符合预期？(Y/N，或提出修改意见)
```

**进度保存**：status: design → test

---

### 4. Test 阶段 - 先写测试（Red）

**这是 TDD 的核心阶段**：基于 Design 阶段的测试意图，编写测试代码。

**流程**：
1. **先调研测试方案**：用 `mcp__context7` 查询测试框架 API
2. **创建测试文件**：按测试意图编写测试用例
3. **编写顺序**：
   - 先写单元测试（Service/Manager 层业务逻辑）
   - 再写 ViewModel 集成测试（用户操作流程）
4. **运行测试确认全部 Red**：测试必须失败（因为实现代码还不存在）
   - 编译错误也算 Red（类/方法不存在）
   - 如果测试意外通过，说明测试写错了，需要修改

**Red 确认**：
```markdown
🔴 Red 确认: [功能名称]

测试文件: IdeaServiceTests.cs, IdeaViewModelTests.cs
测试用例: 8 个
状态: 全部 Red ✓（编译错误：IdeaManager 类不存在）

测试清单:
  ✗ CreateIdea_WithValidContent_ShouldPersist
  ✗ CreateIdea_WithEmptyContent_ShouldReject
  ✗ CreateIdea_ShouldAppearInList
  ✗ CreateIdea_WithLongContent_ShouldSucceed
  ✗ CreateIdea_WithSpecialChars_ShouldSucceed
  ✗ DeleteIdea_ShouldRemoveFromDatabase
  ✗ DeleteIdea_ShouldUpdateList
  ✗ GetAllIdeas_ShouldReturnOrderedByDate

全部 Red，开始实现代码？(Y/继续调整测试)
```

**详细测试策略**：见 [testing-guide.md](testing-guide.md)

**进度保存**：status: test → build

---

### 5. Build 阶段 - 写实现代码（Green）

**目标**：写最少的代码让所有测试从 Red 变 Green。

**流程**：
1. **先调研实现方案**：编码前先查阅资料
   - **优先查官方文档**：用 `mcp__context7` 查询框架 API 和最佳实践
   - 再用 WebSearch 搜索具体实现方案和常见问题
   - 用 `mcp__fetch__fetch` 阅读参考文章
   - 简要告知用户参考了哪些方案
2. **按测试驱动实现**：
   - 从最简单的测试开始
   - 每写一段代码后运行测试，观察从 Red → Green 的变化
   - 不写测试未覆盖的功能代码
3. **实现前端 UI（引用共享样式，不内联写死属性）**
4. **如有新 UI 模式**：先在样式文件中创建命名样式 → 在代码中引用 → 更新 design-guide.md

> **原则**：官方文档优先 → 网上成熟方案 → 自行实现。先查后写，避免重复造轮子。

**Green 确认**：
```markdown
🟢 Green 确认: [功能名称]

测试状态: 8/8 通过
  ✓ CreateIdea_WithValidContent_ShouldPersist
  ✓ CreateIdea_WithEmptyContent_ShouldReject
  ✓ CreateIdea_ShouldAppearInList
  ✓ CreateIdea_WithLongContent_ShouldSucceed
  ✓ CreateIdea_WithSpecialChars_ShouldSucceed
  ✓ DeleteIdea_ShouldRemoveFromDatabase
  ✓ DeleteIdea_ShouldUpdateList
  ✓ GetAllIdeas_ShouldReturnOrderedByDate

创建文件:
  - Models/Idea.cs
  - Data/IdeaRepository.cs
  - Services/IdeaManager.cs
  - ViewModels/IdeaViewModel.cs

全部 Green，开始重构？(Y/跳过重构直接验证)
```

**进度保存**：status: build → refactor

---

### 6. Refactor 阶段 - 重构

**目标**：改善代码结构，不改变行为。测试必须始终保持绿色。

**检查清单**：
- [ ] 有没有重复代码可以提取？
- [ ] 命名是否清晰表达意图？
- [ ] 方法是否过长需要拆分？
- [ ] 有没有内联样式需要提升为共享样式？
- [ ] design-guide.md 是否需要更新？

**重构原则**：
- **每次小改动后运行测试**，确保绿色
- 如果重构导致测试失败，立即回滚该改动
- 不在重构阶段添加新功能
- 重构完成后全部测试仍然通过

**"两顶帽子"原则**（Martin Fowler）：
- Green 阶段戴"功能帽"：只管让测试通过，代码丑没关系
- Refactor 阶段戴"结构帽"：只管让代码漂亮，不加新功能
- 一次只戴一顶帽子，不要同时追求"正确"和"优雅"

**大局观检查**（每 3-5 个 Red-Green 循环后暂停）：
- 当前代码结构是否适合整体架构？还是只适合当前这个测试？
- 有没有"为了通过测试而写的 hack"需要替换为正式方案？
- 新模块和已有模块的交互方式是否合理？

> 如果发现架构问题，在 Refactor 阶段处理（测试保护下安全调整）。

**如果代码已经干净**：可以跳过此阶段，直接进入 Verify。

**进度保存**：status: refactor → verify

---

### 7. Verify 阶段 - 编译并验证

**流程**：执行 [project-config.md](project-config.md) 中定义的命令：
1. 执行**构建命令**
2. 执行**测试命令**（应该全部通过，因为 Build 阶段已经 Green）
3. 执行**UI 测试命令**（如有）
4. 执行**启动命令**，进行手动测试

**UI 一致性检查**：使用 [project-config.md](project-config.md) 中的 UI 一致性检查清单

**详细测试策略**：见 [testing-guide.md](testing-guide.md)

**进度保存**：status: verify → commit

---

### 问题处理流程

**触发时机**：Verify 阶段发现问题

**问题分类**：

| 问题类型 | 定义 | 处理建议 |
|---------|------|---------|
| **测试失败** | 重构或集成导致已有测试 Red | 必须修复（回到 Build/Refactor） |
| **阻塞性问题** | 功能不可用、数据丢失、崩溃 | 必须修复 |
| **非阻塞性问题** | UI 细节、性能优化、视觉不完美 | 可选择处理方式 |

**处理选项**：

```markdown
发现问题后，提供三个选项：

1. **TDD 修复**（推荐）
   - 先写一个重现问题的测试（Red）
   - 修复代码让测试通过（Green）
   - 这样问题不会再次出现
   - 适用于：逻辑问题、可重现的 Bug
   - **注意**：写重现测试时，基于"期望的正确行为"而非"当前代码的行为"来写断言

2. **标记为已知问题**
   - 记录到 PRD.md 的"已知问题"章节
   - 继续 Commit，后续迭代解决
   - 适用于：非阻塞性问题、UI 细节

3. **直接提交**
   - 忽略问题，继续 Commit
   - 适用于：极小的视觉瑕疵、不影响使用的细节
```

**问题排查**：如果尝试修复 2 次仍未解决，参考 [troubleshooting-guide.md](troubleshooting-guide.md) 搜索解决方案

**known-issues.json 维护规则**（自动执行，无需用户提醒）：

| 触发条件 | 操作 |
|---------|------|
| 2次尝试失败后通过搜索找到方案 | 解决后写入 known-issues.json（问题+解决方案+参考链接） |
| 确认是框架/系统级 Bug | 写入 known-issues.json（open 状态） |
| 快速解决（1次成功） | 不记录 |
| 极小视觉瑕疵直接跳过 | 不记录 |

> 定位：知识库，记录"下次还会踩的坑"。不是每个问题都记录，只记录非显而易见的解决方案。

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

---

### 8. 更新 PRD.md

标记功能完成：

```markdown
### MVP 功能（必须实现）

- [ ] 用户注册
- [x] 用户登录 ✅ commit: a1b2c3d
- [ ] 发布文章
```

### 9. 询问下一步

```
✅ [功能名称] 完成（TDD）

版本信息已记录到: .claude/version-history.log
测试覆盖: [N] 个测试用例全部通过

建议运行 /compact 清理上下文，为下一个功能准备干净的环境

继续下一个功能？
- 输入序号继续
- 或告诉我新需求
- 或说"结束"退出
```

---

## 过程中发现新需求

用户可以随时提出新功能：

```
用户: 等等，文章还得能置顶

AI: 好，加到 PRD 里：
    - [ ] 文章置顶

    先做置顶还是继续当前功能？
```

---

## 进度维护

### 进度文件位置

`.claude/feat-progress.json` - 功能开发进度文件

### 进度文件格式

```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "test",
    "createdAt": "2026-03-01T14:30:00Z",
    "description": "开始实现功能",
    "designLayout": "垂直布局",
    "designComponents": ["Button", "TextBox", "Card"],
    "designSketch": "[简单的ASCII布局图]",
    "testIntents": [
      "输入内容后点击按钮，数据应持久化",
      "输入为空时不应创建条目",
      "创建成功后列表应立即显示"
    ],
    "testsCreated": [],
    "filesCreated": [],
    "testResults": { "total": 0, "red": 0, "green": 0 },
    "lastCheckpoint": "Design 完成，开始写测试"
  }
}
```

### Status 值说明

| Status | 说明 | 已完成阶段 |
|--------|------|-----------|
| `design` | 正在进行 UI 设计和测试意图 | - |
| `test` | 设计完成，正在写测试（Red） | design |
| `build` | 测试写完，正在写实现（Green） | design, test |
| `refactor` | 实现完成，正在重构 | design, test, build |
| `verify` | 重构完成，正在编译验证 | design, test, build, refactor |
| `commit` | 验证完成，准备提交 | design, test, build, refactor, verify |
| `completed` | 功能完成 | 全部 |

### 进度保存时机

| 阶段 | 操作 | 文件变更 |
|------|------|---------|
| 开始功能 | 创建进度文件 | status: design |
| Design 完成 | 更新进度 | status: test, designLayout, testIntents |
| Test 完成 | 更新进度 | status: build, testsCreated, testResults |
| Build 完成 | 更新进度 | status: refactor, filesCreated |
| Refactor 完成 | 更新进度 | status: verify |
| Verify 完成 | 更新进度 | status: commit |
| Commit 完成 | 删除进度文件 | 清除 |

### 进度恢复流程

1. **检测进度**：`/feat` 执行时读取 `feat-progress.json`
2. **提示用户**：显示当前功能、状态、测试状态（Red/Green）、最后记录
3. **用户选择**：
   - Y: 跳转到对应阶段继续开发
   - N: 删除进度文件，重新开始

### 进度文件管理规则

- 功能完成 commit 后**必须删除**进度文件
- 进度文件**不提交**到 git（在 .gitignore 中）
- 用户选择"重新开始"时**立即清除**进度
- 每个阶段完成后**立即更新**进度文件

---

## 错误处理

### 测试始终无法通过
1. 检查测试本身是否合理
2. 检查测试意图是否正确
3. 尝试修复（最多 2 次）
4. **仍未解决 → 参考 [troubleshooting-guide.md](troubleshooting-guide.md) 搜索解决方案**
5. 多次失败考虑回滚: `git reset --hard`

### 需求变更
1. 停止当前实现
2. 讨论变更内容
3. 更新 PRD
4. 重新规划（从 Design 开始，已有测试可复用）

---

## 注意事项

- **一次只做一个功能**，不要贪多
- **测试先行**：没有失败的测试，不写实现代码
- **Design 阶段不可跳过**：必须确认 UI 组件、共享样式、测试意图
- **必须使用共享样式**：禁止内联写死样式属性
- **新 UI 模式必须提升为共享样式**：先在样式文件中定义，再引用，最后更新 design-guide.md
- **每写一段代码后运行测试**：观察 Red → Green 的变化
- **重构不改行为**：Refactor 阶段测试必须始终绿色
- **每次功能完成后必须编译验证**，确保代码可运行
- **手动测试验证功能正常**，不要跳过
- **UI 一致性检查**：确保新功能与已有页面风格一致
- **每个阶段完成后立即更新进度文件**，支持中断恢复
- commit 信息简洁清晰
- 提交后**删除进度文件**，运行 `/compact` 清理上下文
- 随时可以加新功能到 PRD
- 遇到问题及时讨论，不要硬撑

---

## 对话示例

详细的对话示例见 [examples.md](examples.md)
