# SentinelAnalytics — Project Context

> Generated: 2026-04-16

---

## Purpose

Self-hosted enterprise mobile analytics platform — a .NET-native alternative to Microsoft AppCenter. Tracks crashes, events, and sessions from iOS/Android apps. Live at `https://analytics-mobile.com/`.

---

## Solution Structure

`SentinelAnalytics.slnx` (XML solution format) — two projects:

| Project | Path | Tech | Role |
|---|---|---|---|
| Backend + Dashboard | `SentinelAnalytics/` | ASP.NET Core 9 MVC | REST API, Razor UI, background services |
| Client SDK | `SentinelAnalytics.Client/` | .NET MAUI 10 | NuGet SDK for iOS + Android |

---

## Key Technologies

### Backend (`SentinelAnalytics/`)
| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core MVC | .NET 9 | Web framework (controllers + Razor views) |
| Entity Framework Core | 9.0.12 | ORM / SQL Server (LocalDB for dev) |
| ASP.NET Core Identity | 9.0.12 | Authentication, email confirmation |
| Serilog | 10.0.0 | Structured logging (console + rolling file) |
| MailKit / MimeKit | 4.15.1 | SMTP email sending |
| Google Gemini API | REST (gemini-2.0-flash) | AI crash analysis |

### Client SDK (`SentinelAnalytics.Client/`)
| Technology | Version | Purpose |
|---|---|---|
| .NET MAUI | 10.0.41 | Cross-platform mobile targets |
| Target Frameworks | `net10.0-ios`, `net10.0-android` | iOS and Android support |

---

## Backend Architecture

### Controllers (7)

| Controller | Auth | Responsibility |
|---|---|---|
| `HomeController` | Public | Landing page, pricing, privacy, support, terms |
| `IngestController` | `X-Sentinel-Key` header | Core ingestion API: sessions, events, crashes |
| `DashboardController` | `[Authorize]` | Crash dashboard, filtering, AI analysis, CSV export, resolution workflow |
| `AnalyticsController` | `[Authorize]` | DAU, session duration, regional/language breakdowns |
| `ProjectController` | `[Authorize]` | Project CRUD, plan limits, invite acceptance |
| `TeamController` | `[Authorize]` | Team invites, role enforcement, access revocation |
| `BillingController` | Auth | Subscription plans, usage tracking, plan switching |

### Data Entities (11)

| Entity | Key Fields |
|---|---|
| `Project` | `ApiKey` (GUID, unique index), `Platform`, owner |
| `Session` | Device ID, country, language, app version, OS version, device model |
| `CrashReport` | Exception name/message/stack trace, `SeverityType`, optional user ID, JSON properties, `IsResolved`, `ResolvedAt`, `ResolutionComment` |
| `MobileEvent` | Named event, optional JSON properties, linked session |
| `ProjectMember` | `ProjectRoleType` (`Manager`/`Developer`), invite/acceptance flow |
| `UserDetail` | Links Identity user to `PricingPlan` |
| `UserSubscription` | Per-user notification preferences (critical/error/regression) |
| `PricingPlan` | Seeded tiers: Free ($0, 1 project, 1K events/mo), Pro ($49, 10 projects, 100K events/mo), Max ($199, 100 projects, 1M events/mo) |
| `AuditableEntity` | Abstract base — auto `CreatedAt` / `UpdatedAt` via EF interceptor |

### Services (5)

| Service | Role |
|---|---|
| `GeminiService` | Calls Gemini 2.0 Flash to analyze crashes and suggest fixes |
| `CrashNotificationService` | HTML email notifications to team on new crashes (single + batch) |
| `CrashReportNotificatorBackgroundService` | `BackgroundService` — daily digest notification loop |
| `SentinelEmailSender` | `IEmailSender` implementation via MailKit/MimeKit SMTP |
| `DashboardService` | Stub/placeholder (not yet implemented) |

### Database

- **9 EF Core migrations** (Feb–Apr 2026): init → crash resolution → project teams → sessions → audit → subscriptions → pricing plans
- `SentinelDbContext` extends `IdentityDbContext`
- `AuditSaveChangesInterceptor` auto-stamps `CreatedAt`/`UpdatedAt`
- 7 EF Fluent API configuration classes in `Data/Configurations/`

---

## Client SDK Architecture

### Public API — `SentinelTracker` (static)

| Method | Description |
|---|---|
| `Initialize(apiKey, url, options)` | Sets up the tracker; hooks global exception handlers |
| `TrackEvent[Async](name, props)` | Sends a named event with optional properties |
| `TrackError[Async](ex, severity)` | Sends a crash/error report |
| `GenerateTestCrash()` | Throws `TestCrashException` for SDK verification |

Auto-hooks: `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`, Android/iOS native crash handlers.

### Internal Classes

| Class | Role |
|---|---|
| `SentinelHttpClient` | HTTP client targeting `/api/ingest/`; sends `X-Sentinel-Key` header |
| `DeviceInfoProvider` | Collects model, OS version, country, language, device ID |

### DTOs (shared client ↔ server)
`InitSessionDto`, `CrashReportDto`, `MobileEventDto`, `Severity`

---

## Notable Patterns

- **Plan enforcement at ingestion** — monthly crash/event quota checks; returns HTTP 429 when exceeded
- **API key auth** for ingest endpoints — stateless, `X-Sentinel-Key` header
- **Session breadcrumbs** — crash detail view shows events from same session prior to crash
- **Regression detection** — crashes flagged as regressions if they reoccur after a prior resolution
- **Emulator suppression** — `isIgnoreEmulators: true` option in SDK init

---

## Configuration

- `appsettings.json` / `appsettings.Development.json` — SQL Server connection string, Gemini API key, SMTP credentials
- User Secrets (`UserSecretsId`) for local secret overrides
- `Properties/launchSettings.json` — standard ASP.NET Core launch profiles

---

## CI/CD

`.github/workflows/nuget-publish.yml` — triggers on `v*.*.*` tags; builds + packs `SentinelAnalytics.MAUI.csproj`; publishes to NuGet.org via `NUGET_API_KEY` secret. Runs on `ubuntu-latest` with .NET 10.

---

## Current Working State (as of 2026-04-16)

Several files are modified but not yet committed, including:
- `BillingController.cs` (new)
- `CrashNotificationService.cs` / `CrashReportNotificatorBackgroundService.cs` (new)
- `PricingPlan.cs` / `UserDetail.cs` entities (new)
- 3 new EF migrations (AddProjectMemberUser, RemoveFrequencySubscription, AddPricePlans)
- `Severity.cs` DTO added to client SDK
- Updates across controllers, views, and configuration files
