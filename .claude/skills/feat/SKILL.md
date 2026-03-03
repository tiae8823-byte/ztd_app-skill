---
name: feat
description: Implement single feature using Design-Build-Test-Commit cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能

采用 **Design-Build-Test-Commit** 循环实现**单个**功能。

## 核心理念

- **一次只实现一个功能**，不贪多
- **智能测试**：简单功能跳过，复杂功能用 TDD 意图模式
- **进度自动保存**，支持随时中断和恢复
- **UI 风格一致**，使用项目定义的共享样式

## 快速流程

**简单功能**：Design → Build → Verify → Commit
**复杂功能**：Design → Intent → Test → Build → Verify → Commit

| 阶段 | 核心任务 | 检查点 |
|------|---------|--------|
| **Design** | UI 设计 + 复杂度评估 | 用户确认 Y/N |
| **Intent** | 定义测试意图（复杂功能） | 人工确认预期行为 |
| **Test** | 编写测试（可选） | 覆盖关键路径 |
| **Build** | 调研 → 编码 → UI 实现 | 使用共享样式 |
| **Verify** | 编译 + 手动测试 | 功能可运行 |
| **Commit** | 提交代码 | 清理进度文件 |

## 执行流程

### 0. 检查进度恢复

读取 `.claude/feat-progress.json`，如有未完成功能，提示是否继续。

### 1. 读取配置

加载 `docs/PRD.md` 和 `project-config.md` 获取功能清单和项目配置。

### 2. 选择功能

显示 PRD 中的功能清单，用户选择要实现的功能。

### 3. Design - UI 设计 + 复杂度评估

AI 自动生成布局方案，并评估复杂度：

**简单功能**（跳过测试）：
- 纯UI展示：改颜色、字体、间距、布局
- 简单导航：页面切换、按钮跳转
- 静态内容：标题、说明文字

**复杂功能**（需要测试意图）：
- 数据计算：统计、汇总、分析
- 状态流转：待办→完成→删除
- 边界逻辑：计时器、过期判断
- 跨模块：数据同步、转换

**确认提示**：
```markdown
🎨 布局设计: [功能名称]

布局: [垂直/水平/网格]
组件: [Button x2, TextBox x1]

🧪 复杂度评估:
- 类型: [简单/复杂]
- 建议测试: [跳过/需要测试意图]
- 理由: [为什么这样建议]

测试意图（如需要）:
1. [场景1]: 输入X，预期Y
2. [场景2]: 边界条件，预期结果

是否符合预期？(Y/N/调整)
```

### 4. Intent - 定义测试意图（仅复杂功能）

在写代码前，人工确认预期行为：

- AI 根据复杂度评估列出测试意图
- 用户审查、补充、确认
- 确认后进入 Test 阶段编写测试框架

详见 [tdd-intent.md](references/tdd-intent.md)

### 5. Test - 编写测试（可选）

**简单功能**：跳过此阶段，直接 Build

**复杂功能**：
1. 根据确认的测试意图编写测试框架
2. 用户审查测试（确认测的是"正确行为"）
3. 保存进度

详见 [testing-guide.md](references/testing-guide.md)

### 6. Build - 编码实现

**步骤**：
1. **调研**：先用 `mcp__context7` 查官方文档，再 WebSearch 搜索方案
2. **编码**：实现功能逻辑
3. **UI**：引用共享样式，不内联写死
4. **新样式**：如需新样式，先在样式文件定义，再引用

**关键**：复杂功能参考测试意图写代码，确保覆盖场景

### 7. Verify - 编译验证

执行 project-config.md 中的命令：
1. 构建
2. 运行测试（如有）
3. 手动测试（所有功能都需要）

**问题处理**：
- 阻塞性问题：返回 Build 修复
- 非阻塞性问题：标记到 PRD 已知问题，继续提交

### 8. Commit - 提交代码

```bash
git add .
git commit -m "feat: [功能名称]"
rm .claude/feat-progress.json
/compact
```

更新 PRD.md 标记功能完成。

## 文件结构

| 文件 | 用途 |
|------|------|
| [project-config.md](project-config.md) | 构建命令、样式系统、技术栈 |
| [design-guide.md](design-guide.md) | 设计规范、颜色、组件样式 |
| [references/workflow-guide.md](references/workflow-guide.md) | 详细工作流程 |
| [references/testing-guide.md](references/testing-guide.md) | 测试策略详解 |
| [references/tdd-intent.md](references/tdd-intent.md) | TDD意图模式详解 |
| [examples/feature-example.md](examples/feature-example.md) | 标准对话示例 |
| [examples/tdd-example.md](examples/tdd-example.md) | TDD模式示例 |

## 关键原则

1. **必须使用共享样式** - 禁止内联写死样式属性
2. **一次一个功能** - 不要并行开发多个功能
3. **先查后写** - 优先查官方文档，避免重复造轮子
4. **测试意图先行** - 复杂功能先定义预期行为，再写代码
5. **及时保存进度** - 每个阶段后更新 feat-progress.json
6. **完成清理** - commit 后删除进度文件，运行 /compact

## 进度管理

进度保存在 `.claude/feat-progress.json`，支持随时中断和恢复。

| Status | 说明 |
|--------|------|
| `design` | 正在进行 UI 设计 |
| `intent` | 定义测试意图（复杂功能） |
| `test` | 编写测试（复杂功能） |
| `build` | 编码实现 |
| `verify` | 编译验证 |
| `commit` | 准备提交 |
