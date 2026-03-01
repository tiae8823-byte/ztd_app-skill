---
name: feat
description: Implement single feature using Design-Build-Test-Commit cycle. Use when implementing features, or when user says "实现功能", "开发功能", "写代码".
---

# 功能实现技能

采用 **Design-Build-Test-Commit** 循环实现**单个**功能，确保 UI 风格一致。

## 核心理念

**小步快跑**：
- 一次只实现一个功能
- 每个功能完成后立即测试验证
- 验证通过后立即 commit
- PRD 是活的，可以随时加功能
- **进度自动保存**：随时中断，下次可以继续
- **UI 风格一致**：每个功能都遵循统一的设计规范

**Design-Build-Test-Commit 循环**：
```
UI设计(自动) → 写代码 → 写测试 → 编译+手动测试 → Commit
      ↓             ↓         ↓            ↓            ↓
    修改         修改      修改        修复        回滚
    ↓             ↓         ↓          ↓            ↓
  保存进度      保存进度  保存进度   保存进度   清除进度
    ↓             ↓         ↓          ↓            ↓
  feat-progress.json (各阶段自动更新)
```

> **说明**: Design 阶段由 AI 自动判断组件和样式，用户只需确认 Y/N

## 可用 MCP 工具

| MCP 工具 | 用途 |
|----------|------|
| `mcp__context7__resolve-library-id` + `mcp__context7__query-docs` | 查询框架 API 用法 |
| `mcp__github__search_code` | 搜索参考实现 |
| `mcp__sequential-thinking__sequentialthinking` | 复杂功能设计 |

## 参考文件索引

本技能包含以下参考文件，在需要时读取：

| 文件 | 内容 | 何时读取 |
|------|------|---------|
| [design-guide.md](design-guide.md) | 详细的设计规范和样式说明 | Design 阶段需要参考样式时 |
| [testing-guide.md](testing-guide.md) | 详细的测试策略和方法 | Test 和 Verify 阶段 |
| [troubleshooting-guide.md](troubleshooting-guide.md) | 问题排查和搜索策略 | 遇到问题需要排查时 |
| [examples.md](examples.md) | 对话示例 | 需要了解使用方式时 |

---

## 与 PRD 的联动

```
/feat 执行
  │
  ├─ 检查 feat-progress.json 是否存在且有未完成功能
  │   │
  │   ├─ 存在 → 提示恢复进度
  │   │         │
  │   │         ├─ Y: 继续开发（跳到对应阶段）
  │   │         └─ N: 重新开始（清除进度）
  │   │
  │   └─ 不存在 → 读取 PRD.md
  │               │
  │               ├── PRD 不存在 → 提示先运行 /prd
  │               │
  │               └── PRD 存在 → 显示功能清单
  │                            │
  │                            ▼
  │                       用户选择一个功能
  │                            │
  │                            ▼
  │                       Design 阶段 → Build → Test → Verify → Commit
  │                            │
  │                       ├─ Design → 保存进度 (status: build, 记录设计)
  │                       ├─ Build → 保存进度 (status: test, 记录文件)
  │                       ├─ Test → 保存进度 (status: verify, 记录测试)
  │                       ├─ Verify → 保存进度 (status: commit)
  │                       ├─ Commit → PostGitCommand Hook 记录版本
  │                       └─ 完成 → 清除进度 → 更新 PRD.md
  │
  └─ 询问是否继续下一个
```

---

## 执行流程

### 0. 检查进度恢复

```
检查 .claude/feat-progress.json 是否存在

存在且有未完成功能 → 提示恢复进度
```

**恢复提示示例**：
```markdown
📋 检测到未完成的功能：

当前功能: 想法收集
状态: verify（已完成: design, build, test）
设计方案: 垂直布局, Button, TextBox, CardBorder
最后记录: Test阶段完成，等待编译验证
创建时间: 2026-02-27 14:30

是否继续？
- Y: 继续开发
- N: 重新开始
```

---

### 1. 读取 PRD

```
读取 docs/PRD.md

失败 → "未找到 PRD.md，请先运行 /prd 创建需求文档"

成功 → 显示功能清单
```

### 2. 显示功能清单，让用户选择

```markdown
📋 当前功能清单：

MVP 功能：
1. [ ] 用户注册
2. [ ] 用户登录
3. [ ] 发布文章

已完成：
（暂无）

做哪个功能？（输入序号或名称）
```

---

### 3. Design 阶段 - 布局设计

**只做布局**，不选样式。样式直接复用 BrandColors.xaml 中的共享样式。

**流程**：
1. AI 根据 PRD 自动生成布局方案（什么组件放哪里）
2. **匹配共享样式**：为每个组件指定 BrandColors.xaml 中的命名样式（如 `PageTitleTextBlock`、`CardBorder`、`CardDeleteButton` 等）
3. 展示布局草图给用户确认
4. 用户确认后保存进度到 feat-progress.json

**样式规范**：使用 BrandColors.xaml 中的共享样式，详见 [design-guide.md](design-guide.md)

**共享样式优先原则**：
- **必须优先使用已有共享样式**，禁止内联写死 FontSize/Padding/Color
- 如果现有样式不满足需求，先在 BrandColors.xaml 中新增命名样式，再在 XAML 中引用
- 如果发现重复出现的 UI 模式（2+ 处），应提升为共享样式

**确认提示**：
```markdown
🎨 布局设计: [功能名称]

布局: [垂直/水平/网格]
组件: Button x2, TextBox x1, CardBorder

┌─────────────────────────┐
│  [标题]                 │
│                         │
│  [输入框]               │
│  [按钮]                 │
└─────────────────────────┘

是否符合预期？(Y/N，或提出修改意见)
```

**进度保存**：status: design → build

---

### 4. Build 阶段 - 调研 + 编写代码

**流程**：
1. **先调研实现方案**：编码前先查阅资料，减少返工
   - **优先查官方文档**：用 `mcp__context7` 查询框架 API 和最佳实践
   - 再用 WebSearch 搜索具体实现方案和常见问题
   - 用 `mcp__fetch__fetch` 阅读参考文章
   - 简要告知用户参考了哪些方案
2. 按需创建文件
3. 实现功能逻辑
4. **实现前端 UI（引用 BrandColors.xaml 共享样式，不内联写死属性）**
5. **如有新 UI 模式**：先在 BrandColors.xaml 创建命名样式 → 在 XAML 中引用 → 更新 design-guide.md

> **原则**：官方文档优先 → 网上成熟方案 → 自行实现。先查后写，避免重复造轮子和反复修改。

**进度保存**：status: build → test

---

### 5. Test 阶段 - 编写测试

**流程**：
- 核心逻辑的单元测试
- 边界条件测试
- 死循环防御测试
- 日志功能测试

**详细测试策略**：见 [testing-guide.md](testing-guide.md)

**进度保存**：status: test → verify

---

### 6. Verify 阶段 - 编译并测试

**流程**：
```bash
# 编译生成可执行版本
dotnet build --configuration Release

# 运行单元测试
dotnet test

# 运行 UI 自动化测试
dotnet test --filter "FullyQualifiedName~UITests"

# 启动程序进行手动测试
ZtdApp\bin\Release\net8.0-windows\ZtdApp.exe
```

**UI 一致性检查**：
```markdown
✅ UI 一致性检查:
- [ ] 页面标题使用 PageTitleTextBlock 样式
- [ ] 页面说明使用 PageDescriptionTextBlock 样式
- [ ] 卡片容器使用 CardBorder 样式
- [ ] 卡片内文本使用 CardContentTextBlock / CardTagTextBlock / CardDateTextBlock
- [ ] 卡片内按钮使用 CardActionButton / CardDeleteButton
- [ ] 筛选按钮使用 FilterChipButton 为基础样式
- [ ] 按钮颜色符合品牌规范（主按钮橙色，次要按钮蓝色）
- [ ] **无内联 FontSize/Padding/Color** — 所有属性引用共享样式或资源键
- [ ] 整体风格与已有页面一致
```

**详细测试策略**：见 [testing-guide.md](testing-guide.md)

**进度保存**：status: verify → commit

---

### 问题处理流程

**触发时机**：手动测试后发现问题

**问题分类**：

| 问题类型 | 定义 | 处理建议 |
|---------|------|---------|
| **阻塞性问题** | 功能不可用、数据丢失、崩溃 | 必须修复 |
| **非阻塞性问题** | UI 细节、性能优化、视觉不完美 | 可选择处理方式 |

**处理选项**：

```markdown
发现问题后，提供三个选项：

1. **继续修复**
   - 返回 Build 阶段重新修改
   - 适用于：可快速解决的问题

2. **标记为已知问题**
   - 记录到 PRD.md 的"已知问题"章节
   - 继续 Commit，后续迭代解决
   - 适用于：非阻塞性问题、需要更多时间研究的问题

3. **直接提交**
   - 忽略问题，继续 Commit
   - 适用于：极小的视觉瑕疵、不影响使用的细节
```

**问题排查**：如果尝试修复 2 次仍未解决，参考 [troubleshooting-guide.md](troubleshooting-guide.md) 搜索解决方案

**known-issues.json 维护规则**（自动执行，无需用户提醒）：

| 触发条件 | 操作 |
|---------|------|
| 2次尝试失败后通过搜索找到方案 | 解决后写入 known-issues.json（问题+解决方案+参考链接） |
| 确认是框架/系统级 Bug | 写入 known-issues.json（open 状态） |
| 快速解决（1次成功） | 不记录 |
| 极小视觉瑕疵直接跳过 | 不记录 |

> 定位：知识库，记录"下次还会踩的坑"。不是每个问题都记录，只记录非显而易见的解决方案。

**已知问题记录格式**（PRD.md）：

```markdown
## 已知问题

### UI 优化
- [ ] 想法操作按钮高度对齐问题（视觉细节，不影响功能）
  - 影响：快速完成按钮高度略有不一致
  - 优先级：低
  - 计划：后续 UI 优化时统一处理
```

---

**Commit - 提交代码**：
```bash
git add .
git commit -m "feat: [功能名称]"
```

> 提交后，PostGitCommand Hook 会自动记录版本信息到 `.claude/version-history.log`

**Commit 完成后，清除进度**：
```bash
# 删除进度文件
rm .claude/feat-progress.json
```

**Compact - 清理上下文**：
```
/compact  清理历史消息，为下一个功能准备干净的上下文
```

---

### 7. 更新 PRD.md

标记功能完成：

```markdown
### MVP 功能（必须实现）

- [ ] 用户注册
- [x] 用户登录 ✅ commit: a1b2c3d
- [ ] 发布文章
```

### 8. 询问下一步

```
✅ [功能名称] 完成

版本信息已记录到: .claude/version-history.log

UI 风格: 使用 Anthropic 品牌样式
设计组件: Button, TextBox, CardBorder

建议运行 /compact 清理上下文，为下一个功能准备干净的环境

继续下一个功能？
- 输入序号继续
- 或告诉我新需求
- 或说"结束"退出
```

---

## 过程中发现新需求

用户可以随时提出新功能：

```
用户: 等等，文章还得能置顶

AI: 好，加到 PRD 里：
    - [ ] 文章置顶

    先做置顶还是继续当前功能？
```

更新 PRD.md：
```markdown
### MVP 功能（必须实现）

- [ ] 用户注册
- [ ] 用户登录
- [ ] 发布文章
- [ ] 文章置顶 ← 新增
```

---

## 进度维护

### 进度文件位置

`.claude/feat-progress.json` - 功能开发进度文件

### 进度文件格式

```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "design",              // design | build | test | verify | commit | completed
    "createdAt": "2026-02-27T14:30:00Z",
    "description": "开始实现功能",
    "designLayout": "垂直布局",
    "designComponents": ["Button", "TextBox", "CardBorder"],
    "designSketch": "[简单的ASCII布局图]",
    "filesCreated": [],
    "testsCreated": [],
    "lastCheckpoint": "UI 设计完成，等待用户确认"
  }
}
```

### Status 值说明

| Status | 说明 | 已完成阶段 |
|--------|------|-----------|
| `design` | 正在进行 UI 设计 | - |
| `build` | 设计完成，正在编写代码 | design |
| `test` | 已完成代码，正在写测试 | design, build |
| `verify` | 已完成测试，正在编译验证 | design, build, test |
| `commit` | 已完成验证，准备提交 | design, build, test, verify |
| `completed` | 功能完成 | 全部 |

### 进度保存时机

| 阶段 | 操作 | 文件变更 |
|------|------|---------|
| 开始功能 | 创建进度文件 | status: design |
| Design 完成 | 更新进度 | status: build, designLayout, designComponents |
| Build 完成 | 更新进度 | status: test, filesCreated |
| Test 完成 | 更新进度 | status: verify, testsCreated |
| Verify 完成 | 更新进度 | status: commit |
| Commit 完成 | 删除进度文件 | 清除 |

### 进度恢复流程

1. **检测进度**：`/feat` 执行时读取 `feat-progress.json`
2. **提示用户**：显示当前功能、状态、最后记录、设计信息
3. **用户选择**：
   - Y: 跳转到对应阶段继续开发
   - N: 删除进度文件，重新开始

### 进度文件管理规则

- ✅ 功能完成 commit 后**必须删除**进度文件
- ✅ 进度文件**不提交**到 git（在 .gitignore 中）
- ✅ 用户选择"重新开始"时**立即清除**进度
- ✅ 每个阶段完成后**立即更新**进度文件

---

## 错误处理

### 验证失败
1. 分析错误原因
2. 尝试修复（最多 2 次）
3. **仍未解决 → 参考 [troubleshooting-guide.md](troubleshooting-guide.md) 搜索解决方案**
4. 应用方案并重新验证
5. 多次失败考虑回滚: `git reset --hard`

### 需求变更
1. 停止当前实现
2. 讨论变更内容
3. 更新 PRD
4. 重新规划

---

## 注意事项

- **一次只做一个功能**，不要贪多
- **Design 阶段不可跳过**：必须确认 UI 组件和共享样式匹配
- **必须使用 BrandColors.xaml 共享样式**：禁止内联写死 FontSize/Padding/Color，所有 UI 属性通过命名样式或资源键引用
- **新 UI 模式必须提升为共享样式**：先在 BrandColors.xaml 定义，再在 XAML 中引用，最后更新 design-guide.md
- 测试要覆盖核心逻辑，不追求完美
- **每次功能完成后必须编译生成可执行版本**，确保代码可运行
- **手动测试验证功能正常**，不要跳过
- **UI 一致性检查**：确保新功能与已有页面风格一致
- **每个阶段完成后立即更新进度文件**，支持中断恢复
- commit 信息简洁清晰
- 提交后**删除进度文件**，运行 `/compact` 清理上下文
- 随时可以加新功能到 PRD
- 遇到问题及时讨论，不要硬撑

---

## 对话示例

详细的对话示例见 [examples.md](examples.md)
