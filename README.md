# CivicConnect

A community service-request management platform. CivicConnect replaces the current fragmented mix of email, phone calls, WhatsApp messages, spreadsheets and paper records with a single controlled system for submitting, tracking, actioning and reporting on service requests (facility faults, maintenance issues, lost property, IT support and similar).

> **Project status: Milestone 2 (Architecture, Technology & Initial Design Baseline).** Architecture, data model, technology stack and initial design-pattern decisions are all decided and baselined. Application code has not been written yet — this is expected at this stage and is not a gap (see [Current Implementation Status](#current-implementation-status)).

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
- **Staff** view, search, assign, action and resolve requests assigned to them, through controlled status transitions.
- **Management** get visibility into open, overdue and resolved work for accountability and reporting.

The full business context, stakeholder analysis, requirements (FR-001–FR-017, NFR-001–008) and constraints are baselined in the [Project Engineering Document (PED)](#engineering-evidence-links).

## Current Implementation Status

| Area | Status | Evidence |
|---|---|---|
| Architecture | **Decided** — modular monolith across four modules (Requests, Audit log, Users and auth, Reporting) | ADR-001 |
| Data model | **Decided** — five entities (User, Category, Request, RequestHistory, Comment), hybrid DB + application-layer integrity, optimistic concurrency on status updates | ADR-002 |
| Technology stack | **Decided** — C# .NET 10 WinForms desktop app, EF Core 10, PostgreSQL 16+ | ADR-003 |
| Design patterns | **Decided** — State Pattern for status transitions; Observer Pattern for audit/notification dispatch | ADR-004, ADR-005 |
| Module interfaces & integration | **Decided** — `IRequestService`, `IUserService`, `IAuditLogService`, DTOs, FluentValidation, `Result<T>` error handling | §5.7 |
| Application code | **Not started** — construction begins next; this is the current priority | — |
| Automated tests / CI | **Not started** — xUnit v3 chosen as the test framework; not required at this milestone | ADR-003 |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/) (target framework: `net10.0-windows`)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) (Windows — required for WinForms designer support)
- [PostgreSQL 16+](https://www.postgresql.org/) — local instance or connection details for a hosted instance
- Windows 10/11 (WinForms is a Windows-only UI framework; there is no cross-platform client)

## Setup & Run Instructions

```bash
# 1. Clone the repository
git clone <repo-url>
cd civicconnect

# 2. Restore dependencies
dotnet restore

# 3. Configure the database connection
# copy appsettings.example.json to appsettings.json and set your PostgreSQL
# connection string locally — appsettings.json is gitignored and must never be committed

# 4. Apply EF Core migrations
dotnet ef database update

# 5. Run the application
dotnet run --project src/CivicConnect.App
```

> Exact project/folder names above are illustrative until the solution is scaffolded — update this block once the actual `.sln`/`.csproj` layout exists.

## Technology / Runtime / Dependency Versions

| Layer | Choice | Version | ADR |
|---|---|---|---|
| Client UI | C# WinForms (Visual Studio 2026) | .NET 10 (`net10.0-windows`) | ADR-003 |
| Backend / domain | C# .NET 10 class libraries (modular monolith) | .NET 10 | ADR-001, ADR-003 |
| ORM | Entity Framework Core | 10 | ADR-003 |
| Database | PostgreSQL | 16+ | ADR-003 |
| Testing | xUnit | v3 | ADR-003 |
| Validation | FluentValidation | — | §5.7 |
| Version control | GitHub | — | ADR-003 |

## Repository Structure

Folders map to the four architecture modules from ADR-001, using the interfaces decided in §5.7:

```
/
├── README.md
├── docs/
│   ├── PED/                  # Project Engineering Document (PED v2.4)
├── db/
│   ├── README.md                       # explains the scripts
│   ├── 00a_drop_database.sql           # DEV ONLY reset
│   ├── 00b_drop_roles.sql              # DEV ONLY reset
│   ├── 01_roles.sql                    # placeholders only, no real passwords
│   ├── 02_create_database.sql
│   └── 03_schema.sql
├── Civic Connect/
│   ├── CivicConnect.App/         # WinForms UI (forms, presenters)
│   ├── CivicConnect.Requests/    # Requests module — IRequestService, RequestAggregate,
│   │                             #   IRequestState + state classes (ADR-004)
│   ├── CivicConnect.AuditLog/    # Audit log module — IAuditLogService, AuditLogEventHandler,
│   │                             #   domain event dispatching (ADR-005)
│   ├── CivicConnect.Auth/        # Users and auth module — IUserService, roles
│   ├── CivicConnect.Common/
│   └── CivicConnect.Reporting/   # Reporting module — management/oversight queries
├── tests/                    # xUnit v3 test projects
├── appsettings.example.json  # placeholder config, safe to commit
├── .git/
│   ├── workflows/
│   └── pull_request_template.md
└── .gitignore                 
```

*(Illustrative until the solution is actually scaffolded — update once real project names exist.)*

## Database / Schema

Five entities are baselined in ADR-002 and the PED's Data & Persistence Baseline section:

| Entity | Purpose | Key Relationships |
|---|---|---|
| `User` | Requesters, staff and managers; holds the role used for access control | Referenced by `Request.requester_id`, `Request.assigned_staff_id` |
| `Category` | Controlled, bounded list of request categories | Referenced by `Request.category_id` |
| `Request` | Core service-request record: status, category, requester, assignee | FK to `User`, `Category`; has many `RequestHistory`, `Comment` |
| `RequestHistory` | Append-only log of every status change (creation, assignment, resolution, closure) | FK to `Request`; populated by `AuditLogEventHandler` (ADR-005) |
| `Notes` | Staff-recorded actions/notes against a request | FK to `Request`, `User` |

Integrity is enforced through a **hybrid model** (database constraints + application-layer checks), with **optimistic concurrency** using PostgreSQL's native `xmin` system column to prevent lost updates under concurrent edits (ADR-002, ADR-003). Migrations run via `dbContext.Database.Migrate()` on startup or via PostgreSQL setup scripts. The full ERD lives in `docs/architecture/`.

## API / Module Interfaces

CivicConnect is a single desktop application, not a networked API — there are no HTTP endpoints. Modules communicate through strongly typed C# interfaces (§5.7):

| Interface | Module | Responsibility |
|---|---|---|
| `IRequestService` | Requests | Create, query, assign and transition requests |
| `IUserService` | Users and auth | Account and role lookups |
| `IAuditLogService` | Audit log | Read access to `RequestHistory` / `Comment` |
| `IDomainEventDispatcher` | Cross-cutting | Publishes domain events (e.g. `RequestStatusChangedEvent`) to subscribed handlers |

Data crosses these boundaries as strongly typed DTOs, validated by a FluentValidation pipeline before domain execution. Failures surface as `Result<T>` objects, with PostgreSQL concurrency conflicts (`DbUpdateConcurrencyException`) mapped to user-facing WinForms dialogs rather than raw exceptions.

## Environment / Configuration

- **Connection string** — stored in `appsettings.json`, which is gitignored. Never commit real values; `appsettings.example.json` holds placeholder keys only (NFR-003).
- **Target environment** — Windows 10/11 desktop client, packaged via `dotnet publish -c Release -r win-x64`.
- No other environment-specific secrets are required at this stage.

> ⚠️ Authentication mechanism (e.g. password hashing / session handling within WinForms) is **not yet decided** — tracked as FEC-01. Because this is a desktop client connecting directly to PostgreSQL with no separate server/API tier, access control has to be enforced in the C# application layer itself rather than at a network boundary.

## Module Responsibilities & Design Patterns

| Module | Responsibility | Design Pattern Applied |
|---|---|---|
| Requests | Request creation, status transitions, assignment, comments | **State Pattern** (`IRequestState` + `SubmittedState`, `AssignedState`, `InProgressState`, `ResolvedState`, `ClosedState`) — ADR-004 |
| Audit log | Append-only history of status changes (NFR-005), notification triggering (FR-005) | **Observer Pattern** (`RequestStatusChangedEvent`, `IDomainEventDispatcher`, `AuditLogEventHandler`, `NotificationEventHandler`) — ADR-005 |
| Users and auth | Accounts, roles, authorisation (NFR-003, NFR-008) | Authentication mechanism itself: *pending — see FEC-01* |
| Reporting | Aggregate/oversight views for management (NFR-007) | EF Core LINQ queries via `IRequestService` — no dedicated pattern beyond module interfaces |

**Why these patterns:** the State Pattern replaces fragile procedural status checks with an explicit, testable class per state, enforcing valid transitions at compile time and automatically writing audit records on every transition. The Observer Pattern decouples the core request-update transaction from side effects (audit logging, email/in-app notifications per DEC-001), so a notification failure can never roll back a legitimate status update. Both trade some added indirection and extra small classes for that decoupling — see ADR-004/ADR-005 for the full trade-off discussion, and RSK-012 in the Risk Register for the associated over-engineering risk being tracked.

## Testing / Running Checks

No automated tests exist yet. **xUnit v3** is the chosen test framework (ADR-003); this section will document how to run the suite once tests are written. Expected first candidates, given the design decisions above: unit tests per `IRequestState` implementation, and tests verifying `AuditLogEventHandler`/`NotificationEventHandler` fire correctly on `RequestStatusChangedEvent`.

## Known Limitations / TODOs

- No implementation, automated tests, or CI pipeline yet — expected from here onward, not required until Milestone 3.
- Authentication/authorisation mechanism not yet selected — tracked as FEC-01.
- Deployment target confirmed as a Windows desktop client + PostgreSQL, but whether PostgreSQL itself is self-hosted or hosted on a free-tier provider (Render/Railway) is not yet stated explicitly anywhere — see FEC-06.
- NFR-002's performance threshold (≤2s for 200 requests) was a placeholder pending a hosting/technology decision (DEC-002); now that ADR-003 is decided, this figure needs to be reviewed against the actual desktop-to-database latency profile rather than the original hosted-web-API assumption it was written against.
- It's not yet confirmed whether `Comment` creation (FR-012) dispatches a domain event through the same Observer Pattern as status changes — needs a decision from whoever owns the audit/notification design.

## Engineering Evidence Links

- Project Engineering Document (PED v2.4): `docs/PED/`
- Requirements Traceability Matrix (RTM): `docs/requirements/`
- Architecture Decision Records (ADR-001 – ADR-005): `docs/decisions/`
- Risk Register: `docs/risk/`

*(Update these paths once the docs are actually committed at those locations.)*
