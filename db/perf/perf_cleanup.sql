-- perf_cleanup.sql  -  removes the rows added by perf_seed_requests.sql.
-- Run connected to "civicconnect_db" as the postgres superuser (the app role has no DELETE grant).
-- A seeded request that someone has since assigned or changed has request_history rows, which are
-- append-only; the foreign key then blocks deleting it, and it is listed by the second query instead.

DELETE FROM request r
WHERE r.description LIKE 'PERF-TEST request %'
  AND NOT EXISTS (SELECT 1 FROM request_history h WHERE h.request_id = r.id)
  AND NOT EXISTS (SELECT 1 FROM notes n WHERE n.request_id = r.id);

-- Seeded requests that could not be removed (they have history or notes): expected to be empty.
SELECT id, status FROM request WHERE description LIKE 'PERF-TEST request %';
