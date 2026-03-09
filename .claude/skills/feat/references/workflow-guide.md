# 功能实现详细工作流程

完整的设计-意图-构建-验证-提交流程

---

## 流程概览

```
/feat 执行
  │
  ├─ 检查 feat-progress.json
  │   │
  │   ├─ 存在未完成 → 提示恢复
  │   │
  │   └─ 不存在 → 读取 PRD.md
  │               │
  │               └─ 显示功能清单 → 用户选择
  │
  └─ Design → Intent → Build → Verify → Commit
```

---

## 阶段 0：进度恢复

**检查**：读取 `.claude/feat-progress.json`

**存在未完成**：
```markdown
📋 检测到未完成的功能：
当前功能: 番茄钟计时
状态: verify（已完成: design, intent, build）

是否继续？(Y/N)
```

**用户选择**：
- Y: 恢复到对应阶段继续
- N: 删除进度，重新开始

---

## 阶段 1：Design - UI设计 + 调研 + 复杂度评估

### 步骤 1.1：调研 UI 模式

**搜索策略**：
```markdown
1. GitHub 搜索（真实项目）
   mcp__github__search_code: "WPF timer card layout"

2. 博客搜索（最佳实践）
   mcp__exa__web_search_exa: "material design timer UI patterns"

3. 官方文档（API 参考）
   mcp__context7__query-docs: "WPF animation opacity"
```

**记录调研来源**：
```markdown
调研来源:
- GitHub: [链接1], [链接2]
- 博客: [链接3]
- 官方文档: [链接4]
```

### 步骤 1.2：生成布局方案

```markdown
🎨 布局设计: [功能名称]

调研来源:
- UI 模式: [GitHub 链接]
- 交互方案: [博客链接]

布局: [垂直/水平/网格]
组件: [Button x2, TextBox x1, Card x1]

草图:
┌─────────────────────────┐
│  [标题]                 │
│  [输入框]               │
│  [按钮]                 │
└─────────────────────────┘

🧪 复杂度评估:
- 类型: [CRITICAL / NON-CRITICAL]
- 测试策略: [冒烟测试 / 手动测试]
- 理由: [为什么这样分类]
```

### 步骤 1.3：生成测试意图草稿

**CRITICAL 功能**：
```markdown
📝 测试意图（请确认）：

功能级别: CRITICAL（核心业务流程）

测试场景:

1. ✅ [场景名称]
   Given: [初始状态]
   When: [用户操作]
   Then: [预期结果]

2. ✅ [场景名称]
   Given: [初始状态]
   When: [用户操作]
   Then: [预期结果]

边界条件:
- [边界场景]: [描述]

异常处理:
- [异常情况]: [预期行为]

这些场景对吗？需要补充吗？
```

**NON-CRITICAL 功能**：
```markdown
📝 测试意图（请确认）：

功能级别: NON-CRITICAL（UI 交互）

测试场景:

1. ✅ [场景名称]
   Given: [初始状态]
   When: [用户操作]
   Then: [预期结果]

这些场景对吗？
```

### 步骤 1.4：用户确认

```markdown
用户: [审查、补充、确认]
```

**保存进度**：
```json
{
  "feature": "番茄钟计时",
  "status": "design",
  "level": "CRITICAL",
  "testIntent": [...]
}
```

---

## 阶段 2：Intent - 测试意图确认（仅 CRITICAL）

**CRITICAL 功能**：
1. 用户再次审查测试意图
2. 确认不可修改（AI 不能改）
3. AI 根据意图生成冒烟测试代码
4. 运行测试（应该失败，因为还没实现）

**NON-CRITICAL 功能**：
- 跳过此阶段，直接 Build

详见 [test-intent-first.md](test-intent-first.md)

---

## 阶段 3：Build - 调研方案 + 编码实现

### 步骤 3.1：调研实现方案

**搜索策略**（贯穿全流程）：
```markdown
1. GitHub 搜索实现代码
   mcp__github__search_code: "WPF dispatcher timer implementation"

2. 博客搜索最佳实践
   mcp__exa__web_search_exa: "C# timer best practices"

3. 官方文档查 API
   mcp__context7__query-docs: "DispatcherTimer WPF"
```

**告知用户参考来源**：
```markdown
实现方案参考:
- GitHub: [项目链接] (高星项目)
- 博客: [文章链接] (C# 官方博客)
- 官方文档: [文档链接]
```

### 步骤 3.2：编码实现

**参考测试意图写代码**：
- 意图1: 开始计时 → 实现 `Start()` 方法
- 意图2: 暂停功能 → 实现 `Pause()` 方法
- ...

**UI 实现规则**：
- 必须使用共享样式（BrandColors.xaml）
- 禁止内联 FontSize/Padding/Color
- 新样式先定义再引用

### 步骤 3.3：保存进度

```json
{
  "feature": "番茄钟计时",
  "status": "build",
  "research": {
    "github": ["链接1", "链接2"],
    "blogs": ["链接3"],
    "docs": ["链接4"]
  }
}
```

---

## 阶段 4：Verify - 编译 + 分层测试

### CRITICAL 功能验证流程

```markdown
1. 编译检查
   ```bash
   dotnet build ZtdApp --configuration Release
   ```

2. 运行冒烟测试（Intent 阶段生成的）
   ```bash
   dotnet test ZtdApp.Tests --filter "FullyQualifiedName~Smoke"
   ```

3. 变异测试（可选，严格模式）
   ```bash
   dotnet stryker --threshold-high 70 --threshold-break 40
   ```

4. 手动测试完整流程（基于测试意图）
   - [ ] 场景1: 开始计时
   - [ ] 场景2: 暂停功能
   - [ ] 场景3: 继续计时
   - [ ] 场景4: 计时完成
   - [ ] 场景5: 撤回逻辑
   - [ ] 边界条件: 暂停超过30分钟

5. UI 一致性检查（使用 project-config.md 清单）
   - [ ] 无内联样式
   - [ ] 使用共享样式
   - [ ] 符合设计规范
```

### NON-CRITICAL 功能验证流程

```markdown
1. 编译检查
   ```bash
   dotnet build ZtdApp --configuration Release
   ```

2. 手动测试清单（基于测试意图）
   - [ ] 场景1: Hover 显示
   - [ ] 场景2: Hover 隐藏
   - [ ] 场景3: 动画流畅

3. UI 样式检查
   - [ ] 颜色正确
   - [ ] 间距一致
   - [ ] 动画时长合理
```

### 问题处理

**阻塞性问题**：
- 返回 Build 阶段修复
- 修复后重新 Verify

**非阻塞性问题**：
- 记录到 PRD.md 已知问题
- 继续 Commit

详见 [testing-guide.md](testing-guide.md)

---

## 阶段 5：Commit - 提交代码

### 步骤 5.1：提交代码

```bash
git add .
git commit -m "feat: [功能名称]"
```

### 步骤 5.2：更新 PRD.md

```markdown
### MVP 功能（必须实现）

- [x] 番茄钟计时 ✅ commit: a1b2c3d
```

### 步骤 5.3：清理进度

```bash
rm .claude/feat-progress.json
```

### 步骤 5.4：清理上下文

```
/compact
```

---

## 调研策略（贯穿全流程）

### 搜索工具优先级

| 工具 | 用途 | 优先级 |
|------|------|--------|
| `mcp__github__search_code` | 真实项目代码 | 高 |
| `mcp__exa__web_search_exa` | 高质量博客文章 | 高 |
| `mcp__context7__query-docs` | 官方 API 文档 | 中 |
| `mcp__bing-cn__bing_search` | 中文资源 | 低 |

### Design 阶段调研

```markdown
搜索内容:
- UI 模式: "WPF [组件] layout examples"
- 交互方案: "material design [功能] interaction"
- 动画效果: "[框架] animation [效果名]"
```

### Build 阶段调研

```markdown
搜索内容:
- 实现方案: "[语言/框架] [功能] implementation"
- 代码示例: "[框架] [API] example"
- 最佳实践: "[技术] best practices"
```

---

## 关键原则

1. **调研贯穿全流程** - GitHub > 博客 > 官方文档
2. **测试意图先行** - Intent 阶段定义，人工确认
3. **AI 角色限制** - 实现代码，不修改意图
4. **分层测试** - CRITICAL 冒烟测试，NON-CRITICAL 手动测试
5. **必须使用共享样式** - 禁止内联
6. **一次一个功能** - 不并行
7. **及时保存进度** - 每个阶段后更新 feat-progress.json
8. **完成清理** - commit 后删除进度，运行 /compact

---

## 常见问题

### Q: 如何判断 CRITICAL vs NON-CRITICAL？

A: 看影响范围：
- 涉及金钱/数据安全/核心业务 → CRITICAL
- UI 样式/实验功能/内部工具 → NON-CRITICAL

### Q: AI 可以修改测试意图吗？

A: ❌ 不可以！测试意图必须人工确认，AI 只能实现代码。

### Q: 变异测试必须吗？

A: 不必须，可选。生产级关键功能推荐，MVP 快速迭代可跳过。

### Q: 调研来源要记录吗？

A: ✅ 必须记录。告知用户参考了哪些方案。
