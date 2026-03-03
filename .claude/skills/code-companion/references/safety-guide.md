# 安全评估详则

AI 评估改动安全性时参考此文件。

## 文件安全地图

| 文件/目录 | 等级 | 说明 |
|-----------|------|------|
| `Styles/BrandColors.xaml` | 🟢 | 颜色和样式定义，改了立即见效 |
| `Views/MainWindow.xaml` 视觉部分 | 🟢 | Text、Content、Margin、Width、Background |
| `Views/MainWindow.xaml` 绑定部分 | 🟡 | `{Binding}`、`Command`、`DataTemplate` |
| `ViewModels/*.cs` | 🟡 | 界面逻辑，改名会断绑定 |
| `Models/*.cs` | 🟡 | 数据结构，改字段可能影响数据库 |
| `Services/*.cs` | 🔴 | 核心业务逻辑 |
| `Data/*.cs` | 🔴 | 数据库操作，改错会丢数据 |
| `App.xaml.cs` | 🔴 | 启动和依赖注入，改错启动不了 |
| `*.csproj` | 🔴 | 项目配置，改错编译不了 |

## 快速判断规则

改动前过一遍这 3 条：

1. **只改视觉？** 文字/颜色/大小/间距 → 🟢
2. **涉及 `{Binding}` 或 `Command`？** → 🟡 需要确认 ViewModel 对应属性存在
3. **涉及 `.cs` 文件的逻辑代码？** → 🟡🔴 建议用 /feat

## 🟢 安全改动清单

```
文字内容：Button 的 Content、TextBlock 的 Text、Window 的 Title
颜色样式：Background、Foreground、引用 BrandColors 中的资源
尺寸间距：Width、Height、Margin、Padding、FontSize
布局比例：Grid 的 ColumnDefinition/RowDefinition
```

## 🟡 需确认的改动

```
绑定路径：{Binding XXX} — 改名要同步改 ViewModel
命令绑定：Command="{Binding XXXCommand}" — 确保命令存在
事件处理：Click="XXX" — 确保有对应方法
新增控件：可能需要 ViewModel 配合
```

## 🔴 不执行，引导用 /feat

```
Data/ 下的任何文件
Services/ 下的业务逻辑
App.xaml.cs 的服务注册
数据库 schema 变更
新增页面/功能模块
```

## 出错时的回滚

告诉用户：
- `git checkout -- 文件路径` — 恢复单个文件
- `git stash` — 暂存所有改动
- `git stash pop` — 恢复暂存的改动
- 强调：**不用怕改坏，git 记着所有历史**
