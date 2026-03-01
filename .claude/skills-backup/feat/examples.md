# 对话示例

> 本文档提供 feat 技能的对话示例，帮助理解使用方式。

## 正常流程示例

```
用户: /feat

AI: 📋 当前功能清单：

    MVP 功能：
    1. [ ] 用户注册
    2. [ ] 用户登录
    3. [ ] 发布文章

    做哪个？

用户: 2

AI: 🎨 UI 设计: 用户登录

    自动生成的设计方案:

    页面布局: 垂直布局
    使用组件: Button x1, TextBox x2, HeadingTextBlock
    使用样式: Button (橙色主按钮), TextBox (聚焦橙色)

    布局草图:
    ┌─────────────────────────┐
    │  用户登录               │
    │  请输入你的账号          │
    │                         │
    │  [用户名输入框]         │
    │  [密码输入框]           │
    │                         │
    │  [登录按钮]             │
    └─────────────────────────┘

    是否符合预期？(Y/N)

用户: Y

AI: [Build → Test → Verify → Commit]

    ✅ 用户登录 完成

    设计方案: 垂直布局, Button, TextBox x2, HeadingTextBlock
    编译: dotnet build --configuration Release
    测试: dotnet test
    UI 一致性: 通过
    手动测试: 功能验证正常
    Commit: feat: 用户登录

    建议运行 /compact 清理上下文

    继续下一个功能？
    - 输入序号继续
    - 或告诉我新需求
    - 或说"结束"退出
```

---

## 进度恢复流程示例

```
用户: /feat

AI: 📋 检测到未完成的功能：

    当前功能: 想法收集
    状态: verify（已完成: design, build, test）
    最后记录: Test阶段完成，等待编译验证
    设计方案: 垂直布局, Button, TextBox, CardBorder
    创建时间: 2026-02-27 14:30

    是否继续？
    - Y: 继续开发
    - N: 重新开始

用户: Y

AI: 🔨 继续实现: 想法收集

    当前进度: verify（已完成设计、代码、测试）
    开始编译验证...

[继续 Verify → Commit 流程]
```

---

## 需求变更示例

```
用户: /feat

AI: 📋 当前功能清单：

    MVP 功能：
    1. [ ] 用户注册
    2. [ ] 用户登录
    3. [ ] 发布文章

    做哪个？

用户: 等等，文章还得能删除

AI: 好，加到 PRD 里：
    - [ ] 删除文章

    先做删除还是继续当前功能？

用户: 先做删除

AI: 🎨 UI 设计: 删除文章
    自动生成设计方案...

    页面布局: 独立确认对话框
    使用组件: Button x2 (确认/取消), TextBlock
    使用样式: Button (橙色), SecondaryButton (蓝色)

    是否符合预期？(Y/N)

用户: Y

AI: [Build → Test → Verify → Commit]

    ✅ 删除文章 完成

    设计方案: 独立对话框, Button x2, TextBlock
    编译: dotnet build --configuration Release
    测试: dotnet test
    UI 一致性: 通过
    手动测试: 功能验证正常
    Commit: feat: 删除文章
```

---

## 输出格式示例

```markdown
🎨 实现功能: [功能名称]

## Design
✅ 布局: [垂直/水平/网格]
✅ 组件: [Button x2, TextBox x1, CardBorder]
✅ 样式: [BrandColors.xaml 自动应用]

## Build
✅ 创建: [文件列表]

## Test
✅ 测试: [数量] 个用例

## Verify
✅ 编译成功: dotnet build --configuration Release
✅ 单元测试通过: dotnet test
✅ UI 一致性检查: 通过
✅ 手动测试完成: 功能验证正常

## Commit
✅ feat: [功能名称]
✅ 版本记录: .claude/version-history.log

## 更新 PRD
✅ 已标记完成

---

建议运行 /compact 清理上下文

继续下一个？(输入序号 / 新需求 / 结束)
```
