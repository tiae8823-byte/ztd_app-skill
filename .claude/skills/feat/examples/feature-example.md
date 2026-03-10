# 功能实现对话示例

展示 /feat 的完整交互流程。

---

## CRITICAL 功能流程（完整 TDD）

```
用户: /feat

AI: 📋 当前功能清单：
    1. [ ] 番茄钟计时
    2. [ ] 想法收集
    做哪个？

用户: 1

AI: 🎨 Design 阶段: 番茄钟计时

    布局: 垂直布局
    组件: TimerDisplay, Button x3

    ┌─────────────────────────┐
    │  番茄钟                 │
    │       25:00             │
    │  [开始] [暂停] [重置]   │
    └─────────────────────────┘

    🧪 复杂度评估: CRITICAL（核心业务流程）

    📝 测试意图（请确认）：

    1. ✅ 开始计时
       Given: 番茄钟未启动，剩余时间 = 25:00
       When: 用户点击"开始"按钮
       Then: 倒计时启动，每秒更新显示

    2. ✅ 暂停功能
       Given: 番茄钟正在计时
       When: 用户点击"暂停"按钮
       Then: 时间停止

    3. ✅ 计时完成
       Given: 剩余时间 = 0:01
       When: 倒计时到 0:00
       Then: 触发完成事件

    边界条件:
    - 暂停超过 30 分钟: 自动结束

    这些场景对吗？需要补充吗？

用户: 对，继续

AI: 📋 Intent 阶段（CRITICAL）

    ✅ 测试意图已确认
    📝 生成冒烟测试代码...

    // Smoke_Timer_Starts_Counting
    // Smoke_Timer_Pauses
    // Smoke_Timer_Completes

    运行测试: 🔴 3 failed（预期，代码未实现）

    进入 Build 阶段...

AI: 🔨 Build 阶段

    实现方案参考:
    - GitHub: [WPF DispatcherTimer 示例]
    - 官方文档: [Microsoft Docs Timer]

    编码中...

AI: ✅ Verify 阶段

    1. 编译检查: ✅ 通过
    2. 运行测试: 🟢 3 passed
    3. 手动测试:
       - [x] 场景1: 开始计时
       - [x] 场景2: 暂停功能
       - [x] 场景3: 计时完成
    4. UI 检查: ✅ 符合规范

AI: 📦 Commit 阶段

    git commit -m "feat: 番茄钟计时"
    git push origin main

    ✅ 番茄钟计时 完成
    建议运行 /compact 清理上下文
```

---

## NON-CRITICAL 功能流程（跳过测试代码）

```
用户: /feat → 修改想法卡片颜色

AI: 🎨 Design 阶段: 修改想法卡片颜色

    布局: 复用现有卡片
    修改: CardBorder 背景色

    🧪 复杂度评估: NON-CRITICAL（UI 调整）

    📝 测试意图（请确认）：

    1. ✅ 颜色正确
       Given: 想法卡片存在
       When: 页面加载
       Then: 背景色为 BrandLightGray

    这些场景对吗？

用户: 对

AI: 🚀 NON-CRITICAL 功能，跳过测试代码生成
    直接进入 Build 阶段...

AI: 🔨 Build 阶段

    修改 BrandColors.xaml...

AI: ✅ Verify 阶段

    1. 编译检查: ✅ 通过
    2. 手动测试:
       - [x] 颜色正确
    3. UI 检查: ✅ 符合规范

AI: 📦 Commit 阶段

    git commit -m "feat: 修改想法卡片颜色"
    git push origin main

    ✅ 修改想法卡片颜色 完成
```

---

## 进度恢复

```
用户: /feat

AI: 📋 检测到未完成的功能：

    当前功能: 番茄钟计时
    功能级别: CRITICAL
    状态: build（已完成: design, intent）
    测试意图:
      1. 开始计时: 点击开始 → 倒计时启动
      2. 暂停功能: 点击暂停 → 倒计时停止
    最后记录: Intent 阶段完成，已生成测试代码
    创建时间: 2026-02-27 14:30

    是否继续？
    - Y: 继续开发
    - N: 重新开始

用户: Y

AI: 🔨 继续实现: 番茄钟计时
    [继续 Build → Verify → Commit 流程]
```

---

## TDD 红灯-绿灯循环示意

```
Design 阶段
    │ 定义测试意图（Given-When-Then）
    ↓
Intent 阶段（仅 CRITICAL）
    │ 生成测试代码
    │ 运行测试 → 🔴 失败（预期）
    ↓
Build 阶段
    │ 实现代码
    ↓
Verify 阶段
    │ 运行测试 → 🟢 通过
    │ 手动测试
    ↓
Commit 阶段
    │ 提交代码
    ↓
完成
```
