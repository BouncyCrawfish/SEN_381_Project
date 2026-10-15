# CivicConnect

A community service-request management platform. CivicConnect replaces the current fragmented mix of email, phone calls, WhatsApp messages, spreadsheets and paper records with a single controlled system for submitting, tracking, actioning and reporting on service requests (facility faults, maintenance issues, lost property, IT support and similar).

> **Project status: Milestone 3 (Controlled Construction, Integration, Quality & Release Readiness).** The Windows Forms application is built and runs against PostgreSQL for all three roles (Requester, Staff, Manager). Email notification and a few smaller items are deferred (see [Known Limitations / TODOs](#known-limitations--todos)).

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

The full business context, stakeholder analysis, requirements (FR-001–FR-017, NFR-001–008) and constraints are baselined in the [Project Engineering Document (PED)](#engineering-evidence-links).

## Current Implementation Status

| Area | Status | Notes |
|---|---|---|
| Architecture | **Built** — modular monolith as .NET class libraries (Common, Auth, Requests, AuditLog, Notifications) plus the WinForms app | ADR-001. There is no separate Reporting project: management queries live in the Requests module and the audit trail in the AuditLog module |
| Database | **Built** — PostgreSQL schema, roles, grants and dev seed data from the scripts in `db/` | ADR-002. Schema is owned by the SQL scripts; there are no EF migrations |
| Users and auth | **Built** — registration (always a Requester), login with BCrypt hashes, role permission map, login lockout | See [Module Responsibilities](#module-responsibilities--design-patterns) |
| Requests | **Built** — submit, my requests, staff queue with search and filter, request detail with history and notes, assign, status transitions (State Pattern), notes, optimistic concurrency, manager overview | ADR-004 |
| Audit log | **Built** — read-only history service and query, manager **Audit trail** screen, domain-event dispatcher | History rows are written by the Requests module. See the [known issue](#known-limitations--todos) about `AuditLogEventHandler` |
| Notifications | **Partly built** — in-app popup for the person who changes a request's status. **Email is deferred** | ADR-005, DEC-001. Test cases are listed in the PED (TC-03 to TC-14 in the PED test case table) |
| Automated tests | **Built** — xUnit v3 tests for Auth, Requests, AuditLog and Notifications | Run on Windows only; no PostgreSQL integration tests yet |
| CI pipeline | **Not started** | |

### Screens by role

| Role | Screens |
|---|---|
| Requester | Submit request, My requests, request detail (own requests only) |
| Staff | Staff queue (search and filter), request detail with response panel (assign, change status, add note) |
| Manager | Overview and all requests (counts by status and category, filterable list), **Audit trail** (every status change, filter by request, status and date range). Read-only: managers cannot change requests |

Each screen is also protected in the service layer: hiding a button is a convenience, never the only check.

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
| 1 | `01_roles.sql` | Edit the three `CHANGE_ME_...` passwords **locally**. Never commit real passwords. Use a different password per role |
| 2 | `02_create_db.sql` | Run on its own (`CREATE DATABASE` cannot run inside a transaction) |
| 3 | `03_schema.sql` | Tables, constraints, indexes, append-only history, grants |
| 4 | `04_requests_module.sql` | Seed categories and extra integrity rules |
| 5 | `05_dev_seed_users.sql` | **Development only.** Throw-away Requester, Staff (two) and Manager accounts so every role can sign in. Read the header of the script and `db/README.md` section on seed users |

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

**4. Run**

```bash
dotnet run --project CivicConnect/CivicConnect.App
```

or set `CivicConnect.App` as the startup project in Visual Studio and press F5. Sign in with one of the dev seed accounts (the e-mail addresses are in the header of `db/05_dev_seed_users.sql`; see `db/README.md` for how the password is set).

## Technology / Runtime / Dependency Versions

| Layer | Choice | Version | ADR |
|---|---|---|---|
| Client UI | C# WinForms (Visual Studio 2026) | .NET 10 (`net10.0-windows`) | ADR-003 |
| Backend / domain | C# .NET 10 class libraries (modular monolith) | .NET 10 | ADR-001, ADR-003 |
| ORM | Entity Framework Core with `Npgsql.EntityFrameworkCore.PostgreSQL` | 10 / 10.0.3 | ADR-003 |
| Database | PostgreSQL | 16+ | ADR-003 |
| Password hashing | BCrypt.Net-Next | 4.2.0 | NFR-003 |
| Validation | FluentValidation | 12.2.0 | PED §5.7 |
| Config and DI | Microsoft.Extensions.Hosting | 10.0.12 | |
| Testing | xUnit | v3 (4.0.2) | ADR-003 |
| Version control | GitHub | n/a | ADR-003 |

## Repository Structure

```
/
├── README.md
├── CivicConnect.slnx
├── appsettings.example.json          # placeholder config, safe to commit
├── Docs/
│   ├── PED_v3.2.md                   # Project Engineering Document
│   ├── Change_log.md                 # PED document control log
│   └── (architecture and ERD diagrams)
├── db/
│   ├── README.md                     # explains the scripts
│   ├── 00a_drop_database.sql         # DEV ONLY reset
│   ├── 00b_drop_roles.sql            # DEV ONLY reset
│   ├── 01_roles.sql                  # placeholders only, no real passwords
│   ├── 02_create_db.sql
│   ├── 03_schema.sql
│   ├── 04_requests_module.sql
│   └── 05_dev_seed_users.sql         # DEV ONLY test accounts
├── CivicConnect/
│   ├── CivicConnect.App/             # WinForms UI: forms, theme, form factory, Program.cs (composition root)
│   ├── CivicConnect.Common/          # shared kernel: Result<T>, UserRole, domain events, notification contract
│   ├── CivicConnect.Auth/            # users and auth: IAuthService, IUserService, session, permissions, BCrypt
│   ├── CivicConnect.Requests/        # requests: IRequestService, Request aggregate, State Pattern, EF persistence
│   ├── CivicConnect.AuditLog/        # audit log: IAuditLogService (read), domain-event dispatcher
│   └── CivicConnect.Notifications/   # notifications: in-app popup handler and delivery services
└── Tests/
    └── CivicConnect.Tests/           # xUnit v3: Auth, Requests, AuditLog, Notifications
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

The application connects as `civicconnect_app`, which has no `DELETE` and no DDL rights. A separate `civicconnect_owner` (migrations and category management) and a read-only `civicconnect_report` role exist. The `request_history` row for a status change is written by the Requests module in the **same transaction** as the change itself. See [`db/README.md`](db/README.md) for the full model, grants and verification queries.

## API / Module Interfaces

CivicConnect is a single desktop application, not a networked API: there are no HTTP endpoints. Modules communicate through strongly typed C# interfaces:

| Interface | Module | Responsibility |
|---|---|---|
| `IAuthService` | Auth | Sign in, sign out, change password |
| `IUserService` | Auth | Register a requester, look up users, list staff |
| `ISessionContext`, `IAccessControl` | Auth | Who is signed in; permission checks (fail closed) |
| `IRequestService` | Requests | Submit, list, search, assign, change status, add notes, manager summary |
| `IAuditLogService` | AuditLog | Read-only access to `request_history`: history of one request, or a query by request, user, status and date range (capped at 500 rows) |
| `IDomainEventDispatcher` | Common (implemented in AuditLog) | Publishes domain events such as `RequestStatusChangedEvent` to registered handlers |
| `INotificationService` | Common (implemented in Notifications) | Shows a message to the user (message box, or silent) |

Data crosses these boundaries as DTOs, validated by FluentValidation before domain execution. Failures surface as `Result<T>` objects; the WinForms screens show a generic message instead of raw exceptions.

## Environment / Configuration

- **Connection string**: read from `appsettings.json` (gitignored), user-secrets or the `ConnectionStrings__CivicConnect` environment variable. Never commit real values; `appsettings.example.json` holds placeholders only (NFR-003). For a non-local database add `SSL Mode=Require`.
- **Database role**: the connection string must use `civicconnect_app`.
- **Target environment**: Windows 10/11 desktop client, packaged via `dotnet publish -c Release -r win-x64`.
- **Access control**: because this is a desktop client connecting directly to PostgreSQL with no server tier, the C# layer enforces roles (`RolePermissions`), and the database limits what a compromised client could do. The desktop client holds the app role's credentials; this accepted risk is described in `db/README.md` section 6.

## Module Responsibilities & Design Patterns

| Module | Responsibility | Design pattern / mechanism |
|---|---|---|
| Auth | Accounts, roles, authorisation (NFR-003, NFR-008) | BCrypt password hashing; one permission map (`RolePermissions`); in-memory login lockout (5 failures locks an email for 5 minutes) |
| Requests | Request creation, status transitions, assignment, notes, manager summary | **State Pattern** (`IRequestState` and one class per state): ADR-004. EF Core with optimistic concurrency |
| AuditLog | Read side of the audit trail (NFR-005), event dispatcher, `AuditLogEventHandler` (see the deviation above) | **Observer Pattern** dispatcher (`IDomainEventDispatcher`): ADR-005 |
| Notifications | Tell the person who changed a request what happened (FR-005) | **Observer Pattern** listener (`NotificationEventHandler`) over `INotificationService` |

**How a status change flows.** Staff act on a request in `RequestDetailForm`. `RequestService` checks the permission, validates the input, applies the State Pattern rule and saves the change together with its `request_history` row in one transaction. Only then does it publish a `RequestStatusChangedEvent`. The `NotificationEventHandler` turns that into an in-app popup for the staff member. A failing listener is swallowed after the save, so a notification problem can never undo a legitimate status update. A note on its own does not publish an event.

**Deviation from the PED (needs attention).** The PED describes `AuditLogEventHandler` writing the history rows, but in the build the Requests module writes them itself in the same transaction (ADR-002). PED v3.2 states that `AuditLogEventHandler` is deliberately **not registered**, so that a status change produces exactly one history row. In the code as it currently stands, `AddAuditLogModule` **does** register it (`AuditLogModuleExstentions.cs`), so each status change should produce a second, duplicate row in `request_history` and in the Audit trail. Remove that one registration line (or move the responsibility into the handler and stop the Requests module writing history) before relying on the audit trail.

**Why these patterns:** the State Pattern replaces fragile procedural status checks with an explicit, testable class per state. The Observer Pattern decouples the core request-update transaction from side effects (audit and notifications, per DEC-001). Both trade some added indirection for that decoupling; see ADR-004/ADR-005 and RSK-012 in the Risk Register.

## Testing / Running Checks

Run all tests from Visual Studio's Test Explorer, or:

```bash
dotnet test Tests/CivicConnect.Tests
```

The test project targets `net10.0-windows` (the Notifications module uses WinForms), so the tests run on Windows only. Any future CI runner must be Windows.

| Area | What is covered |
|---|---|
| Auth | Password hashing, login and lockout, registration, role permissions and access control |
| Requests | Submission, State Pattern transitions, row-level security rules, concurrency conflict handling, status-change events |
| AuditLog | History service and query, dispatcher, handler, history immutability |
| Notifications | Notification handler wording, delivery services, DI registration, event-to-notification path |

Tests use in-memory fakes for repositories, so they need no database. That also means they cannot catch EF Core query translation or SQL problems (a sort applied after building a DTO failed only when run against the real database). A PostgreSQL integration test suite is still to do. Showing the real popup needs a person to click it, so it is a manual check: as Staff, change a request's status and confirm the popup appears; adding only a note should not show one.

## Known Limitations / TODOs

- **Email notifications are deferred** because the project must use free services only (DEC-001). Today the only notification is an in-app popup on the computer of the person who made the change. A requester sees a new status only by opening My requests. Notifying requesters needs a persistent notification store, which is a database change.
- **Overdue requests (FR-015)** are not implemented; the SLA threshold has not been decided.
- **Duplicate history rows (known issue).** `AuditLogEventHandler` is registered in `AddAuditLogModule` while the Requests module also writes `request_history`, which contradicts PED v3.2. Confirm by changing one request's status as Staff and opening the Audit trail: you should see one row, not two. Fix by removing the registration.
- **Listener failures are silent.** The event handler errors are swallowed after the change is saved, and the project has no logging yet.
- **Audit trail permission** is checked on the manager tile and the Audit trail screen (`ViewReports`), not inside `IAuditLogService` itself.
- **Login lockout** is held in memory, so a restart or a tampered client resets it.
- **Desktop client credentials**: the client holds the app role's database credentials (accepted risk, `db/README.md` section 6).
- **No CI pipeline** and no PostgreSQL integration tests yet; tests run on Windows only.
- NFR-002's performance threshold (2 s for 200 requests) was a placeholder written for a hosted web API. It needs reviewing against the real desktop-to-database latency (DEC-002).
- Whether PostgreSQL is self-hosted or on a free-tier provider is not stated explicitly anywhere (FEC-06).
- Notes (FR-012) do not publish a domain event; only status changes do.

## Engineering Evidence Links

- Project Engineering Document: [`Docs/PED_v3.2.md`](Docs/PED_v3.2.md)
- PED document control log: [`Docs/Change_log.md`](Docs/Change_log.md)
- Requirements Traceability Matrix, Architecture Decision Records (ADR-001 to ADR-005), Engineering Decision Log and Risk Register: sections inside the PED
- Database design, setup and verification: [`db/README.md`](db/README.md)
