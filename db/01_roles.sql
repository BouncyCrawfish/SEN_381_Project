--01_roles.sql - run FIRST, connected to the default "postgres" database as the postgres superuser
--EDIT the three CHANGE_ME passwords locally. NEVER COMMIT REAL PASSWORDS.

CREATE ROLE civicconnect_owner LOGIN PASSWORD 'CHANGE_ME_OWNER'
	NOSUPERUSER NOCREATEDB NOCREATEROLE; --migrations only

CREATE ROLE civicconnect_app LOGIN PASSWORD 'CHANGE_ME_APP'
	NOSUPERUSER NOCREATEDB NOCREATEROLE CONNECTION LIMIT 50; --WinForms app

CREATE ROLE civicconnect_report LOGIN PASSWORD 'CHANGE_ME_REPORT'
	NOSUPERUSER NOCREATEDB NOCREATEROLE CONNECTION LIMIT 10; --read-only

ALTER ROLE civicconnect_app SET statement_timeout = '10s';
ALTER ROLE civicconnect_app SET idle_in_transaction_session_timeout = '30s';