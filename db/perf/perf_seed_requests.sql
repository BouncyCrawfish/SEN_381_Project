-- perf_seed_requests.sql  -  DEVELOPMENT / STAGING TEST DATA ONLY (performance test TC-21).
-- Run connected to "civicconnect_db" as the postgres superuser, AFTER 03, 04 and 05.
-- Adds 1000 open ("Submitted") requests owned by requester@civicconnect.test so the staff queue has a
-- realistic amount of data to load. Every seeded row starts with 'PERF-TEST' so it can be found and removed.
-- To change the volume, change 1000 in the generate_series line (and record the number you used).

DO $perf$
BEGIN
    IF current_database() <> 'civicconnect_db' THEN
        RAISE EXCEPTION 'Refusing to run: connect to civicconnect_db (currently %).', current_database();
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE email = 'requester@civicconnect.test') THEN
        RAISE EXCEPTION 'Run db/05_dev_seed_users.sql first: the seeded requests belong to requester@civicconnect.test.';
    END IF;
END
$perf$;

WITH cats AS (
    SELECT array_agg(id ORDER BY id) AS ids FROM category WHERE is_active
), requester AS (
    SELECT id FROM users WHERE email = 'requester@civicconnect.test'
)
INSERT INTO request (requester_id, category_id, description, location, status, created_at)
SELECT requester.id,
       cats.ids[1 + (g % array_length(cats.ids, 1))],
       'PERF-TEST request ' || g || ': generated load-test data for the staff queue.',
       'PERF-TEST location ' || (g % 50),
       'Submitted',
       now() - make_interval(mins => g)
FROM generate_series(1, 1000) AS g, cats, requester;

-- Check: how many open requests the staff queue will now load.
SELECT count(*) AS open_requests FROM request WHERE status <> 'Closed';
