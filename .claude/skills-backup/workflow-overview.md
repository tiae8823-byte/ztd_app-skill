# 技能工作流总览

> 审阅文档：描述 /prd 和 /feat 两个技能的职责、流程和文件分工。

---

## 整体流程

```
新项目从 0 开始：

1. 复制 .claude/skills/ (prd + feat) 到新项目
2. 运行 /prd  → 对话确定需求和技术栈
3. 运行 /init → Claude Code 生成 CLAUDE.md（可选，随时可跑）
4. 运行 /feat → 逐个实现功能
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

## /feat 技能 — 功能实现

### 职责

按 Design-Build-Test-Verify-Commit 循环，一次实现一个功能。

### 流程

```
0. 检查进度恢复     → 有未完成的功能？继续 or 重新开始
1. 读取 PRD + 配置  → PRD.md + project-config.md
2. 用户选择功能     → 显示功能清单，用户选一个
3. Design          → AI 生成布局方案，匹配共享样式，用户确认
4. Build           → 调研 → 写代码 → 实现 UI（引用共享样式）
5. Test            → 写单元测试（核心逻辑、边界条件）
6. Verify          → 执行 project-config.md 中的构建/测试命令 + 手动测试
7. Commit          → git commit → 更新 PRD.md → 删除进度文件
8. 下一个？         → 继续 or 结束
```

### 依赖文件

| 文件 | 性质 | 来源 | 用途 |
|------|------|------|------|
| `skill.md` | 通用 | 复制 | 工作流程定义 |
| `project-config.md` | 项目专属 | /prd 生成 | 构建命令、样式系统、UI 检查清单、测试约定 |
| `design-guide.md` | 项目专属 | /prd 生成 | 设计规范参考 |
| `testing-guide.md` | 通用 | 复制 | 测试方法论（通用原则） |
| `troubleshooting-guide.md` | 通用 | 复制 | 问题排查策略 |
| `examples.md` | 通用 | 复制 | 对话示例 |
| `feat-progress.json` | 临时 | 运行时生成 | 进度跟踪（gitignored） |

### DBTVC 测试策略

Design-Build-Test-Verify-Commit 循环中的测试分工：

| 阶段 | 测试职责 |
|------|---------|
| Design | 识别 3-5 条核心测试场景（测试意图） |
| Build | 不写测试，专注实现（保持可测试性） |
| Test | 写单元测试 + ViewModel 集成测试 |
| Verify | 运行全部测试 + UI 测试 + 手动冒烟 |
| Commit | 全绿才提交 |

**测试分层（钻石型）**：
- 单元测试（70-80%）：Service/Manager 层的业务逻辑
- 集成测试（15-20%）：ViewModel 层，最有价值
- UI 自动化测试（5-10%）：仅关键路径

详细方法论见 `testing-guide.md`，项目特定的测试框架和命令见 `project-config.md`。

### 关键原则

- **一次一个功能**：不贪多
- **共享样式优先**：禁止内联写死 UI 属性，必须引用样式系统
- **新模式必须提升**：重复出现的 UI 模式 → 提取到共享样式 → 更新 design-guide.md
- **进度可中断恢复**：每个阶段自动保存进度到 feat-progress.json
- **增量测试**：每个功能完成后运行全部测试确保无回归

---

## 文件分工汇总

### 换项目时的操作

| 文件 | 操作 |
|------|------|
| `skills/prd/skill.md` | 直接复制，不改 |
| `skills/feat/skill.md` | 直接复制，不改 |
| `skills/feat/testing-guide.md` | 直接复制，不改（通用测试方法论） |
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
| `testing-guide.md` | 测试方法论（分层策略、DBTVC 测试职责、AI 测试规则、常见陷阱） | 通用，不随项目变 |
| `project-config.md` 的"测试约定"章节 | 测试框架、内存数据库配置、测试命令 | 项目专属，/prd 生成 |

---

## 常见问题

### 换项目 / 换技术栈怎么办？

这套技能适用于任何技术栈（Web 前后端分离、Android 原生、桌面应用等）。

```
1. 复制 .claude/skills/ 到新项目
2. 运行 /prd → 对话确定新技术栈 → 自动生成 project-config.md 和 design-guide.md
3. 运行 /feat → 正常开发
```

通用文件（skill.md、testing-guide.md、troubleshooting-guide.md、examples.md）不需要改。项目专属文件由 /prd 根据新技术栈自动生成。

### UI 优化需求怎么提？

**直接在 /feat 里说就行**，不需要回到 /prd。

UI 优化本质上和新功能一样，都是 PRD 里的一个待办项，走同一个 DBTVC 循环。

```
# 示例提示语（在 /feat 对话中直接说）：
加个功能：卡片出现时加淡入动画
加个功能：列表为空时显示空状态提示
加个功能：深色模式
```

AI 会自动加到 PRD.md，然后走 Design-Build-Test-Verify-Commit。

### UI 设计和前端开发是同一件事吗？

不是，但在独立开发者的 MVP 流程中合并处理：

- **Design 阶段** = UI 设计（布局、组件、样式匹配、交互状态）
- **Build 阶段** = 前端开发 + 后端逻辑（写代码实现功能）

大团队会拆成设计师和开发两个角色，独立开发者合并是合理的。

### /prd 和 /feat 分别什么时候用？

| 场景 | 用哪个 |
|------|--------|
| 新项目从零开始 | /prd |
| 大方向调整（换技术栈、改架构） | /prd |
| 实现具体功能 | /feat |
| UI 优化 | /feat |
| 加新需求 | /feat（对话中直接说，自动加到 PRD） |
| Bug 修复 | /feat |
