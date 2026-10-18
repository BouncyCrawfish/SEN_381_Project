# CivicConnect Database (`civicconnect_db`)

PostgreSQL 16+ database scripts for the SEN381 CivicConnect project.
Baseline: PED v4.0 (ADR-002, ADR-003, ADR-004, ADR-005, ADR-006; NFR-003, NFR-005, NFR-008).

| File | Purpose |
|---|---|
| `00a_drop_database.sql`, `00b_drop_roles.sql` | Dev-only reset: drops the database, then the roles |
| `01_roles.sql` | Creates the three login roles (run first, on the `postgres` database) |
| `02_create_db.sql` | Creates `civicconnect_db` (run alone, on the `postgres` database) |
| `03_schema.sql` | Tables, constraints, indexes, append-only trigger, grants (run on `civicconnect_db`) |
| `04_requests_module.sql` | Seed categories, length/owner constraints, assignee-must-be-Staff trigger (run on `civicconnect_db`, after 03) |
| `05_dev_seed_users.sql` | **Development only.** Four test accounts (Requester, two Staff, Manager) with known passwords (run on `civicconnect_db`, after 04) |
| `perf/perf_seed_requests.sql`, `perf/perf_cleanup.sql` | **Development/staging only.** Adds, then removes, 1000 open `PERF-TEST` requests for the TC-21 load test (run on `civicconnect_db`, after 05) |

---

## 1. Design principle: who owns which rule

The PED adopts a **hybrid model**: the database enforces structure, the C# application enforces business rules. These scripts follow that split so nothing is implemented twice.

| Concern | Database (this folder) | C# application |
|---|---|---|
| Tables, foreign keys, `NOT NULL`, uniqueness | ✅ | |
| Allowed `role` / `status` values | ✅ `CHECK` list | ✅ enums |
| Which status transition is legal | | ✅ State Pattern (ADR-004) |
| Writing `request_history` | | ✅ `Request` aggregate, same transaction as the change (ADR-002) |
| Notifications | | ✅ Observer / domain events (ADR-005) |
| Input validation | | ✅ FluentValidation |
| Role and permission checks | | ✅ service layer (`IAccessControl`, ADR-006) |
| Concurrent edit detection | ✅ `xmin` column | ✅ EF Core concurrency token |
| History cannot be edited or deleted | ✅ grants + trigger | |
| App cannot delete data or alter schema | ✅ grants | |
| Assignee must be a Staff user | ✅ trigger (`04_requests_module.sql`) | ✅ `RequestService` |

There are deliberately **no** workflow triggers, row-level security, views or stored procedures. The only triggers are the append-only guard on `request_history` (03) and the narrow assignee-must-be-Staff check (04), which is a safety net for the C# rule, not a replacement.

---

## 2. Data model

```
users 1──N request N──1 category
users 1──N request (assigned_staff_id, optional)
request 1──N request_history      users 1──N request_history (changed_by)
request 1──N notes            users 1──N notes (author_id)
```

| Table | Notes |
|---|---|
| `users` | Roles: `Requester`, `Staff`, `Manager`. Email must match a basic format and is unique case-insensitively. `password_hash` must look like a bcrypt/argon2 hash (plaintext is rejected, NFR-003). |
| `category` | Fixed lookup list (FR-002), seeded by `04_requests_module.sql`. `is_active` retires a category without breaking old requests. |
| `request` | Status: `Submitted`, `Assigned`, `InProgress`, `Resolved`, `Closed`. Uses PostgreSQL `xmin` for optimistic concurrency. A request that has left `Submitted` must have an assignee. |
| `request_history` | Append-only audit log (NFR-005). `old_status` is `NULL` for the initial entry; a row must change the status. |
| `notes` | Staff action / resolution notes (FR-012). The PED and ERD call this entity "Comment". |

All foreign keys use `ON DELETE RESTRICT`.

---

## 3. Database roles and permissions

| Role | Used by | Can do |
|---|---|---|
| `civicconnect_owner` | Migrations / admin only | Owns all objects. Creates Staff/Manager accounts and manages categories. |
| `civicconnect_app` | The WinForms application | `SELECT`/`INSERT` on users, request, request_history, notes; `SELECT` on category; `UPDATE` only `users(name,email,password_hash)` and `request(assigned_staff_id,status)`. **No `DELETE`, no DDL, no history/notes edits, cannot change a user's role.** |
| `civicconnect_report` | Read-only reporting | `SELECT` on request data. Cannot see `email` or `password_hash` (NFR-008). Not used by the application at present. |

---

## 4. Running the scripts

Run as the PostgreSQL superuser (`postgres`), in order. Works in **pgAdmin's Query Tool** or `psql`.

1. Open `01_roles.sql`, replace the three `CHANGE_ME_*` passwords **locally** (a different one per role), and run it while connected to the `postgres` database. **Never commit real passwords** (NFR-003).
2. Run `02_create_db.sql` on its own (`CREATE DATABASE` cannot run inside a transaction block; in pgAdmin select the statement and press F5).
3. Connect to `civicconnect_db` (right-click it, then Query Tool) and run `03_schema.sql`.
4. On `civicconnect_db`, run `04_requests_module.sql`.
5. **Development only:** on `civicconnect_db`, run `05_dev_seed_users.sql` (see section 10).

`psql` equivalent:

```bash
psql -U postgres -d postgres        -v ON_ERROR_STOP=1 -f db/01_roles.sql
psql -U postgres -d postgres        -v ON_ERROR_STOP=1 -f db/02_create_db.sql
psql -U postgres -d civicconnect_db -v ON_ERROR_STOP=1 -f db/03_schema.sql
psql -U postgres -d civicconnect_db -v ON_ERROR_STOP=1 -f db/04_requests_module.sql
psql -U postgres -d civicconnect_db -v ON_ERROR_STOP=1 -f db/05_dev_seed_users.sql
```

Requires PostgreSQL 16+. Scripts 01 to 04 are **not re-runnable** on an existing setup (05 can be re-run). To reset a **dev** database (destroys all data), run on the `postgres` database:

1. `00a_drop_database.sql` (alone, in its own run)
2. `00b_drop_roles.sql`

Then repeat steps 1 to 5 above.

### Application connection string

The app must connect as `civicconnect_app`, never as `postgres` or the owner role:

```
Host=localhost;Port=5432;Database=civicconnect_db;Username=civicconnect_app;Password=<from local config>
```

Put it in one of: the gitignored `CivicConnect/CivicConnect.App/appsettings.json` (copy `appsettings.example.json`), user-secrets (`dotnet user-secrets set "ConnectionStrings:CivicConnect" "..." --project CivicConnect/CivicConnect.App`) or the `ConnectionStrings__CivicConnect` environment variable. See the main `README.md`.

---

## 5. Notes for the C# code

- Enums are mapped to strings (`HasConversion<string>()`) so values match the `CHECK` constraints. Status names are `Submitted`, `Assigned`, `InProgress`, `Resolved`, `Closed`.
- The concurrency token is mapped to `xmin` in `RequestsDbContext` (`HasColumnName("xmin")`, `IsConcurrencyToken()`); a conflict surfaces as `RequestConcurrencyException`.
- The `Request` and its `request_history` rows are saved in **one transaction** by the Requests module. The database does not write history for you.
- Passwords are hashed with bcrypt (BCrypt.Net-Next, SHA-384 pre-hash, work factor 12) before insert. If the algorithm changes, adjust the `users_hash_format` check.
- All queries are parameterised (EF Core). Never build SQL with string concatenation or interpolation.
- New users register with `role = 'Requester'`. Staff and Manager accounts are created by the owner role.
- The schema is owned by these scripts: do **not** create EF Core migrations.

---

## 6. Security notes

- No real credentials are committed: role passwords in `01_roles.sql` are placeholders. The only passwords in the repository are the **public development passwords** in `05_dev_seed_users.sql`, which are safe only on a throw-away development database.
- Least-privilege grants limit the damage if the application is compromised.
- **Known limitation:** the desktop client connects directly to PostgreSQL, so the app credentials exist on each client machine. Authorisation is enforced in the C# layer, which a tampered client could bypass. This is an accepted risk (RSK-013); mitigation options are a thin API tier or row-level security.
- Free-tier hosting provides no automatic backups (RSK-009). Take a manual `pg_dump` before any staging or demo with real data.

---

## 7. Open items

- The overdue SLA threshold (FR-015) is **not** implemented and still to be agreed.
- There is no notification table. Notifications are an in-app popup for the person who made the change (FR-005, DEC-001); notifying requesters later would need a new table.

---

## 8. Suggested verification (M3 evidence)

| Check | Expected result |
|---|---|
| Insert a request with status `Foo` | Rejected by `request_status_valid` |
| Insert a user with a plaintext password | Rejected by `users_hash_format` |
| Insert a user with email `not-an-email` | Rejected by `users_email_format` |
| Insert two users with the same email in different case | Second insert rejected |
| `UPDATE` / `DELETE` on `request_history` (any role) | Rejected by trigger |
| `DELETE FROM request` as `civicconnect_app` | Permission denied |
| `SELECT password_hash` as `civicconnect_report` | Permission denied |
| Two concurrent updates to one request | Second update raises a concurrency exception |

Automated coverage today: `HistoryImmutabilityTests` checks that the app role cannot update or delete `request_history` (runs only when `CIVICCONNECT_TEST_CONNECTION` is set). The concurrency case is covered with an in-memory fake (`RequestSecurityTests.LostUpdate_IsReportedAsConflict_NotAsACrash`). The other checks are manual.

---

## 9. Traceability

| Requirement / decision | Where it appears in these scripts |
|---|---|
| NFR-003 Security | Hash-format check, no committed secrets, least-privilege roles |
| NFR-005 Auditability | `request_history` table, append-only trigger, no `UPDATE`/`DELETE` grants |
| NFR-007 Scalability | Indexes on foreign keys and `(status, created_at)` |
| NFR-008 Data handling | Restricted report role, column-level grants |
| FR-002 Controlled categories | `category` table with foreign key from `request`; seed list in 04 |
| FR-010 Assignment | `request_assignee_present` CHECK and assignee-must-be-Staff trigger (04) |
| ADR-002 Atomicity and concurrency | `xmin` token; transaction handled in C# |
| ADR-003 PostgreSQL + EF Core | PostgreSQL 16+ schema, string-mapped enums |
| ADR-004 / ADR-005 | Intentionally implemented in C#, not in the database |

---

## 10. Staff and Manager accounts for testing (development only)

Self-registration only creates Requesters, and the app role cannot change roles, so staff and managers are created on the database side. `05_dev_seed_users.sql` creates four ready-made accounts. There is nothing to edit; run it as it is.

| Email | Role | Password | Use it to test |
|---|---|---|---|
| `requester@civicconnect.test` | Requester | `Requester-Test-123` | Submit a request, see only your own requests |
| `staff1@civicconnect.test` | Staff | `Staff-Test-1234` | Queue, assign, progress, resolve, close |
| `staff2@civicconnect.test` | Staff | `Staff-Test-1234` | "Only the assignee can change status", assigning to another staff member |
| `manager@civicconnect.test` | Manager | `Manager-Test-123` | Read-only overview, all requests, audit trail |

These passwords are **public** because they are in the repository. That is acceptable only for throw-away accounts on a development database: the script refuses to run on any database other than `civicconnect_db`, and the accounts must be deleted before any demo that uses real data. **Never run it against a real or shared system.**

The password is hashed inside PostgreSQL in the same format as the app (SHA-384 pre-hash + bcrypt work factor 12) using `pgcrypto` in a temporary schema that is dropped straight away, so the app role never gets the extension. Re-running the script resets these four accounts. The `.test` domain is reserved and can never be a real address.

Promoting a self-registered account instead (as the owner role, then sign out and in again):

```sql
UPDATE users SET role = 'Staff' WHERE lower(email) = lower('someone@example.com');
```

## 11. Extra checks for 04_requests_module.sql

| Check | Expected result |
|---|---|
| `UPDATE request SET assigned_staff_id = <a Requester id>` | Rejected by `trg_request_assignee_is_staff` |
| Update a request to `Assigned` with `assigned_staff_id` NULL | Rejected by `request_assignee_present` |
| Insert a note or description over 1000 characters | Rejected by the length CHECKs |
| Insert a `request_history` row whose old and new status are the same | Rejected by `history_status_changed` |
| `UPDATE request SET description = 'x'` as `civicconnect_app` | Permission denied (column not granted) |

To run these as the app role in pgAdmin or `psql`, connect as `postgres` and use `SET ROLE civicconnect_app;` before the statement and `RESET ROLE;` afterwards. There is no automated SQL security-check script in this repository.
