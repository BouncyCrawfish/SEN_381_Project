# CivicConnect

A community service-request management platform. CivicConnect replaces the current fragmented mix of email, phone calls, WhatsApp messages, spreadsheets and paper records with a single controlled system for submitting, tracking, actioning and reporting on service requests (facility faults, maintenance issues, lost property, IT support and similar).

> **Project status: Final project (Milestone 4) complete. The final release is tagged on `main` and remains in simulated staging: production is a deliberate no-go until the security and operational gaps in the PED are closed.** The Windows Forms application is built and runs against PostgreSQL for all three roles (Requester, Staff, Manager). Email notification, overdue requests (FR-015) and a few smaller items are deferred, and the duplicate audit-row defect (DEF-01) is fixed. A GitHub Actions CI pipeline checks for committed secrets, builds, runs the tests, runs static analysis and checks dependencies on every push and pull request. The final scope completion, go-live decision and closure of defects, risks and technical debt are in the PED's "Final project closure" chapter; the M3 release evidence is kept there as history. See [Known Limitations / TODOs](#known-limitations--todos).

---

## Table of Contents

- [Purpose](#purpose)
- [Current Implementation Status](#current-implementation-status)
- [Prerequisites](#prerequisites)
- [Setup & Run Instructions](#setup--run-instructions)
- [Technology / Runtime / Dependency Versions](#technology--runtime--dependency-versions)
- [Repository Structure](#repository-structure)
- [Database / Schema](#database--schema)
- [API / Module Interfaces](#api--module-interfaces)
- [Environment / Configuration](#environment--configuration)
- [Module Responsibilities & Design Patterns](#module-responsibilities--design-patterns)
- [Testing / Running Checks](#testing--running-checks)
- [Known Limitations / TODOs](#known-limitations--todos)
- [Engineering Evidence Links](#engineering-evidence-links)

---

## Purpose

CivicConnect gives a community organisation a reliable, traceable and auditable way to manage service requests end to end:

- **Requesters** submit, categorise and track the status of their own requests.
- **Staff** view, search, assign, action and resolve requests through controlled status transitions.
- **Management** get visibility into all requests, counts by status and category, and the full audit trail of status changes.

The full business context, stakeholder analysis, requirements (FR-001 to FR-017, NFR-001 to NFR-008) and constraints are baselined in the [Project Engineering Document (PED)](#engineering-evidence-links).

## Current Implementation Status

| Area | Status | Notes |
|---|---|---|
| Architecture | **Built**: modular monolith as .NET class libraries (Common, Auth, Requests, AuditLog, Notifications) plus the WinForms app | ADR-001. There is no separate Reporting project: management queries live in the Requests module and the audit trail in the AuditLog module |
| Database | **Built**: PostgreSQL schema, roles, grants, seed categories and dev seed users from the scripts in `db/` | ADR-002. The schema is owned by the SQL scripts; there are no EF migrations |
| Users and auth | **Built**: full-screen sign-in and registration (registration always creates a Requester), BCrypt password hashes, one role-permission map, login lockout | ADR-006, DEC-004. `ChangePasswordAsync` exists in the service, but there is no change-password screen |
| Requests | **Built**: submit, my requests, staff queue with search and filter, request detail with history and notes, assign, status transitions (State Pattern), notes, optimistic concurrency, manager overview | ADR-002, ADR-004 |
| Audit log | **Built**: read-only history service and query, manager **Audit trail** screen, domain-event dispatcher | History rows are written by the Requests module in the same transaction as the change; `AuditLogEventHandler` is deliberately not registered (DEF-01 fix) |
| Notifications | **Partly built**: in-app popup for the person who changes a request's status. **Email is deferred** | ADR-005, DEC-001 |
| Automated tests | **Built**: xUnit v3 tests for Auth, Requests, AuditLog and Notifications, plus a staff-queue load test (TC-21) | Windows only; mostly in-memory fakes. The two database tests run only when `CIVICCONNECT_TEST_CONNECTION` is set |
| Staging | **Simulated**: tagged Release build published to a folder, database rebuilt from the scripts, configuration from an environment variable | [`Docs/STAGING.md`](Docs/STAGING.md) |
| CI pipeline | **Built**: GitHub Actions (`.github/workflows/ci.yml`) secrets guard, Release build, tests, .NET static analysis and a vulnerable-package check on a Windows runner for every push and pull request to `main` and the development branch; Dependabot for updates | See [Continuous integration](#continuous-integration). No database tests run in CI |

### Screens by role

All screens open maximized and share one theme (`Forms/Theme.cs`).

| Role | Screens |
|---|---|
| Everyone | Sign in, Create a requester account |
| Requester | Submit request, My requests, request detail (own requests only, no staff notes or contact details) |
| Staff | Staff queue (status filter, search, "assigned to me"), request detail with response panel (assign, change status, add note) |
| Manager | Overview and all requests (counts by status and category, list filtered by status and search), **Audit trail** (every status change, filter by request, status and date range). Read-only: managers cannot change requests |

Each screen is also protected in the service layer: hiding a button is a convenience, never the only check. Closing the home screen signs the user out.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/) (target framework: `net10.0-windows`)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) (Windows, required for WinForms designer support)
- [PostgreSQL 16+](https://www.postgresql.org/) and pgAdmin 4 or `psql`
- Windows 10/11 (WinForms is a Windows-only UI framework; there is no cross-platform client)

## Setup & Run Instructions

**1. Clone and open**

```bash
git clone https://github.com/BouncyCrawfish/SEN_381_Project.git
cd SEN_381_Project
# open CivicConnect.slnx in Visual Studio 2026
```

**2. Create the database.** Run the scripts in `db/` in order. Scripts 01 and 02 run on the `postgres` database, 03 to 05 on `civicconnect_db`. Full instructions, checks and troubleshooting are in [`db/README.md`](db/README.md).

| Order | Script | Notes |
|---|---|---|
| 1 | `01_roles.sql` | Edit the three `CHANGE_ME_...` passwords **locally** (or in a gitignored `01_roles.local.sql` copy). Never commit real passwords. Use a different password per role |
| 2 | `02_create_db.sql` | Run on its own (`CREATE DATABASE` cannot run inside a transaction) |
| 3 | `03_schema.sql` | Tables, constraints, indexes, append-only history, grants |
| 4 | `04_requests_module.sql` | Seed categories and extra integrity rules |
| 5 | `05_dev_seed_users.sql` | **Development only.** Throw-away Requester, two Staff and Manager accounts with public passwords, so every role can sign in |

`00a_drop_database.sql` and `00b_drop_roles.sql` reset a development database. Do not run them anywhere else.

**3. Configure the connection string.** The app must connect as the least-privilege `civicconnect_app` role, never as `postgres`. Use one of:

```bash
# Option A: copy the example file and edit it (appsettings.json is gitignored)
copy appsettings.example.json CivicConnect\CivicConnect.App\appsettings.json

# Option B: user-secrets (kept outside the repo)
dotnet user-secrets set "ConnectionStrings:CivicConnect" "Host=localhost;Port=5432;Database=civicconnect_db;Username=civicconnect_app;Password=<app password>" --project CivicConnect/CivicConnect.App

# Option C: environment variable
# ConnectionStrings__CivicConnect=Host=localhost;...
```

If no connection string is found, the app shows a message and closes.

**4. Run**

```bash
dotnet run --project CivicConnect/CivicConnect.App
```

or set `CivicConnect.App` as the startup project in Visual Studio and press F5. Sign in with one of the development accounts:

| Email | Role | Password |
|---|---|---|
| `requester@civicconnect.test` | Requester | `Requester-Test-123` |
| `staff1@civicconnect.test` | Staff | `Staff-Test-1234` |
| `staff2@civicconnect.test` | Staff | `Staff-Test-1234` |
| `manager@civicconnect.test` | Manager | `Manager-Test-123` |

These passwords are public and exist only for a development database. Delete these accounts before any demo with real data.

## Technology / Runtime / Dependency Versions

| Layer | Choice | Version | ADR |
|---|---|---|---|
| Client UI | C# WinForms (Visual Studio 2026) | .NET 10 (`net10.0-windows`) | ADR-003 |
| Backend / domain | C# .NET 10 class libraries (modular monolith) | .NET 10 | ADR-001, ADR-003 |
| ORM | Entity Framework Core with `Npgsql.EntityFrameworkCore.PostgreSQL` | 10 / 10.0.3 | ADR-003 |
| Database | PostgreSQL | 16+ | ADR-003 |
| Password hashing | BCrypt.Net-Next (enhanced mode, work factor 12) | 4.2.0 | ADR-006 |
| Validation | FluentValidation | 12.2.0 | PED §5.7 |
| Config and DI | Microsoft.Extensions.Hosting | 10.0.12 | |
| Testing | xUnit v3 with Microsoft.NET.Test.Sdk | 4.0.2 / 18.10.1 | ADR-003 |
| Version control | GitHub | n/a | ADR-003 |

## Repository Structure

```
/
├── README.md
├── CivicConnect.slnx
├── .gitignore                        # keeps secrets, dumps and build output out of Git
├── global.json                       # makes dotnet test use Microsoft.Testing.Platform (.NET 10 + xUnit v3)
├── .github/
│   ├── dependabot.yml                # weekly dependency update pull requests
│   └── workflows/
│       └── ci.yml                    # CI pipeline: secrets guard, build, tests, static analysis, dependency check
├── appsettings.example.json          # placeholder config, safe to commit
├── Docs/
│   ├── PED_v4.0.md                   # Project Engineering Document (final) and evidence index
│   ├── STAGING.md                    # simulated staging: deploy, verify, roll back, deployment record
│   ├── Change_log.md                 # PED document control log
│   ├── Architecture.png              # architecture diagram
│   └── ERD.png                       # entity relationship diagram
├── db/
│   ├── README.md                     # explains the scripts
│   ├── 00a_drop_database.sql         # DEV ONLY reset
│   ├── 00b_drop_roles.sql            # DEV ONLY reset
│   ├── 01_roles.sql                  # placeholders only, no real passwords
│   ├── 02_create_db.sql
│   ├── 03_schema.sql
│   ├── 04_requests_module.sql
│   ├── 05_dev_seed_users.sql         # DEV ONLY test accounts
│   └── perf/                         # DEV/STAGING ONLY load-test data for TC-21 (seed and cleanup)
├── CivicConnect/
│   ├── CivicConnect.App/             # WinForms UI: forms, theme, form factory, Program.cs (composition root)
│   ├── CivicConnect.Common/          # shared kernel: Result<T>, UserRole, domain events, notification contract
│   ├── CivicConnect.Auth/            # users and auth: IAuthService, IUserService, session, permissions, BCrypt
│   ├── CivicConnect.Requests/        # requests: IRequestService, Request aggregate, State Pattern, EF persistence
│   ├── CivicConnect.AuditLog/        # audit log: IAuditLogService (read), domain-event dispatcher
│   └── CivicConnect.Notifications/   # notifications: in-app popup handler and delivery services
└── Tests/
    └── CivicConnect.Tests/           # xUnit v3: Auth, Requests, AuditLog, Notifications, Performance
```

## Database / Schema

Five tables are baselined in ADR-002 and the PED's Data & Persistence Baseline. The PED calls the notes entity "Comment"; in the database it is the `notes` table.

| Table | Purpose | Key relationships |
|---|---|---|
| `users` | Requesters, staff and managers; holds the role used for access control and the password hash | Referenced by `request.requester_id`, `request.assigned_staff_id` |
| `category` | Controlled list of request categories (seeded by `04_requests_module.sql`) | Referenced by `request.category_id` |
| `request` | Core service-request record: status, category, requester, assignee | FK to `users`, `category`; has many `request_history` and `notes` |
| `request_history` | Append-only log of every status change (creation, assignment, resolution, closure). A trigger blocks updates, deletes and truncates | FK to `request`, `users` |
| `notes` | Staff-recorded actions and notes against a request | FK to `request`, `users` |

Integrity follows a **hybrid model**: the database enforces structure (types, foreign keys, CHECK constraints, append-only history, least-privilege grants) and the C# layer enforces business rules (which status change is legal, who may do it). **Optimistic concurrency** uses PostgreSQL's `xmin` system column to prevent lost updates (ADR-002).

The application connects as `civicconnect_app`, which has no `DELETE` and no DDL rights and cannot change a user's role. A separate `civicconnect_owner` (schema and category management) and a read-only `civicconnect_report` role exist. The `request_history` row for a status change is written by the Requests module in the **same transaction** as the change itself. See [`db/README.md`](db/README.md) for the full model, grants and verification queries.

## API / Module Interfaces

CivicConnect is a single desktop application, not a networked API: there are no HTTP endpoints. Modules communicate through strongly typed C# interfaces:

| Interface | Module | Responsibility |
|---|---|---|
| `IAuthService` | Auth | Sign in, sign out, change password |
| `IUserService` | Auth | Register a requester, look up users, list staff |
| `ISessionContext`, `IAccessControl` | Auth | Who is signed in; permission checks (fail closed) |
| `IRequestService` | Requests | Categories, submit, my requests, staff queue, all requests, summary, detail, assign, change status, add note |
| `IAuditLogService` | AuditLog | Read-only access to `request_history`: history of one request, or a query by request, user, status and date range (capped at 500 rows) |
| `IDomainEventDispatcher` | Common (implemented in AuditLog) | Publishes domain events such as `RequestStatusChangedEvent` to registered handlers |
| `INotificationService` | Common (implemented in Notifications) | Shows a message to the user (message box, or silent) |
| `IFormFactory` | App | Creates forms through dependency injection, so forms only receive interfaces |

Data crosses these boundaries as DTOs, validated by FluentValidation before domain execution. Failures surface as `Result<T>` objects; the WinForms screens show a generic message instead of raw exceptions. Each module registers itself with one extension method (`AddAuthModule`, `AddRequestsModule`, `AddAuditLogModule`, `AddNotificationsModule`) in `Program.cs`.

## Environment / Configuration

- **Connection string**: read from `appsettings.json` (gitignored), user-secrets or the `ConnectionStrings__CivicConnect` environment variable. Never commit real values; `appsettings.example.json` holds placeholders only (NFR-003). For a non-local database add `SSL Mode=Require`.
- **Database role**: the connection string must use `civicconnect_app`.
- **Target environment**: Windows 10/11 desktop client, packaged via `dotnet publish -c Release -r win-x64`.
- **Access control**: because this is a desktop client connecting directly to PostgreSQL with no server tier, the C# layer enforces roles (`RolePermissions`), and the database limits what a compromised client could do. The desktop client holds the app role's credentials; this accepted risk is RSK-013 (see also `db/README.md` section 6).

## Module Responsibilities & Design Patterns

| Module | Responsibility | Design pattern / mechanism |
|---|---|---|
| Auth | Accounts, roles, authorisation (FR-006, NFR-003, NFR-008) | BCrypt password hashing (SHA-384 pre-hash, work factor 12); one permission map (`RolePermissions`, DEC-004); in-memory login lockout (5 failures locks an email for 5 minutes); same error for an unknown email and a wrong password. ADR-006 |
| Requests | Request creation, status transitions, assignment, notes, manager queries | **State Pattern** (`IRequestState` and one class per state): ADR-004. EF Core with optimistic concurrency |
| AuditLog | Read side of the audit trail (NFR-005), event dispatcher, `AuditLogEventHandler` | **Observer Pattern** dispatcher (`IDomainEventDispatcher`): ADR-005 |
| Notifications | Tell the person who changed a request what happened (FR-005) | **Observer Pattern** listener (`NotificationEventHandler`) over `INotificationService` |

**How a status change flows.** Staff act on a request in `RequestDetailForm`. `RequestService` checks the permission, validates the input, applies the State Pattern rule and saves the change together with its `request_history` row in one transaction. Only then does it publish a `RequestStatusChangedEvent`. The `NotificationEventHandler` turns that into an in-app popup for the staff member. A failing listener is swallowed after the save, so a notification problem can never undo a legitimate status update. A note on its own does not publish an event.

**DEF-01 (fixed).** An earlier version registered `AuditLogEventHandler` as a second listener, so each status change wrote a duplicate `request_history` row. `AddAuditLogModule` no longer registers it; the class and its tests are kept in case history writing ever moves out of the Requests module. `AuditLogModuleRegistrationTests` (TC-22) guards against the registration coming back, and manual test TC-13 confirms one history row per status change on the real database.

**Why these patterns:** the State Pattern replaces fragile procedural status checks with an explicit, testable class per state. The Observer Pattern decouples the core request-update transaction from side effects (notifications, per DEC-001). Both trade some added indirection for that decoupling; see ADR-004/ADR-005 and RSK-012 in the Risk Register.

## Testing / Running Checks

Run all tests from Visual Studio's Test Explorer, or:

```bash
dotnet test --project Tests/CivicConnect.Tests/CivicConnect.Tests.csproj
```

The test project targets `net10.0-windows` (the Notifications module uses WinForms), so the tests run on Windows only. The `global.json` in the repository root makes `dotnet test` use Microsoft.Testing.Platform, which the .NET 10 SDK requires for xUnit v3 projects. Without it, `dotnet test` stops with an error saying VSTest is not supported.

| Area | Test classes | What is covered | PED test case |
|---|---|---|---|
| Auth | `PasswordHasherTests`, `AuthServiceTests`, `UserServiceTests`, `AccessControlTests` | Password hashing, login and lockout, change password, registration, role permissions and access control | TC-16 |
| Requests | `RequestStateTest`, `RequestSubmissionTests`, `RequestSecurityTests`, `RequestNotificationTests` | State Pattern transitions, submission validation, row-level security, assignment rules, concurrency conflicts, status-change events | TC-03 to TC-06, TC-15, TC-17 |
| AuditLog | `AuditLogServiceTests`, `AuditLogEventHandlerTests`, `DomainEventDispatcherTests`, `HistoryImmutabilityTests`, `AuditLogModuleRegistrationTests` | History service and query, handler, dispatcher, history immutability (database), DEF-01 regression | TC-09, TC-18, TC-22 |
| Notifications | `NotificationEventHandlerTests`, `NotificationServicesTests`, `NotificationsModuleTests` | Notification wording, delivery services, DI registration, event-to-notification path | TC-07, TC-08 |
| Performance | `StaffQueuePerformanceTests` | Staff queue response time against PostgreSQL: 20 timed calls, p95 within 2 s (database only) | TC-21 |

Test method names that start with a test case ID use the PED's IDs: `TC03_` to `TC05_` for the state machine and `TC15_` for request submission (renamed in v4.0; they used to start with `TC06`, `TC07` and `TC08`).

Tests use in-memory fakes for repositories, so they need no database. That also means they cannot catch EF Core query translation or SQL problems (a sort applied after building a DTO failed only when run against the real database). `HistoryImmutabilityTests` and `StaffQueuePerformanceTests` run against PostgreSQL only when the `CIVICCONNECT_TEST_CONNECTION` environment variable is set (connection string for the `civicconnect_app` role); otherwise they are skipped. For TC-21, seed data with `db/perf/perf_seed_requests.sql` first; the timings are written to `perf-tc21-result.txt` next to the test DLL. `Docs/STAGING.md` shows the full procedure. Manual UI checks (TC-10 to TC-14, TC-19, TC-20) are listed in the PED: for example, as Staff change a request's status and confirm the popup appears and the Audit trail shows one new row; adding only a note should not show a popup.

### Continuous integration

`.github/workflows/ci.yml` runs on GitHub Actions for every push and pull request to `main` and the development branch, and can also be started by hand from the repository's **Actions** tab.

| Step | Command |
|---|---|
| Install the SDK | `actions/setup-dotnet` with .NET 10 |
| Restore | `dotnet restore CivicConnect.slnx` |
| Build | `dotnet build CivicConnect.slnx --configuration Release --no-restore` |
| Secrets guard | Fails if a real `appsettings*.json`, `.env`, `.pfx/.p12`, `*.local.sql` or SSH key is tracked, or `db/01_roles.sql` holds a real password |
| Test | `dotnet test --project Tests/CivicConnect.Tests/CivicConnect.Tests.csproj --configuration Release --no-build` |
| Static analysis | Release build of the app and all modules with `-p:AnalysisLevel=latest-recommended`; security rules CA2100, CA3075, CA5350 and CA5351 fail the run, other analyzer warnings are reported |
| Vulnerable package check | `dotnet list CivicConnect.slnx package --vulnerable --include-transitive`; fails if any package has a known vulnerability |

- It runs on `windows-latest` because the app and test project target `net10.0-windows`.
- It needs no secrets and gets a read-only token. The database tests (`HistoryImmutabilityTests`, TC-21) are skipped in CI because `CIVICCONNECT_TEST_CONNECTION` is not set.
- Dependabot (`.github/dependabot.yml`) opens weekly pull requests for NuGet and GitHub Actions updates, which go through the same checks.
- If the vulnerable package check fails, update the named package in a pull request; if no fix exists yet, record it as an accepted risk in the PED.
- A newer push to the same branch cancels a run that is still going.
- To make a failing build block merging, turn on **Require status checks to pass** for `main` and the development branch in **Settings → Branches** and select **Build and test (Windows)**.
- Before pushing, run the restore, build and test commands locally from the repository root; if they pass locally they should pass in CI. A failed run shows the error under the pull request's **Details** link.

### Staging

The release candidate is checked in a simulated staging environment: the tagged commit is published as a Release build into its own folder, the database is rebuilt from the `db/` scripts, and the connection string comes only from an environment variable. Deployment, verification (TC-10 to TC-14, TC-19 to TC-21), rollback and the deployment record are in [`Docs/STAGING.md`](Docs/STAGING.md).

## Known Limitations / TODOs

- **Email notifications are deferred** because the project must use free services only (DEC-001). The only notification is an in-app popup on the computer of the person who made the change. A requester sees a new status only by opening My requests. Notifying requesters needs a persistent notification store, which is a database change.
- **Overdue requests (FR-015)** are not implemented; the SLA threshold was never agreed.
- **Manager filters and export.** The manager list filters by status and search but not by category (FR-016 partly met), and the audit trail cannot be exported (FR-017 asks for export).
- **No change-password screen.** The service method and its tests exist, but no form calls it. There is also no session timeout and no password reset.
- **Login lockout** is held in memory, so a restart or a tampered client resets it (RSK-014).
- **Desktop client credentials**: the client holds the app role's database credentials (accepted risk RSK-013).
- **Listener failures are silent.** Event handler errors are swallowed after the change is saved, and the project has no logging.
- **Audit trail permission** is checked on the manager tile and the Audit trail screen (`ViewReports`), not inside `IAuditLogService` itself.
- **No database tests in CI.** The pipeline runs the in-memory tests only; the two PostgreSQL tests run on the staging machine, and tests run on Windows only (RSK-015).
- **No paging.** The staff queue and the manager list load every matching request, so they slow down as open requests grow (TD-07). The audit-trail query is capped at 500 rows.
- NFR-002's threshold (2 s) is still the DEC-002 placeholder. TC-21 measures the staff queue against it on one local machine; it says nothing about concurrent users or a hosted database.
- PostgreSQL has only been run locally; no hosted database was set up (FEC-06).
- **Not released to production.** The final release stays in simulated staging until clients stop holding database credentials (RSK-013), backups are scheduled (RSK-009), authentication gaps are closed (RSK-014) and logging exists. See the go-live decision in the PED.
- Notes (FR-012) do not publish a domain event; only status changes do.

## Engineering Evidence Links

- Project Engineering Document: [`Docs/PED_v4.0.md`](Docs/PED_v4.0.md)
- PED document control log: [`Docs/Change_log.md`](Docs/Change_log.md)
- Requirements Traceability Matrix, end-to-end traces (FR-001, FR-006), test cases (TC-03 to TC-22), Architecture Decision Records (ADR-001 to ADR-006), Engineering Decision Log (DEC-001 to DEC-004) and Risk Register (RSK-001 to RSK-016): sections inside the PED
- Architecture and ERD diagrams: [`Docs/Architecture.png`](Docs/Architecture.png), [`Docs/ERD.png`](Docs/ERD.png)
- CI pipeline: [`.github/workflows/ci.yml`](.github/workflows/ci.yml) and its runs in the repository's **Actions** tab
- Final project closure (final release, M1 scope to final delivery, Scope Completion Percentage, M3 issue closure, go-live decision, reflection): chapter "Final project closure (M4)" in the PED
- M3 release evidence (submission information, baseline conformance, Defect Register, Technical Debt Register, Release Evidence Summary, residual risks): chapter "M3 release evidence" in the PED
- Simulated staging and deployment record: [`Docs/STAGING.md`](Docs/STAGING.md)
- Database design, setup and verification: [`db/README.md`](db/README.md)
