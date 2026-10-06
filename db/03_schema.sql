-- 03_schema.sql  -  run THIRD, connected to the "civicconnect_db" database as the postgres superuser.
-- Division of responsibility, see README.md: DB = structure/integrity/grants,
-- C# = business rules (State Pattern, Observer, FluentValidation).

ROLLBACK;                      -- clears an aborted transaction left by a previous failed run (a warning here is harmless)
RESET ROLE;                    -- clears a leftover SET ROLE if a previous run failed

-- Superuser fixes so the owner role can create objects, on any PostgreSQL version
-- (PostgreSQL <= 14, or a database created with a different owner, otherwise
-- gives "permission denied for schema public").
ALTER DATABASE civicconnect_db OWNER TO civicconnect_owner;
ALTER SCHEMA public OWNER TO civicconnect_owner;

REVOKE ALL ON DATABASE civicconnect_db FROM PUBLIC;
GRANT CONNECT ON DATABASE civicconnect_db TO civicconnect_app, civicconnect_report;

SET ROLE civicconnect_owner; -- objects below are owned by the owner role

BEGIN;

REVOKE ALL ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO civicconnect_app, civicconnect_report;

-- 2. Tables (ERD)
--    Role values  : Requester | Staff | Manager
--    Status values: Submitted | Assigned | InProgress | Resolved | Closed
--    EF Core: store enums as strings (HasConversion<string>()).
--    The DB only checks the value is in the list; WHICH change is legal
--    is decided by the C# State Pattern.

CREATE TABLE users (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(254) NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'Requester',
    password_hash TEXT NOT NULL,

    CONSTRAINT users_name_not_blank CHECK (btrim(name) <> ''),
    -- basic format safety net; full validation stays in FluentValidation (C#)
    CONSTRAINT users_email_format CHECK (email ~* '^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$'),
    CONSTRAINT users_role_valid CHECK (role IN ('Requester','Staff','Manager')),
    -- safety net for NFR-003: hashing is done in C#, DB just refuses plaintext
    CONSTRAINT users_hash_format CHECK (password_hash ~ '^\$(2[aby]|argon2(id|i|d))\$')
);
CREATE UNIQUE INDEX ux_users_email_lower ON users (lower(email));  -- case-insensitive unique

CREATE TABLE category (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,-- retire instead of delete

    CONSTRAINT category_name_not_blank CHECK (btrim(name) <> '')
);

CREATE TABLE request (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    requester_id BIGINT NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    assigned_staff_id BIGINT REFERENCES users(id) ON DELETE RESTRICT,
    category_id BIGINT NOT NULL REFERENCES category(id) ON DELETE RESTRICT,
    description TEXT NOT NULL,
    location VARCHAR(200)NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Submitted',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    -- Optimistic concurrency (ADR-002) uses PostgreSQL's built-in xmin column.
    -- EF Core: [Timestamp] public uint Version { get; set; }  -> maps to "xmin".

    CONSTRAINT request_description_not_blank CHECK (btrim(description) <> ''),
    CONSTRAINT request_location_not_blank CHECK (btrim(location) <> ''),
    CONSTRAINT request_status_valid CHECK (status IN ('Submitted','Assigned','InProgress','Resolved','Closed'))
);

CREATE TABLE request_history (-- NFR-005: append-only
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    request_id BIGINT NOT NULL REFERENCES request(id) ON DELETE RESTRICT,
    old_status VARCHAR(20),-- NULL for the initial entry
    new_status VARCHAR(20) NOT NULL,
    changed_by BIGINT NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    "timestamp" TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT history_old_valid CHECK (old_status IS NULL OR old_status IN
        ('Submitted','Assigned','InProgress','Resolved','Closed')),
    CONSTRAINT history_new_valid CHECK (new_status IN
        ('Submitted','Assigned','InProgress','Resolved','Closed'))
);

CREATE TABLE notes (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    request_id BIGINT NOT NULL REFERENCES request(id) ON DELETE RESTRICT,
    author_id BIGINT NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    note TEXT NOT NULL,
    "timestamp" TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT note_text_not_blank CHECK (btrim(note) <> '')
);

-- 3. Indexes (foreign keys + staff-queue access pattern, NFR-002 / NFR-007)

CREATE INDEX idx_request_requester ON request(requester_id);
CREATE INDEX idx_request_staff ON request(assigned_staff_id);
CREATE INDEX idx_request_category ON request(category_id);
CREATE INDEX idx_request_status_created ON request(status, created_at DESC);
CREATE INDEX idx_history_request ON request_history(request_id, "timestamp");
CREATE INDEX idx_notes_request ON notes(request_id, "timestamp");
CREATE INDEX idx_notes_author ON notes(author_id);

-- 4. Immutable history (NFR-005). This is a structural guarantee, not a
--    business rule: C# never updates or deletes history, so no overlap.
--    The app role also has no UPDATE/DELETE grant (section 5); this stops
--    even the owner role editing history by accident.

CREATE FUNCTION forbid_history_change() RETURNS TRIGGER
LANGUAGE plpgsql AS $$
BEGIN
    RAISE EXCEPTION 'request_history is append-only (% not allowed)', TG_OP;
END $$;

CREATE TRIGGER trg_history_append_only
    BEFORE UPDATE OR DELETE ON request_history
    FOR EACH ROW EXECUTE FUNCTION forbid_history_change();
CREATE TRIGGER trg_history_no_truncate
    BEFORE TRUNCATE ON request_history
    FOR EACH STATEMENT EXECUTE FUNCTION forbid_history_change();

-- 5. Least-privilege grants (limits damage if the app is compromised)
--    No DELETE and no DDL for the app role. Role changes and category
--    management are done by the owner role, not the running app.

GRANT SELECT, INSERT ON users TO civicconnect_app;
GRANT UPDATE (name, email, password_hash) ON users TO civicconnect_app;

GRANT SELECT ON category TO civicconnect_app;

GRANT SELECT, INSERT ON request TO civicconnect_app;
GRANT UPDATE (assigned_staff_id, status) ON request TO civicconnect_app;

GRANT SELECT, INSERT ON request_history TO civicconnect_app;   -- no UPDATE/DELETE
GRANT SELECT, INSERT ON notes TO civicconnect_app;   -- no UPDATE/DELETE

-- Reporting role: read-only, no password hashes or contact details (NFR-008)
GRANT SELECT (id, name, role) ON users TO civicconnect_report;
GRANT SELECT ON category, request, request_history, notes TO civicconnect_report;

--NO SAMPLE DATA YET
COMMIT;