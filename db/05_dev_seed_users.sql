-- 05_dev_seed_users.sql  -  DEVELOPMENT / TEST ONLY. Run connected to "civicconnect_db" as postgres.
--
-- Creates one Manager, two Staff and one Requester so every role can be signed in to and tested.
-- NO password and NO hash is stored in this repository: you choose the password when you run it.
--
--   psql -U postgres -d civicconnect_db -v ON_ERROR_STOP=1 \
--        -v confirm_dev=yes -v dev_password="YourDevPassword-123" -f db/05_dev_seed_users.sql
--
-- The password is hashed inside PostgreSQL the same way the app does it (SHA-384 pre-hash, then
-- bcrypt work factor 12, like BCrypt.Net EnhancedHashPassword), so the normal login form accepts it.
-- Re-running resets these four accounts' roles and passwords. NEVER run this against production.

\if :{?confirm_dev}
\else
    \echo 'Refusing to run: pass -v confirm_dev=yes to confirm this is a DEVELOPMENT database.'
    \quit
\endif
\if :{?dev_password}
\else
    \echo 'Refusing to run: pass -v dev_password="..." (at least 12 characters).'
    \quit
\endif
SELECT (:'confirm_dev' = 'yes' AND length(:'dev_password') >= 12 AND current_database() = 'civicconnect_db') AS inputs_ok \gset
\if :inputs_ok
\else
    \echo 'Refusing to run: confirm_dev must be "yes", dev_password needs 12+ characters, and the database must be civicconnect_db.'
    \quit
\endif

BEGIN;

-- Hash the password as the superuser, with pgcrypto in a throw-away schema that is dropped right
-- after, so the app role never gets crypt()/digest(). One salt is generated per run.
CREATE SCHEMA seed_tmp;
CREATE EXTENSION pgcrypto SCHEMA seed_tmp;
SELECT seed_tmp.crypt(encode(seed_tmp.digest(:'dev_password', 'sha384'), 'base64'), seed_tmp.gen_salt('bf', 12)) AS dev_hash \gset
DROP SCHEMA seed_tmp CASCADE;     -- removes the extension too

SET LOCAL ROLE civicconnect_owner;

INSERT INTO users (name, email, role, password_hash) VALUES
    ('Dev Manager',   'manager@civicconnect.test',   'Manager',   :'dev_hash'),
    ('Dev Staff One', 'staff1@civicconnect.test',    'Staff',     :'dev_hash'),
    ('Dev Staff Two', 'staff2@civicconnect.test',    'Staff',     :'dev_hash'),
    ('Dev Requester', 'requester@civicconnect.test', 'Requester', :'dev_hash')
ON CONFLICT ((lower(email))) DO UPDATE
    SET role = EXCLUDED.role, password_hash = EXCLUDED.password_hash, name = EXCLUDED.name;

RESET ROLE;

COMMIT;

\echo 'Dev accounts ready (all use the password you supplied):'
\echo '  manager@civicconnect.test    Manager'
\echo '  staff1@civicconnect.test     Staff'
\echo '  staff2@civicconnect.test     Staff'
\echo '  requester@civicconnect.test  Requester'
