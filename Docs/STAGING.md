# CivicConnect simulated staging (M3)

CivicConnect is a Windows desktop client that connects straight to PostgreSQL, so there is no web server to "deploy". The M3 brief accepts a credible **simulated** staging environment (section 14). This page defines ours, how to deploy the release candidate into it, how to check it, and how to roll it back. Fill in the record at the bottom every time you deploy.

## 1. How staging differs from development

| | Development | Staging (simulated) |
|---|---|---|
| Code | Whatever is checked out, Debug build, run from Visual Studio | The **tagged release candidate** only (`git checkout <tag>`), Release build, run from a published folder |
| Database | `civicconnect_db` that each developer has been changing | `civicconnect_db` **rebuilt from scratch** from the `db/` scripts on the staging machine, so it contains only what the scripts create |
| Machine | Developer's own PC | One agreed team laptop (or a separate Windows user account) that is not used for development during the demo |
| Configuration | `appsettings.json` or user-secrets | The `ConnectionStrings__CivicConnect` **environment variable** only; no `appsettings.json` in the published folder |
| Database role passwords | Developer's own | Staging-only passwords set in `01_roles.local.sql` (gitignored), different from development |
| Test data | Anything | `05_dev_seed_users.sql` (four test accounts) and, for TC-21 only, `db/perf/perf_seed_requests.sql` |

Known differences from a real production environment: one machine and a local database (no network latency, no TLS), one user at a time, test accounts with public passwords, and no automated backups.

## 2. Deploy the release candidate

Run these on the staging machine in PowerShell. Replace `<tag>` with the release candidate tag, for example `v3.0-m3`.

```powershell
# 1. Get exactly the release candidate
git fetch --tags
git checkout <tag>
git rev-parse HEAD                      # write this commit hash in the record below

# 2. Rebuild the database from the scripts (DESTROYS the staging database)
#    In pgAdmin or psql, as postgres, run in order:
#    00a_drop_database.sql, 00b_drop_roles.sql   (only if a previous staging database exists)
#    01_roles.local.sql   (copy of 01_roles.sql with staging passwords; never commit it)
#    02_create_db.sql, 03_schema.sql, 04_requests_module.sql, 05_dev_seed_users.sql

# 3. Publish the Release build into a folder named after the tag
dotnet publish CivicConnect/CivicConnect.App/CivicConnect.App.csproj -c Release -r win-x64 --self-contained false -o ..\staging\<tag>

# 4. Configure it for this PowerShell window only (the password is not saved anywhere)
$env:ConnectionStrings__CivicConnect = "Host=localhost;Port=5432;Database=civicconnect_db;Username=civicconnect_app;Password=<staging app password>"

# 5. Start the published app
..\staging\<tag>\CivicConnect.App.exe
```

The published folder must not contain an `appsettings.json`. If one appears, delete it: staging gets its configuration from the environment variable only.

## 3. Verify the deployed candidate

1. Confirm the commit: `git rev-parse HEAD` matches the commit the tag points to on GitHub (Releases or Tags page).
2. Confirm the database came only from the scripts: in pgAdmin, `SELECT count(*) FROM request;` returns 0 before the checks start.
3. Run the system (E2E) checks against the published app and record the result of each in the PED test case table:
   - **TC-19** sign-in, registration and lockout
   - **TC-20** each role sees only its own home-screen tiles
   - **TC-13** a staff status change shows the popup and writes exactly **one** history row (DEF-01 regression)
   - **TC-10 / TC-11** manager audit trail and its filters
   - **TC-14** manager overview loads
4. Optional performance run (TC-21): run `db/perf/perf_seed_requests.sql`, then from the repository folder:
   ```powershell
   $env:CIVICCONNECT_TEST_CONNECTION = $env:ConnectionStrings__CivicConnect
   dotnet test --project Tests/CivicConnect.Tests/CivicConnect.Tests.csproj -c Release
   Get-Content Tests\CivicConnect.Tests\bin\Release\net10.0-windows\perf-tc21-result.txt
   ```
   This runs the whole suite; with the variable set, the two database tests (`HistoryImmutabilityTests` and TC-21) run as well instead of being skipped. Copy the timings from `perf-tc21-result.txt` into the PED. Run `db/perf/perf_cleanup.sql` afterwards if you need a clean database for the demo.

## 4. Rollback and recovery

- **Application:** every deployment is published into its own folder named after its tag. To roll back, close the app and start the previous folder's `CivicConnect.App.exe`. Nothing else changes.
- **Database:** before deploying a new candidate over an existing staging database, take a backup:
  ```powershell
  pg_dump -U postgres -d civicconnect_db -F c -f ..\staging\backup-before-<tag>.dump
  ```
  To restore it: run `00a_drop_database.sql`, recreate the empty database with `02_create_db.sql`, then `pg_restore -U postgres -d civicconnect_db ..\staging\backup-before-<tag>.dump`. Dump files are gitignored (`*.dump`) and must never be committed because they contain user data and password hashes.
- **Schema concern:** the schema is owned by the SQL scripts and has no down-migrations. A candidate that needs a different schema cannot be rolled back by swapping the app folder alone; restore the matching database backup as well.

## 5. Deployment record

Copy this table for each deployment and keep it in the PED (Release readiness section).

| Field | Value |
|---|---|
| Date and time | |
| Deployed by | |
| Release candidate tag | |
| Commit hash (`git rev-parse HEAD`) | |
| Staging machine / Windows user | |
| PostgreSQL version (`SELECT version();`) | |
| Database rebuilt from scripts (yes/no) | |
| Configuration source | `ConnectionStrings__CivicConnect` environment variable |
| E2E checks run (TC IDs and result) | |
| Performance run (TC-21) result | |
| Problems found (defect IDs) | |
| Rolled back? To which tag? | |
