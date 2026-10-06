--02_create_db.sql run SECOND, connected to the "postgres", ALONE.
--CREATE DATABASE cannot run inside a transaction block, so execute only this statement.

CREATE DATABASE civicconnect_db OWNER civicconnect_owner ENCODING 'UTF8';