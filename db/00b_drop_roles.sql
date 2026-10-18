-- =====================================================================
-- 00b_drop_roles.sql  -  DEV RESET, step 2 of 2.
-- Run connected to the "postgres" database AFTER 00a_drop_database.sql.
-- Then re-run 01_roles.sql, 02_create_db.sql, 03_schema.sql, 04_requests_module.sql, 05_dev_seed_users.sql.
-- =====================================================================
DROP ROLE IF EXISTS civicconnect_app;
DROP ROLE IF EXISTS civicconnect_report;
DROP ROLE IF EXISTS civicconnect_owner;
