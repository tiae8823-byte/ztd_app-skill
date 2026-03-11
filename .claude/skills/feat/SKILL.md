---
name: feat
description: Implement single feature using Design-Build-Verify-Commit cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能

采用 **Design-Build-Verify-Commit** 循环实现**单个**功能。

## 流程概览

```
Design → Build → Verify → Commit
  │        │        │        │
  ├ 读代码   ├ 按需调研  ├ 编译     ├ 更新PRD
  ├ 设计UI   ├ 编码实现  ├ 自动测试  ├ 提交推送
  ├ 测试意图  │         ├ 人工验收  ├ 提交推送
  ├ 用户确认  │         └ UI检查   └ 教学回顾
  └ 生成测试  │
    → 🔴     │→ 让测试通过 🟢
```

| 阶段 | 核心任务 | 检查点 |
|------|---------|-------|
| **Design** | 读代码 + UI设计 + 测试意图 + 生成测试代码 | 用户确认测试意图后生成测试，运行应失败 🔴 |
| **Build** | 按需调研 + 编码实现 | 参考测试意图逐步实现，让测试通过 |
| **Verify** | 编译 + 自动测试 + 人工验收 + UI检查 | 测试全部通过 🟢 → 用户运行应用确认 |
| **Commit** | 更新PRD + 提交 + 推送 + 教学回顾 | PRD 标记完成，用户理解实现 |

详细操作: [references/workflow-phases.md](references/workflow-phases.md)

## 功能分级

| 级别 | 定义 | 测试深度 |
|------|------|---------|
| **CRITICAL** | 数据持久化、核心业务流程、数据安全 | 完整边界测试 + 异常处理 |
| **NON-CRITICAL** | UI 调整、动画、统计展示、实验功能 | 核心路径测试 |

**所有功能都生成自动测试代码**，分级只影响测试场景的数量和深度。

## 测试意图先行

**核心原则**：人工定义预期，AI 实现代码。AI 禁止自己定义或修改测试意图。

**原因**：AI 看着代码写测试 = 验证"当前行为"而非"正确行为"。

**格式**（Given-When-Then，借鉴 ATDD）：

```markdown
📝 测试意图（请确认）：

功能级别: [CRITICAL / NON-CRITICAL]

测试场景:

1. ✅ [场景名称]
   Given: [初始状态]
   When: [用户操作]
   Then: [预期结果]

边界条件:（CRITICAL 必填，NON-CRITICAL 可选）
- [边界场景]: [描述]

这些场景对吗？需要补充吗？
```

**TDD 流程**：
1. Design 阶段：用户确认意图 → AI 生成测试代码 → 运行 🔴（预期失败）
2. Build 阶段：AI 实现功能代码 → 让测试通过
3. Verify 阶段：运行测试 🟢 → 人工验收

## 调研策略（按需）

遇到不确定的技术或尝试 2 次未解决时调研，不强制每次都做。

**搜索优先级**：
1. **GitHub 搜索** - 真实项目代码（`mcp__github__search_code`）
2. **高质量博客** - 中英文技术文章（`mcp__exa-mcp-server__web_search_exa`）
3. **官方文档** - API 参考（`mcp__context7-mcp__query-docs`）
4. **中文搜索** - 中文技术资源（`mcp__zhipu-web-search-sse__webSearchPro`）

## 关键原则

1. **一次一个功能** - 不并行开发
2. **测试意图先行** - 人工定义，AI 实现，所有功能都写测试
3. **必须使用共享样式** - 禁止内联 FontSize/Padding/Color
4. **调研按需** - 不确定时才搜索，告知参考来源
5. **自动测试优先** - 测试通过后才进入人工验收
6. **及时保存进度** - 每阶段后更新 feat-progress.json

## 进度管理

进度保存在 `.claude/feat-progress.json`（不提交到 git）

| Status | 说明 |
|--------|------|
| `design` | 设计 + 测试意图 + 生成测试代码 |
| `build` | 编码实现 |
| `verify` | 编译 + 测试 + 人工验收 |
| `commit` | 提交推送 |

详见 [references/progress-management.md](references/progress-management.md)

## 文件结构

| 文件 | 用途 |
|------|------|
| [project-config.md](project-config.md) | 构建命令、技术栈、测试约定 |
| [design-guide.md](design-guide.md) | 设计规范、样式清单、UI检查清单 |
| [references/workflow-phases.md](references/workflow-phases.md) | 各阶段详细操作指南 |
| [references/progress-management.md](references/progress-management.md) | 进度保存与恢复 |
| [references/troubleshooting.md](references/troubleshooting.md) | 问题排查策略 |
| [examples/feature-example.md](examples/feature-example.md) | 完整对话示例 |
