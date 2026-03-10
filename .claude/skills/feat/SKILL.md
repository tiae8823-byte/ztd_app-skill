---
name: feat
description: Implement single feature using Design-Intent-Build-Verify-Commit cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能

采用 **Design-Intent-Build-Verify-Commit** 循环实现**单个**功能。

## 核心理念

- **一次只实现一个功能**
- **测试意图先行**：人工定义预期，AI 实现代码
- **调研贯穿全流程**：GitHub > 博客 > 官方文档
- **分层测试**：CRITICAL 冒烟测试，NON-CRITICAL 手动测试
- **进度自动保存**

## 流程

```
Design → Intent → Build → Verify → Commit
  ↓        ↓        ↓        ↓
调研UI   定义预期  实现     验证
         (人工)          (分层)
```

| 阶段 | 核心任务 | 强制检查点 |
|------|---------|-----------|
| **Design** | UI布局 + 复杂度评估 + 调研 | **必须**输出 Given-When-Then + 用户确认 |
| **Intent** | 测试意图确认（仅 CRITICAL） | **必须**人工确认 |
| **Build** | 调研方案 → 编码实现 | **必须**告知参考来源 |
| **Verify** | 编译 + 分层测试 | **必须**运行检查清单 |
| **Commit** | 提交代码 + 推送远程 | **必须**更新PRD + 清理进度 |

详细流程: [references/workflow-guide.md](references/workflow-guide.md)

## 调研策略（全流程）

**优先级**：
1. **GitHub 搜索** - 真实项目代码（`mcp__github__search_code`）
2. **高质量博客** - 中英文技术文章（`mcp__exa__web_search_exa`）
3. **官方文档** - API 参考（`mcp__context7__query-docs`）

**Design 阶段**：
- 搜索 UI 模式：`site:github.com WPF card layout`
- 搜索交互方案：`material design dialog animation`

**Build 阶段**：
- 搜索实现方案：`site:github.com WPF global hotkey`
- 搜索代码示例：`WPF animation opacity fade`

**关键**：先看别人怎么做，再自己实现

## 功能分级标准

| 级别 | 定义 | 测试策略 | 时间 |
|------|------|---------|------|
| **CRITICAL** | 支付、认证、数据迁移、核心业务 | 冒烟测试 + 手动测试 | 10-15分钟 |
| **NON-CRITICAL** | UI 调整、实验性功能、内部工具 | 手动测试 | 2-5分钟 |

**判断标准**：
- CRITICAL：涉及金钱、数据安全、核心业务流程
- NON-CRITICAL：UI 样式、动画、统计展示、实验功能

## 测试意图先行（强制）

### Intent 阶段格式

**Given-When-Then**（借鉴 ATDD）：

```markdown
📝 测试意图（请确认）：

功能级别: [CRITICAL / NON-CRITICAL]

测试场景:

1. ✅ [场景名称]
   Given: [初始状态]
   When: [用户操作/输入]
   Then: [预期结果]

2. ✅ [场景名称]
   Given: [初始状态]
   When: [用户操作/输入]
   Then: [预期结果]

边界条件:
- [边界场景]: [描述]

这些场景对吗？需要补充吗？
```

### AI 角色限制 ⚠️

```markdown
❌ 禁止：AI 自己定义测试意图
✅ 允许：AI 根据人工确认的意图实现代码
```

**原因**：AI 看着代码写测试 = 验证"当前行为"而非"正确行为"

详见 [references/test-intent-first.md](references/test-intent-first.md)

## Verify 阶段（分层验证）

### CRITICAL 功能

```markdown
1. 编译检查
2. AI 生成冒烟测试（基于测试意图）
3. 运行测试
4. 手动测试完整流程
5. UI 一致性检查（使用 project-config.md 检查清单）
```

### NON-CRITICAL 功能

```markdown
1. 编译检查
2. 手动测试清单（基于测试意图）
3. UI 样式检查
```

详见 [references/testing-guide.md](references/testing-guide.md)

## 关键原则

1. **必须使用共享样式** - 禁止内联 ⚠️
2. **一次一个功能** - 不并行
3. **测试意图先行** - 人工定义，AI 实现
4. **调研贯穿全流程** - GitHub > 博客 > 官方文档
5. **Given-When-Then 格式** - 结构化测试场景（强制）
6. **分层测试** - CRITICAL 冒烟测试，NON-CRITICAL 手动测试
7. **及时保存进度** - 每个阶段后更新 feat-progress.json
8. **完成清理** - commit + push 后删除进度文件，运行 /compact

## 文件结构

| 文件 | 用途 |
|------|------|
| [project-config.md](project-config.md) | 构建命令、样式系统、技术栈 |
| [design-guide.md](design-guide.md) | 设计规范、颜色、组件样式 |
| [references/workflow-guide.md](references/workflow-guide.md) | 详细工作流程 |
| [references/test-intent-first.md](references/test-intent-first.md) | 测试意图先行详解 |
| [references/testing-guide.md](references/testing-guide.md) | 分层测试策略 |

## 进度管理

进度保存在 `.claude/feat-progress.json`

| Status | 说明 |
|--------|------|
| `design` | 正在设计 |
| `intent` | 定义测试意图（CRITICAL） |
| `build` | 正在实现 |
| `verify` | 正在验证 |
| `commit` | 准备提交 |

## 质量门禁（可选）

变异测试验证测试有效性：

```bash
dotnet stryker --threshold-high 70 --threshold-break 40
```

- 分数 > 60%：测试真的有效
- 分数 < 40%：测试只是摆设

详见 [references/testing-guide.md](references/testing-guide.md)
