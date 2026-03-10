# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository serves two purposes:
1. **Claude Code skills** (`/prd` and `/feat`) that guide the development workflow
2. **ZTD App** - a WPF desktop productivity app built with .NET 10

The `/prd` skill generates PRDs through conversation. The `/feat` skill implements features one at a time using a Design-Build-Test-Commit cycle with progress tracking in `.claude/feat-progress.json`.

## Development Environment

- **.NET SDK**: 10.0 or later
- **NuGet Source**: Must configure official NuGet source

```bash
# Check .NET version
dotnet --version  # Should be 10.0.x

# Configure NuGet source (if not already configured)
dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
```

## Build & Test Commands

```bash
# Build (Release, self-contained win-x64)
dotnet build ZtdApp --configuration Release

# Run the app
ZtdApp/bin/Release/net10.0-windows/win-x64/ZtdApp.exe

# Run all unit tests
dotnet test ZtdApp.Tests

# Run a specific test class
dotnet test ZtdApp.Tests --filter "FullyQualifiedName~IdeaTests"

# Run UI automation tests (requires Release build first)
dotnet test ZtdApp.Tests --filter "FullyQualifiedName~UITests"
```

No `.sln` file exists - use project paths directly with `dotnet` commands. The app is self-contained (`win-x64`), so `dotnet publish` is not needed for local development.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | WPF, .NET 10 (`net10.0-windows`) |
| MVVM | CommunityToolkit.Mvvm 8.4.0 (`[ObservableProperty]`, `[RelayCommand]`) |
| Database | SQLite via Microsoft.Data.Sqlite 9.0.0 |
| DI | Microsoft.Extensions.DependencyInjection 9.0.0 |
| Testing | xunit 2.9.2 + FlaUI.UIA3 4.0.0 (UI automation) |

## Architecture

MVVM with 4-layer separation. All services registered in `App.xaml.cs`: data repositories and managers as singletons, `QuickAddViewModel` as transient.

```
Views (XAML)  →  ViewModels  →  Services (*Manager)  →  Data (*Repository)  →  SQLite
```

- **Models** (`ZtdApp/Models/`): Plain C# classes with `INotifyPropertyChanged` for UI-bound properties. IDs are GUIDs, timestamps are Unix milliseconds.
- **Data** (`ZtdApp/Data/`): Repository pattern with raw SQL via `Microsoft.Data.Sqlite`. `DatabaseService` manages connection string and schema creation.
- **Services** (`ZtdApp/Services/`): `*Manager` classes encapsulate business logic, one per model. `HotKeyService` handles global hotkey registration for the quick add dialog.
- **ViewModels** (`ZtdApp/ViewModels/`): Use CommunityToolkit.Mvvm source generators. `MainWindowViewModel` handles sidebar navigation by swapping `CurrentPage` between page ViewModels (`IdeaViewModel`, `TodoViewModel`, `TodayViewModel`, `NotesViewModel`, `WeeklyReviewViewModel`). `QuickAddViewModel` is registered as transient (per dialog instance).
- **Views**: Single-window app (`MainWindow.xaml`) with `ContentControl` + `DataTemplate` per page ViewModel for navigation. All page templates are inline in MainWindow.xaml.

### Navigation Pattern

`MainWindow.xaml` uses `ContentControl.Content` bound to `MainWindowViewModel.CurrentPage`. Each page ViewModel type gets a `DataTemplate` in `ContentControl.Resources`. Navigation commands on `MainWindowViewModel` set `CurrentPage` and `CurrentTitle`.

### Database

SQLite stored at `%LOCALAPPDATA%/ZtdApp/ztd.db`. Schema created in `DatabaseService.Initialize()`. Tables: `Ideas`, `Tasks`, `Notes`, `Tomatoes`.

## Styling

All styles defined in `ZtdApp/Styles/BrandColors.xaml` (loaded as merged resource in `App.xaml`). Uses Anthropic brand colors:

| Resource Key | Usage |
|-------------|-------|
| `BrandOrange` (#d97757) | Primary buttons, accents |
| `BrandBlue` (#6a9bcc) | Secondary buttons (e.g., idea action buttons) |
| `BrandGreen` (#788c5d) | Status indicators |
| `BrandLight` (#faf9f5) | Background |
| `BrandDark` (#141413) | Text |
| `BrandLightGray` (#e8e6dc) | Borders, sidebar background |
| `BrandMidGray` (#b0aea5) | Disabled state, subtext |

Named styles: `CardBorder`, `NavButton`, `NavButtonActive`, `DeleteButton`, `IdeaActionButton`, `SecondaryButton`, `HeadingTextBlock`, `SubheadingTextBlock`, `SidebarBorder`, `MainWindowStyle`, `PageTitleTextBlock`, `PageDescriptionTextBlock`, `FilterChipButton`, `CardActionButton`, `CardDeleteButton`, `GroupHeaderTextBlock`, `CardContentTextBlock`, `CardTagTextBlock`, `CardDateTextBlock`, `CardDeleteButtonHover`, `CardActionButtonHover`.

**Hover reveal pattern**: Use `CardDeleteButtonHover`/`CardActionButtonHover` styles for buttons that should only appear on card hover. These use `DataTrigger` with `RelativeSource={RelativeSource AncestorType=Border}` and fade in/out via `Opacity` animations (200ms enter, 150ms exit).

**Always use existing BrandColors.xaml styles** - never define new colors inline.

## WPF Button Height Control

**CRITICAL**: Never use `Height` with fixed values on buttons - it causes text clipping.

To ensure multiple buttons have consistent height, ALL of the following properties must be specified together:

```xml
<Style TargetType="Button">
    <Setter Property="Margin" Value="5"/>
    <Setter Property="Padding" Value="16,8"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="VerticalContentAlignment" Value="Center"/>
</Style>
```

WPF button height is determined by: `Margin + BorderThickness + Padding + FontSize + FontWeight`. Even when using `BasedOn` styles, you must explicitly override all these properties to ensure consistency. This pattern is tracked as a recurring issue in `known-issues.json` (ui-001, ui-002, ui-004).

## Testing Patterns

Unit tests use a `SharedMemoryDatabase` class that extends `DatabaseService` with `Mode=Memory;Cache=Shared` SQLite connections. Each test class creates its own named in-memory DB and disposes it. Multiple `SharedMemoryDatabase` variants exist across test files (e.g., `SharedMemoryDatabase`, `SharedMemoryDatabase3`) - when adding a new test file, create a new variant or reuse an appropriate one.

UI tests use FlaUI (UIA3) to launch the compiled `.exe` and interact with the window. They require a Release build before running.

## Key Conventions

- **One feature at a time** via `/feat` workflow
- **Commit format**: `feat: [功能名称]`
- **PRD** lives at `docs/PRD.md` - updated after each feature commit
- **Progress file** `.claude/feat-progress.json` tracks in-flight feature work (gitignored)
- **Known issues** tracked in `.claude/known-issues.json` and referenced in PRD.md
- Version history auto-recorded to `.claude/version-history.log` by PostGitCommand hook
