# 分层测试策略

**核心原则**：测试意图先行，CRITICAL 冒烟测试，NON-CRITICAL 手动测试

---

## 功能分级标准

| 级别 | 定义 | 测试策略 | 时间 |
|------|------|---------|------|
| **CRITICAL** | 支付、认证、数据迁移、核心业务逻辑 | 冒烟测试 + 变异测试（可选）+ 手动测试 | 10-15分钟 |
| **NON-CRITICAL** | UI 调整、实验性功能、内部工具、展示类 | 手动测试 | 2-5分钟 |

### 判断标准

**CRITICAL**（必须冒烟测试）：
- ✅ 涉及金钱（支付、订单、退款、价格计算）
- ✅ 涉及数据安全（认证、权限、加密、登录）
- ✅ 涉及数据完整性（迁移、备份、删除、同步）
- ✅ 核心业务流程（影响主路径的关键功能）

**NON-CRITICAL**（手动测试即可）：
- ✅ UI 样式调整（颜色、字体、间距、圆角）
- ✅ 动画效果（hover、淡入淡出、过渡）
- ✅ 统计展示（报表、图表、汇总数据）
- ✅ 实验性功能（flag 控制、A/B 测试）
- ✅ 内部工具（容忍度高、用户少）

---

## CRITICAL 功能测试流程

### 1. Intent 阶段：生成测试代码（TDD 红灯）

**输入**：Design 阶段的 Given-When-Then

**输出**：5分钟冒烟测试代码

**关键**：测试代码在实现代码**之前**生成（TDD 先写测试）

```csharp
// 示例：番茄钟计时
[Fact]
public void Smoke_Timer_Starts_Counting()
{
    // Given: 番茄钟未启动，剩余时间 = 25:00
    var timer = new TomatoTimer();
    timer.SetDuration(25);

    // When: 用户点击"开始"按钮
    timer.Start();

    // Then: 倒计时启动，每秒更新显示
    Thread.Sleep(1000);
    Assert.Equal("24:59", timer.RemainingTime);
}

[Fact]
public void Smoke_Timer_Pauses()
{
    // Given: 番茄钟正在计时
    var timer = new TomatoTimer();
    timer.Start();

    // When: 用户点击"暂停"按钮
    timer.Pause();

    // Then: 时间停止
    var time1 = timer.RemainingTime;
    Thread.Sleep(1000);
    var time2 = timer.RemainingTime;
    Assert.Equal(time1, time2);
}

[Fact]
public void Smoke_Timer_Completes()
{
    // Given: 剩余时间 = 0:01
    var timer = new TomatoTimer();
    timer.SetDuration(0.0166); // 1 秒

    // When: 倒计时到 0:00
    timer.Start();
    Thread.Sleep(2000);

    // Then: 触发完成事件
    Assert.True(timer.IsCompleted);
}
```

**运行测试**：此时应该失败（代码还没实现）🔴

```bash
dotnet test ZtdApp.Tests --filter "FullyQualifiedName~Smoke"
```

### 2. Build 阶段：实现代码

根据测试意图实现功能代码，使测试通过。

### 3. Verify 阶段：运行测试（TDD 绿灯）

### 3.1 编译检查

```bash
dotnet build ZtdApp --configuration Release
```

### 3.2 运行冒烟测试

**此时应该通过** 🟢

```bash
dotnet test ZtdApp.Tests --filter "FullyQualifiedName~Smoke"
```

### 3.3 变异测试（可选，严格模式）

**目的**：验证测试真的能发现 bug

```bash
dotnet stryker --threshold-high 70 --threshold-break 40
```

**工作原理**：
- 修改你的代码（翻转逻辑、删除行）
- 运行测试看能否发现这些"bug"
- 分数 = 被测试发现的变异 / 总变异

**阈值**：
- > 70%：优秀
- 50-70%：良好
- < 40%：不及格（测试无效）

**何时使用**：
- 生产级关键功能
- 涉及金钱、安全的模块
- 复杂业务逻辑

**何时跳过**：
- MVP 快速迭代阶段
- 实验性功能
- UI 交互优化

### 3.4 手动测试完整流程

使用 Design 阶段的测试意图作为检查清单：

```markdown
- [ ] 场景1: 开始计时 - 点击开始，倒计时启动
- [ ] 场景2: 暂停功能 - 点击暂停，时间停止
- [ ] 场景3: 继续计时 - 点击继续，倒计时恢复
- [ ] 场景4: 计时完成 - 时间到0，锁屏
- [ ] 场景5: 撤回逻辑 - 10秒内可撤回
- [ ] 边界条件: 暂停超过30分钟自动结束
```

### 3.5 UI 一致性检查

使用 `project-config.md` 的检查清单：

```markdown
- [ ] 页面标题使用 PageTitleTextBlock 样式
- [ ] 卡片容器使用 CardBorder 样式
- [ ] 无内联 FontSize/Padding/Color
- [ ] 按钮颜色符合品牌规范
```

---

## NON-CRITICAL 功能测试流程

### 1. 编译检查

```bash
dotnet build ZtdApp --configuration Release
```

### 2. 手动测试清单（基于测试意图）

**输入**：Intent 阶段的 Given-When-Then

```markdown
📝 手动测试清单：

1. ✅ Hover 显示
   Given: 鼠标不在卡片上
   When: 鼠标进入卡片
   Then: 删除按钮渐变显示

2. ✅ Hover 隐藏
   Given: 删除按钮可见
   When: 鼠标离开卡片
   Then: 删除按钮渐变隐藏

3. ✅ 动画流畅
   When: 快速进出卡片
   Then: 动画平滑，不闪烁

测试结果：
- [ ] 场景1 通过
- [ ] 场景2 通过
- [ ] 场景3 通过
```

### 3. UI 样式检查

```markdown
- [ ] 颜色符合设计规范
- [ ] 字体大小正确
- [ ] 间距一致
- [ ] 动画时长合理（200ms 淡入，150ms 淡出）
```

---

## 质量门禁（可选）

### CI/CD 配置

```yaml
# .github/workflows/test-quality.yml
name: Test Quality

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build
        run: dotnet build ZtdApp --configuration Release

      - name: Run Tests
        run: dotnet test ZtdApp.Tests

      - name: Mutation Testing (CRITICAL only)
        if: contains(github.event.head_commit.message, '[CRITICAL]')
        run: dotnet stryker
        env:
          STRYKER_THRESHOLD_HIGH: 70
          STRYKER_THRESHOLD_LOW: 50
          STRYKER_THRESHOLD_BREAK: 40
```

---

## 常见陷阱

| 陷阱 | 描述 | 解决 |
|------|------|------|
| 看着代码写测试 | 测试只验证现有代码行为 | 测试意图先行 |
| 过度测试 | 简单UI功能写大量测试 | NON-CRITICAL 手动测试 |
| 追求 100% 覆盖率 | 测试代码比功能代码还多 | CRITICAL 70%+ 即可 |
| 快照测试陷阱 | 快照变了就改快照 | 用变异测试验证 |
| AI 自己定义测试意图 | AI 改变"正确行为" | 人工必须确认 |

---

## 测试数据

| 项目 | 代码覆盖率 | 变异分数 | 说明 |
|------|-----------|---------|------|
| AI 生成测试 | 91% | 34% ❌ | 高覆盖率但测试无效 |
| AI 生成测试 | 87% | 41% ❌ | 测试验证"当前行为" |
| 人工定义意图 + AI 实现 | 76% | 68% ✅ | 测试验证"正确行为" |

**结论**：代码覆盖率 ≠ 测试有效性

---

## 关键原则

1. **测试意图先行** - Design 阶段定义，人工确认
2. **Intent 阶段生成测试** - CRITICAL 功能在 Intent 阶段生成测试代码（TDD）
3. **TDD 红灯绿灯** - Intent 阶段测试失败 🔴，Verify 阶段测试通过 🟢
4. **AI 只实现测试代码** - 不修改测试意图
5. **CRITICAL 冒烟测试** - 5分钟核心流程
6. **NON-CRITICAL 手动测试** - 快速验证
7. **变异测试可选** - 生产级严格验证
8. **Given-When-Then 格式** - 结构化测试场景

---

## 参考资料

- [AI-Generated Tests Give False Confidence](https://codeintelligently.com/blog/ai-generated-tests-false-confidence)
- [How to Test AI-Generated Code the Right Way](https://www.twocents.software/blog/how-to-test-ai-generated-code-the-right-way)
- [Mutation Testing with Stryker](https://stryker-mutator.io/)
- [Smoke Testing 101](https://www.altexsoft.com/blog/smoke-testing/)
