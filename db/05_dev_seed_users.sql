-- 05_dev_seed_users.sql  -  DEVELOPMENT / TEST DATA ONLY.
-- Run connected to "civicconnect_db" as the postgres superuser. Plain SQL: works in pgAdmin's Query Tool
-- (open a Query Tool on civicconnect_db, paste, press F5) and in psql. NOTHING TO EDIT.
--
-- Creates test accounts with KNOWN passwords so every role can be signed in to:
--
--     Email                          Role       Password
--     manager@civicconnect.test      Manager    Manager-Test-123
--     staff1@civicconnect.test       Staff      Staff-Test-1234
--     staff2@civicconnect.test       Staff      Staff-Test-1234
--     requester@civicconnect.test    Requester  Requester-Test-123
--
-- These passwords are public (they are in the repository). That is fine ONLY because these are throw-away
-- test accounts on a development database; the guard below refuses any database that is not
-- civicconnect_db. NEVER run this on a real or shared system, and delete these accounts before any demo
-- that uses real data. Re-running resets these four accounts' roles and passwords.
--
-- Passwords are hashed inside PostgreSQL the same way the app does it (SHA-384 pre-hash, then bcrypt work
-- factor 12, like BCrypt.Net EnhancedHashPassword), so the normal login screen accepts them.

DO $seed$
BEGIN
    IF current_database() <> 'civicconnect_db' THEN
        RAISE EXCEPTION 'Refusing to run: connect to the civicconnect_db development database (currently %).', current_database();
    END IF;

    -- pgcrypto lives in a throw-away schema that is dropped straight away, so the app role never gets it.
    CREATE SCHEMA seed_tmp;
    CREATE EXTENSION pgcrypto SCHEMA seed_tmp;

    INSERT INTO users (name, email, role, password_hash)
    SELECT v.name, v.email, v.role,
           seed_tmp.crypt(encode(seed_tmp.digest(v.pw, 'sha384'), 'base64'), seed_tmp.gen_salt('bf', 12))
    FROM (VALUES
        ('Dev Manager',   'manager@civicconnect.test',   'Manager',   'Manager-Test-123'),
        ('Dev Staff One', 'staff1@civicconnect.test',    'Staff',     'Staff-Test-1234'),
        ('Dev Staff Two', 'staff2@civicconnect.test',    'Staff',     'Staff-Test-1234'),
        ('Dev Requester', 'requester@civicconnect.test', 'Requester', 'Requester-Test-123')
    ) AS v(name, email, role, pw)
    ON CONFLICT ((lower(email))) DO UPDATE
        SET role = EXCLUDED.role, password_hash = EXCLUDED.password_hash, name = EXCLUDED.name;

    DROP SCHEMA seed_tmp CASCADE;     -- removes the extension too
END
$seed$;

-- Check: should list the four accounts.
SELECT id, name, email, role FROM users WHERE email LIKE '%@civicconnect.test' ORDER BY id;