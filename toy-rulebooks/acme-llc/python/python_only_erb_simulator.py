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
    result['name'] = calc_customers_name(result.get('email_address'))
    result['full_name'] = calc_customers_full_name(result.get('first_name'), result.get('last_name'))

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
    entity_lower = entity_name.lower().replace('-', '_')

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


def parse_countifs_formula(formula: str) -> tuple:
    """Parse =COUNTIFS(RelatedTable!{{LookupField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=COUNTIFS\((\w+)!\{\{(\w+)\}\},\s*\w+!\{\{(\w+)\}\}\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3))
    return (None, None, None)


def parse_countifs(formula: str) -> tuple:
    """Parse any COUNTIFS into (table, [(range_field, criteria), ...]).

    COUNTIFS is variadic — (range, criteria) repeated — so it is parsed
    structurally rather than with one regex per shape. Each criteria is
    ('field', Name) to compare against the current record, or ('literal', v).
    All ranges must name the same table, since COUNTIFS counts rows of one
    table. Returns (None, None) when the formula is not a COUNTIFS.
    """
    match = re.match(r"\s*=\s*COUNTIFS\s*\((.*)\)\s*$", formula, re.S)
    if not match:
        return (None, None)

    args = [a.strip() for a in _split_top_level_args(match.group(1))]
    if len(args) < 2 or len(args) % 2 != 0:
        raise ValueError(
            f"COUNTIFS takes (range, criteria) pairs, got {len(args)} argument(s): {formula}")

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
    """Classify one COUNTIFS criteria argument."""
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
        return ('literal', arg[1:-1])
    try:
        return ('literal', int(arg))
    except ValueError:
        pass
    try:
        return ('literal', float(arg))
    except ValueError:
        pass
    raise ValueError(f"Unrecognized COUNTIFS criteria: {arg!r}")


def count_matching_rows(rows: list, criteria: list, record: dict) -> int:
    """Count rows satisfying every (range_field, criteria) pair."""
    count = 0
    for row in rows:
        for range_field, (kind, value) in criteria:
            expected = record.get(to_snake_case(value)) if kind == 'field' else value
            if row.get(to_snake_case(range_field)) != expected:
                break
        else:
            count += 1
    return count


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


def parse_sumifs_formula(formula: str) -> tuple:
    """Parse =SUMIFS(RelatedTable!{{SumField}}, RelatedTable!{{CriteriaField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=SUMIFS\((\w+)!\{\{(\w+)\}\},\s*(\w+)!\{\{(\w+)\}\},\s*\w+!\{\{(\w+)\}\}\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(4), match.group(5))
    return (None, None, None, None)


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
    """INDEX/MATCH lookup interpreter. PYTHON SIMULATOR ONLY."""
    schema = get_entity_schema(rulebook, entity_name)
    lookup_fields = get_lookup_fields(schema)

    if not lookup_fields:
        return records

    related_data_cache = {}

    for field in lookup_fields:
        field_name = field.get("name")
        formula = field.get("formula", "")
        snake_field_name = to_snake_case(field_name)

        lookup_table, return_field, key_field, pk_field = parse_index_match_formula(formula)

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

        for record in records:
            key_value = record.get(snake_key_field)
            if key_value is not None and key_value in lookup_map:
                record[snake_field_name] = lookup_map[key_value]
            else:
                record[snake_field_name] = None

    return records


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

        if related_table:
            if related_table not in related_data_cache:
                related_data_cache[related_table] = load_related_data(project_root, related_table)

            related_records = related_data_cache[related_table]
            snake_lookup_field = to_snake_case(lookup_field)
            snake_match_field = to_snake_case(match_field)

            count_map = {}
            for related_record in related_records:
                lookup_value = related_record.get(snake_lookup_field)
                if lookup_value is not None:
                    count_map[lookup_value] = count_map.get(lookup_value, 0) + 1

            for record in records:
                match_value = record.get(snake_match_field)
                if match_value is not None:
                    record[snake_field_name] = count_map.get(match_value, 0)
                else:
                    record[snake_field_name] = 0
            continue

        related_table, sum_field, criteria_field, match_field = parse_sumifs_formula(formula)

        if related_table:
            if related_table not in related_data_cache:
                related_data_cache[related_table] = load_related_data(project_root, related_table)

            related_records = related_data_cache[related_table]
            snake_sum_field = to_snake_case(sum_field)
            snake_criteria_field = to_snake_case(criteria_field)
            snake_match_field = to_snake_case(match_field)

            is_distinct = "distinct" in field_name.lower()

            related_pk_field = to_snake_case(related_table[:-1] + "Id")
            pk_to_record = {}
            for rec in related_records:
                pk_val = rec.get(related_pk_field)
                if pk_val:
                    pk_to_record[pk_val] = rec

            relationship_field = None
            for f in schema:
                if f.get("type") == "relationship" and f.get("RelatedTo") == related_table:
                    relationship_field = to_snake_case(f.get("name"))
                    break

            for record in records:
                match_value = record.get(snake_match_field)
                values = []
                has_any_match = False

                if relationship_field and relationship_field in record and record[relationship_field]:
                    rel_ids = [rid.strip() for rid in str(record[relationship_field]).split(",") if rid.strip()]
                    for rel_id in rel_ids:
                        rel_rec = pk_to_record.get(rel_id)
                        if rel_rec:
                            criteria_value = rel_rec.get(snake_criteria_field)
                            if criteria_value == match_value:
                                has_any_match = True
                                sum_value = rel_rec.get(snake_sum_field)
                                if sum_value is not None and sum_value != "" and sum_value != 0:
                                    str_value = str(sum_value)
                                    if is_distinct:
                                        if str_value not in values:
                                            values.append(str_value)
                                    else:
                                        values.append(str_value)
                                else:
                                    values.append("")
                else:
                    sorted_records = sorted(
                        related_records,
                        key=lambda r: r.get("sequence_position", 0) if r.get("sequence_position") is not None else 0,
                    )
                    for related_record in sorted_records:
                        criteria_value = related_record.get(snake_criteria_field)
                        if criteria_value == match_value:
                            has_any_match = True
                            sum_value = related_record.get(snake_sum_field)
                            if sum_value is not None and sum_value != "" and sum_value != 0:
                                str_value = str(sum_value)
                                if is_distinct:
                                    if str_value not in values:
                                        values.append(str_value)
                                else:
                                    values.append(str_value)

                non_empty_values = [v for v in values if v]
                if non_empty_values:
                    if is_distinct:
                        record[snake_field_name] = ", ".join(non_empty_values)
                    else:
                        record[snake_field_name] = ", ".join(values)
                elif has_any_match:
                    record[snake_field_name] = 0
                elif is_distinct:
                    record[snake_field_name] = ""
                else:
                    record[snake_field_name] = 0

    return records
