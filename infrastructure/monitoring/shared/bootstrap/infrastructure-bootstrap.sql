\set ON_ERROR_STOP on

\getenv glitchtip_password MONITORING_GLITCHTIP_PASSWORD
\getenv grafana_password MONITORING_GRAFANA_PASSWORD

SELECT format('CREATE ROLE %I', name)
FROM unnest(ARRAY['monitoring_glitchtip', 'monitoring_grafana']) AS name
WHERE NOT EXISTS (SELECT FROM pg_roles WHERE rolname = name)
\gexec

ALTER ROLE monitoring_glitchtip WITH LOGIN PASSWORD :'glitchtip_password';
ALTER ROLE monitoring_grafana WITH LOGIN PASSWORD :'grafana_password';

SELECT format('CREATE DATABASE %I OWNER %I', name, owner)
FROM (VALUES ('glitchtip', 'monitoring_glitchtip'), ('grafana', 'monitoring_grafana')) AS databases(name, owner)
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = name)
\gexec

ALTER DATABASE glitchtip OWNER TO monitoring_glitchtip;
ALTER DATABASE grafana OWNER TO monitoring_grafana;

REVOKE ALL ON DATABASE glitchtip, grafana FROM PUBLIC;

\connect glitchtip
\ir transfer-ownership.sql
ALTER SCHEMA public OWNER TO monitoring_glitchtip;
CALL pg_temp.transfer_ownership('public', 'monitoring_glitchtip');

\connect grafana
\ir transfer-ownership.sql
ALTER SCHEMA public OWNER TO monitoring_grafana;
CALL pg_temp.transfer_ownership('public', 'monitoring_grafana');
