"""
ERB Calculation Library (GENERATED - DO NOT EDIT)
=================================================
Generated from: effortless-rulebook/acme-llc-rulebook.json

PYTHON SUBSTRATE ONLY. Importing this module from any other substrate
is cheating. That substrate must execute the rulebook in its own native
semantics (its $ENGINE). The whole point of ERB conformance is that each
substrate computes calculated, lookup, and aggregation fields by
interpreting/compiling the rulebook itself — not by calling out to a
Python simulator. If a substrate cannot natively compute a field, it must
leave it null and accept the 0 score for that field.

This file contains:
  - Generated calc_* functions for calculated (scalar) fields
  - Generated compute_*_fields(record) dispatchers per entity
  - The compute_all_calculated_fields(record, entity_name) entry point
  - Hand-written compute_lookups() and compute_aggregations() — the
    INDEX/MATCH and COUNTIFS/SUMIFS interpreters. These used to live in
    orchestration/shared.py where any substrate could import them, which
    let 7 substrates report 100% without executing anything native. They
    now live inside the Python-only fence by design.
"""

import json
import re
from pathlib import Path
from typing import Optional, Any

from orchestration import formula_parser as _erb

from orchestration.shared import (
    to_snake_case,
    get_entity_schema,
    get_lookup_fields,
    get_aggregation_fields,
    is_index_match_formula,
    is_lookup_call_formula,
    is_cross_table_lookup_formula,
    discover_primary_key,
)


# =============================================================================
# CUSTOMERS CALCULATIONS
# Table: Customers
# =============================================================================

# Level 1

def calc_customers_name(email_address):
    """
    Identifier for the customer.
    
    Formula: =SUBSTITUTE({{EmailAddress}}, "@", "-")
    """
    return ((email_address or "").replace('@', '-'))

def calc_customers_full_name(first_name, last_name):
    """
    Full name is computed from the first and last name of the customer
    
    Formula: ={{FirstName}} & " " & {{LastName}}
    """
    return (str(first_name or "") + ' ' + str(last_name or ""))


def compute_customers_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Customers.
    
    Table: Customers
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_customers_name(result.get('email_address'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['full_name'] = calc_customers_full_name(result.get('first_name'), result.get('last_name'))
    except Exception as _field_exc:
        result['full_name'] = None
        result.setdefault('_erb_errors', {})['full_name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'full_name']:
        if result.get(key) == '':
            result[key] = None

    return result


# =============================================================================
# DISPATCHER FUNCTION
# =============================================================================

def compute_all_calculated_fields(record: dict, entity_name: str = None) -> dict:
    """
    Compute all calculated fields for a record.
    
    This is the main entry point for computing calculated fields.
    It routes to the appropriate entity-specific compute function.
    
    Args:
        record: The record dict with raw field values
        entity_name: Entity name (snake_case or PascalCase)
    
    Returns:
        Record dict with calculated fields filled in
    """
    if entity_name is None:
        # No entity specified - return record unchanged
        return dict(record)

    # Normalize to snake_case to support "LineItem", "line_item", "line-item"
    entity_lower = to_snake_case(entity_name.replace('-', '_'))

    if entity_lower == 'customers':
        return compute_customers_fields(record)
    else:
        raise KeyError(f"compute_all_calculated_fields called with unknown entity {entity_name!r}. "f"Known entities in this generated erb_calc.py: ['customers']. "f"Check that the rulebook used to generate this file matches the data being computed.")

# =============================================================================
# INDEX/MATCH LOOKUP INTERPRETER (PYTHON SIMULATOR — DO NOT CALL FROM OTHER SUBSTRATES)
# =============================================================================


def parse_index_match_formula(formula: str) -> tuple:
    """
    Parse an INDEX/MATCH formula to extract the lookup components.

    Formula format: =INDEX(Table!{{FieldToReturn}}, MATCH(CurrentTable!{{KeyField}}, Table!{{PrimaryKeyField}}, 0))
    Returns: (lookup_table, return_field, key_field, pk_field) or all None.
    """
    # The MATCH key is a field on the record being computed, so the rulebook
    # writes it bare ({{WorkflowStep}}); an explicit table prefix
    # (ApprovalGates!{{WorkflowStep}}) means the same thing. Both spellings
    # must parse — requiring the prefix silently nulled every bare lookup.
    pattern = (
        r"=\s*INDEX\(\s*(\w+)!\{\{(\w+)\}\}\s*,"
        r"\s*MATCH\(\s*(?:\w+!)?\{\{(\w+)\}\}\s*,"
        r"\s*(\w+)!\{\{(\w+)\}\}\s*,\s*0\s*\)\s*\)"
    )
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3), match.group(5))
    return (None, None, None, None)


def parse_lookup_formula(formula: str) -> tuple:
    """
    Parse an Airtable-style LOOKUP formula — the second cross-table lookup
    dialect this rulebook format uses alongside INDEX/MATCH.

    Formula format: =LOOKUP(Table!{{FieldToReturn}}, CurrentTable!{{FkField}}, Table!{{PrimaryKeyField}})
    Returns: (lookup_table, return_field, key_field) or all None. Unlike
    INDEX/MATCH, the third argument (the other table's PK column) is not
    parsed out — the caller resolves the other table's actual primary key via
    discover_primary_key() instead, since that argument is often just an
    echo of the schema's own PK convention rather than new information.
    """
    pattern = (
        r"=\s*LOOKUP\(\s*(\w+)!\{\{(\w+)\}\}\s*,"
        r"\s*(?:\w+!)?\{\{(\w+)\}\}"
    )
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3))
    return (None, None, None)


def parse_countifs_formula(formula: str) -> tuple:
    """Parse =COUNTIFS(RelatedTable!{{LookupField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=COUNTIFS\((\w+)!\{\{(\w+)\}\},\s*\w+!\{\{(\w+)\}\}\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3))
    return (None, None, None)


def parse_countifs(formula: str) -> tuple:
    """Parse any COUNTIF/COUNTIFS into (table, [(range_field, criteria), ...]).

    COUNTIFS is variadic — (range, criteria) repeated — so it is parsed
    structurally rather than with one regex per shape. COUNTIF (no S) is
    Excel's single-pair predecessor, `=COUNTIF(range, criteria)` — exactly
    one (range, criteria) pair, which is already a valid special case of the
    same grammar, so one parser covers both spellings. Each criteria is
    ('field', Name) to compare against the current record, or ('literal', v).
    All ranges must name the same table, since COUNTIF(S) counts rows of one
    table. Returns (None, None) when the formula is neither.
    """
    match = re.match(r"\s*=\s*COUNTIFS?\s*\((.*)\)\s*$", formula, re.S)
    if not match:
        return (None, None)

    args = [a.strip() for a in _split_top_level_args(match.group(1))]
    if len(args) < 2 or len(args) % 2 != 0:
        raise ValueError(
            f"COUNTIF(S) takes (range, criteria) pairs, got {len(args)} argument(s): {formula}")

    table = None
    criteria = []
    for range_arg, criteria_arg in zip(args[0::2], args[1::2]):
        range_match = re.match(r"^(\w+)!\{\{(\w+)\}\}$", range_arg)
        if not range_match:
            raise ValueError(f"COUNTIFS range must be Table!{{{{Field}}}}, got {range_arg!r}")
        range_table, range_field = range_match.groups()
        if table is None:
            table = range_table
        elif range_table != table:
            raise ValueError(
                f"COUNTIFS ranges must all name the same table; "
                f"got {table!r} and {range_table!r}")
        criteria.append((range_field, _parse_countifs_criteria(criteria_arg)))

    return (table, criteria)


def _split_top_level_args(text: str) -> list:
    """Split on commas that are not inside parentheses or quotes."""
    args = []
    depth = 0
    quote = None
    current = ''
    for char in text:
        if quote:
            current += char
            if char == quote:
                quote = None
            continue
        if char in '"\'':
            quote = char
            current += char
        elif char == '(':
            depth += 1
            current += char
        elif char == ')':
            depth -= 1
            current += char
        elif char == ',' and depth == 0:
            args.append(current)
            current = ''
        else:
            current += char
    if current.strip():
        args.append(current)
    return args


def _parse_countifs_criteria(arg: str):
    """Classify one COUNTIFS/SUMIFS-family criteria argument: a same-record
    {{Field}} ref, TRUE()/FALSE(), a number, or a quoted string — which may
    ITSELF be a comparison operator (">500", "<=10", "<>0"), Excel's standard
    COUNTIFS criteria syntax for "match a range instead of an exact value".
    """
    field_match = re.match(r"^(?:\w+!)?\{\{(\w+)\}\}$", arg)
    if field_match:
        return ('field', field_match.group(1))
    # TRUE and FALSE appear with or without call parentheses.
    bare = arg.upper().replace('()', '').strip()
    if bare == 'TRUE':
        return ('literal', True)
    if bare == 'FALSE':
        return ('literal', False)
    if len(arg) >= 2 and arg[0] == arg[-1] and arg[0] in '"\'':
        inner = arg[1:-1]
        op_match = re.match(r"^\s*(<=|>=|<>|<|>|=)\s*(.+)$", inner)
        if op_match:
            op, rhs = op_match.groups()
            try:
                rhs_value = float(rhs) if '.' in rhs else int(rhs)
            except ValueError:
                rhs_value = rhs
            return ('op', (op, rhs_value))
        return ('literal', inner)
    try:
        return ('literal', int(arg))
    except ValueError:
        pass
    try:
        return ('literal', float(arg))
    except ValueError:
        pass
    raise ValueError(f"Unrecognized COUNTIFS criteria: {arg!r}")


def _erb_compare(cell_value, op: str, target) -> bool:
    """Evaluate one Excel-style comparison-operator criteria (">500" etc.)
    against a cell value. Numeric-only (every real usage compares a number);
    a blank/non-numeric cell never satisfies an inequality.
    """
    if cell_value is None or cell_value == "":
        return False
    try:
        cell_num = float(cell_value)
        target_num = float(target)
    except (TypeError, ValueError):
        return False
    if op == '>':
        return cell_num > target_num
    if op == '>=':
        return cell_num >= target_num
    if op == '<':
        return cell_num < target_num
    if op == '<=':
        return cell_num <= target_num
    if op == '<>':
        return cell_num != target_num
    return cell_num == target_num  # '='


def _row_matches_criteria(row: dict, criteria: list, record: dict) -> bool:
    """True if `row` satisfies every (range_field, (kind, value)) pair, where
    kind is 'field' (compare to the CURRENT record's same-named field),
    'literal' (exact match), or 'op' (a comparison operator — see
    _erb_compare). Shared by the plain-count path (count_matching_rows) and
    compute_aggregations' SUM/AVERAGE/MIN/MAX-family matching, so the two
    interpreters agree on criteria semantics.
    """
    for range_field, (kind, value) in criteria:
        cell = row.get(to_snake_case(range_field))
        if kind == 'field':
            # A calculated text key the oracle computes as '' (the composite-key
            # echo idiom's "no key") matches another '' — but this substrate
            # stores a blank calculated string as None, so both spellings of
            # blank are one value here.
            if ('' if cell is None else cell) != ('' if record.get(to_snake_case(value)) is None else record.get(to_snake_case(value))):
                return False
        elif kind == 'op':
            op, target = value
            if not _erb_compare(cell, op, target):
                return False
        else:  # literal
            if cell != value:
                return False
    return True


def count_matching_rows(rows: list, criteria: list, record: dict) -> int:
    """Count rows satisfying every (range_field, criteria) pair."""
    return sum(1 for row in rows if _row_matches_criteria(row, criteria, record))


def parse_countifs_literal_formula(formula: str) -> tuple:
    """Parse =COUNTIFS(Table!{{Field}}, TRUE()) / FALSE().

    Distinct from parse_countifs_formula, whose second argument is another
    table's field rather than a literal. Returns (table, field, bool).
    """
    pattern = r"=COUNTIFS\((\w+)!\{\{(\w+)\}\},\s*(TRUE|FALSE)\(\)\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3) == 'TRUE')
    return (None, None, None)


def count_closure_rows(closure_rows: list, column: str, expected) -> int:
    """Count materialized closure rows whose column equals expected."""
    return sum(1 for row in closure_rows if row.get(column) == expected)


def parse_ifs_aggregate(formula: str) -> tuple:
    """Parse the numeric rollup shapes that reach compute_aggregations:

      =SUM(Table!{{Field}})                                    -- whole table, no criteria
      =AVERAGE(Table!{{Field}})
      =COUNT(Table!{{Field}})
      =MIN(Table!{{Field}})
      =MAX(Table!{{Field}})
      =SUMIFS(Table!{{Field}}, Table!{{Crit}}, {{Match}}, ...)  -- (range, criteria) pairs
      =AVERAGEIFS(Table!{{Field}}, Table!{{Crit}}, {{Match}}, ...)
      =MINIFS(Table!{{Field}}, Table!{{Crit}}, {{Match}}, ...)
      =MAXIFS(Table!{{Field}}, Table!{{Crit}}, {{Match}}, ...)

    (Plain COUNTIFS is handled separately by parse_countifs() above — it needs no
    target field, only criteria; it is also variadic there already.) COUNTIFS'
    criteria grammar is reused here via _parse_countifs_criteria/_split_top_level_args
    so a criteria arg may be a bare or table-prefixed {{Field}} ref, TRUE()/FALSE(),
    a quoted string, or a number.

    Returns (op, table, field, criteria, suffix): op is one of 'SUM', 'AVERAGE',
    'COUNT', 'MIN', 'MAX'; field is the column to aggregate (present even for
    COUNT, though COUNT ignores it — counting only needs matching rows); criteria
    is a list of (range_field, (kind, value)) pairs, empty for the five bare
    forms. `suffix` is a trailing arithmetic expression on the target range, e.g.
    the "count without COUNTIFS" idiom `Table!{{AnyField}}*0+1` (always 1 for any
    matching row, since `x*0+1` is 1 regardless of x — some rulebooks spell a
    per-row count this way inside SUMIFS instead of using COUNTIFS); '' when the
    target range is a bare `Table!{{Field}}`. Returns (None, None, None, None,
    None) when formula matches none of these nine shapes.
    """
    bare = re.match(
        r"\s*=\s*(SUM|AVERAGE|COUNT|MIN|MAX)\s*\(\s*(\w+)!\{\{(\w+)\}\}\s*\)\s*$",
        formula)
    if bare:
        op, table, field = bare.groups()
        return (op, table, field, [], '')

    ifs = re.match(r"\s*=\s*(SUMIFS|AVERAGEIFS|MINIFS|MAXIFS)\s*\((.*)\)\s*$", formula, re.S)
    if not ifs:
        return (None, None, None, None, None)

    op_raw, rest = ifs.groups()
    op = {'SUMIFS': 'SUM', 'AVERAGEIFS': 'AVERAGE', 'MINIFS': 'MIN', 'MAXIFS': 'MAX'}[op_raw]
    args = [a.strip() for a in _split_top_level_args(rest)]
    if len(args) < 3 or len(args) % 2 != 1:
        raise ValueError(
            f"{op_raw} takes (target_range, range1, criteria1, ...) -- an odd "
            f"argument count >= 3, got {len(args)}: {formula}")

    target_match = re.match(r"^(\w+)!\{\{(\w+)\}\}(.*)$", args[0])
    if not target_match:
        raise ValueError(f"{op_raw} target range must be Table!{{{{Field}}}}, got {args[0]!r}")
    table, field, suffix = target_match.groups()
    suffix = suffix.strip()
    if suffix and op != 'SUM':
        raise ValueError(
            f"{op_raw}: an arithmetic suffix on the target range ({suffix!r}) is "
            f"only supported for SUMIFS (the 'count without COUNTIFS' idiom), got {op_raw}")

    criteria = []
    for range_arg, criteria_arg in zip(args[1::2], args[2::2]):
        range_match = re.match(r"^(\w+)!\{\{(\w+)\}\}$", range_arg)
        if not range_match:
            raise ValueError(f"{op_raw} criteria range must be Table!{{{{Field}}}}, got {range_arg!r}")
        range_table, range_field = range_match.groups()
        if range_table != table:
            raise ValueError(
                f"{op_raw} criteria ranges must reference the same table as the "
                f"target range ({table!r}); got {range_table!r}: {formula}")
        criteria.append((range_field, _parse_countifs_criteria(criteria_arg)))

    return (op, table, field, criteria, suffix)


def _get_testing_dir(project_root: Path) -> Path:
    """Return the active domain's testing/ dir. ERB_TESTING_DIR is required.

    The injector runs at build time and must operate on the same domain the
    orchestrator chose. There is no implicit per-substrate testing dir.
    """
    import os
    erb = os.environ.get("ERB_TESTING_DIR")
    if not erb:
        raise RuntimeError(
            "ERB_TESTING_DIR is not set. inject-into-python.py must be invoked "
            "by the orchestrator with ERB_TESTING_DIR pointing at the active "
            "domain's testing/ directory."
        )
    return Path(erb)


def load_related_data(project_root: Path, related_table: str) -> list:
    """
    Load data from testing/answer-keys for a related table; falls back to blank-tests.
    Prefers answer-keys so that aggregations referencing computed fields in related
    tables resolve correctly.
    """
    snake_name = to_snake_case(related_table)
    testing_dir = _get_testing_dir(project_root)

    answer_keys_path = testing_dir / "answer-keys" / f"{snake_name}.json"
    if answer_keys_path.exists():
        with open(answer_keys_path, "r", encoding="utf-8") as f:
            return json.load(f)

    blank_tests_path = testing_dir / "blank-tests" / f"{snake_name}.json"
    if blank_tests_path.exists():
        with open(blank_tests_path, "r", encoding="utf-8") as f:
            return json.load(f)

    return []


def compute_lookups(records: list, entity_name: str, rulebook: dict, project_root: Path) -> list:
    """INDEX/MATCH and Airtable-style LOOKUP() interpreter. PYTHON SIMULATOR ONLY."""
    schema = get_entity_schema(rulebook, entity_name)
    lookup_fields = get_lookup_fields(schema)

    if not lookup_fields:
        return records

    related_data_cache = {}

    for field in lookup_fields:
        field_name = field.get("name")
        formula = field.get("formula", "")
        snake_field_name = to_snake_case(field_name)

        if is_index_match_formula(formula):
            lookup_table, return_field, key_field, pk_field = parse_index_match_formula(formula)
        else:
            # LOOKUP(Other!Field, This!Fk, Other!Pk): the PK column isn't
            # parsed from the formula (see parse_lookup_formula) — the other
            # table's actual primary key is resolved from its own schema.
            lookup_table, return_field, key_field = parse_lookup_formula(formula)
            pk_field = discover_primary_key(rulebook, lookup_table) if lookup_table else None

        if not lookup_table:
            continue

        if lookup_table not in related_data_cache:
            related_data_cache[lookup_table] = load_related_data(project_root, lookup_table)

        related_records = related_data_cache[lookup_table]
        snake_return_field = to_snake_case(return_field)
        snake_key_field = to_snake_case(key_field)
        snake_pk_field = to_snake_case(pk_field)

        lookup_map = {}
        for related_record in related_records:
            pk_value = related_record.get(snake_pk_field)
            if pk_value is not None:
                lookup_map[pk_value] = related_record.get(snake_return_field)

        # What a JOIN with nothing to match against should render — see
        # _resolve_blank_join_value: it is NOT always None. Computed once per
        # field (it doesn't depend on `record`), not per row.
        no_match_value = _resolve_blank_join_value(rulebook, lookup_table, return_field)

        for record in records:
            key_value = record.get(snake_key_field)
            if key_value is not None and key_value in lookup_map:
                record[snake_field_name] = lookup_map[key_value]
            else:
                record[snake_field_name] = no_match_value

    return records


def _resolve_blank_join_value(rulebook: dict, table_name: str, field_name: str):
    """What INDEX/MATCH should render when nothing matches — mirrors what a SQL
    LEFT JOIN produces, which is NOT uniformly None/NULL.

    A LEFT JOIN with no matching row still yields a row — all-NULL — and the
    joined table's OWN column is evaluated normally against it, the same as
    any other row. A raw column stays NULL (nothing to compute). But a
    CALCULATED column's formula still runs: one built from literal text
    fragments (e.g. `{{Season}} & "-Episode-" & ...`) can produce a real,
    non-null string even when every field it reads is NULL, because the
    literals survive regardless. Collapsing every unmatched lookup straight
    to None — as this interpreter used to — is only correct for the raw-field
    case; it silently drops the literal-survival case, which the frozen
    Postgres oracle exhibits (e.g. star-trek's Ratings.EpisodeName on a
    rating with no Episode: Episodes.Name's formula still emits "-Episode-").

    Recurses when the target field is itself a cross-table lookup (either
    dialect): its own key would come from the same nonexistent blank row, so
    it can never match either, and the question becomes whether ITS target
    survives blank inputs.
    """
    schema = get_entity_schema(rulebook, table_name)
    target = next((f for f in schema if f.get("name") == field_name), None)
    formula = target.get("formula", "") if target else ""
    if not formula:
        return None

    if is_index_match_formula(formula):
        nested_table, nested_field, _key, _pk = parse_index_match_formula(formula)
        if not nested_table:
            return None
        return _resolve_blank_join_value(rulebook, nested_table, nested_field)

    if is_lookup_call_formula(formula):
        nested_table, nested_field, _key = parse_lookup_formula(formula)
        if not nested_table:
            return None
        return _resolve_blank_join_value(rulebook, nested_table, nested_field)

    # A same-record scalar formula (calculated, or a "lookup"-labeled field
    # that isn't actually cross-table shaped — see is_cross_table_lookup_formula):
    # evaluate it against an all-blank record for this table, exactly as
    # Postgres evaluates the joined row's own calculated column against an
    # all-NULL row rather than skipping it. Fields this table computes via
    # aggregation aren't part of compute_all_calculated_fields's DAG, so they
    # pass through unchanged (None, the blank seed) — a reasonable rendering
    # for "aggregated over a row that doesn't exist."
    blank_record = {to_snake_case(f["name"]): None for f in schema}
    try:
        computed = compute_all_calculated_fields(blank_record, table_name)
    except Exception:
        return None
    return computed.get(to_snake_case(field_name))


# =============================================================================
# TRANSITIVE CLOSURE ENGINE (PYTHON SIMULATOR — DO NOT CALL FROM OTHER SUBSTRATES)
# =============================================================================


def compute_closure_relation(rows: list, to_field: str,
                             pk_field: str = None, from_field: str = None) -> list:
    """Cycle-safe transitive closure, matching Postgres vw_<entity>_closure.

    Two edge shapes: pass from_field for an edge/junction table, or pk_field
    for a self-referential FK on the entity's own rows.

    Returns dicts of from_id, to_id, hop_distance (shortest derivation) and
    is_inferred (TRUE iff no directly-asserted hop-1 edge states the pair).
    A NULL or empty-string endpoint is not an edge — the transpiler stores
    absent relationships as '' rather than NULL, so both must be excluded.
    """
    source_field = from_field or pk_field
    if source_field is None:
        raise ValueError("compute_closure_relation requires from_field or pk_field")

    edges = []
    for row in rows:
        src = row.get(source_field)
        dst = row.get(to_field)
        if src is None or src == '' or dst is None or dst == '':
            continue
        edges.append((src, dst))

    if not edges:
        return []

    asserted = set(edges)
    adjacency = {}
    for src, dst in edges:
        adjacency.setdefault(src, []).append(dst)

    shortest = {}
    for origin in {src for src, _ in edges}:
        # BFS keeps the first arrival shortest; the path set makes it cycle-safe.
        frontier = [(origin, (origin,))]
        hop = 0
        while frontier:
            hop += 1
            next_frontier = []
            for node, path in frontier:
                for neighbor in adjacency.get(node, []):
                    pair = (origin, neighbor)
                    if pair not in shortest:
                        shortest[pair] = hop
                    if neighbor not in path:
                        next_frontier.append((neighbor, path + (neighbor,)))
            frontier = next_frontier

    return [
        {
            'from_id': from_id,
            'to_id': to_id,
            'hop_distance': hop_distance,
            'is_inferred': (from_id, to_id) not in asserted,
        }
        for (from_id, to_id), hop_distance in sorted(shortest.items())
    ]


def compute_closures(rulebook: dict, project_root: Path) -> dict:
    """Materialize every closure field in the rulebook as a pseudo-table.

    Aggregations address these by view name, e.g.
    =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE()) — so the
    result is keyed by vw_<entity>_closure and joins the related-data lookup
    path alongside real tables.
    """
    from orchestration.shared import (
        discover_entities,
        discover_primary_key,
        get_closure_fields,
        closure_view_name,
    )

    materialized = {}

    for entity_name in discover_entities(rulebook):
        schema = get_entity_schema(rulebook, entity_name)
        for field in get_closure_fields(schema):
            edge_table = field.get('EdgeTable')
            to_column = field.get('ToColumn')
            if not to_column:
                continue

            to_field = to_snake_case(to_column)

            if edge_table and field.get('FromColumn'):
                source_entity = edge_table
                source_rows = load_related_data(project_root, edge_table)
                kwargs = {'from_field': to_snake_case(field['FromColumn'])}
            else:
                source_entity = entity_name
                source_rows = load_related_data(project_root, entity_name)
                kwargs = {'pk_field': to_snake_case(
                    discover_primary_key(rulebook, entity_name))}

            materialized[closure_view_name(source_entity)] = compute_closure_relation(
                source_rows, to_field=to_field, **kwargs)

    return materialized


_AGGREGATE_CALL_NAMES = ('COUNTIFS', 'COUNTIF', 'SUMIFS', 'AVERAGEIFS', 'MINIFS', 'MAXIFS')


def _split_aggregate_calls(formula: str) -> tuple:
    """Lift each aggregate call out of a scalar formula.

    Returns the formula with every COUNTIFS/SUMIFS/AVERAGEIFS/MINIFS/MAXIFS call
    replaced by an {{ErbAggregateN}} placeholder, and the calls themselves as
    standalone `=CALL(...)` formulas. Quoted text is left alone.
    """
    body = formula.strip()
    if body.startswith('='):
        body = body[1:]
    scalar = ''
    calls = []
    i = 0
    quote = None
    while i < len(body):
        ch = body[i]
        if quote:
            scalar += ch
            if ch == quote:
                quote = None
            i += 1
            continue
        if ch == '"' or ch == "'":
            quote = ch
            scalar += ch
            i += 1
            continue
        name = next((n for n in _AGGREGATE_CALL_NAMES
                     if body[i:i + len(n) + 1].upper() == n + '('
                     and (i == 0 or not (body[i - 1].isalnum() or body[i - 1] == '_'))), None)
        if name is None:
            scalar += ch
            i += 1
            continue
        depth = 0
        inner_quote = None
        end = None
        for k in range(i + len(name), len(body)):
            c = body[k]
            if inner_quote:
                if c == inner_quote:
                    inner_quote = None
            elif c == '"' or c == "'":
                inner_quote = c
            elif c == '(':
                depth += 1
            elif c == ')':
                depth -= 1
                if depth == 0:
                    end = k
                    break
        if end is None:
            raise ValueError(f"unbalanced parentheses in the {name} call of {formula!r}")
        scalar += '{{ErbAggregate%d}}' % len(calls)
        calls.append('=' + body[i:end + 1])
        i = end + 1
    return '=' + scalar, calls


def _aggregate_datatype(rulebook: dict, call: str) -> str:
    """MINIFS/MAXIFS return the aggregated column's datatype; the rest count or sum."""
    op, table, field, _criteria, _suffix = parse_ifs_aggregate(call)
    if op in ('MIN', 'MAX'):
        target = next(f for f in get_entity_schema(rulebook, table) if f.get('name') == field)
        return target.get('datatype', 'string')
    return 'number'


def compute_composite_aggregation(records: list, entity_name: str, field: dict, rulebook: dict,
                                  project_root: Path, closures: dict) -> None:
    """A scalar formula wrapped around aggregates, e.g.
    =DATETIME_DIFF({{AsOfInstant}}, MAXIFS(ReviewEvents!{{ReviewedAt}}, ...), "days").

    Each aggregate is computed into a placeholder column by compute_aggregations
    itself, then the scalar formula is compiled against the table's schema plus
    those placeholders — the same compiler every calculated field goes through,
    so the two cannot disagree about DATETIME_DIFF, NULLs or integer casts.
    """
    formula = field.get('formula', '')
    scalar_formula, calls = _split_aggregate_calls(formula)
    if not calls:
        raise ValueError(f"aggregation formula {formula!r} has no aggregate call this interpreter can compute")
    table_key = next(key for key in rulebook if to_snake_case(key) == to_snake_case(entity_name))
    placeholders = [
        {'name': f'ErbAggregate{index}', 'type': 'aggregation', 'formula': call,
         'datatype': _aggregate_datatype(rulebook, call)}
        for index, call in enumerate(calls)
    ]
    compute_aggregations(records, entity_name, {table_key: {'schema': placeholders}}, project_root, closures)

    _erb.set_compile_fields(list(get_entity_schema(rulebook, entity_name)) + placeholders)
    expr = _erb.parse_formula(scalar_formula)
    code = _erb.compile_to_python(expr)
    if field.get('datatype') == 'integer':
        code = f'_erb.erb_integer({code})'
    compiled = compile(code, f"<{entity_name}.{field.get('name')}>", 'eval')
    dependencies = [to_snake_case(d) for d in _erb.get_field_dependencies(expr)]
    for record in records:
        scope = {dependency: record.get(dependency) for dependency in dependencies}
        record[to_snake_case(field['name'])] = eval(compiled, {'_erb': _erb}, scope)
        for placeholder in placeholders:
            record.pop(to_snake_case(placeholder['name']), None)


def compute_aggregations(records: list, entity_name: str, rulebook: dict, project_root: Path,
                         closures: dict = None) -> list:
    """COUNTIFS / SUMIFS aggregation interpreter. PYTHON SIMULATOR ONLY."""
    schema = get_entity_schema(rulebook, entity_name)
    agg_fields = get_aggregation_fields(schema)

    if not agg_fields:
        return records

    related_data_cache = {}
    closures = closures or {}

    for field in agg_fields:
        field_name = field.get("name")
        formula = field.get("formula", "")
        snake_field_name = to_snake_case(field_name)

        # Every field gets its OWN try/except: one aggregation formula this
        # interpreter can't parse must not take down every OTHER aggregation
        # field on the same entity (they're independent formulas; a schema-
        # level typo in field N has nothing to do with field N+1's result).
        # The failure is never swallowed silently — it's stashed on
        # records[i]['_erb_errors'] the same way a scalar calc_* failure is
        # (see generate_entity_compute_function), which the harness surfaces
        # as a diagnostic rather than a false pass.
        try:
            # COUNTIFS handles any number of (range, criteria) pairs over one
            # table, which may be an ordinary table or a materialized closure.
            countifs_table, countifs_criteria = parse_countifs(formula)
            if countifs_table:
                if countifs_table in closures:
                    target_rows = closures[countifs_table]
                else:
                    if countifs_table not in related_data_cache:
                        related_data_cache[countifs_table] = load_related_data(
                            project_root, countifs_table)
                    target_rows = related_data_cache[countifs_table]
                for record in records:
                    record[snake_field_name] = count_matching_rows(
                        target_rows, countifs_criteria, record)
                continue

            # NOTE: parse_countifs() already subsumes the old 2-arg
            # `=COUNTIFS(Table!{{Field}}, CurrentTable!{{Match}})` shape (its
            # criteria parser accepts a bare or table-prefixed {{Field}} ref),
            # so there is no separate plain-COUNTIFS case to handle here.

            agg_op, agg_table, agg_field, agg_criteria, agg_suffix = parse_ifs_aggregate(formula)
            if agg_op is None:
                # Not a bare aggregate: a scalar formula around one or more of
                # them. A formula with no aggregate this interpreter knows raises
                # there, and the error lands on _erb_errors below.
                compute_composite_aggregation(records, entity_name, field, rulebook,
                                              project_root, closures)
                continue

            if agg_table in closures:
                target_rows = closures[agg_table]
            else:
                if agg_table not in related_data_cache:
                    related_data_cache[agg_table] = load_related_data(project_root, agg_table)
                target_rows = related_data_cache[agg_table]

            snake_target_field = to_snake_case(agg_field)

            for record in records:
                matching = [row for row in target_rows
                            if _row_matches_criteria(row, agg_criteria, record)]

                if agg_op == 'COUNT':
                    record[snake_field_name] = len(matching)
                    continue

                if agg_op in ('MIN', 'MAX'):
                    # MIN/MAX are not numeric-only in this dialect — MAXIFS over
                    # a text column (e.g. a "SessionLabel" date-as-string) is a
                    # real, legitimate rollup ("latest label"), and Python's
                    # min()/max() already do the right thing on strings
                    # (lexicographic, which agrees with chronological order for
                    # ISO-8601 date/datetime strings) as well as numbers. Only
                    # None/"" are excluded, same as the numeric path below.
                    values = [row.get(snake_target_field) for row in matching]
                    values = [v for v in values if v is not None and v != ""]
                    if agg_op == 'MIN':
                        record[snake_field_name] = min(values) if values else None
                    else:
                        record[snake_field_name] = max(values) if values else None
                    continue

                numbers = []
                for row in matching:
                    raw_value = row.get(snake_target_field)
                    if raw_value is None or raw_value == "":
                        continue
                    try:
                        value = float(raw_value)
                    except (TypeError, ValueError):
                        continue
                    if agg_suffix:
                        # The "count without COUNTIFS" idiom and its relatives:
                        # a target range like `Table!{{AnyField}}*0+1` — a
                        # small trusted arithmetic expression (from the
                        # rulebook schema, not runtime data) evaluated with
                        # the row's own field value substituted in. Sandboxed
                        # (no builtins) since it's still `eval`.
                        try:
                            value = eval(f"({value}){agg_suffix}", {"__builtins__": {}}, {})
                        except Exception:
                            continue
                    numbers.append(value)

                if agg_op == 'SUM':
                    # SUM over zero matching rows is 0, matching the Postgres
                    # oracle's COALESCE(SUM(...), 0) convention for "Total*"
                    # rollups.
                    record[snake_field_name] = sum(numbers) if numbers else 0
                else:  # AVERAGE
                    # No matching rows means no average to report, not zero.
                    record[snake_field_name] = (sum(numbers) / len(numbers)) if numbers else None
        except Exception as agg_exc:
            for record in records:
                record.setdefault('_erb_errors', {})[snake_field_name] = str(agg_exc)

    return records
