# Vibe Coding 软件开发经验

> 独立开发者 + AI 辅助的 MVP 开发实践总结。
> 持续更新，记录在实际项目中验证过的经验。

---

## 1. 整体开发流程

```
PRD（产品设计）→ feat（逐个功能实现）→ 迭代
```

**两个核心技能**：
- `/prd`：通过对话确定产品需求，输出 PRD 文档
- `/feat`：一次实现一个功能，走 Design-Build-Test-Commit 循环

**关键原则**：PRD 是活的，随时可以加功能、改需求。

---

## 2. UI/样式属于技术栈，不要单独拆

### 经验

UI 组件库和样式体系本质上是技术栈的一部分，应该在选技术栈时一起确定，而不是单独作为一个"设计"步骤。

### 之前的问题

```
PRD 步骤 3: 选技术栈（框架、数据库）
PRD 步骤 4: 选 UI 风格（组件库、样式）    ← 重复
feat Design: 选组件和样式                  ← 又重复
```

三个地方都在做样式决策，浪费时间且容易不一致。

### 正确做法

```
PRD: 选技术栈时一并确定 UI 方案（框架 + 组件库 + 样式体系）
feat Design: 只画布局（什么组件放哪里），样式直接用 PRD 定好的
feat Build: 编码时直接套用，不需要再做样式选择
```

### 参考

业界共识：先选好 UI 组件库再开始编码，MVP 阶段用现成组件库，不要从零写样式。独立开发者应优先选择预置样式的组件库（如 MUI、Mantine），减少设计决策。

---

## 3. 先调研再动手

### 经验

不管是产品设计还是技术实现，动手之前先看看别人怎么做的。

### 两个层面的调研

| 阶段 | 调研什么 | 目的 |
|------|---------|------|
| **PRD 阶段** | 同类产品的功能设计、交互模式 | 优化产品设计，借鉴成熟的 UX |
| **feat Build 阶段** | 技术实现方案、官方文档 | 写出更好的代码，减少返工 |

### 资料查找优先级

```
1. 官方技术文档（最权威，最准确）
2. 网上成熟方案（博客、Stack Overflow、GitHub）
3. 自行实现（以上都没找到合适方案时）
```

### 避免的坑

- 不调研就动手 → 重复造轮子，代码质量差
- 只看一篇文章就跟着做 → 可能是过时方案
- 调研太久不动手 → MVP 要快，快速了解主流做法就够了

---

## 4. 技术栈选择不要用固定模板

### 经验

技术选型应该根据实际需求分析推荐，而不是从一个固定的"推荐表"里选。

### 分析维度

- **产品类型**：Web / 桌面 / 移动 / 跨平台
- **用户规模**：个人 / 团队 / 公开
- **团队背景**：熟悉什么语言和框架（减少学习成本）
- **离线需求**：是否需要离线使用
- **交互复杂度**：简单 / 中等 / 复杂

### 为什么不用固定表

我们实际项目是 WPF + .NET 8 桌面应用，但之前的推荐表只列了 Next.js/Electron/Tauri 等 Web 相关方案，完全不适用。让 AI 根据需求分析推荐，覆盖面更广，结果更准确。

---

## 5. 增量更新用 /feat 带描述调用

### 经验

MVP 功能做完后想给它加子功能，不需要改技能或流程。

### 操作方式

```
/feat 想法收集 - 增加搜索功能
```

直接带描述调用，AI 会自动把新功能加到 PRD，然后走正常的 Design-Build-Test-Commit 流程。

### 注意

直接调用 `/feat`（不带描述）只显示 PRD 中未完成的功能列表。已完成模块的增量需求不会自动出现，所以必须带上描述。

---

## 6. MVP 阶段的设计原则

### 功能优先，样式其次

1. 先让功能跑通
2. 用组件库保证基本一致性
3. 视觉打磨放到后面迭代

> "Working in grayscale first totally changed how fast I ship new features." — 某独立开发者

### 保持简单

- 一次只做一个功能
- 不追求完美，先做出来再优化
- 每个功能完成后立即提交，不要积攒大改动

### 测试要做但不追求完美

- 核心逻辑写单元测试
- 边界条件要覆盖
- UI 细节靠手动验证

---

## 7. 工具链总结

| 工具 | 用途 | 何时用 |
|------|------|--------|
| `/prd` | 生成产品需求文档 | 项目启动时 |
| `/feat` | 实现单个功能 | 每次开发功能时 |
| `context7` | 查官方技术文档 | Build 阶段编码前 |
| `WebSearch` | 搜索产品方案和技术方案 | PRD 调研 + Build 调研 |
| `feat-progress.json` | 保存开发进度 | 自动，支持中断恢复 |

---

## 参考资料

- [A Simple Framework for Designing UIs (Solo Devs & Small Teams)](https://dev.to/shayy/a-simple-framework-for-designing-user-interfaces-for-solo-devs-small-teams-36hj)
- [MVP UI/UX Design Guide 2026](https://dbbsoftware.com/insights/mvp-ui-ux-design-guide)
- [Software Development Process: 7 Phases Explained](https://monday.com/blog/rnd/software-development-process/)
- [What Is a UI Component Library and When to Use It?](https://pagepro.co/blog/what-is-a-ui-component-library/)
