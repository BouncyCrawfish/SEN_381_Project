# CivicConnect Database (`civicconnect_db`)

PostgreSQL 16+ database script for the SEN381 CivicConnect project.
Baseline: PED v2.4 (ADR-002, ADR-003, ADR-004, ADR-005; NFR-003, NFR-005, NFR-008).

| File | Purpose |
|---|---|
| `00a_drop_database.sql`, `00b_drop_roles.sql` | Dev-only reset: drops the database, then the roles |
| `01_roles.sql` | Creates the three login roles (run first, on the `postgres` database) |
| `02_create_database.sql` | Creates `civicconnect_db` (run alone, on the `postgres` database) |
| `03_schema.sql` | Tables, constraints, indexes, append-only trigger, grants, seed categories (run on `civicconnect_db`) |

---

## 1. Design principle: who owns which rule

The PED adopts a **hybrid model**: the database enforces structure, the C# application enforces business rules. This script follows that split so nothing is implemented twice.

| Concern | Database (this repo folder) | C# application |
|---|---|---|
| Tables, foreign keys, `NOT NULL`, uniqueness | ✅ | |
| Allowed `role` / `status` values | ✅ `CHECK` list | ✅ enums |
| Which status transition is legal | | ✅ State Pattern (ADR-004) |
| Writing `request_history`, notifications | | ✅ Observer / domain events (ADR-005) |
| Input validation | | ✅ FluentValidation |
| Role and permission checks | | ✅ service layer |
| Atomic request + initial history write | | ✅ one EF Core transaction (ADR-002) |
| Concurrent edit detection | ✅ `xmin` column | ✅ EF Core concurrency token |
| History cannot be edited or deleted | ✅ grants + trigger | |
| App cannot delete data or alter schema | ✅ grants | |

There are deliberately **no** business-rule triggers, row-level security, views or stored procedures in the database.

---

## 2. Data model

```
users 1──N request N──1 category
users 1──N request (assigned_staff_id, optional)
request 1──N request_history      users 1──N request_history (changed_by)
request 1──N comment              users 1──N comment (author_id)
```

| Table | Notes |
|---|---|
| `users` | Roles: `Requester`, `Staff`, `Manager`. Email must match a basic format and is unique case-insensitively. `password_hash` must look like a bcrypt/argon2 hash (plaintext is rejected, NFR-003). |
| `category` | Fixed lookup list (FR-002). `is_active` retires a category without breaking old requests. |
| `request` | Status: `Submitted`, `Assigned`, `InProgress`, `Resolved`, `Closed`. Uses PostgreSQL `xmin` for optimistic concurrency. |
| `request_history` | Append-only audit log (NFR-005). `old_status` is `NULL` for the initial entry. |
| `comment` | Staff action / resolution notes (FR-012). |

All foreign keys use `ON DELETE RESTRICT`.

---

## 3. Database roles and permissions

| Role | Used by | Can do |
|---|---|---|
| `civicconnect_owner` | Migrations / admin only | Owns all objects. Creates Staff/Manager accounts and manages categories. |
| `civicconnect_app` | The WinForms application | `SELECT`/`INSERT` on users, request, request_history, comment; `SELECT` on category; `UPDATE` only `users(name,email,password_hash)` and `request(assigned_staff_id,status)`. **No `DELETE`, no DDL, no history/comment edits, cannot change a user's role.** |
| `civicconnect_report` | Read-only reporting | `SELECT` on request data. Cannot see `email` or `password_hash` (NFR-008). |

---

## 4. Running the scripts

Run as the PostgreSQL superuser (`postgres`), in order. Works in **pgAdmin's Query Tool** or `psql`.

1. Open `01_roles.sql`, replace the three `CHANGE_ME_*` passwords **locally**, and run it while connected to the `postgres` database. **Never commit real passwords** (NFR-003).
2. Run `02_create_database.sql` on its own (`CREATE DATABASE` cannot run inside a transaction block; in pgAdmin select the statement and press F5).
3. Connect to `civicconnect_db` (right-click it, then Query Tool) and run `03_schema.sql`.

`psql` equivalent:

```bash
psql -U postgres -d postgres       -v ON_ERROR_STOP=1 -f db/01_roles.sql
psql -U postgres -d postgres       -v ON_ERROR_STOP=1 -f db/02_create_database.sql
psql -U postgres -d civicconnect_db -v ON_ERROR_STOP=1 -f db/03_schema.sql
```

Requires PostgreSQL 16+. The scripts are **not re-runnable** on an existing setup. To reset a **dev** database (destroys all data), run on the `postgres` database:

1. `00a_drop_database.sql` (alone, in its own run)
2. `00b_drop_roles.sql`

Then repeat steps 1 to 3 above.

### Application connection string
Store it in the **gitignored** `appsettings.json` (see `appsettings.example.json`), using the `civicconnect_app` role:

```
Host=localhost;Port=5432;Database=civicconnect_db;Username=civicconnect_app;Password=<from local config>
```

---

## 5. Notes for the C# code

- Map enums to strings: `HasConversion<string>()` so values match the `CHECK` constraints. Status names are `Submitted`, `Assigned`, `InProgress`, `Resolved`, `Closed`.
- Map the concurrency token to `xmin`:
  ```csharp
  [Timestamp] public uint Version { get; set; }   // Npgsql maps this to xmin
  ```
- Create the `Request` and its initial `RequestHistory` row in **one transaction**. The database does not write history for you.
- Hash passwords with bcrypt or argon2 before insert. If you choose a different algorithm, adjust the `users_hash_format` check.
- Always use parameterised queries (EF Core default). Never build SQL with string concatenation or interpolation.
- Register new users with `role = 'Requester'`. Staff and Manager accounts are created by the owner role.

---

## 6. Security notes

- No credentials are committed. Use `-v` variables, environment variables or user secrets.
- Least-privilege grants limit the damage if the application is compromised.
- **Known limitation:** the desktop client connects directly to PostgreSQL, so the app credentials exist on each client machine. Authorisation is enforced in the C# layer, which a tampered client could bypass. This is recorded as an accepted risk; mitigation options are a thin API tier or row-level security (see Risk Register / ADR).
- Free-tier hosting provides no automatic backups (RSK-009). Take a manual `pg_dump` before staging.

---

## 7. Placeholders to review

- Seed categories (`Roads & Potholes`, `Water & Sanitation`, ...) are placeholders. Replace them with the agreed fixed list (FR-002).
- The overdue SLA threshold (FR-015) is **not** in the database; it is applied in C# and is still to be agreed.
- In-app/email notification storage (FR-005) is not in the ERD and is not part of this script.

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

Tests that prove these belong in the xUnit integration suite and should be linked from the RTM.

---

## 9. Traceability

| Requirement / decision | Where it appears in this script |
|---|---|
| NFR-003 Security | Hash-format check, no committed secrets, least-privilege roles |
| NFR-005 Auditability | `request_history` table, append-only trigger, no `UPDATE`/`DELETE` grants |
| NFR-007 Scalability | Indexes on foreign keys and `(status, created_at)` |
| NFR-008 Data handling | Restricted report role, column-level grants |
| FR-002 Controlled categories | `category` table with foreign key from `request` |
| ADR-002 Atomicity and concurrency | `xmin` token; transaction handled in C# |
| ADR-003 PostgreSQL + EF Core | PostgreSQL 16+ schema, string-mapped enums |
| ADR-004 / ADR-005 | Intentionally implemented in C#, not in the database |
