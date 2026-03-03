# 项目地图

> 随探索自动更新。你问到新文件，这里就多一条。

## 项目结构总览

```
ZtdApp/
├── Views/          ← 🟢 界面文件
├── ViewModels/     ← 🟡 界面逻辑
├── Models/         ← 🟡 数据结构
├── Services/       ← 🔴 业务逻辑
├── Data/           ← 🔴 数据库
└── Styles/         ← 🟢 样式定义
```

## 探索过的区域

### Styles/BrandColors.xaml 🟢
所有颜色和样式定义。改颜色来这里。

### Views/MainWindow.xaml 🟢🟡
主窗口界面，侧边栏 + 内容区都在这里。
- 第 13 行附近：列宽定义（侧边栏宽度）
- 视觉部分 🟢 / 绑定部分 🟡

### ViewModels/ 🟡
界面背后的"大脑"，控制页面切换和数据。改名会断绑定。

### Models/ 🟡
数据结构定义（Idea、Task、Note、Tomato）。

### Services/ 🔴
核心业务逻辑（增删改查）。不懂别动。

### Data/ 🔴
直接操作数据库。不要碰。

---

*随探索持续更新...*
