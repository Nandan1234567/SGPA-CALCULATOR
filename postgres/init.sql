CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

SET timezone = 'UTC';

-- Log slow queries (queries taking more than 200ms)
ALTER SYSTEM SET log_min_duration_statement = 200;
ALTER SYSTEM SET log_statement = 'none';

SELECT pg_reload_conf();

