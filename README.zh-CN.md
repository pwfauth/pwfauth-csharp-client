<p align="center"><a href="README.md">English</a> · <b>简体中文</b> · <a href="README.tr.md">Türkçe</a> · <a href="README.ar.md">العربية</a></p>
<p align="center">视频教程：<a href="https://www.youtube.com/@pwfauth">youtube.com/@pwfauth</a></p>

# PWF Auth C# 桌面客户端

本版本使用 **PWFAuth 1.7.0**。仓库自带的 NuGet.Config 会从 vendor/ 还原经过验证的官方 SDK，nuget.org 上也有同一个包。PWFAuth 1.4.0 及更早版本无法与当前服务器通信（服务器改用新密钥为响应签名），请升级旧副本

这是官方 [`PWFAuth` NuGet 包](https://www.nuget.org/packages/PWFAuth)（1.7.0 版）的英文界面 Windows Forms 示例，基于 .NET Framework 4.8.1。该应用可以：

- 使用许可证密钥登录；
- 通过后台心跳保持会话；
- 在服务器结束会话时自动退出登录；
- 在仪表盘中显示完整的登录响应；
- 在服务器上为该密钥保存一个值（用户变量）

![登录后的仪表盘](docs/dashboard.png)

## 功能演示

- **使用许可证密钥登录。** `LoginAsync` 会把密钥绑定到当前电脑，随后仪表盘会显示：
  - 密钥的状态、类型、到期时间和剩余时间；
  - 密钥的售出者；
  - 响应中的每一个字段
- **远程停用（kill switch）。** 登录后会启动 `StartHeartbeat`。当你在后台封禁、暂停或重置该密钥，或者密钥过期时，包会触发 `SessionEnded` 事件，应用随即返回登录页并显示原因。该事件在 UI 线程上触发，因此无需调用 `Invoke`
- **将许可证迁移到新电脑。** 如果密钥已绑定到另一台电脑，登录页会提供 **Move this license to this PC** 选项。它会调用 `ResetHardwareIdAsync`，然后重新登录。两次迁移之间受开发者设置的冷却时间限制（默认 12 小时）

  ![Move this license to this PC 选项](docs/move-license.png)

- **用户变量。** 登录后，应用会读取 `sample_opens`，加一后通过 `GetVariableAsync` / `SetVariableAsync` 保存，在标题下方显示 “Opened N times with this key”，并在详情表格中列出该密钥的全部变量（`GetVariablesAsync`）。在后台打开该密钥（**Licences → 该密钥 → Variables**）就能看到这个计数；你在那里修改的值，就是应用下次读取到的值。变量会随密钥一起迁移到新电脑
- **清晰的错误提示。** 出现以下情况时，应用会显示简明的提示：
  - 没有网络连接；
  - 服务器拒绝了应用密钥（app secret），返回 HTTP 401；
  - `App.config` 中的应用密钥仍是占位符
- **干净地退出登录。** 点击 **Sign Out** 或直接关闭窗口，都会立即在服务器上结束会话

该包还会自动修正不准确的电脑时钟，并拒绝任何声称成功却未加密的响应

## 环境要求

- Windows 10 或更高版本
- 安装了 “.NET 桌面开发” 工作负载的 Visual Studio
- .NET Framework 4.8.1

## 配置

在 `PWFAuthCSharp/App.config` 中填入 64 个字符的应用密钥：

```xml
<add key="PWFAuthAppSecret" value="YOUR_64_CHARACTER_APP_SECRET" />
```

请在分发前配置好密钥，切勿将其提交到仓库。服务器地址固定为 `https://pwfauth.com`，通过环境变量进行的覆盖会被忽略

## 构建与运行

1. 在 Visual Studio 中打开 `PWFAuthCSharp.slnx`
2. 按提示还原 NuGet 包
3. 选择 `Debug | Any CPU`，然后按 `F5`

从旧版示例升级？请还原一次 NuGet 包，让 `PWFAuth` 1.7.0 替换 `packages` 文件夹中的旧版本
