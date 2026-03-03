# TDD 重构报告

> 从 DBTVC（传统）到 DTBRVC（TDD）的重构总结，含改进方案。

---

## 一、重构概况

### 做了什么

将现有 DBTVC（Design-Build-Test-Verify-Commit）循环重构为 TDD 版本 DTBRVC（Design-Test-Build-Refactor-Verify-Commit）。

```
传统:  Design → Build → Test    → Verify → Commit
TDD:   Design → Test  → Build → Refactor → Verify → Commit
                ^^^^              ^^^^^^^^
              测试先行             新增重构阶段
```

### 文件变化

| 文件 | 变化类型 | 说明 |
|------|---------|------|
| `feat/skill.md` | 重写 | 核心循环从 DBTVC → DTBRVC，增加 Red/Green 确认步骤 |
| `feat/testing-guide.md` | 重写 | 增加 Red-Green-Refactor 详解和 AI+TDD 防御 |
| `feat/examples.md` | 重写 | 对话示例改为 TDD 流程 |
| `workflow-overview.md` | 重写 | 流程图、对比表、FAQ 更新 |
| `feat/troubleshooting-guide.md` | 直接复制 | 通用，无需改 |
| `feat/project-config.md` | 直接复制 | 项目专属模板 |
| `feat/design-guide.md` | 直接复制 | 项目专属模板 |
| `prd/SKILL.md` | 直接复制 | PRD 流程无需改 |

### 联动检查结果

| 检查项 | 状态 |
|--------|------|
| 文件交叉引用（链接有效性） | 全部通过 |
| Status 值一致性（design→test→build→refactor→verify→commit） | 全部一致 |
| 术语一致性（DTBRVC） | 无残留 DBTVC |
| workflow-overview.md 文件表 | 与实际文件匹配 |
| testing-guide.md TDD 阶段 | Red-Green-Refactor 正确集成 |
| examples.md 对话示例 | 遵循 TDD 流程 |
| 项目专属文件完整性 | 已复制，未修改 |

---

## 二、TDD 版本的优势

| 维度 | 传统 DBTVC | TDD DTBRVC |
|------|-----------|------------|
| **代码质量** | 先写后测，测试容易"迁就"代码 | 先定义"对"，代码天然可测试 |
| **过度实现** | 容易写多余功能 | 只写测试要求的代码 |
| **重构信心** | 改代码担心破坏功能 | 测试保护，放心重构 |
| **需求理解** | 写着写着才发现需求不清楚 | Design 阶段就要想清楚测试意图 |
| **Bug 修复** | 修完不确定是否真修好 | 先写重现测试，修复后确认 |
| **AI 协作** | AI 容易过度实现 | 测试约束 AI 的实现范围 |

---

## 三、基于调研的改进方案（待审阅）

以下改进基于 Martin Fowler、Kent Beck、Uncle Bob 的经典 TDD 理论，以及 2024-2026 年 AI+TDD 实践文章。

### 改进 1：增加"务实 TDD"指导

**来源**：Microsoft Developer Blog "Pragmatic TDD"、2024-2025 多篇实践文章

**问题**：当前 TDD 版本偏向严格 TDD，但 MVP 阶段并非所有功能都适合严格 TDD。纯 UI 探索、快速原型不适合先写测试。

**改进方案**：在 `skill.md` 增加一个章节，明确何时严格 TDD、何时可以灵活：

```markdown
## 务实 TDD 指南

不是所有功能都需要严格 Red-Green-Refactor：

| 功能类型 | 推荐方式 | 原因 |
|---------|---------|------|
| 业务逻辑（创建/删除/状态转换） | 严格 TDD | 逻辑复杂，测试驱动设计最有价值 |
| 数据持久化（增删改查） | 严格 TDD | 数据正确性关键 |
| UI 布局和样式 | Design-Build，后补测试 | UI 需要视觉探索，难以先写测试 |
| 快速原型/验证想法 | Build 先行，后补核心测试 | 速度优先 |
| Bug 修复 | TDD 修复（先写重现测试） | 防止回归 |

核心业务逻辑 = 严格 TDD
纯 UI 展示 = 可以灵活
```

**影响文件**：`feat/skill.md`（增加章节）、`testing-guide.md`（补充说明）

---

### 改进 2：增加 Martin Fowler 的"两顶帽子"隐喻

**来源**：Martin Fowler "bliki: TestDrivenDevelopment"

**问题**：当前 Refactor 阶段只列了检查清单，没解释**为什么**要把"让测试通过"和"清理代码"分开做。

**改进方案**：在 `testing-guide.md` 的 Refactor 节增加：

```markdown
### "两顶帽子"原则（Martin Fowler）

为什么 Green 和 Refactor 要分开？因为人的大脑不能同时追求两个目标：
1. 让代码**正确运行**（Green 阶段的帽子）
2. 让代码**结构优雅**（Refactor 阶段的帽子）

一次只戴一顶帽子。Green 阶段允许写丑代码，Refactor 阶段只改结构不改行为。
```

**影响文件**：`feat/testing-guide.md`（补充解释）

---

### 改进 3：增加"测试可以删除"原则

**来源**：Kent Beck 在 "Is TDD Dead?" 讨论中的观点

**问题**：当前版本暗示"写了的测试都要保留"。但 Kent Beck 明确说过：如果测试不再提供价值（比如被更高层测试覆盖了），应该删掉。

**改进方案**：在 `testing-guide.md` 增加：

```markdown
### 测试也可以删除

Kent Beck："如果同一个行为被多种方式测试，那就是耦合。耦合有成本。"

删除测试的场景：
- 重构后，旧测试验证的行为已被新测试覆盖
- 多个测试验证同一件事（冗余）
- 测试维护成本 > 提供的信心价值
- 闪烁测试（不稳定，时过时不过）
```

**影响文件**：`feat/testing-guide.md`

---

### 改进 4：增加 AI+TDD 的"上下文污染"警告

**来源**：8th Light "TDD: The Missing Protocol for Effective AI Collaboration"、Medium 多篇文章

**问题**：当前版本有 AI 测试反模式表，但缺少一个关键问题——AI 同时看到测试和实现代码时，测试的"约束力"会减弱。

**改进方案**：在 `testing-guide.md` 的 AI 规则部分增加：

```markdown
### AI 上下文污染

当 AI 同时看到测试代码和实现代码时：
- 测试编写会受实现代码"暗示"，失去独立性
- AI 可能写出"验证当前行为"而非"验证正确行为"的测试

防御措施：
- Test 阶段：只给 AI 看设计意图和接口定义，不给看实现
- Build 阶段：给 AI 看测试代码，让它写实现
- 人类审查：每个 Assert 都问"这验证的是正确行为还是当前行为？"
```

**影响文件**：`feat/testing-guide.md`

---

### 改进 5：增加"大局观"检查点

**来源**：Uncle Bob "The Cycles of TDD" — 避免局部最优陷阱

**问题**：严格的小步 Red-Green-Refactor 可能导致局部最优——每个小测试都通过了，但整体架构不合理。

**改进方案**：在 `feat/skill.md` 的 Refactor 阶段末尾增加：

```markdown
### 大局观检查（每 3-5 个 Red-Green 循环后）

暂停小步循环，问自己：
- 当前的代码结构是否适合整体架构？还是只适合当前这个测试？
- 有没有"为了通过测试而写的 hack"需要替换为正式方案？
- 新模块和已有模块的交互方式是否合理？

如果发现架构问题，在 Refactor 阶段处理（测试保护下安全调整）。
```

**影响文件**：`feat/skill.md`

---

## 四、改进实施状态

| 改进 | 状态 | 实施位置 |
|------|------|---------|
| 1. 务实 TDD 指南 | **已实施** | `feat/skill.md` 新增"务实 TDD 指南"章节（含需求模糊场景处理） |
| 2. 两顶帽子隐喻 | **已实施** | `feat/skill.md` Refactor 阶段 + `testing-guide.md` Refactor 详解 |
| 3. 测试可以删除 | **已实施** | `testing-guide.md` 新增"测试也可以删除"章节 |
| 4. AI 上下文污染（简化版） | **已实施** | `testing-guide.md` 新增"Bug 修复时的上下文污染"+ `skill.md` TDD 修复提醒 |
| 5. 大局观检查 | **已实施** | `feat/skill.md` Refactor 阶段新增"大局观检查" |

---

## 五、讨论中形成的关键决策

### 测试意图由谁设计？

**结论**：AI 设计，用户审查。

用户不需要是专业测试工程师。AI 负责生成测试意图和代码，用户只需确认"这些场景覆盖了我想要的功能吗？"——这是产品判断，不是技术判断。

### 需求不清晰时会不会阻塞开发？

**结论**：不会。通过"务实 TDD"处理。

- 确定的需求 → 严格 TDD
- 模糊的需求 → 标记"待确认"，先跳过，Build 中确认后补测试
- 完全不确定 → Build 先行探索

TDD 本身就是澄清需求的工具：写不出测试意图的地方，正好就是需求模糊的地方。在写代码之前发现比之后发现好。

### 上下文污染在 TDD 中是否是问题？

**结论**：新功能开发不是问题（Test 先于 Build，实现不存在），Bug 修复和功能修改时需注意。

简化防御：写 Bug 修复测试时，先用一句话描述"正确行为应该是什么"，基于描述写断言，不参考现有代码行为。

### 改进 4 原方案中"只给 AI 看设计意图"是什么意思？

**澄清**：在 Claude Code 中无法限制 AI 看哪些文件。此建议的实际含义是**流程约束**——TDD 的顺序（Test 先于 Build）天然解决了新功能场景的上下文污染。真正需要注意的只有 Bug 修复场景。

---

## 六、总结

TDD 版本已完善，5 个改进全部实施。可以直接复制到新项目验证。

### 参考来源

- Martin Fowler: [TestDrivenDevelopment](https://martinfowler.com/bliki/TestDrivenDevelopment.html)
- Martin Fowler: [Is TDD Dead?](https://martinfowler.com/articles/is-tdd-dead/)
- Uncle Bob: [The Cycles of TDD](https://blog.cleancoder.com/uncle-bob/2014/12/17/TheCyclesOfTDD.html)
- 8th Light: [TDD: The Missing Protocol for AI Collaboration](https://8thlight.com/insights/tdd-effective-ai-collaboration)
- Microsoft: [Pragmatic TDD](https://devblogs.microsoft.com/premier-developer/pragmatic-tdd/)
- Mark Seemann: [A Red-Green-Refactor Checklist](https://blog.ploeh.dk/2019/10/21/a-red-green-refactor-checklist/)
- Qodo: [AI Code Assistants & TDD](https://www.qodo.ai/blog/ai-code-assistants-test-driven-development/)
- ACM/IEEE ASE 2024: [TDD and LLM-based Code Generation](https://dl.acm.org/doi/10.1145/3691620.3695527)
