-- =====================================================================
-- 00b_drop_roles.sql  -  DEV RESET, step 2 of 2.
-- Run connected to the "postgres" database AFTER 00a_drop_database.sql.
-- Then re-run 01_roles.sql, 02_create_database.sql, 03_schema.sql.
-- =====================================================================
DROP ROLE IF EXISTS civicconnect_app;
DROP ROLE IF EXISTS civicconnect_report;
DROP ROLE IF EXISTS civicconnect_owner;
