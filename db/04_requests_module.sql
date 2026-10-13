-- 04_requests_module.sql  -  run FOURTH, connected to "civicconnect_db" as the postgres superuser.
-- Adds what the Requests module needs on top of 03_schema.sql: seed categories, extra integrity
-- constraints, and one narrow security trigger. Safe to run once on a database built from 03.
-- Run it in one go: it is a single transaction, so a failure leaves nothing half-applied.

ROLLBACK;
RESET ROLE;
SET ROLE civicconnect_owner;   -- objects stay owned by the owner role, never by the app role

BEGIN;

-- 1. Controlled category list (FR-002). The app role can only SELECT from category, so the list
--    is managed here by the owner. is_active retires a category without breaking old requests.
INSERT INTO category (name) VALUES
    ('Facility fault'),
    ('Maintenance issue'),
    ('Lost property'),
    ('IT support'),
    ('Safety concern'),
    ('Other')
ON CONFLICT (name) DO NOTHING;

-- 2. Structural integrity (database = structure, C# = business rules; see README section 1).
--    Lengths mirror RequestRules.cs so the DB is a safety net, never the first line of defence.
ALTER TABLE request
    ADD CONSTRAINT request_description_length CHECK (char_length(description) <= 1000);
ALTER TABLE notes
    ADD CONSTRAINT note_length CHECK (char_length(note) <= 1000);

-- A request that has left "Submitted" must have an owner (FR-010). Pure structure, no workflow logic.
ALTER TABLE request
    ADD CONSTRAINT request_assignee_present CHECK (status = 'Submitted' OR assigned_staff_id IS NOT NULL);

-- A history row records a CHANGE (NULL old_status is the initial entry).
ALTER TABLE request_history
    ADD CONSTRAINT history_status_changed CHECK (old_status IS DISTINCT FROM new_status);

-- 3. Security trigger: only a Staff account may own a request.
--    DELIBERATE, NARROW EXCEPTION to "no business-rule triggers": the C# service already checks
--    this, but the desktop client holds the app credentials (README section 6 accepted risk), so a
--    tampered client could otherwise assign work to a requester or manager. The workflow itself
--    (which status change is legal) stays in C# (ADR-004). Drop this trigger if the team disagrees.
CREATE FUNCTION enforce_assignee_is_staff() RETURNS TRIGGER
LANGUAGE plpgsql
SET search_path = public, pg_temp      -- fixed path: no object hijacking through a user schema
AS $$
BEGIN
    IF NEW.assigned_staff_id IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM users WHERE id = NEW.assigned_staff_id AND role = 'Staff') THEN
        RAISE EXCEPTION 'assigned_staff_id must reference a Staff user'
            USING ERRCODE = 'check_violation';
    END IF;
    RETURN NEW;
END $$;

REVOKE ALL ON FUNCTION enforce_assignee_is_staff() FROM PUBLIC;

CREATE TRIGGER trg_request_assignee_is_staff
    BEFORE INSERT OR UPDATE OF assigned_staff_id ON request
    FOR EACH ROW EXECUTE FUNCTION enforce_assignee_is_staff();

-- 4. Grants: already correct in 03 (app: SELECT/INSERT, UPDATE only status + assigned_staff_id;
--    history and notes INSERT-only; no DELETE anywhere). Re-stated as a check, not widened.
--    Verify afterwards with the queries in db/README.md section 8.

COMMIT;
