---
name: prd
description: Generate MVP-focused PRD through natural conversation. Use when starting a new project, or when user says "生成PRD", "需求文档", "产品需求".
---

# PRD 生成技能

通过自然语言对话，确定 MVP 边界，生成产品需求文档。

## 核心理念

- **MVP 思维**：不追求一次性想清楚所有细节
- **先调研再设计**：参考现有成熟方案，避免重复造轮子
- **PRD 是活的**：可以在 feat 过程中持续更新

## 快速流程

```
理解需求 → 调研同类产品 → 确定技术栈 → 设计架构
    → 确定MVP功能 → 生成PRD.md → 生成feat配置
```

| 步骤 | 核心任务 | 输出 |
|------|---------|------|
| **理解需求** | 确定解决的问题、目标用户、核心价值 | - |
| **调研** | 搜索同类产品，分析设计亮点 | 参考方案列表 |
| **技术栈** | 推荐框架、数据库、UI方案 | 技术栈表格 |
| **架构设计** | 目录结构、分层架构、数据流转 | 架构图 |
| **MVP功能** | 必须有 vs 以后加 | 功能清单 |
| **生成文档** | PRD.md + feat配置 | 项目文档 |

## 执行流程

### 1. 理解需求

通过对话理解用户想要什么：
- 这个产品解决什么问题？
- 目标用户是谁？
- 核心价值是什么？（一句话说清楚）

### 2. 调研同类产品

在确定技术栈前，**先搜索同类产品**：
- WebSearch 搜索同类产品和竞品分析
- `mcp__fetch__fetch` 阅读产品介绍

**输出**：参考方案列表 + 对本项目的启发

### 3. 确定技术栈

AI 分析后推荐技术栈，**必须说明理由**：
- 产品类型 → 决定基础框架
- 用户规模 → 决定后端和部署
- 团队背景 → 减少学习成本
- 性能需求 → 决定数据存储

**输出**：技术栈表格（含 UI 方案）

### 4. 设计技术架构

确定技术栈后，设计架构：
- 项目目录结构
- 分层架构（UI/业务/数据）
- 数据访问方式
- 状态管理

**输出**：架构图 + 数据流转说明

### 5. 确定 MVP 功能清单

讨论哪些是**必须有**的，哪些是**以后加**的：
- MVP 控制在 3-5 个核心功能
- 区分必须实现 vs 未来迭代

### 6. 生成 PRD.md

创建 `docs/PRD.md`，包含：
- 核心价值
- 参考方案
- 技术栈
- 技术架构
- 功能清单

### 7. 生成 feat 配置

自动生成两个配置文件：
- `.claude/skills/feat/project-config.md` - 构建命令、样式系统
- `.claude/skills/feat/design-guide.md` - 设计规范

### 8. 确认

```
✅ PRD 已生成: docs/PRD.md
✅ feat 项目配置已生成

需要调整吗？没问题的话可以开始 /feat 实现功能。
```

## 文件结构

| 文件 | 用途 |
|------|------|
| [references/workflow-detailed.md](references/workflow-detailed.md) | 详细工作流程 |
| [references/templates.md](references/templates.md) | PRD模板集合 |
| [examples/conversation-examples.md](examples/conversation-examples.md) | 对话示例 |

## 注意事项

- 不要一次性问太多问题，自然对话
- 技术栈推荐要说明理由
- MVP 功能控制在 3-5 个核心功能
- **先调研再设计**：技术选型前先看别人怎么做的
