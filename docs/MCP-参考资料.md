# MCP (Model Context Protocol) 参考资料

## 什么是 MCP

MCP (Model Context Protocol) 是由 Anthropic 于 2024 年 11 月开源的标准，定义了 AI 模型如何与外部工具、数据源和系统连接。

- **开源时间**：2024 年 11 月 25 日
- **归属变更**：2025 年 12 月，Anthropic 将 MCP 捐赠给 Agentic AI Foundation（Linux Foundation），成为真正供应商中立的标准
- **目标**：取代碎片化的集成，提供单一协议连接 AI 系统与数据源

## 官方资源

| 资源 | 地址 |
|------|------|
| 官方文档 | https://modelcontextprotocol.io |
| Anthropic 官方公告 | https://www.anthropic.com/news/model-context-protocol |
| GitHub 组织 | https://github.com/modelcontextprotocol |

## GitHub 权威仓库

| 仓库 | 描述 | Stars | 用途 |
|------|------|-------|------|
| [modelcontextprotocol/servers](https://github.com/modelcontextprotocol/servers) | MCP 服务器集合 | 80K+ | 获取各种官方 MCP 服务器 |
| [modelcontextprotocol/python-sdk](https://github.com/modelcontextprotocol/python-sdk) | Python SDK | 21.9K | 开发 Python MCP 服务器/客户端 |
| [modelcontextprotocol/typescript-sdk](https://github.com/modelcontextprotocol/typescript-sdk) | TypeScript SDK | 11.7K | 开发 TypeScript MCP 服务器/客户端 |
| [modelcontextprotocol/modelcontextprotocol](https://github.com/modelcontextprotocol/modelcontextprotocol) | 规范和文档 | 7.3K | 查看官方规范 |
| [microsoft/mcp](https://github.com/microsoft/mcp) | Microsoft 官方 MCP 仓库 | - | Microsoft 官方服务器实现 |
| [modelcontextprotocol/inspector](https://github.com/modelcontextprotocol/inspector) | 可视化测试工具 | 8.9K | 调试 MCP 服务器 |

## 如何获取官方 MCP 工具

1. **访问官方 servers 仓库**：`https://github.com/modelcontextprotocol/servers`
2. **浏览可用服务器**：按类别搜索（Web、数据库、文件系统等）
3. **查看文档**：每个服务器都有 README 说明配置方法
4. **配置到 Claude**：在 `.claude/settings.json` 或 `.mcp.json` 中添加

## MCP 官方定义的核心组件

- **Host**：运行 AI 应用程序的宿主（如 Claude Desktop、VS Code）
- **Client**：MCP 协议的客户端实现
- **Server**：提供工具、资源、提示词的服务器
- **Transport**：客户端与服务器之间的通信层（stdio 或 SSE）

## 相关文档

- [GitHub Docs - MCP and Copilot coding agent](https://docs.github.com/en/copilot/concepts/coding-agent/mcp-and-coding-agent)
- [What Is MCP? A Practical Guide for 2026](https://www.agentwhispers.com/agent-guides/what-is-mcp)
- [MCP Developer Guide 2026](https://lushbinary.com/blog/mcp-model-context-protocol-developer-guide-2026/)
