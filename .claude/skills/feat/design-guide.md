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

### 内置样式

| 样式名称 | 目标类型 | 用途 |
|---------|---------|------|
| `MainWindowStyle` | Window | 主窗口背景和前景色 |
| `Button` | Button | 主按钮（橙色，圆角） |
| `SecondaryButton` | Button | 次要按钮（蓝色） |
| `DeleteButton` | Button | 删除按钮（灰色，悬停变红） |
| `TextBox` | TextBox | 输入框（聚焦变橙色） |
| `TextBlock` | TextBlock | 文本基础样式 |
| `HeadingTextBlock` | TextBlock | 标题（24pt，粗体） |
| `SubheadingTextBlock` | TextBlock | 副标题（灰色） |
| `CardBorder` | Border | 卡片容器（圆角背景） |

### 字号系统

| 资源键 | 大小 | 用途 |
|--------|------|------|
| `FontSizeSmall` | 12pt | 辅助文本、时间戳 |
| `FontSizeMedium` | 14pt | 正文、按钮文字 |
| `FontSizeLarge` | 18pt | 小标题 |
| `FontSizeTitle` | 24pt | 页面标题 |
| `FontSizeHeading` | 28pt | 大标题 |

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

## UI 一致性检查清单

在 Verify 阶段，使用此清单检查 UI 一致性：

- [ ] 按钮颜色符合品牌规范（主按钮橙色，次要按钮蓝色）
- [ ] 输入框聚焦状态为橙色边框
- [ ] 卡片使用 CardBorder 样式
- [ ] 文本使用对应字体大小资源
- [ ] 间距符合标准间距系统
- [ ] 圆角符合圆角系统
- [ ] 整体风格与已有页面一致
