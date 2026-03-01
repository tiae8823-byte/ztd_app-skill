# 设计规范参考

> 本文档提供详细的 UI 设计规范和样式说明，供 Design 阶段参考使用。

## 推荐设计系统

| 设计系统 | 适用场景 | 参考链接 |
|---------|---------|---------|
| **Apple Human Interface Guidelines** | iOS/macOS 风格，简洁优雅 | https://developer.apple.com/design/human-interface-guidelines/ |
| **Google Material Design** | Android/Web 风格，色彩丰富 | https://m3.material.io/ |
| **Anthropic 品牌风格** | 当前项目已配置，直接使用 | `ZtdApp/Styles/BrandColors.xaml` |

---

## Anthropic 品牌样式（当前项目）

### 颜色系统

| 资源键 | 颜色值 | 用途 |
|--------|--------|------|
| `BrandDark` | `#141413` | 主文本、深色背景 |
| `BrandLight` | `#faf9f5` | 浅色背景、深色文字 |
| `BrandMidGray` | `#b0aea5` | 次要文本、说明 |
| `BrandLightGray` | `#e8e6dc` | 卡片背景、边框 |
| `BrandOrange` | `#d97757` | 主按钮、主强调色 |
| `BrandBlue` | `#6a9bcc` | 次要按钮、信息提示 |
| `BrandGreen` | `#788c5d` | 成功状态、完成标记 |

### Typography 层级（Apple 风格）

| 资源键 | 大小 | 用途 | 使用场景 |
|--------|------|------|---------|
| `FontSizeHeading` | 28px | 大标题 | ZTD logo |
| `FontSizeTitle` | 22px | 页面标题 | 每个页面的主标题 |
| `FontSizeLarge` | 18px | 区域标题 | 统计卡片标题 |
| `FontSizeBody` | 15px | 正文 | 卡片内容文本、分组标题 |
| `FontSizeMedium` | 14px | 按钮/输入框 | 按钮文字、输入框默认 |
| `FontSizeCaption` | 13px | 说明/标签 | 页面说明、筛选标签 |
| `FontSizeSmall` | 12px | 元数据 | 时间戳、分类标签、卡片内小按钮 |

### 页面布局样式

| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `PageTitleTextBlock` | TextBlock | 页面标题（22px, Bold, 居中） |
| `PageDescriptionTextBlock` | TextBlock | 页面说明（13px, 灰色, 居中） |

### 卡片内样式

| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `CardBorder` | Border | 卡片容器（圆角背景） |
| `CardContentTextBlock` | TextBlock | 卡片正文（15px, 自动换行） |
| `CardTagTextBlock` | TextBlock | 分类标签（12px, 蓝色） |
| `CardDateTextBlock` | TextBlock | 日期元信息（12px, 灰色） |
| `CardActionButton` | Button | 卡片内操作按钮（蓝色, 6,6 padding, 12px） |
| `CardDeleteButton` | Button | 卡片内删除按钮（灰色, 6,6 padding, 12px） |

### 分组和筛选样式

| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `GroupHeaderTextBlock` | TextBlock | 分组标题（15px, SemiBold, 灰色） |
| `FilterChipButton` | Button | 筛选标签基础态（灰底, 13px, 需配合 DataTrigger 实现激活态） |

### 按钮样式

| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `Button`（默认） | Button | 主按钮（橙色，圆角） |
| `SecondaryButton` | Button | 次要按钮（蓝色） |
| `DeleteButton` | Button | 删除按钮（灰色，悬停变红） |
| `IdeaActionButton` | Button | 想法操作按钮（蓝色，固定 100x36） |
| `CardActionButton` | Button | 卡片内小操作按钮（蓝色，紧凑 padding） |
| `CardDeleteButton` | Button | 卡片内小删除按钮（灰色，紧凑 padding） |
| `NavButton` | Button | 导航按钮（透明底，悬停灰色） |
| `NavButtonActive` | Button | 导航按钮激活态（橙色底） |

### 其他样式

| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `MainWindowStyle` | Window | 主窗口背景和前景色 |
| `HeadingTextBlock` | TextBlock | 通用标题（22px, Bold）— 建议优先用 PageTitleTextBlock |
| `SubheadingTextBlock` | TextBlock | 通用副标题（灰色）— 建议优先用 PageDescriptionTextBlock |
| `SidebarBorder` | Border | 侧边栏容器 |

---

## 组件交互状态

已在 BrandColors.xaml 中实现的组件状态定义。新增组件时必须遵循相同模式。

### 按钮状态

| 组件 | 默认态 | Hover | Pressed | Disabled |
|------|--------|-------|---------|----------|
| `Button`（主按钮） | 橙色 `#d97757` | 浅橙 `#e08869` | 深橙 `#c46a4d` | 灰底 + 灰字 |
| `SecondaryButton` | 蓝色 `#6a9bcc` | 浅蓝 `#7da8d9` | 深蓝 `#5c8ab6` | 继承主按钮 |
| `DeleteButton` | 灰底 `BrandLightGray` | 深灰底 `#d8d6cc` + 橙字 | - | - |
| `NavButton` | 透明 | 灰底 `#d8d6cc` | - | - |
| `NavButtonActive` | 橙色 `BrandOrange` | 浅橙 `#e08869` | - | - |
| `FilterChipButton` | 灰底 `BrandLightGray` | 深灰底 `#d8d6cc` | - | - |

### 输入框状态

| 组件 | 默认态 | Focused | Disabled |
|------|--------|---------|----------|
| `TextBox` | 灰色边框 1px | 橙色边框 2px | 灰底 + 灰字 |
| `ComboBox` | 灰色边框 1px | 展开时橙色边框 | - |

### 状态设计规则

- **Hover**：在默认色基础上提亮或加深，变化幅度约 10-15%
- **Pressed**：在默认色基础上加深，变化幅度约 15-20%
- **Focused**：使用品牌橙色 `BrandOrange` 作为焦点指示色
- **Disabled**：统一使用 `BrandLightGray` 背景 + `BrandMidGray` 前景
- 新增组件时，按此规则推导状态颜色，保持体系一致

---

## 动画规范（标准先行）

> 当前 MVP 阶段暂未实现动画，以下为预定义标准。后续添加动画时遵循此规范，避免风格不一致。

### 基础参数

| 参数 | 值 | 用途 |
|------|-----|------|
| 快速过渡 | 150ms | 按钮 hover、颜色变化 |
| 标准过渡 | 250ms | 面板展开收起、页面切换 |
| 慢速过渡 | 400ms | 模态弹出、数据加载完成 |
| 缓动函数 | Ease-Out（快进慢出） | 大部分交互动效 |
| 弹性缓动 | Ease-In-Out | 拖拽、弹窗 |

### 常用动效模式

| 场景 | 动效 | 参数 |
|------|------|------|
| 按钮状态切换 | 背景色渐变 | 150ms, Ease-Out |
| 卡片出现 | 淡入 + 微上移 | 250ms, Ease-Out |
| 面板展开/收起 | 高度动画 | 250ms, Ease-Out |
| 页面切换 | 淡入淡出 | 250ms, Ease-Out |
| 删除项目 | 淡出 + 高度收缩 | 250ms, Ease-In |
| 拖拽排序 | 位移跟随 | 实时, Ease-Out |

### 原则

- **自然**：动画服务于交互反馈，不是装饰
- **快速**：用户不应等待动画完成才能操作
- **一致**：同类交互使用相同的时长和缓动

---

## 通用设计原则（适用于所有风格）

### 间距系统

| 元素 | 标准间距 | 紧凑间距 | 宽松间距 |
|------|---------|---------|---------|
| 组件内边距 | 12-16px | 8-10px | 20-24px |
| 组件间距 | 8-12px | 4-6px | 16-20px |
| 页面边距 | 20-24px | 12-16px | 32-40px |

### 圆角系统

| 元素 | 圆角半径 |
|------|---------|
| 按钮 | 6-8px |
| 输入框 | 6-8px |
| 卡片 | 8-12px |
| 弹窗 | 12-16px |

### 阴影系统

| 深度 | 用途 |
|------|------|
| 浅阴影 | 卡片、浮层 |
| 中阴影 | 下拉菜单 |
| 深阴影 | 模态弹窗 |

---

## 提升共享样式流程

在 Design 阶段，如果发现以下情况，应将样式提升到 BrandColors.xaml：

1. **重复出现的 UI 模式**：同样的属性组合在 2 个以上位置使用
2. **跨模块一致性需求**：两个模块需要外观相同的组件
3. **布局细节硬编码**：Padding、FontSize、Color 等直接写在 XAML 模板中

**提升步骤**：
1. 识别重复模式 → 在 BrandColors.xaml 创建命名样式
2. 替换所有使用该模式的内联属性 → 引用共享样式
3. 更新本文档的样式表 → 保持文档同步
4. 编译验证 → 确保所有页面视觉一致

**命名规范**：
- 页面级：`Page*`（如 `PageTitleTextBlock`）
- 卡片级：`Card*`（如 `CardContentTextBlock`, `CardActionButton`）
- 分组级：`Group*`（如 `GroupHeaderTextBlock`）
- 筛选级：`Filter*`（如 `FilterChipButton`）
- 按钮：描述用途（如 `DeleteButton`, `SecondaryButton`）

---

## UI 一致性检查清单

在 Verify 阶段，使用此清单检查 UI 一致性：

- [ ] 页面标题使用 `PageTitleTextBlock` 样式
- [ ] 页面说明使用 `PageDescriptionTextBlock` 样式
- [ ] 卡片容器使用 `CardBorder` 样式
- [ ] 卡片内文本使用 `CardContentTextBlock` / `CardTagTextBlock` / `CardDateTextBlock`
- [ ] 卡片内按钮使用 `CardActionButton` / `CardDeleteButton`
- [ ] 筛选按钮使用 `FilterChipButton` 为基础样式
- [ ] 按钮颜色符合品牌规范（主按钮橙色，次要按钮蓝色）
- [ ] 输入框聚焦状态为橙色边框
- [ ] **无内联 FontSize/Padding/Color**：所有属性引用共享样式或资源键
- [ ] 新增组件的交互状态（hover/pressed/focused/disabled）符合状态设计规则
- [ ] 整体风格与已有页面一致

---

## 待补充区域

> 以下设计细节将在功能开发过程中按需补充，不在 MVP 初期强制定义。

- [ ] 复杂组合组件的内部间距规范（如番茄钟计时器卡片）
- [ ] 特定页面的微交互（如拖拽排序、滑动删除）
- [ ] 边界场景 UI（空状态、错误状态、加载状态）
- [ ] 动画实现（标准已定义，代码待实现）
- [ ] 深色模式配色方案
