CREATE PROCEDURE pg_temp.transfer_ownership(target_schema text, new_owner text)
LANGUAGE plpgsql
AS $$
DECLARE
    statement text;
BEGIN
    FOR statement IN
        SELECT format(
            'ALTER %s %I.%I OWNER TO %I',
            CASE c.relkind
                WHEN 'S' THEN 'SEQUENCE'
                WHEN 'v' THEN 'VIEW'
                WHEN 'm' THEN 'MATERIALIZED VIEW'
                WHEN 'f' THEN 'FOREIGN TABLE'
                WHEN 'c' THEN 'TYPE'
                ELSE 'TABLE'
            END,
            n.nspname, c.relname, new_owner
        )
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname = target_schema
          AND c.relkind IN ('r', 'p', 'v', 'm', 'f', 'S', 'c')
          AND c.relowner <> new_owner::regrole
          AND (
              c.relispartition
              OR NOT EXISTS (
                  SELECT FROM pg_depend d
                  WHERE d.classid = 'pg_class'::regclass
                    AND d.objid = c.oid
                    AND d.refobjid <> c.oid
                    AND d.deptype IN ('a', 'i', 'e')
              )
          )
        UNION ALL
        SELECT format('ALTER TYPE %I.%I OWNER TO %I', n.nspname, t.typname, new_owner)
        FROM pg_type t
        JOIN pg_namespace n ON n.oid = t.typnamespace
        WHERE n.nspname = target_schema
          AND t.typrelid = 0
          AND t.typcategory <> 'A'
          AND t.typtype <> 'm'
          AND t.typowner <> new_owner::regrole
        UNION ALL
        SELECT format('ALTER ROUTINE %s OWNER TO %I', p.oid::regprocedure, new_owner)
        FROM pg_proc p
        JOIN pg_namespace n ON n.oid = p.pronamespace
        WHERE n.nspname = target_schema
          AND p.proowner <> new_owner::regrole
          AND NOT EXISTS (
              SELECT FROM pg_depend d
              WHERE d.classid = 'pg_proc'::regclass AND d.objid = p.oid AND d.deptype = 'e'
          )
    LOOP
        EXECUTE statement;
    END LOOP;
END
$$;
