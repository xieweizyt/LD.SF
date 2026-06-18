# LD.SF

LD.SF 是基于现有 `需求文档.md` 重新设计的三端项目，用 Vue + ABP vNext + EF Core + SQL Server + Kotlin Android App 替换原 PHP + Auto.js 实现。

- `api`：ABP vNext 分层后端，包含 `Domain`、`Application.Contracts`、`Application`、`EntityFrameworkCore`、`HttpApi`、`HttpApi.Host`。提供管理员登录、分后台管理、授权标识、次数管理、任务管理、发送记录、App 拉取任务和 RSA 加密次数返回。
- `web/ld-sf-web`：Vue 3 + TypeScript + Element Plus + Pinia + Vue Router 管理端，目录结构参考 `D:\公司项目\EMP2J.Tunnel7Web` 的 `src/api`、`src/router`、`src/store`、`src/layout`、`src/views` 分层。
- `android/LD.SF.App`：Android 原生 Kotlin App，使用 Jetpack Compose、Retrofit、OkHttp、Kotlin Coroutines、Lifecycle ViewModel。用于输入标识、生成 RSA 公钥、获取授权次数、拉取任务，并打开系统短信编辑界面让用户确认发送。

## 合规设计边界

Android 端不实现后台静默群发，也不自动点击系统短信发送按钮。App 只会通过系统短信 Intent 打开短信编辑界面并填入收件人和内容，由用户在系统短信 App 中确认发送。该设计用于已获授权号码的通知任务，避免未授权批量发送、骚扰或其他违法用途。

## 快速启动 API

```powershell
cd D:\项目\收发源码\LD.SF\api\LD.SF.Api
dotnet run
```

请改为：

```powershell
cd D:\项目\收发源码\LD.SF\api\src\LdSf.HttpApi.Host
dotnet restore
dotnet run
```

默认监听 `http://localhost:5000`，接口前缀为 `/api/ldsf`。

默认连接字符串：

```json
"Server=localhost;Database=LDSF;Trusted_Connection=True;TrustServerCertificate=True"
```

如你的 SQL Server 地址不同，请修改 `api/LD.SF.Api/appsettings.json` 的 `ConnectionStrings:SqlServer`。

当前源码使用 ABP + EF Core，并在开发启动时使用 `Database.EnsureCreated()` 自动建库建表，适合开发验证。正式项目建议改为 ABP/EF Core Migrations。
启动时会先尝试连接 SQL Server 的 `master` 库并自动创建目标数据库，再执行建表和种子数据初始化。

内置演示账号：

```text
账号：admin
密码：admin123
授权标识：DEMO-001
```

## 快速启动 Vue

```powershell
cd D:\项目\收发源码\LD.SF\web\ld-sf-web
npm install
npm run dev
```

前端默认通过 Vite 代理访问 `/api`，代理目标是 `http://localhost:5000`。如 API 地址不同，请修改 `web/ld-sf-web/vite.config.ts` 或 `.env` 中的 `VITE_API_BASE`。

## Android 构建

当前机器未检测到 Java/Android 构建环境。请安装 Android Studio 和 JDK 17 后打开：

```text
D:\项目\收发源码\LD.SF\android\LD.SF.App
```

在 `ApiConfig.kt` 中配置 API 服务地址后构建运行。

模拟器访问本机 API 使用：

```kotlin
const val BASE_URL = "http://10.0.2.2:5000/"
```

真机访问时请改成电脑局域网 IP，例如：

```kotlin
const val BASE_URL = "http://192.168.1.10:5000/"
```

## 核心数据表

- `LdSfAdminUsers`：总后台管理员。
- `LdSfSubaccounts`：分后台，保存剩余次数和已发送次数。
- `LdSfAuthorizationCodes`：授权标识，保存唯一标识、剩余次数、累计授权次数、累计使用次数、App RSA 公钥。
- `LdSfSmsTasks`：任务单，保存任务名称、批量大小、短信内容、状态。
- `LdSfTaskPhoneNumbers`：任务号码明细。
- `LdSfSendRecords`：发送记录，App 用户确认发送后写入。
- `LdSfUsageLedgers`：次数流水，记录平台增次和 App 消耗。

## 主要接口

- `POST /api/ldsf/auth/login`：平台登录。
- `GET /api/ldsf/admin/subaccounts`：分后台列表。
- `POST /api/ldsf/admin/subaccounts`：创建分后台和授权标识。
- `POST /api/ldsf/admin/subaccounts/{id}/balance`：设置分后台余额。
- `POST /api/ldsf/admin/authorizations/{identifier}/grant`：给授权标识增加次数。
- `GET /api/ldsf/admin/ledger`：次数流水。
- `GET /api/ldsf/subaccounts/{id}/tasks`：分后台任务列表。
- `POST /api/ldsf/subaccounts/{id}/tasks`：创建任务。
- `POST /api/ldsf/app/authorize`：App 输入标识获取次数，返回明文对象和 RSA 加密信封。
- `GET /api/ldsf/app/tasks/{identifier}`：App 根据标识拉取任务。
- `POST /api/ldsf/app/tasks/{taskId}/confirm-sent`：App 在用户确认系统短信发送后回传扣次并写发送记录。
