# 项目技术配置

> 此文件由 /prd 自动生成，包含当前项目的技术栈配置。
> feat 技能通过引用此文件获取构建命令、样式系统、测试框架等信息。
> 换项目时只需重新生成此文件。

---

## 构建与运行

| 操作 | 命令 |
|------|------|
| 构建（Release） | `dotnet build ZtdApp --configuration Release` |
| 运行 | `ZtdApp/bin/Release/net8.0-windows/win-x64/ZtdApp.exe` |
| 单元测试 | `dotnet test ZtdApp.Tests` |
| 指定测试类 | `dotnet test ZtdApp.Tests --filter "FullyQualifiedName~类名"` |
| UI 自动化测试 | `dotnet test ZtdApp.Tests --filter "FullyQualifiedName~UITests"` |

---

## 技术栈

| 层级 | 技术 | 说明 |
|------|------|------|
| 框架 | WPF + .NET 8 | 桌面应用 |
| MVVM | CommunityToolkit.Mvvm 8.4.0 | `[ObservableProperty]`, `[RelayCommand]` |
| 数据库 | SQLite (Microsoft.Data.Sqlite) | 本地持久化 |
| 依赖注入 | Microsoft.Extensions.DependencyInjection | .NET 内置 |
| 测试 | xUnit 2.9.2 + FlaUI.UIA3 4.0.0 | 单元测试 + UI 自动化 |

---

## 样式系统

- **样式文件**: `ZtdApp/Styles/BrandColors.xaml`
- **加载方式**: `App.xaml` 中作为 MergedDictionary 引入
- **规则**: 所有 UI 属性通过命名样式或资源键引用，禁止内联写死 FontSize/Padding/Color

### 可用样式清单

详见 [design-guide.md](design-guide.md) 的完整样式表。

---

## UI 一致性检查

Verify 阶段使用此清单：

- [ ] 页面标题使用 `PageTitleTextBlock` 样式
- [ ] 页面说明使用 `PageDescriptionTextBlock` 样式
- [ ] 卡片容器使用 `CardBorder` 样式
- [ ] 卡片内文本使用 `CardContentTextBlock` / `CardTagTextBlock` / `CardDateTextBlock`
- [ ] 卡片内按钮使用 `CardActionButton` / `CardDeleteButton`
- [ ] 筛选按钮使用 `FilterChipButton` 为基础样式
- [ ] 按钮颜色符合品牌规范（主按钮橙色，次要按钮蓝色）
- [ ] **无内联 FontSize/Padding/Color** — 所有属性引用共享样式或资源键
- [ ] 整体风格与已有页面一致

---

## 测试约定

- 单元测试使用 `SharedMemoryDatabase` 扩展类（SQLite 内存模式 `Mode=Memory;Cache=Shared`）
- 每个测试文件创建独立的命名内存数据库变体
- UI 测试使用 FlaUI (UIA3) 启动编译后的 `.exe` 进行交互
- UI 测试需先完成 Release 构建
