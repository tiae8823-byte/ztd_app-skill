# 进度管理详解

> 功能开发进度保存、恢复和管理机制

---

## 进度文件位置

`.claude/feat-progress.json` - 功能开发进度文件

**注意**：此文件不提交到 git（在 .gitignore 中）

---

## 进度文件格式

```json
{
  "currentFeature": {
    "name": "功能名称",
    "status": "design",
    "level": "CRITICAL",
    "createdAt": "2026-02-27T14:30:00Z",
    "description": "开始实现功能",
    "testIntent": [
      {
        "scenario": "开始计时",
        "given": "番茄钟未启动，剩余时间 = 25:00",
        "when": "用户点击开始按钮",
        "then": "倒计时启动，每秒更新显示"
      }
    ],
    "designLayout": "垂直布局",
    "designComponents": ["Button", "TextBox", "Card"],
    "designSketch": "[简单的ASCII布局图]",
    "filesCreated": [],
    "testsCreated": [],
    "lastCheckpoint": "UI 设计完成，等待用户确认测试意图"
  }
}
```

---

## Status 值说明

| Status | 说明 | 已完成阶段 |
|--------|------|-----------|
| `design` | 正在进行 UI 设计和测试意图定义 | - |
| `intent` | 设计完成，CRITICAL 功能生成测试代码 | design |
| `build` | 测试意图确认，正在编写代码 | design, intent |
| `verify` | 代码完成，正在编译验证 | design, intent, build |
| `commit` | 验证完成，准备提交 | design, intent, build, verify |

---

## 进度保存时机

| 阶段 | 操作 | 文件变更 |
|------|------|---------|
| 开始功能 | 创建进度文件 | status: design |
| Design 完成 | 更新进度 | status: intent, testIntent, designLayout, designComponents |
| Intent 完成（仅CRITICAL） | 更新进度 | status: build, testsCreated |
| Build 完成 | 更新进度 | status: verify, filesCreated |
| Verify 完成 | 更新进度 | status: commit |
| Commit 完成 | 删除进度文件 | 清除 |

---

## 进度恢复流程

1. **检测进度**：`/feat` 执行时读取 `feat-progress.json`
2. **提示用户**：显示当前功能、状态、最后记录、设计信息
3. **用户选择**：
   - Y: 跳转到对应阶段继续开发
   - N: 删除进度文件，重新开始

### 恢复提示示例

```markdown
📋 检测到未完成的功能：

当前功能: 番茄钟计时
功能级别: CRITICAL
状态: build（已完成: design, intent）
测试意图:
  1. 开始计时: 点击开始 → 倒计时启动
  2. 暂停功能: 点击暂停 → 倒计时停止
设计方案: 垂直布局, Button, TextBox, CardBorder
最后记录: Intent 阶段完成，已生成测试代码
创建时间: 2026-02-27 14:30

是否继续？
- Y: 继续开发
- N: 重新开始
```

---

## 进度文件管理规则

- 功能完成 commit 后**必须删除**进度文件
- 进度文件**不提交**到 git（在 .gitignore 中）
- 用户选择"重新开始"时**立即清除**进度
- 每个阶段完成后**立即更新**进度文件

---

## 中断恢复场景

### 场景1：设计阶段中断

```
用户: /feat
→ 选择功能
→ Design 阶段，确认布局和测试意图后
→ 用户关闭终端

下次 /feat:
→ 检测到 status: design
→ 提示："正在设计阶段，已确认布局和测试意图，是否进入 Intent 阶段？"
→ Y: 进入 Intent 阶段（CRITICAL 生成测试代码）
→ N: 清除进度，重新开始
```

### 场景2：Intent 阶段中断

```
用户: /feat
→ Intent 阶段，已生成测试代码
→ 用户有事离开

下次 /feat:
→ 检测到 status: intent
→ 提示："Intent 阶段完成，已生成测试代码，是否继续编码？"
→ Y: 进入 Build 阶段
→ N: 清除进度（已创建测试文件保留，可手动删除）
```

### 场景3：编码阶段中断

```
用户: /feat
→ Build 阶段，创建了部分文件
→ 用户有事离开

下次 /feat:
→ 检测到 status: build
→ 提示："正在编码阶段，已创建 X 个文件，是否继续？"
→ Y: 继续 Build 阶段
→ N: 清除进度（已创建文件保留，可手动删除）
```

### 场景4：验证失败中断

```
用户: /feat
→ Verify 阶段，编译失败或测试未通过
→ 尝试修复 1 次仍未解决
→ 用户决定明天再处理

下次 /feat:
→ 检测到 status: verify
→ 提示："验证阶段遇到问题，已尝试修复 1 次，是否继续？"
→ Y: 继续尝试修复
→ N: 清除进度，可回滚代码
```
