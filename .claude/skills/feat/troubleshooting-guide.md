# 问题排查与解决方案搜索

> 本文档提供详细的问题排查策略和搜索方法，供遇到问题时参考使用。

## 何时应该搜索（判断标准）

| 场景 | 是否搜索 | 理由 |
|------|---------|------|
| 简单的语法错误、拼写错误 | ❌ 不搜索 | 自己能快速定位和修复 |
| 明确的编译错误（缺少引用、类型不匹配） | ❌ 不搜索 | 错误信息已经很明确 |
| 逻辑错误（业务逻辑实现问题） | ❌ 不搜索 | 需要理解业务，搜索无用 |
| **尝试修复 2 次仍未解决的问题** | ✅ 搜索 | 可能是不熟悉的技术问题 |
| **框架/库的 Bug 或限制** | ✅ 搜索 | 社区可能已有解决方案 |
| **UI 渲染问题（对齐、裁剪、样式异常）** | ✅ 搜索 | 通常是框架特性或已知问题 |
| **性能问题、内存泄漏** | ✅ 搜索 | 需要专业的优化方案 |
| **平台特定问题（Windows/Linux/Mac）** | ✅ 搜索 | 需要了解平台特性 |
| **第三方库使用问题** | ✅ 搜索 | 官方文档或社区有答案 |

**核心原则**：
- 能自己快速解决的，不要搜索（浪费时间）
- 尝试 2 次仍未解决的，立即搜索（避免盲目试错）
- 涉及框架/库的问题，优先搜索（站在巨人肩膀上）

---

## 搜索策略（按优先级）

### 1. GitHub Issues 搜索（最优先）

```
使用 WebSearch 搜索：
site:github.com [技术栈] [问题关键词]

示例：
- site:github.com WPF Button text clipped
- site:github.com dotnet build error CS0246
- site:github.com microsoft-ui-xaml padding issue
```

- ✅ 优势：真实案例、有解决方案、有讨论、可能是官方确认的 Bug
- 🎯 适用：框架 Bug、UI 问题、已知限制、最佳实践
- 💡 技巧：搜索项目的官方仓库（如 dotnet/wpf, microsoft/microsoft-ui-xaml）

### 2. 官方文档查询

```
使用 mcp__context7__query-docs 查询：
- 框架 API 用法
- 配置选项说明
- 官方推荐方案
```

- ✅ 优势：权威、准确、最新
- 🎯 适用：API 使用、配置问题、功能说明
- 💡 技巧：先用 resolve-library-id 确定库 ID，再 query-docs

### 3. Stack Overflow 搜索

```
使用 WebSearch 搜索：
site:stackoverflow.com [问题关键词]

示例：
- site:stackoverflow.com WPF Button content vertical alignment
- site:stackoverflow.com C# async await deadlock
```

- ✅ 优势：问答形式、多种方案对比、有投票排序
- 🎯 适用：通用编程问题、算法实现、设计模式
- 💡 技巧：关注高赞回答和被采纳的答案

### 4. 技术博客/文章

```
使用 WebSearch 搜索：
[问题关键词] solution fix workaround

示例：
- WPF text clipping solution
- dotnet memory leak fix
```

- ✅ 优势：详细教程、实战经验、深度分析
- 🎯 适用：复杂问题、性能优化、架构设计
- 💡 技巧：优先选择近期文章（技术更新快）

---

## 搜索关键词技巧

| 技巧 | 示例 | 说明 |
|------|------|------|
| 包含技术栈 | `WPF Button`, `dotnet build` | 缩小搜索范围 |
| 使用英文 | `text clipped` 而不是 `文字被遮挡` | 搜索结果更丰富 |
| 包含错误代码 | `CS0246`, `NullReferenceException` | 精确定位问题 |
| 添加动作词 | `fix`, `solution`, `workaround`, `resolve` | 找到解决方案 |
| 使用引号 | `"Button content clipped"` | 精确匹配短语 |
| 排除无关词 | `-android -ios` | 排除其他平台 |

---

## 搜索后的处理流程

```
1. 阅读多个解决方案（至少 3 个）
   ↓
2. 对比方案的优缺点
   - 是否适用当前场景？
   - 是否有副作用？
   - 社区反馈如何？
   ↓
3. 优先选择
   - 官方推荐方案
   - 高赞/被采纳的方案
   - 最新的方案（技术更新快）
   ↓
4. 理解原理后再应用
   - 不要盲目复制代码
   - 理解为什么这样能解决问题
   - 考虑是否需要调整
   ↓
5. 应用并测试验证
   ↓
6. 记录到 known-issues.json
   - 如果是框架已知问题
   - 记录问题、解决方案、参考链接
```

---

## 示例：本次 UI 问题的完整流程

```
问题：WPF 按钮文字底部被遮挡
  ↓
第 1 次尝试：增加 Height (36 → 40)
  ↓ 失败
第 2 次尝试：增加 Padding (8,6 → 10,10)
  ↓ 更糟糕（遮挡更严重）
  ↓
触发搜索条件：尝试 2 次仍未解决
  ↓
GitHub 搜索：site:github.com WPF Button text clipped
  ↓
找到相关 Issue：microsoft/microsoft-ui-xaml#9543
  - 标题：Button cuts the content at some scaling levels
  - 状态：Open (官方确认的 Bug)
  - 解决方案：设置 Padding="0"
  - 原理：DPI 缩放时 Padding 计算有问题
  ↓
应用方案：Padding="0" + UseLayoutRounding="True"
  ↓
测试验证 → 成功！
  ↓
更新 known-issues.json：
{
  "id": "ui-001",
  "solution": "设置 Padding='0'",
  "reference": "https://github.com/microsoft/microsoft-ui-xaml/issues/9543"
}
  ↓
提交代码
```

---

## 注意事项

- ⚠️ 不要过度依赖搜索：简单问题自己解决更快
- ⚠️ 不要盲目复制代码：理解原理后再应用
- ⚠️ 不要只看一个方案：对比多个方案选最优
- ⚠️ 注意方案的时效性：优先选择最新的方案
- ⚠️ 记录解决方案：避免下次遇到同样问题
