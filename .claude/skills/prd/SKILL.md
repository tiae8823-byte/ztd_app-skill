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
| **技术栈** | 推荐框架、数据库、UI方案（必须说明理由） | 技术栈表格 |
| **架构设计** | 目录结构、分层架构、数据流转 | 架构图 |
| **MVP功能** | 必须有 vs 以后加，控制 3-5 个 | 功能清单 |
| **生成文档** | PRD.md + feat 配置 | 项目文档 |

详细操作: [references/workflow-detailed.md](references/workflow-detailed.md)
模板: [references/templates.md](references/templates.md)

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

## 轻量更新模式

当已有 PRD 时，用户可以说 `/prd 更新` 进行局部更新，无需重走完整流程：

- **添加功能**：直接添加到功能清单，标注依赖关系
- **调整优先级**：重新排列功能顺序
- **记录已知问题**：添加到「已知问题」章节
- **更新技术栈**：修改技术选型（需说明变更理由）

更新后只修改 PRD.md 中受影响的章节，不重新生成整个文档。
