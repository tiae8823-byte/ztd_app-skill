# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Claude Code skill repository** for developing the "Zen To Done" (ZTD) productivity app. It's not a traditional software project - it contains two user-invocable skills that guide the development workflow.

- **prd**: Generate MVP-focused PRD through natural conversation
- **feat**: Implement features using Build-Test-Commit cycle

## Skills Workflow

### `/prd` - PRD Generation

Use when starting a new project or user says "生成PRD", "需求文档", "产品需求".

The PRD skill follows MVP thinking:
- Don't try to figure out all details upfront
- Define core value and minimum viable form
- Tech stack must be determined (high refactoring cost)
- Features can be added incrementally

**Key questions to ask**:
- What problem does this product solve?
- Who is the target user?
- What is the core value (one sentence)?

**Tech stack recommendation approach**:
- Business complexity → Backend complexity
- User scale → Deployment method
- Team situation → Tech selection
- Interaction complexity → Frontend framework

Available MCP tools:
- `mcp__sequential-thinking__sequentialthinking` - Complex requirement analysis
- `mcp__github__search_repositories` - Search similar projects
- `mcp__context7__resolve-library-id` + `mcp__context7__query-docs` - Query framework docs
- `mcp__web_reader__webReader` - Read reference websites

### `/feat` - Feature Implementation

Use when implementing features or user says "实现功能", "开发功能", "写代码".

The feat skill follows Build-Test-Commit cycle for implementing **one feature at a time**.

**Cycle**:
```
Write code → Write tests → Verify pass → Commit
   ↓           ↓            ↓
  Modify     Modify     Rollback
```

**Workflow**:
1. Read `docs/PRD.md` - If missing, prompt user to run `/prd` first
2. Display feature list and let user choose one feature
3. Implement: Build → Test → Verify → Commit
4. Update `docs/PRD.md` to mark feature as completed
5. Ask if user wants to continue to next feature

**Commit message format**: `feat: [feature name]`

**PRD update format**:
```markdown
### MVP 功能（必须实现）

- [x] Feature name ✅ commit: abc123
```

## Target App: ZTD (Zen To Done)

The PRD describes a productivity/time management/habit tracking app:

**Tech Stack**:
- Frontend: React Native + Expo SDK 54
- Local DB: Expo SQLite (offline-first)
- Backend: Supabase (Auth + Data Sync)
- Sync: Turso / RxDB (bidirectional)
- Deploy: Expo EAS Build

**Core MVP Features**:
1. 快速收集 (Quick capture) - One-click capture of ideas/tasks/notes to inbox
2. 今日要事 (Today's important things) - Set 1-3 most important tasks per day
3. 习惯追踪 (Habit tracking) - Track daily habits with streak count
4. 每周回顾 (Weekly review) - Guided weekly review to reflect and plan
5. 任务处理 (Task processing) - Inbox classification: do/delegate/delete/archive

**Data Models**:
- Task (inbox | todo | today | done | deleted)
- Habit (with streak tracking)
- HabitLog (completion records)
- WeeklyReview (accomplishments, challenges, next_week_focus)

## Important Principles

- **One feature at a time** - Don't implement multiple features in one session
- **PRD is alive** - Can update PRD at any time during feat process
- **MVP thinking** - Start small, iterate fast
- **Offline-first** - App works fully offline, syncs when connected
- **Minimal UI library** - shadcn/ui or similar for consistent Apple-style design

## File Structure

```
.claude/
├── settings.local.json    # Permissions config
└── skills/
    ├── feat/SKILL.md     # Feature implementation skill
    └── prd/SKILL.md      # PRD generation skill
docs/
└── PRD.md               # Product requirements document
```
