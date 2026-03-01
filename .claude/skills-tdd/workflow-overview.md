# 技能工作流总览（TDD 版）

> 审阅文档：描述 /prd 和 /feat 两个技能的职责、流程和文件分工。
> 本版本采用 TDD（测试驱动开发），与传统 DBTVC 版本的区别在于 **Test 在 Build 之前**。

---

## 整体流程

```
新项目从 0 开始：

1. 复制 .claude/skills/ (prd + feat) 到新项目
2. 运行 /prd  → 对话确定需求和技术栈
3. 运行 /init → Claude Code 生成 CLAUDE.md（可选，随时可跑）
4. 运行 /feat → 逐个实现功能（TDD 方式）
```

---

## /prd 技能 — 需求与技术决策

### 职责

通过自然对话，确定产品需求、技术栈、技术架构，输出 PRD 和项目配置文件。

### 流程

```
1. 理解需求        → 这个产品解决什么问题？目标用户是谁？
2. 调研同类产品     → 搜索竞品，了解主流设计思路
3. 确定技术栈      → AI 推荐框架 + UI 方案 + 数据库，用户确认
4. 设计技术架构     → 目录结构、分层架构、数据流转
5. 确定 MVP 功能   → 哪些必须有，哪些以后加
6. 生成 PRD.md     → docs/PRD.md
7. 生成项目配置     → feat 技能所需的项目专属文件
8. 确认            → 用户审阅，可调整
```

### 输出文件

| 文件 | 位置 | 内容 |
|------|------|------|
| `PRD.md` | `docs/PRD.md` | 核心价值、技术栈、架构、功能清单 |
| `project-config.md` | `.claude/skills/feat/` | 构建/测试/运行命令、样式系统位置、UI 检查清单、测试约定 |
| `design-guide.md` | `.claude/skills/feat/` | 颜色系统、Typography 层级、组件样式清单、共享样式流程 |

### 不生成的文件

| 文件 | 原因 | 谁管 |
|------|------|------|
| `CLAUDE.md` | /init 会覆盖，职责冲突 | /init 或用户手动 |
| `testing-guide.md` | 测试方法论是通用知识，不随项目变化 | 通用文件，随技能模块复制 |

---

## /feat 技能 — TDD 功能实现

### 职责

按 **Design-Test-Build-Refactor-Verify-Commit** 循环，一次实现一个功能。
**测试先行**：先写失败的测试，再写代码让测试通过。

### 流程

```
0. 检查进度恢复     → 有未完成的功能？继续 or 重新开始
1. 读取 PRD + 配置  → PRD.md + project-config.md
2. 用户选择功能     → 显示功能清单，用户选一个
3. Design          → AI 生成布局方案 + 测试意图（5-8条），用户确认
4. Test (Red)      → 写测试代码 → 运行确认全部失败（Red）
5. Build (Green)   → 调研 → 写代码 → 逐个让测试通过（Green）
6. Refactor        → 重构代码 → 保持测试绿色
7. Verify          → 执行构建/测试命令 + 手动测试
8. Commit          → git commit → 更新 PRD.md → 删除进度文件
9. 下一个？         → 继续 or 结束
```

### 与传统 DBTVC 的区别

```
传统 DBTVC:   Design → Build → Test    → Verify → Commit
TDD DTBRVC:   Design → Test  → Build → Refactor → Verify → Commit
                        ^^^^              ^^^^^^^^
                      测试先行             新增重构阶段
```

**核心差异**：
- Test 在 Build 之前（先定义"对"，再实现）
- 新增 Refactor 阶段（测试保护下安全重构）
- Design 阶段增加测试意图（更详细的行为期望）
- Build 阶段目标是"让测试通过"而非"实现功能"

### 依赖文件

| 文件 | 性质 | 来源 | 用途 |
|------|------|------|------|
| `skill.md` | 通用 | 复制 | TDD 工作流程定义 |
| `project-config.md` | 项目专属 | /prd 生成 | 构建命令、样式系统、UI 检查清单、测试约定 |
| `design-guide.md` | 项目专属 | /prd 生成 | 设计规范参考 |
| `testing-guide.md` | 通用 | 复制 | TDD 测试方法论（Red-Green-Refactor） |
| `troubleshooting-guide.md` | 通用 | 复制 | 问题排查策略 |
| `examples.md` | 通用 | 复制 | TDD 对话示例 |
| `feat-progress.json` | 临时 | 运行时生成 | 进度跟踪（gitignored） |

### DTBRVC 测试策略

Design-Test-Build-Refactor-Verify-Commit 循环中的测试分工：

| 阶段 | 测试职责 |
|------|---------|
| Design | 定义 5-8 条测试意图（具体的行为期望） |
| Test | 编写测试代码，确认全部 Red |
| Build | 逐个让测试从 Red 变 Green |
| Refactor | 重构代码，保持测试 Green |
| Verify | 运行全部测试 + UI 测试 + 手动冒烟 |
| Commit | 全绿才提交 |

**测试分层（钻石型）**：
- 单元测试（70-80%）：Service/Manager 层的业务逻辑
- 集成测试（15-20%）：ViewModel 层，最有价值
- UI 自动化测试（5-10%）：仅关键路径

详细方法论见 `testing-guide.md`，项目特定的测试框架和命令见 `project-config.md`。

### 关键原则

- **测试先行**：没有失败的测试，不写实现代码
- **一次一个功能**：不贪多
- **Red-Green-Refactor**：小步循环，持续反馈
- **共享样式优先**：禁止内联写死 UI 属性，必须引用样式系统
- **新模式必须提升**：重复出现的 UI 模式 → 提取到共享样式 → 更新 design-guide.md
- **进度可中断恢复**：每个阶段自动保存进度到 feat-progress.json
- **增量测试**：每个功能完成后运行全部测试确保无回归

---

## 文件分工汇总

### 换项目时的操作

| 文件 | 操作 |
|------|------|
| `skills/prd/SKILL.md` | 直接复制，不改 |
| `skills/feat/skill.md` | 直接复制，不改 |
| `skills/feat/testing-guide.md` | 直接复制，不改（TDD 方法论） |
| `skills/feat/troubleshooting-guide.md` | 直接复制，不改 |
| `skills/feat/examples.md` | 直接复制，不改 |
| `skills/workflow-overview.md` | 直接复制，不改（本文件） |
| `skills/feat/project-config.md` | /prd 自动生成 |
| `skills/feat/design-guide.md` | /prd 自动生成 |
| `docs/PRD.md` | /prd 自动生成 |
| `CLAUDE.md` | /init 生成或手动写 |

### 生命周期

```
/prd 阶段:
  PRD.md              ← 创建
  project-config.md   ← 创建（含测试约定：框架、命令）
  design-guide.md     ← 创建

/feat 阶段（每个功能循环）:
  feat-progress.json  ← 创建 → 更新 → 删除
  design-guide.md     ← 可能更新（新增共享样式时）
  PRD.md              ← 更新（标记完成、新增功能、记录问题）

/init（随时可跑）:
  CLAUDE.md           ← 创建/更新
```

### 测试相关文件的分工

| 文件 | 内容 | 性质 |
|------|------|------|
| `testing-guide.md` | TDD 方法论（Red-Green-Refactor、分层策略、AI 测试规则、常见陷阱） | 通用，不随项目变 |
| `project-config.md` 的"测试约定"章节 | 测试框架、内存数据库配置、测试命令 | 项目专属，/prd 生成 |

---

## 常见问题

### 换项目 / 换技术栈怎么办？

这套技能适用于任何技术栈（Web 前后端分离、Android 原生、桌面应用等）。

```
1. 复制 .claude/skills/ 到新项目
2. 运行 /prd → 对话确定新技术栈 → 自动生成 project-config.md 和 design-guide.md
3. 运行 /feat → 正常开发（TDD 方式）
```

通用文件（skill.md、testing-guide.md、troubleshooting-guide.md、examples.md）不需要改。项目专属文件由 /prd 根据新技术栈自动生成。

### UI 优化需求怎么提？

**直接在 /feat 里说就行**，不需要回到 /prd。

UI 优化本质上和新功能一样，都是 PRD 里的一个待办项，走同一个 TDD 循环。

```
# 示例提示语（在 /feat 对话中直接说）：
加个功能：卡片出现时加淡入动画
加个功能：列表为空时显示空状态提示
加个功能：深色模式
```

AI 会自动加到 PRD.md，然后走 Design-Test-Build-Refactor-Verify-Commit。

### TDD 和传统方式怎么选？

| 场景 | 推荐方式 |
|------|---------|
| 业务逻辑复杂的功能 | TDD（测试驱动设计更有价值） |
| 纯 UI 展示功能 | 传统 DBTVC（UI 难以先写测试） |
| 快速原型/探索 | 传统 DBTVC（速度优先） |
| 重要的核心功能 | TDD（测试保护更安全） |
| 小修改/Bug 修复 | TDD 修复（先写重现测试再修） |

### /prd 和 /feat 分别什么时候用？

| 场景 | 用哪个 |
|------|--------|
| 新项目从零开始 | /prd |
| 大方向调整（换技术栈、改架构） | /prd |
| 实现具体功能 | /feat |
| UI 优化 | /feat |
| 加新需求 | /feat（对话中直接说，自动加到 PRD） |
| Bug 修复 | /feat（TDD 修复：先写重现测试） |
