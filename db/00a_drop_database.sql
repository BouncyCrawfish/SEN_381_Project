-- =====================================================================
-- 00a_drop_database.sql  -  DEV RESET, step 1 of 2.  *** DESTROYS ALL DATA ***
-- Run connected to the "postgres" database, ALONE (select it and press F5).
-- WITH (FORCE) disconnects anyone still connected (PostgreSQL 13+).
-- =====================================================================
DROP DATABASE IF EXISTS civicconnect_db WITH (FORCE);
