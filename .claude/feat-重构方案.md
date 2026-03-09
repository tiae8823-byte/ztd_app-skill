# feat 技能重构方案

基于全网最佳实践（GitHub、英文高质量博客）的完整方案

---

## 🔍 核心发现

### 网上一致结论

**AI 生成测试的致命问题**：
- AI 看着代码写测试 = 验证"当前行为"而非"正确行为"
- 94% 代码覆盖率但生产崩溃（真实案例）
- 16/18 CTOs 报告 AI 代码导致生产灾难

**最佳实践共识**：
> Humans decide **what** to test, AI helps with **how** to test it.
> （人类决定测什么，AI 帮助怎么测）

---

## 📊 三种主流方案对比

| 方案 | 来源 | 适用场景 | 时间成本 | 可靠性 |
|------|------|---------|---------|--------|
| **Test-First AI Workflow** | codeintelligently.com | 生产级代码 | 中（7-10分钟）| 高（变异测试验证）|
| **AI Coding Loop** | freecodecamp.org | 中等复杂度 | 中（5-8分钟）| 中高 |
| **Smoke Testing System** | vibecoders | MVP 快速迭代 | 低（2-5分钟）| 中（手动测试为主）|

---

## ✅ 优化后的完整方案

### 核心流程

```
Design → Intent → Build → Verify → Commit
  ↓        ↓        ↓        ↓
调研UI   定义预期  实现     验证
         (人工)          (分层测试)
```

### 关键改进点

1. **测试意图先行**（保留我的方案）✅
2. **用 Given-When-Then 格式**（借鉴 ATDD）
3. **AI 只实现测试代码，不定义测试意图**（网上共识）
4. **引入质量门禁**（变异测试）
5. **明确 CRITICAL vs NON-CRITICAL 标准**

---

## 📝 详细设计

### 1. Intent 阶段（测试意图先行）

**强制格式：Given-When-Then**（借鉴 ATDD）

```markdown
📝 测试意图（请确认）：

功能级别: [CRITICAL / NON-CRITICAL]

测试场景:

1. ✅ 开始计时
   Given: 番茄钟未启动
   When: 用户点击开始
   Then: 倒计时启动，每秒更新显示

2. ✅ 暂停功能
   Given: 番茄钟正在计时
   When: 用户点击暂停
   Then: 时间停止，可继续

3. ✅ 计时完成
   Given: 剩余时间 = 0
   When: 计时结束
   Then: 触发完成事件，锁屏

4. ⚠️ 撤回逻辑
   Given: 计时完成 < 10秒
   When: 用户点击撤回
   Then: 不计入统计

边界条件:
- 暂停超过 30 分钟 → 自动结束
- 计时过程中关闭应用 → 恢复状态

这些场景对吗？需要补充吗？
```

**关键**：
- 用 Given-When-Then 格式（强制）
- 人工必须确认（强制）
- 明确标注功能级别（CRITICAL/NON-CRITICAL）

---

### 2. Build 阶段（参考测试意图实现）

```markdown
参考"测试意图 1: 开始计时"实现 Start() 方法
参考"测试意图 2: 暂停功能"实现 Pause() 方法
...

编码完成。
```

**调研策略**（贯穿全流程）：
```markdown
优先级：
1. GitHub 真实项目（`mcp__github__search_code`）
2. 高质量博客（`mcp__exa__web_search_exa`）
3. 官方文档（`mcp__context7__query-docs`）

Design 阶段：搜索 UI 模式
Build 阶段：搜索实现方案
```

---

### 3. Verify 阶段（分层验证）

#### 方案 A：CRITICAL 功能（完整流程）

```markdown
1. 编译检查
   ```bash
   dotnet build ZtdApp --configuration Release
   ```

2. AI 生成测试代码（基于测试意图）
   - AI 读取 Intent 阶段的 Given-When-Then
   - AI 只实现测试代码，不修改测试意图
   - 示例：
   ```csharp
   [Fact]
   public void Timer_Starts_Counting() // 意图1
   {
       // Given: 番茄钟未启动
       var timer = new TomatoTimer();

       // When: 用户点击开始
       timer.Start(25);

       // Then: 倒计时启动，每秒更新显示
       Thread.Sleep(1000);
       Assert.Equal("24:59", timer.RemainingTime);
   }
   ```

3. 运行测试
   ```bash
   dotnet test ZtdApp.Tests --filter "FullyQualifiedName~Smoke"
   ```

4. 变异测试（可选，严格模式）
   ```bash
   # 检查测试是否真的能发现 bug
   dotnet stryker --threshold-high 70 --threshold-break 40
   ```

5. 手动测试完整流程
   - [ ] 启动应用
   - [ ] 测试核心流程
   - [ ] 验证边界条件

6. UI 一致性检查
   - 使用 project-config.md 检查清单
```

#### 方案 B：NON-CRITICAL 功能（快速流程）

```markdown
1. 编译检查

2. 手动测试清单（基于测试意图）
   - [ ] 意图1: hover 显示删除按钮
   - [ ] 意图2: 淡入淡出动画流畅
   - [ ] 意图3: 卡片样式符合设计规范

3. UI 样式检查
```

---

### 4. 功能分级标准

| 级别 | 定义 | 测试策略 | 时间 |
|------|------|---------|------|
| **CRITICAL** | 支付、认证、数据迁移、核心业务逻辑 | 冒烟测试 + 变异测试（可选）+ 手动测试 | 10-15分钟 |
| **NON-CRITICAL** | UI 调整、实验性功能、内部工具、展示类 | 手动测试 | 2-5分钟 |

**判断标准**：

```markdown
CRITICAL（必须冒烟测试）：
- 涉及金钱（支付、订单、退款）
- 涉及数据安全（认证、权限、加密）
- 涉及数据完整性（迁移、备份、删除）
- 核心业务流程（影响主路径）

NON-CRITICAL（手动测试即可）：
- UI 样式调整（颜色、字体、间距）
- 动画效果（hover、淡入淡出）
- 统计展示（报表、图表）
- 实验性功能（flag 控制）
- 内部工具（容忍度高）
```

---

## 🎯 质量门禁（可选）

### 变异测试阈值

```yaml
# .github/workflows/test-quality.yml
- name: Mutation Testing
  run: dotnet stryker
  env:
    STRYKER_THRESHOLD_HIGH: 70    # 优秀
    STRYKER_THRESHOLD_LOW: 50     # 及格
    STRYKER_THRESHOLD_BREAK: 40   # 低于此值失败
```

**说明**：
- 变异测试会修改你的代码（翻转逻辑、删除行）
- 检查测试是否能发现这些"bug"
- 分数 > 60% 表示测试真的有效

---

## 📋 完整示例

### 场景：实现"番茄钟计时"（CRITICAL）

```markdown
用户: /feat 番茄钟计时

AI: 🎨 Design 完成
    调研来源:
    - GitHub: WPF timer implementation (5个高星项目)
    - 博客: C# stopwatch best practices

    布局: [卡片 + 计时器 + 控制按钮]

    🧪 复杂度评估:
    - 类型: 复杂（时间逻辑）
    - 功能级别: CRITICAL（核心功能）
    - 测试策略: 冒烟测试 + 变异测试

    📝 测试意图（请确认）：

    1. ✅ 开始计时
       Given: 番茄钟未启动
       When: 用户点击开始
       Then: 倒计时启动，每秒更新显示

    2. ✅ 暂停功能
       Given: 番茄钟正在计时
       When: 用户点击暂停
       Then: 时间停止，可继续

    3. ✅ 计时完成
       Given: 剩余时间 = 0
       When: 计时结束
       Then: 触发完成事件，锁屏

    4. ⚠️ 撤回逻辑
       Given: 计时完成 < 10秒
       When: 用户点击撤回
       Then: 不计入统计

    边界条件:
    - 暂停超过 30 分钟 → 自动结束

    这些场景对吗？

用户: 对，再加：已完成的计时不能撤回

AI: ✅ 测试意图已更新
    5. ⚠️ 已完成不可撤回
       Given: 计时已完成
       When: 用户点击撤回
       Then: 撤回按钮不可用

    现在开始实现...

    [Build 阶段]
    参考"测试意图 1"实现 Start() 方法...
    参考"测试意图 2"实现 Pause() 方法...
    编码完成。

    [Verify 阶段]
    1. 编译通过 ✓
    2. 生成测试代码（基于测试意图）...
    3. 运行测试 ✓
    4. 变异测试: 72%（通过）✓
    5. 手动测试 ✓

    [Commit]
    feat: 番茄钟计时
    更新 PRD ✓
    清理进度 ✓
```

**总耗时**：50分钟（Build 30 + Verify 20）

---

## 🔑 关键原则（最终版）

1. **测试意图先行** - 先定义预期，再实现（强制）
2. **人工确认测试意图** - 避免AI自己定义"正确行为"（强制）
3. **Given-When-Then 格式** - 结构化测试场景（强制）
4. **AI 只实现测试代码** - 不修改测试意图（强制）
5. **调研贯穿全流程** - GitHub > 博客 > 官方文档
6. **分层测试** - CRITICAL 冒烟测试，NON-CRITICAL 手动测试
7. **质量门禁** - 变异测试验证测试有效性（可选）
8. **必须使用共享样式** - 禁止内联（强制）

---

## 📊 方案对比（最终版）

| 项目 | 旧版 feat | 网上最佳实践 | 优化后 feat |
|------|----------|-------------|------------|
| **调研** | Build 阶段查文档 | 全流程搜索 | 全流程搜索（GitHub > 博客 > 文档）|
| **测试意图** | 无 | ATDD Given-When-Then | Given-When-Then + 人工确认 |
| **AI 角色** | 自己写测试 | AI 实现测试代码 | AI 实现测试代码，不定义意图 |
| **测试策略** | TDD 意图模式 | 分层（CRITICAL/NON-CRITICAL）| 分层 + 变异测试（可选）|
| **质量验证** | 无 | 变异测试 | 变异测试（可选）|
| **约束力** | 弱 | 强（质量门禁）| 强（强制检查点）|
| **MVP 友好** | 中（TDD 太重）| 高（轻量级）| 高（CRITICAL 严格，NON-CRITICAL 轻量）|

---

## 🎯 立即行动

你希望我：

1. **立即重构 feat 技能**？
   - 更新 skill.md（精简到 100 行）
   - 更新 testing-guide.md（Given-When-Then 格式）
   - 添加 workflow-guide.md（完整流程）
   - 删除 TDD 相关过时内容

2. **还是先看完整的文件对比**？
   - 我可以生成 diff
   - 或者先重构一个文件让你看效果

**这个方案结合了网上的最佳实践，同时保持 MVP 的轻量级特点，你觉得可行吗？**
