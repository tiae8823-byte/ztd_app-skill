# TDD 对话示例

> 本文档提供 TDD 版 feat 技能的对话示例，帮助理解使用方式。

## 正常 TDD 流程示例

```
用户: /feat

AI: 📋 当前功能清单：

    MVP 功能：
    1. [ ] 想法收集
    2. [ ] 待办列表
    3. [ ] 今日待办

    做哪个？

用户: 1

AI: 🎨 布局设计 + 测试意图: 想法收集

    自动生成的设计方案:

    页面布局: 垂直布局
    使用组件: Button x1, TextBox x1, CardBorder, ItemsControl
    使用样式: Button (橙色主按钮), TextBox (聚焦橙色), CardBorder

    布局草图:
    ┌─────────────────────────┐
    │  想法收集               │
    │  快速记录你的灵感        │
    │                         │
    │  [输入框          ][添加]│
    │                         │
    │  ┌─ 卡片 ────────────┐ │
    │  │ 想法内容...       │ │
    │  │ 2026-03-01  [删除]│ │
    │  └───────────────────┘ │
    └─────────────────────────┘

    🧪 测试意图:
    1. 输入内容点击添加 → 数据持久化到数据库
    2. 输入为空点击添加 → 不创建条目
    3. 创建成功后 → 列表立即显示新条目
    4. 超长内容（500+字符）→ 正常保存
    5. 特殊字符（emoji、换行）→ 正常处理
    6. 点击删除 → 数据库同步清除
    7. 重启应用 → 数据仍然存在

    是否符合预期？(Y/N)

用户: Y

AI: 🔴 Test 阶段: 编写测试 (Red)

    正在基于测试意图编写测试...

    [创建 IdeaServiceTests.cs]
    [创建 IdeaViewModelTests.cs]

    运行测试确认 Red...

    🔴 Red 确认:
    测试文件: IdeaServiceTests.cs, IdeaViewModelTests.cs
    测试用例: 10 个
    状态: 全部 Red ✓（IdeaManager 类不存在）

      ✗ CreateIdea_WithValidContent_ShouldPersist
      ✗ CreateIdea_WithEmptyContent_ShouldNotCreate
      ✗ CreateIdea_ShouldAppearInList
      ✗ CreateIdea_WithLongContent_ShouldSucceed
      ✗ CreateIdea_WithSpecialChars_ShouldSucceed
      ✗ DeleteIdea_ShouldRemoveFromDb
      ✗ DeleteIdea_ShouldUpdateList
      ✗ GetAllIdeas_ShouldReturnOrdered
      ✗ ViewModel_AddCommand_ShouldCreateIdea
      ✗ ViewModel_DeleteCommand_ShouldRemoveIdea

    全部 Red，开始实现代码？(Y)

用户: Y

AI: 🟢 Build 阶段: 实现代码 (Green)

    先调研实现方案...
    参考: CommunityToolkit.Mvvm 官方文档 (ObservableProperty, RelayCommand)

    逐步实现，让测试从 Red → Green:

    1. 创建 Models/Idea.cs → 2 个测试 Green
    2. 创建 Data/IdeaRepository.cs → 4 个测试 Green
    3. 创建 Services/IdeaManager.cs → 6 个测试 Green
    4. 创建 ViewModels/IdeaViewModel.cs → 8 个测试 Green
    5. 实现 UI (MainWindow.xaml 想法页面模板) → 10 个测试 Green

    🟢 Green 确认:
    测试状态: 10/10 通过 ✓

      ✓ CreateIdea_WithValidContent_ShouldPersist
      ✓ CreateIdea_WithEmptyContent_ShouldNotCreate
      ✓ CreateIdea_ShouldAppearInList
      ✓ CreateIdea_WithLongContent_ShouldSucceed
      ✓ CreateIdea_WithSpecialChars_ShouldSucceed
      ✓ DeleteIdea_ShouldRemoveFromDb
      ✓ DeleteIdea_ShouldUpdateList
      ✓ GetAllIdeas_ShouldReturnOrdered
      ✓ ViewModel_AddCommand_ShouldCreateIdea
      ✓ ViewModel_DeleteCommand_ShouldRemoveIdea

    开始重构？(Y/跳过)

用户: Y

AI: 🔧 Refactor 阶段

    检查清单:
    ✅ 无重复代码
    ✅ 命名清晰
    ✅ 方法长度合理
    ✅ 无内联样式
    ✅ 测试仍然全部 Green (10/10)

    无需大改。进入验证阶段。

    [Verify: 编译成功 + 全部测试通过 + 手动测试正常]

    ✅ 想法收集 完成（TDD）

    测试覆盖: 10 个测试用例全部通过
    Commit: feat: 想法收集

    建议运行 /compact 清理上下文

    继续下一个功能？
```

---

## TDD 修复 Bug 示例

```
用户: /feat

AI: [Verify 阶段手动测试中发现问题]

    ⚠️ 手动测试发现问题:
    输入包含换行符的内容时，卡片显示异常（高度不正确）

    处理选项:
    1. TDD 修复（推荐）：先写重现测试，再修复
    2. 标记为已知问题
    3. 直接提交

用户: 1

AI: 🔴 TDD 修复: 换行符显示问题

    写重现测试:
      ✗ CreateIdea_WithNewlines_ShouldDisplayCorrectly

    运行确认 Red ✓

    修复代码: TextWrapping + 自适应高度

    🟢 测试通过:
      ✓ CreateIdea_WithNewlines_ShouldDisplayCorrectly
      ✓ 其余 10 个测试仍然 Green

    继续 Verify...
```

---

## 进度恢复示例

```
用户: /feat

AI: 📋 检测到未完成的功能：

    当前功能: 待办列表
    状态: build（已完成: design, test）
    测试状态: 8 个测试全部 Red，等待实现
    设计方案: 垂直布局, CheckBox, CardBorder, FilterChip
    创建时间: 2026-03-01 14:30

    是否继续？
    - Y: 继续开发（从 Build 阶段开始，让测试变 Green）
    - N: 重新开始

用户: Y

AI: 🟢 继续 Build: 待办列表

    当前进度: 8 个测试全部 Red
    开始逐个实现...

    [继续 Build → Refactor → Verify → Commit 流程]
```

---

## 输出格式示例

```markdown
🎨 实现功能: [功能名称]（TDD）

## Design
✅ 布局: [垂直/水平/网格]
✅ 组件: [Button x2, TextBox x1, CardBorder]
✅ 样式: [BrandColors.xaml 自动应用]
✅ 测试意图: [N] 条行为期望

## Test (Red)
🔴 测试: [数量] 个用例，全部 Red

## Build (Green)
🟢 实现: [数量] 个测试全部 Green
✅ 创建: [文件列表]

## Refactor
✅ 代码清理完成，测试仍然 Green

## Verify
✅ 编译成功
✅ 全部测试通过: [数量] 个
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
