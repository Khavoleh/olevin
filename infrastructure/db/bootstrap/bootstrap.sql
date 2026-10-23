\set ON_ERROR_STOP on

\getenv logto_password OLEVIN_LOGTO_PASSWORD
\getenv atlas_password OLEVIN_ATLAS_PASSWORD
\getenv wolverine_password OLEVIN_WOLVERINE_PASSWORD
\getenv settings_password OLEVIN_SETTINGS_PASSWORD
\getenv financial_wellness_password OLEVIN_FINANCIAL_WELLNESS_PASSWORD

SELECT format('CREATE ROLE %I', name)
FROM unnest(ARRAY['olevin_logto', 'olevin_atlas', 'olevin_wolverine', 'olevin_settings', 'olevin_financial_wellness']) AS name
WHERE NOT EXISTS (SELECT FROM pg_roles WHERE rolname = name)
\gexec

ALTER ROLE olevin_logto WITH LOGIN CREATEROLE PASSWORD :'logto_password';
ALTER ROLE olevin_atlas WITH LOGIN PASSWORD :'atlas_password';
ALTER ROLE olevin_wolverine WITH LOGIN PASSWORD :'wolverine_password';
ALTER ROLE olevin_settings WITH LOGIN PASSWORD :'settings_password';
ALTER ROLE olevin_financial_wellness WITH LOGIN PASSWORD :'financial_wellness_password';

SELECT format('GRANT %I TO olevin_logto WITH ADMIN OPTION, INHERIT FALSE, SET FALSE', rolname)
FROM pg_roles
WHERE rolname LIKE 'logto\_tenant\_%'
  AND NOT pg_has_role('olevin_logto', oid, 'MEMBER WITH ADMIN OPTION')
\gexec

SELECT format('CREATE DATABASE %I OWNER %I', name, owner)
FROM (VALUES ('auth', 'olevin_logto'), ('atlas', 'olevin_atlas')) AS databases(name, owner)
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = name)
\gexec

ALTER DATABASE auth OWNER TO olevin_logto;
ALTER DATABASE atlas OWNER TO olevin_atlas;

REVOKE ALL ON DATABASE olevin FROM PUBLIC;
GRANT CONNECT ON DATABASE olevin TO olevin_atlas, olevin_wolverine, olevin_settings, olevin_financial_wellness;
GRANT CREATE ON DATABASE olevin TO olevin_atlas;

\ir transfer-ownership.sql

CREATE SCHEMA IF NOT EXISTS outbox AUTHORIZATION olevin_wolverine;
ALTER SCHEMA outbox OWNER TO olevin_wolverine;
CALL pg_temp.transfer_ownership('outbox', 'olevin_wolverine');

CREATE SCHEMA IF NOT EXISTS settings AUTHORIZATION olevin_atlas;
ALTER SCHEMA settings OWNER TO olevin_atlas;
CALL pg_temp.transfer_ownership('settings', 'olevin_atlas');

CREATE SCHEMA IF NOT EXISTS financial_wellness AUTHORIZATION olevin_atlas;
ALTER SCHEMA financial_wellness OWNER TO olevin_atlas;
CALL pg_temp.transfer_ownership('financial_wellness', 'olevin_atlas');

ALTER SCHEMA public OWNER TO olevin_atlas;
CALL pg_temp.transfer_ownership('public', 'olevin_atlas');

GRANT USAGE ON SCHEMA outbox TO olevin_settings, olevin_financial_wellness;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA outbox TO olevin_settings, olevin_financial_wellness;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA outbox TO olevin_settings, olevin_financial_wellness;
ALTER DEFAULT PRIVILEGES FOR ROLE olevin_wolverine IN SCHEMA outbox
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO olevin_settings, olevin_financial_wellness;
ALTER DEFAULT PRIVILEGES FOR ROLE olevin_wolverine IN SCHEMA outbox
    GRANT USAGE, SELECT ON SEQUENCES TO olevin_settings, olevin_financial_wellness;

GRANT USAGE ON SCHEMA settings TO olevin_settings;
GRANT USAGE ON SCHEMA financial_wellness TO olevin_settings;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA settings TO olevin_settings;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA settings TO olevin_settings;
ALTER DEFAULT PRIVILEGES FOR ROLE olevin_atlas IN SCHEMA settings
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO olevin_settings;
ALTER DEFAULT PRIVILEGES FOR ROLE olevin_atlas IN SCHEMA settings
    GRANT USAGE, SELECT ON SEQUENCES TO olevin_settings;

GRANT USAGE ON SCHEMA financial_wellness TO olevin_financial_wellness;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA financial_wellness TO olevin_financial_wellness;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA financial_wellness TO olevin_financial_wellness;
ALTER DEFAULT PRIVILEGES FOR ROLE olevin_atlas IN SCHEMA financial_wellness
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO olevin_financial_wellness;
ALTER DEFAULT PRIVILEGES FOR ROLE olevin_atlas IN SCHEMA financial_wellness
    GRANT USAGE, SELECT ON SEQUENCES TO olevin_financial_wellness;

\connect auth
\ir transfer-ownership.sql
CALL pg_temp.transfer_ownership('public', 'olevin_logto');

\connect atlas
ALTER SCHEMA public OWNER TO olevin_atlas;
