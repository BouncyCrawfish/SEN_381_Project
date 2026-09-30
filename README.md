# CivicConnect

A community service-request management platform. CivicConnect replaces the current fragmented mix of email, phone calls, WhatsApp messages, spreadsheets and paper records with a single controlled system for submitting, tracking, actioning and reporting on service requests (facility faults, maintenance issues, lost property, IT support and similar).

> **Project status: Milestone 2 (Architecture, Technology & Initial Design Baseline).** Architecture and data model are decided and baselined. Technology stack and design-pattern decisions are in progress. No application code has been written yet — this is expected at this stage and is not a gap (see [Current Implementation Status](#current-implementation-status)).

---

## Table of Contents

- [Purpose](#purpose)
- [Current Implementation Status](#current-implementation-status)
- [Prerequisites](#prerequisites)
- [Setup & Run Instructions](#setup--run-instructions)
- [Technology / Runtime / Dependency Versions](#technology--runtime--dependency-versions)
- [Repository Structure](#repository-structure)
- [Database / Schema](#database--schema)
- [API Endpoints](#api-endpoints)
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
| Data model | **Decided** — five entities (User, Category, Request, RequestHistory, Comment) with defined FK structure, hybrid DB + application-layer integrity enforcement, optimistic concurrency on status updates | ADR-002 |
| Architecturally Significant Requirements (ASRs) | **Formalised** — NFR-002 (performance), NFR-003 (security), NFR-005 (auditability), NFR-007 (scalability), NFR-008 (compliance/data handling) | PED §ASRs |
| Technology stack (framework, database, hosting) | **In progress** — not yet baselined as an ADR | Owned by [teammate] |
| Design patterns | **In progress** — at least two design-pattern decisions required, not yet baselined | Owned by [teammate] |
| Application code | **Not started** — construction begins once the technology stack is baselined | — |
| Automated tests / CI | **Not started** — not required at this milestone | — |

## Prerequisites

> To be completed once the Technology ADR is finalised. Expected to include, at minimum:

- [ ] Runtime / SDK (e.g. .NET SDK version — the architecture ADR references C#/ASP.NET Core as the team's strongest shared language, but this is not yet a formal technology decision)
- [ ] Database engine
- [ ] Package manager / build tool
- [ ] IDE recommendations (optional)

## Setup & Run Instructions

> To be completed once there is a bootstrapped project to run. Expected shape:

```bash
# 1. Clone the repository
git clone <repo-url>
cd civicconnect

# 2. Install dependencies
# [command TBD once stack is chosen]

# 3. Configure environment
# copy .env.example to .env and fill in local values (see Environment / Configuration below)

# 4. Apply database migrations
# [command TBD once stack is chosen]

# 5. Run the application
# [command TBD once stack is chosen]
```

## Technology / Runtime / Dependency Versions

> Not yet finalised. Once the Technology ADR is approved, list pinned versions here, e.g.:

| Layer | Choice | Version | ADR |
|---|---|---|---|
| Backend/runtime | TBD | TBD | ADR-00X |
| Frontend | TBD | TBD | ADR-00X |
| Database | TBD | TBD | ADR-00X |
| Hosting/deployment | TBD | TBD | ADR-00X |

## Repository Structure

Once scaffolded, folders will map to the four architecture modules from ADR-001:

```
/
├── README.md
├── docs/
│   ├── PED/                  # Project Engineering Document (PED v2.2)
│   ├── decisions/            # ADRs (ADR-001, ADR-002, ...)
│   ├── requirements/         # RTM
│   ├── risk/                 # Risk Register
│   └── architecture/         # Architecture & ERD diagrams
├── src/
│   ├── Requests/             # Requests module — submission, status, assignment (ADR-001)
│   ├── AuditLog/             # Audit log module — status-change history (NFR-005)
│   ├── Auth/                 # Users and auth module — accounts, roles, RBAC
│   └── Reporting/            # Reporting module — management/oversight views
├── tests/
├── .github/
│   ├── workflows/
│   └── pull_request_template.md
└── .gitignore
```

*(Illustrative — update once the project is actually scaffolded.)*

## Database / Schema

Five entities are baselined in ADR-002 and the PED's Data & Persistence Baseline section:

| Entity | Purpose | Key Relationships |
|---|---|---|
| `User` | Requesters, staff and managers; holds the role used for access control | Referenced by `Request.requester_id`, `Request.assigned_staff_id` |
| `Category` | Controlled, bounded list of request categories | Referenced by `Request.category_id` |
| `Request` | Core service-request record: status, category, requester, assignee | FK to `User`, `Category`; has many `RequestHistory`, `Comment` |
| `RequestHistory` | Append-only log of every status change (creation, assignment, resolution, closure) | FK to `Request` |
| `Comment` | Staff-recorded actions/notes against a request | FK to `Request`, `User` |

Integrity is enforced through a **hybrid model** — database constraints plus application-layer checks — with **optimistic concurrency** on status transitions to prevent lost updates under concurrent edits (ADR-002). Migration scripts and the full ERD diagram live in `docs/architecture/` once added.

## API Endpoints

> No endpoints exist yet — none of FR-001–FR-017 has been implemented. This section will list method, path, purpose, required role and status once implementation begins, e.g.:

| Method | Path | Purpose | Auth Required | Status |
|---|---|---|---|---|
| `POST` | `/api/requests` | Submit a new service request (FR-001) | Requester | Planned |
| `GET` | `/api/requests/{id}` | View request status/details (FR-003, FR-009) | Requester (own) / Staff | Planned |

## Environment / Configuration

> No environment variables exist yet — this section lists variable **names only**, never real values or secrets, once configuration exists. Expected categories:

- Database connection string
- Authentication/session secret
- Environment indicator (`Development` / `Staging` / `Production`)

A `.env.example` file (with placeholder values only) should be added to the repo root once configuration is introduced, and the real `.env` must never be committed (see Risk RSK-005 / GitHub governance in the PED).

## Module Responsibilities & Design Patterns

| Module | Responsibility | Design Pattern Applied |
|---|---|---|
| Requests | Request creation, status transitions, assignment, comments | *Planned — pending design-pattern ADR* |
| Audit log | Append-only history of status changes (NFR-005) | *Planned — pending design-pattern ADR* |
| Users and auth | Accounts, roles, authentication, authorisation (NFR-003, NFR-008) | *Planned — pending technology ADR* |
| Reporting | Aggregate/oversight views for management (NFR-007) | *Planned — pending design-pattern ADR* |

At least two genuine design-pattern decisions are required for M2 (informed by Assignment 2 research) — update this table once those ADRs are approved.

## Testing / Running Checks

> No automated tests or CI pipeline exist yet. This is expected at this milestone (a mature CI/CD pipeline is not an M2 requirement) and will be introduced progressively from Milestone 3 onward. This section will document how to run the test suite once it exists.

## Known Limitations / TODOs

- Technology stack (backend, frontend, database, hosting) not yet finalised — pending Technology ADR.
- Design-pattern decisions not yet finalised — pending design-pattern ADR(s).
- No implementation, automated tests, or CI pipeline yet.
- Authentication/authorisation mechanism not yet selected.
- Deployment platform not yet finalised (Render vs Railway free-tier comparison exists as ADR-001 supporting evidence, but no formal deployment decision has been baselined).

## Engineering Evidence Links

- Project Engineering Document (PED v2.2): `docs/PED/`
- Requirements Traceability Matrix (RTM): `docs/requirements/`
- Architecture Decision Records (ADR-001, ADR-002, ...): `docs/decisions/`
- Risk Register: `docs/risk/`

*(Update these paths once the docs are actually committed at those locations.)*
