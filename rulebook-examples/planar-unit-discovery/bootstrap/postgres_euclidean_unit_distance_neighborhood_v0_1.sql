-- postgres_euclidean_unit_distance_neighborhood_v0_1.sql
-- A PostgreSQL sketch for ERB_euclidean-unit-distance-neighborhood_v0_1.
-- Goal: strongly typed, bitemporal, ACID-friendly DAG subset for mathematical semantic topology.
-- This is a runnable starting point, but an implementation agent should adapt names to the existing generator conventions.

BEGIN;

CREATE EXTENSION IF NOT EXISTS btree_gist;
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- ---------------------------------------------------------------------------
-- 1. Core vocabulary
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cmcc_contexts (
    context_id            text PRIMARY KEY,
    display_name          text NOT NULL,
    kind                  text NOT NULL,
    assumptions           jsonb NOT NULL DEFAULT '{}'::jsonb,
    description           text NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS cmcc_domains (
    domain_id             text PRIMARY KEY,
    display_name          text NOT NULL,
    role_in_model         text NOT NULL DEFAULT '',
    neighbor_domain_ids   jsonb NOT NULL DEFAULT '[]'::jsonb,
    description           text NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS cmcc_object_types (
    type_id               text PRIMARY KEY,
    parent_type_id        text REFERENCES cmcc_object_types(type_id) DEFERRABLE INITIALLY DEFERRED,
    domain_id             text NOT NULL REFERENCES cmcc_domains(domain_id) DEFERRABLE INITIALLY DEFERRED,
    display_name          text NOT NULL,
    kind                  text NOT NULL,
    description           text NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS cmcc_relation_types (
    relation_type_id       text PRIMARY KEY,
    display_name           text NOT NULL,
    domain_id              text NOT NULL REFERENCES cmcc_domains(domain_id) DEFERRABLE INITIALLY DEFERRED,
    is_dag_edge            boolean NOT NULL DEFAULT false,
    is_symmetric           boolean NOT NULL DEFAULT false,
    is_transitive          boolean NOT NULL DEFAULT false,
    inverse_relation_type_id text REFERENCES cmcc_relation_types(relation_type_id) DEFERRABLE INITIALLY DEFERRED,
    description            text NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS cmcc_relation_type_signatures (
    signature_id          text PRIMARY KEY,
    relation_type_id      text NOT NULL REFERENCES cmcc_relation_types(relation_type_id) DEFERRABLE INITIALLY DEFERRED,
    source_type_id        text NOT NULL REFERENCES cmcc_object_types(type_id) DEFERRABLE INITIALLY DEFERRED,
    target_type_id        text NOT NULL REFERENCES cmcc_object_types(type_id) DEFERRABLE INITIALLY DEFERRED,
    description           text NOT NULL DEFAULT '',
    UNIQUE (relation_type_id, source_type_id, target_type_id)
);

-- ---------------------------------------------------------------------------
-- 2. Bitemporal object identity and versions
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cmcc_math_object_identities (
    object_id             text PRIMARY KEY,
    type_id               text NOT NULL REFERENCES cmcc_object_types(type_id) DEFERRABLE INITIALLY DEFERRED,
    domain_id             text NOT NULL REFERENCES cmcc_domains(domain_id) DEFERRABLE INITIALLY DEFERRED,
    canonical_symbol      text NOT NULL DEFAULT '',
    is_theorem_anchor     boolean NOT NULL DEFAULT false,
    created_at            timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS cmcc_math_object_versions (
    object_version_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    object_id             text NOT NULL REFERENCES cmcc_math_object_identities(object_id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
    context_id            text NOT NULL REFERENCES cmcc_contexts(context_id) DEFERRABLE INITIALLY DEFERRED,
    label                 text NOT NULL,
    description           text NOT NULL DEFAULT '',
    formal_payload        jsonb NOT NULL DEFAULT '{}'::jsonb,
    anchor_role           text NOT NULL DEFAULT '',
    confidence            numeric(5,4) NOT NULL DEFAULT 1.0 CHECK (confidence >= 0 AND confidence <= 1),
    valid_range           tstzrange NOT NULL DEFAULT tstzrange('-infinity'::timestamptz, 'infinity'::timestamptz, '[)'),
    tx_range              tstzrange NOT NULL DEFAULT tstzrange(now(), 'infinity'::timestamptz, '[)'),
    CHECK (NOT isempty(valid_range)),
    CHECK (NOT isempty(tx_range)),
    EXCLUDE USING gist (object_id WITH =, context_id WITH =, tx_range WITH &&)
);

CREATE INDEX IF NOT EXISTS idx_cmcc_object_versions_current
ON cmcc_math_object_versions (object_id, context_id)
WHERE upper(tx_range) = 'infinity'::timestamptz;

-- ---------------------------------------------------------------------------
-- 3. Bitemporal typed relation assertions
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cmcc_relation_assertions (
    assertion_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    relation_key           text NOT NULL,
    source_object_id       text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    relation_type_id       text NOT NULL REFERENCES cmcc_relation_types(relation_type_id) DEFERRABLE INITIALLY DEFERRED,
    target_object_id       text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    context_id             text NOT NULL REFERENCES cmcc_contexts(context_id) DEFERRABLE INITIALLY DEFERRED,
    confidence             numeric(5,4) NOT NULL DEFAULT 1.0 CHECK (confidence >= 0 AND confidence <= 1),
    payload                jsonb NOT NULL DEFAULT '{}'::jsonb,
    valid_range            tstzrange NOT NULL DEFAULT tstzrange('-infinity'::timestamptz, 'infinity'::timestamptz, '[)'),
    tx_range               tstzrange NOT NULL DEFAULT tstzrange(now(), 'infinity'::timestamptz, '[)'),
    description            text NOT NULL DEFAULT '',
    CHECK (source_object_id <> target_object_id OR relation_type_id IN ('REL_IS_A')),
    CHECK (NOT isempty(valid_range)),
    CHECK (NOT isempty(tx_range)),
    EXCLUDE USING gist (relation_key WITH =, tx_range WITH &&)
);

CREATE INDEX IF NOT EXISTS idx_cmcc_rel_source ON cmcc_relation_assertions (source_object_id);
CREATE INDEX IF NOT EXISTS idx_cmcc_rel_target ON cmcc_relation_assertions (target_object_id);
CREATE INDEX IF NOT EXISTS idx_cmcc_rel_type ON cmcc_relation_assertions (relation_type_id);
CREATE INDEX IF NOT EXISTS idx_cmcc_rel_current
ON cmcc_relation_assertions (source_object_id, relation_type_id, target_object_id)
WHERE upper(tx_range) = 'infinity'::timestamptz;

-- ---------------------------------------------------------------------------
-- 4. Type ancestry view: allows signatures to be inherited by subtypes
-- ---------------------------------------------------------------------------

CREATE OR REPLACE VIEW cmcc_type_ancestry AS
WITH RECURSIVE ancestry AS (
    SELECT
        type_id AS child_type_id,
        type_id AS ancestor_type_id,
        0 AS depth
    FROM cmcc_object_types
    UNION ALL
    SELECT
        a.child_type_id,
        t.parent_type_id AS ancestor_type_id,
        a.depth + 1
    FROM ancestry a
    JOIN cmcc_object_types t ON t.type_id = a.ancestor_type_id
    WHERE t.parent_type_id IS NOT NULL
)
SELECT * FROM ancestry;

-- ---------------------------------------------------------------------------
-- 5. Strong relation typing trigger
-- ---------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION cmcc_assert_relation_signature()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    src_type text;
    tgt_type text;
    ok boolean;
BEGIN
    SELECT type_id INTO src_type
    FROM cmcc_math_object_identities
    WHERE object_id = NEW.source_object_id;

    SELECT type_id INTO tgt_type
    FROM cmcc_math_object_identities
    WHERE object_id = NEW.target_object_id;

    SELECT EXISTS (
        SELECT 1
        FROM cmcc_relation_type_signatures sig
        JOIN cmcc_type_ancestry src_a
          ON src_a.child_type_id = src_type
         AND src_a.ancestor_type_id = sig.source_type_id
        JOIN cmcc_type_ancestry tgt_a
          ON tgt_a.child_type_id = tgt_type
         AND tgt_a.ancestor_type_id = sig.target_type_id
        WHERE sig.relation_type_id = NEW.relation_type_id
    ) INTO ok;

    IF NOT ok THEN
        RAISE EXCEPTION
            'Relation type % not allowed for source type % -> target type %',
            NEW.relation_type_id, src_type, tgt_type;
    END IF;

    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_cmcc_relation_signature ON cmcc_relation_assertions;
CREATE CONSTRAINT TRIGGER trg_cmcc_relation_signature
AFTER INSERT OR UPDATE ON cmcc_relation_assertions
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW EXECUTE FUNCTION cmcc_assert_relation_signature();

-- ---------------------------------------------------------------------------
-- 6. DAG-cycle enforcement for the dependency/evaluation subset
--    Semantic edges may be cyclic. Only relation_types.is_dag_edge = true
--    are checked as an acyclic dependency graph.
-- ---------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION cmcc_assert_no_dag_cycle()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    is_dag boolean;
    cycle_exists boolean;
BEGIN
    SELECT is_dag_edge INTO is_dag
    FROM cmcc_relation_types
    WHERE relation_type_id = NEW.relation_type_id;

    IF NOT is_dag THEN
        RETURN NEW;
    END IF;

    WITH RECURSIVE dag_walk(node_id) AS (
        SELECT NEW.target_object_id
        UNION
        SELECT ra.target_object_id
        FROM dag_walk w
        JOIN cmcc_relation_assertions ra
          ON ra.source_object_id = w.node_id
        JOIN cmcc_relation_types rt
          ON rt.relation_type_id = ra.relation_type_id
        WHERE rt.is_dag_edge = true
          AND upper(ra.tx_range) = 'infinity'::timestamptz
    )
    SELECT EXISTS (
        SELECT 1 FROM dag_walk WHERE node_id = NEW.source_object_id
    ) INTO cycle_exists;

    IF cycle_exists THEN
        RAISE EXCEPTION
            'DAG cycle detected by adding % --%--> %',
            NEW.source_object_id, NEW.relation_type_id, NEW.target_object_id;
    END IF;

    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_cmcc_no_dag_cycle ON cmcc_relation_assertions;
CREATE CONSTRAINT TRIGGER trg_cmcc_no_dag_cycle
AFTER INSERT OR UPDATE ON cmcc_relation_assertions
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW EXECUTE FUNCTION cmcc_assert_no_dag_cycle();

-- ---------------------------------------------------------------------------
-- 7. Properties and formulas
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cmcc_property_types (
    property_type_id      text PRIMARY KEY,
    applies_to_type_id    text NOT NULL REFERENCES cmcc_object_types(type_id) DEFERRABLE INITIALLY DEFERRED,
    display_name          text NOT NULL,
    datatype              text NOT NULL CHECK (datatype IN ('number','string','boolean','json','datetime')),
    unit                  text NOT NULL DEFAULT '',
    is_invariant          boolean NOT NULL DEFAULT false,
    description           text NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS cmcc_property_value_assertions (
    value_assertion_id    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    value_key             text NOT NULL,
    object_id             text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    property_type_id      text NOT NULL REFERENCES cmcc_property_types(property_type_id) DEFERRABLE INITIALLY DEFERRED,
    context_id            text NOT NULL REFERENCES cmcc_contexts(context_id) DEFERRABLE INITIALLY DEFERRED,
    value_number          numeric,
    value_string          text,
    value_boolean         boolean,
    value_json            jsonb,
    confidence            numeric(5,4) NOT NULL DEFAULT 1.0 CHECK (confidence >= 0 AND confidence <= 1),
    valid_range           tstzrange NOT NULL DEFAULT tstzrange('-infinity'::timestamptz, 'infinity'::timestamptz, '[)'),
    tx_range              tstzrange NOT NULL DEFAULT tstzrange(now(), 'infinity'::timestamptz, '[)'),
    description           text NOT NULL DEFAULT '',
    EXCLUDE USING gist (value_key WITH =, tx_range WITH &&)
);

CREATE TABLE IF NOT EXISTS cmcc_formula_definitions (
    formula_id             text PRIMARY KEY,
    owner_object_id        text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    formula_kind           text NOT NULL CHECK (formula_kind IN ('lookup','calculated','aggregation','predicate','route_metric')),
    output_property_type_id text REFERENCES cmcc_property_types(property_type_id) DEFERRABLE INITIALLY DEFERRED,
    formula                text NOT NULL,
    depends_on_object_ids  jsonb NOT NULL DEFAULT '[]'::jsonb,
    description            text NOT NULL DEFAULT ''
);

-- ---------------------------------------------------------------------------
-- 8. Semantic routes
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS cmcc_semantic_routes (
    route_id              text PRIMARY KEY,
    start_object_id       text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    end_object_id         text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    route_kind            text NOT NULL,
    status                text NOT NULL,
    description           text NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS cmcc_semantic_route_steps (
    step_id               text PRIMARY KEY,
    route_id              text NOT NULL REFERENCES cmcc_semantic_routes(route_id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
    step_order            integer NOT NULL CHECK (step_order > 0),
    from_object_id        text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    relation_type_id      text NOT NULL REFERENCES cmcc_relation_types(relation_type_id) DEFERRABLE INITIALLY DEFERRED,
    to_object_id          text NOT NULL REFERENCES cmcc_math_object_identities(object_id) DEFERRABLE INITIALLY DEFERRED,
    gap_status            text NOT NULL DEFAULT 'unknown',
    bridge_role           text NOT NULL DEFAULT '',
    description           text NOT NULL DEFAULT '',
    UNIQUE (route_id, step_order)
);

-- ---------------------------------------------------------------------------
-- 9. Useful views
-- ---------------------------------------------------------------------------

CREATE OR REPLACE VIEW cmcc_current_objects AS
SELECT
    oi.object_id,
    oi.type_id,
    ot.display_name AS type_name,
    oi.domain_id,
    d.display_name AS domain_name,
    ov.context_id,
    ov.label,
    oi.canonical_symbol,
    ov.description,
    ov.formal_payload,
    ov.anchor_role,
    oi.is_theorem_anchor,
    ov.confidence
FROM cmcc_math_object_identities oi
JOIN cmcc_object_types ot ON ot.type_id = oi.type_id
JOIN cmcc_domains d ON d.domain_id = oi.domain_id
JOIN cmcc_math_object_versions ov ON ov.object_id = oi.object_id
WHERE upper(ov.tx_range) = 'infinity'::timestamptz;

CREATE OR REPLACE VIEW cmcc_current_relations AS
SELECT
    ra.assertion_id,
    ra.relation_key,
    ra.source_object_id,
    so.label AS source_label,
    ra.relation_type_id,
    rt.display_name AS relation_name,
    rt.is_dag_edge,
    ra.target_object_id,
    tobj.label AS target_label,
    ra.context_id,
    ra.confidence,
    ra.payload,
    ra.description
FROM cmcc_relation_assertions ra
JOIN cmcc_relation_types rt ON rt.relation_type_id = ra.relation_type_id
LEFT JOIN cmcc_current_objects so ON so.object_id = ra.source_object_id
LEFT JOIN cmcc_current_objects tobj ON tobj.object_id = ra.target_object_id
WHERE upper(ra.tx_range) = 'infinity'::timestamptz;

-- Shortest-path sketch from any object to a theorem anchor.
CREATE OR REPLACE VIEW cmcc_paths_to_theorem_anchors AS
WITH RECURSIVE walk AS (
    SELECT
        ra.source_object_id AS start_object_id,
        ra.target_object_id AS node_id,
        ARRAY[ra.source_object_id, ra.target_object_id] AS path,
        1 AS depth
    FROM cmcc_current_relations ra
    WHERE ra.is_dag_edge OR ra.relation_type_id = 'REL_SEMANTIC_BRIDGE_TO'

    UNION ALL

    SELECT
        walk.start_object_id,
        ra.target_object_id AS node_id,
        walk.path || ra.target_object_id,
        walk.depth + 1
    FROM walk
    JOIN cmcc_current_relations ra
      ON ra.source_object_id = walk.node_id
    WHERE (ra.is_dag_edge OR ra.relation_type_id = 'REL_SEMANTIC_BRIDGE_TO')
      AND NOT ra.target_object_id = ANY(walk.path)
      AND walk.depth < 12
)
SELECT
    walk.start_object_id,
    walk.node_id AS anchor_object_id,
    walk.path,
    walk.depth
FROM walk
JOIN cmcc_math_object_identities oi ON oi.object_id = walk.node_id
WHERE oi.is_theorem_anchor = true;

COMMIT;
