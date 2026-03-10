# 测试意图先行

**核心原则**：人工定义预期，AI 实现代码

---

## 为什么必须测试意图先行

### ❌ 错误做法：AI 看着代码写测试

```markdown
1. AI 写代码
2. AI 看着代码生成测试
3. 测试通过 ✌️

问题：
- 测试验证的是"当前行为"，不一定是"正确行为"
- 代码有 bug，测试也验证这个 bug
- 94% 代码覆盖率但生产崩溃（真实案例）
```

### ✅ 正确做法：测试意图先行

```markdown
1. 人工定义测试意图（Given-When-Then）
2. 人工确认测试意图
3. AI 根据意图实现代码
4. AI 根据意图生成测试（仅 CRITICAL）
5. 测试验证"正确行为"

关键：
- 测试意图在代码**前**定义
- 人工必须确认
- AI 不修改测试意图
```

---

## Given-When-Then 格式（强制）

借鉴 ATDD（验收测试驱动开发）的标准格式：

```markdown
📝 测试意图（请确认）：

功能级别: [CRITICAL / NON-CRITICAL]

测试场景:

1. ✅ [场景名称]
   Given: [初始状态/前置条件]
   When: [用户操作/输入]
   Then: [预期结果]

2. ✅ [场景名称]
   Given: [初始状态/前置条件]
   When: [用户操作/输入]
   Then: [预期结果]

边界条件:
- [边界场景]: [描述]

异常处理:
- [异常情况]: [预期行为]
```

---

## 实际示例

### 示例 1：番茄钟计时（CRITICAL）

```markdown
📝 测试意图（请确认）：

功能级别: CRITICAL（核心业务流程）

测试场景:

1. ✅ 开始计时
   Given: 番茄钟未启动，剩余时间 = 25:00
   When: 用户点击"开始"按钮
   Then: 倒计时启动，每秒更新显示（24:59 → 24:58 → ...）

2. ✅ 暂停功能
   Given: 番茄钟正在计时，剩余时间 = 15:30
   When: 用户点击"暂停"按钮
   Then: 时间停止在 15:30，"继续"按钮可用

3. ✅ 继续计时
   Given: 番茄钟已暂停，剩余时间 = 15:30
   When: 用户点击"继续"按钮
   Then: 倒计时继续，从 15:29 开始

4. ✅ 计时完成
   Given: 剩余时间 = 0:01
   When: 倒计时到 0:00
   Then: 触发完成事件，锁屏，计入统计

5. ⚠️ 撤回逻辑
   Given: 计时完成 < 10秒
   When: 用户点击"撤回"
   Then: 不计入统计，恢复到待办列表

边界条件:
- 暂停超过 30 分钟: 自动结束，不计入统计
- 已完成的计时: 不可撤回

异常处理:
- 应用重启: 恢复计时状态（暂停/进行中）

这些场景对吗？需要补充吗？
```

### 示例 2：删除按钮动画（NON-CRITICAL）

```markdown
📝 测试意图（请确认）：

功能级别: NON-CRITICAL（UI 交互优化）

测试场景:

1. ✅ Hover 显示
   Given: 鼠标不在卡片上，删除按钮 Opacity = 0
   When: 鼠标进入卡片
   Then: 删除按钮 Opacity 渐变到 1（200ms，EaseOut）

2. ✅ Hover 隐藏
   Given: 删除按钮 Opacity = 1
   When: 鼠标离开卡片
   Then: 删除按钮 Opacity 渐变到 0（150ms，EaseIn）

边界条件:
- 快速进出: 动画平滑过渡，不闪烁

这些场景对吗？
```

---

## AI 角色限制 ⚠️

### ✅ AI 可以做的

```markdown
1. 根据功能描述生成测试意图草稿
2. 根据人工确认的测试意图实现代码
3. 根据测试意图生成测试代码（仅 CRITICAL）
```

### ❌ AI 禁止做的

```markdown
1. 自己修改测试意图（必须人工确认）
2. 看着代码生成测试（测试意图必须在代码前）
3. 跳过人工确认直接实现
```

---

## 流程图

```
Design 阶段
    ↓
AI 生成测试意图草稿
    ↓
人工审查、补充、确认 ⚠️（关键）
    ↓
CRITICAL → Intent 阶段（确认测试意图）
    ↓         ↓
    ↓    AI 生成冒烟测试代码
    ↓         ↓
    ↓    运行测试 → 🔴 失败（预期）
    ↓
Build 阶段（AI 参考测试意图实现代码）
    ↓
Verify 阶段
    ↓
运行测试 → 🟢 通过
    ↓
手动测试 + UI 检查
```

---

## 关键原则

1. **测试意图必须在代码前定义**
2. **人工必须确认测试意图**
3. **Given-When-Then 格式强制**
4. **AI 不修改测试意图**
5. **测试意图用中文写**（作为需求文档）

---

## 参考资料

- [ATDD（验收测试驱动开发）](https://en.wikipedia.org/wiki/Acceptance_test-driven_development)
- [Given-When-Then 格式](https://martinfowler.com/bliki/GivenWhenThen.html)
- [AI-Generated Tests Give False Confidence](https://codeintelligently.com/blog/ai-generated-tests-false-confidence)
