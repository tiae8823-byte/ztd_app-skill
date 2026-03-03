# Trae + Claude Code CLI 协作工作流

> 最佳实践：一边看代码，一边问AI

---

## 推荐屏幕布局

```
┌────────────────────────┬───────────────────────┐
│                        │                       │
│      Trae IDE          │   Claude Code CLI     │
│                        │   (Terminal)          │
│   - 浏览文件           │                       │
│   - 查看代码           │   - 提问              │
│   - 运行调试           │   - 获取解释          │
│   - 修改代码           │   - 执行命令          │
│                        │                       │
└────────────────────────┴───────────────────────┘
        ↑                           ↑
   看到不懂的代码 ──────────────→ 复制过来问
   需要改代码     ←────────────── 根据回答改
```

---

## 快速开始（3分钟上手）

### Step 1: 在 Trae 中打开项目

1. 打开 Trae
2. File → Open Folder → 选择 `ztd_app-skill`
3. 在 Explorer 中看到项目结构

### Step 2: 打开 Claude Code CLI

1. 在 Trae 中打开 Terminal (Ctrl+`)
2. 或者单独开一个 Terminal 窗口
3. 确保在 `ztd_app-skill` 目录

### Step 3: 测试联动

**在 Trae 中：**
- 打开 `ZtdApp/Views/MainWindow.xaml`
- 选中一行代码，比如：
  ```xml
  <Button Content="想法收集" ... />
  ```
- 复制 (Ctrl+C)

**在 Claude Code CLI 中：**
```
"这段代码是什么意思？
<Button Content="想法收集" ... />"
```

**Claude 应该解释：**
- 这是一个按钮
- Content 是显示的文字
- 意思是显示"想法收集"按钮

✅ **联动成功！**

---

## 日常工作流

### 场景 1: 看到不懂的代码

```
Trae: 浏览文件，发现不懂的代码
  ↓
复制代码片段
  ↓
Claude Code: "这段代码什么意思？"
  ↓
理解后，记录到 VOCABULARY.md
```

**示例：**
```
你: "这是什么意思？ {Binding NavigateToIdeasCommand}"

AI: "这是数据绑定。
     意思是：按钮的点击命令从 ViewModel 的 NavigateToIdeasCommand 属性获取。

     简单说：点击按钮 → 执行 ViewModel 里的 NavigateToIdeas 方法"
```

### 场景 2: 想改某个东西

```
你: "我想把侧边栏变宽一点，安全吗？"

AI: "🟢 安全！这是视觉修改。

     在 MainWindow.xaml 第13行：
     <ColumnDefinition Width="180"/>

     把 180 改成 200 或 220 试试。"

你: 在 Trae 中修改，运行看效果
```

### 场景 3: 出错了

```
Trae: 运行程序，报错
  ↓
复制错误信息
  ↓
Claude Code: "这个错误什么意思？怎么解决？"
  ↓
AI 给出解决方案
  ↓
在 Trae 中修改
```

---

## 快捷键备忘

### Trae 常用快捷键

| 操作 | Windows | Mac |
|------|---------|-----|
| 打开文件 | Ctrl+P | Cmd+P |
| 全局搜索 | Ctrl+Shift+F | Cmd+Shift+F |
| 格式化代码 | Shift+Alt+F | Shift+Option+F |
| 运行程序 | F5 | F5 |
| 打开终端 | Ctrl+` | Cmd+` |
| 复制当前行 | Ctrl+C (无选中) | Cmd+C (无选中) |

### 文件导航

| 目标 | 路径 |
|------|------|
| 主窗口 | `ZtdApp/Views/MainWindow.xaml` |
| 主窗口逻辑 | `ZtdApp/ViewModels/MainWindowViewModel.cs` |
| 样式定义 | `ZtdApp/Styles/BrandColors.xaml` |
| 项目配置 | `ZtdApp/ZtdApp.csproj` |

---

## 实用技巧

### 技巧 1: 快速问"这是什么文件"

```
你: "ZtdApp/ViewModels/MainWindowViewModel.cs 是干嘛的？"

AI: (读取文件后)
"这是主窗口的逻辑代码。
它负责：
1. 处理导航按钮点击
2. 切换当前显示的页面
3. 管理窗口标题"
```

### 技巧 2: 快速问"这个效果怎么改"

```
你: "按钮颜色在哪里改？"

AI: "按钮使用了 Style='{StaticResource NavButton}'

     去 ZtdApp/Styles/BrandColors.xaml 找 NavButton 样式。
     或者直接把 Style 那行删掉，加 Background='Red' 测试。"
```

### 技巧 3: 批量理解一个文件

```
你: "帮我逐行解释 MainWindow.xaml"

AI: (展示关键行解释)
"第12行: <Grid> - 开始一个网格布局
 第13行: <ColumnDefinition Width="180"/> - 第一列宽180像素
 第14行: <ColumnDefinition Width="*"/> - 第二列占剩余空间
 ..."
```

---

## 学习节奏建议

### 日常开发时（每次30秒-2分钟）

```
看到不懂的代码
  ↓
问 Claude Code
  ↓
理解
  ↓
记录到 VOCABULARY.md（可选）
  ↓
继续开发
```

### 深入学习时（每周1-2次，每次30分钟）

```
选择一个文件（如 MainWindow.xaml）
  ↓
逐行问 "这是什么意思"
  ↓
尝试改一个小东西（🟢安全区）
  ↓
看效果
  ↓
记录学到的
```

---

## 常见问题

### Q: 在 Trae 改了代码，怎么让 Claude Code 知道？

**A:** Claude Code 会实时读取文件。直接问就行：
```
"我刚才改了 MainWindow.xaml，对吗？"

AI: (读取文件) "是的，你把宽度从180改成200了。"
```

### Q: Claude Code 建议了修改，怎么在 Trae 里快速定位？

**A:**
1. Claude Code 会给出文件路径和行号
2. 在 Trae 按 `Ctrl+P`，输入文件名
3. 按 `Ctrl+G`，输入行号

### Q: 怎么同时看代码和运行效果？

**A:**
```
Trae: 运行程序 (F5)
App: 弹出窗口显示效果
你: 对比代码和效果
    "哦，原来这行代码控制那个按钮！"
```

---

## 下一步

配置好环境后，开始 **第一次微学习**！

目标：理解 MainWindow.xaml 的结构，完成一个 🟢 安全修改。

```
开始吧！
```
