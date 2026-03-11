# PRD 生成对话示例

> 完整的 /prd 技能使用示例

---

## 标准流程示例

```
用户: /prd 个人生产力工具

AI: 个人生产力工具，具体想解决什么问题？是时间管理、任务管理、还是其他？

用户: 想法收集和任务管理，类似 GTD

AI: 了解，GTD 风格的想法收集 + 任务管理。目标平台是什么？Web、桌面、还是手机？

用户: 先做 Windows 桌面

AI: Windows 桌面应用。让我先调研一下同类产品的做法...

    [调研结果]
    调研了以下参考方案：
    - Todoist：跨平台，Web 优先，功能全面
    - TickTick：原生桌面端，支持番茄钟
    - Obsidian：Electron 架构，插件生态强

    桌面端常见的技术选型：
    - WPF / WinUI 3（C# 原生，性能好）
    - Electron / Tauri（跨平台，Web 技术栈）

    你熟悉哪些语言/框架？这样我好推荐合适的方案。

用户: 熟悉 C#

AI: 那推荐 WPF + .NET 8：
    - 你熟悉 C#，零学习成本
    - WPF 成熟稳定，桌面应用首选
    - SQLite 本地存储，离线可用
    - MVVM 架构，代码结构清晰

    这个方案可以吗？

用户: 可以

AI: 好，MVP 功能你想要哪些？

    建议包含：
    - 想法快速收集
    - 任务列表管理
    - 今日待办

    后续可加：
    - 番茄钟
    - 每周回顾
    - 统计分析

    你觉得呢？

用户: 可以，MVP 加个笔记功能

AI: 好的，总结如下：

    📋 MVP 功能（5个）：
    1. 想法快速收集
    2. 任务列表管理
    3. 今日待办
    4. 笔记功能
    5. 番茄钟（基础版）

    技术栈：WPF + .NET 8 + SQLite + MVVM

    我这就生成 PRD.md 和项目配置？

用户: 好

AI: ✅ PRD 已生成: docs/PRD.md
    ✅ feat 项目配置已生成:
       - .claude/skills/feat/project-config.md
       - .claude/skills/feat/design-guide.md

    📋 内容摘要:
    - 核心价值: 个人 GTD 生产力管理，想法收集到任务执行
    - 参考方案: Todoist, TickTick, Obsidian
    - 技术栈: WPF + .NET 8 + SQLite + MVVM
    - 技术架构: 4层 MVVM (View/ViewModel/Service/Repository)
    - MVP 功能: 5 个

    需要调整吗？没问题的话可以开始 /feat 实现功能。
```

---

## 需求澄清示例

```
用户: /prd 做一个电商网站

AI: 电商网站涉及的功能比较多，我们先聚焦一下：

    1. 你想做 B2C（商家对消费者）还是 C2C（个人对个人）？
    2. 主要卖什么类型的商品？实物还是虚拟商品？
    3. 目标用户是谁？普通消费者还是特定人群？

用户: B2C，卖服装，面向年轻女性

AI: 了解，B2C 服装电商，面向年轻女性。

    这是一个相对复杂的项目，我建议分阶段：

    MVP 阶段（最小可用）：
    - 商品展示
    - 购物车
    - 基础订单流程

    后续再添加：
    - 用户注册登录
    - 支付集成
    - 订单管理
    - 后台管理

    你同意这个分阶段计划吗？还是想把某些功能提前到 MVP？
```

---

## 技术栈讨论示例

```
用户: /prd 一个博客系统

AI: 博客系统，有什么特殊要求吗？比如：
- 只是你自己用，还是支持多用户？
- 需要富文本编辑器，还是 Markdown 就够了？
- 需要评论功能吗？

用户: 多用户，Markdown，需要评论

AI: 了解，多用户 Markdown 博客，带评论。

    技术栈方面，有几个选择：

    方案 A：Next.js + PostgreSQL（推荐）
    - 服务端渲染，SEO 友好
    - React 生态成熟
    - 部署到 Vercel 很方便

    方案 B：纯静态（Hugo/Jekyll + Disqus 评论）
    - 更简单，部署成本更低
    - 但评论依赖第三方，自定义受限

    你倾向哪个方向？或者你有熟悉的技术栈？

用户: 熟悉 Python

AI: 那推荐 Django：
    - 你熟悉 Python，上手快
    - Django admin 可以直接做后台
    - 自带用户系统，省很多事
    - Markdown 渲染、评论功能都有成熟方案

    数据库用 PostgreSQL 还是 SQLite？
    - PostgreSQL：功能更强，适合后期扩展
    - SQLite：零配置，适合快速启动

用户: SQLite 吧，先快速启动

AI: 好的，技术栈确定：
    - 框架: Django + Python
    - 数据库: SQLite（后期可迁移到 PostgreSQL）
    - 前端: Django 模板（简洁，后续可加 Vue/React）
    - 部署: PythonAnywhere 或自己的服务器
```

> 以上示例展示了不同技术栈的对话流程。实际使用时根据用户需求和背景推荐合适的方案。
