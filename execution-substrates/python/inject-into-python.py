#!/usr/bin/env python3
"""
Generate Python calculation library from the Effortless Rulebook.

This script reads formulas from the rulebook and generates
python_only_erb_simulator.py with proper calculation functions for ALL
entities with calculated fields.

The generated file is PYTHON SUBSTRATE ONLY. Other substrates importing it
is cheating — see the banner at the top of the generated file.

================================================================================
ARCHITECTURE OVERVIEW
================================================================================

The ERB (Effortless Rulebook) pattern separates data into:
  - Raw fields: Input data stored directly (e.g., quantity, unit_price)
  - Calculated fields: Derived values computed from formulas (e.g., total = quantity * unit_price)

This injector generates Python code that computes calculated fields in the
correct dependency order using a DAG (Directed Acyclic Graph) approach.

DEPENDENCY ORDER EXAMPLE:
  Raw fields (Level 0):     quantity, unit_price
  Level 1 calculations:     line_total = quantity * unit_price
  Level 2 calculations:     tax_amount = line_total * tax_rate
  Level 3 calculations:     grand_total = line_total + tax_amount

Each level only depends on fields from previous levels, ensuring correct
computation order without circular dependencies.

GENERATED OUTPUT:
  - Individual calc_<entity>_<field>() functions for each calculated field
  - compute_<entity>_fields(record) functions to compute all fields for an entity
  - compute_all_calculated_fields(record, entity_name) dispatcher function

================================================================================
"""

import os
import sys
from pathlib import Path
from typing import Dict, List, Any, Set

# Add project root to path for shared imports (needed for orchestration modules)
sys.path.insert(0, str(Path(__file__).resolve().parent.parent.parent))

# Shared utilities for loading rulebook data and working with entity schemas
from orchestration.shared import (
    load_rulebook, get_candidate_name_from_cwd, handle_clean_arg,
    discover_entities, get_entity_schema, to_snake_case,
    get_calculated_fields, get_raw_fields, get_rulebook_path
)

# Formula parsing utilities to convert ERB formula syntax to Python expressions
from orchestration.formula_parser import (
    parse_formula, compile_to_python, get_field_dependencies,
    ExprNode, FieldRef, FuncCall, Concat, LiteralString
)


# =============================================================================
# STATIC INTERPRETER BLOCK
# =============================================================================
# Hand-written cross-table interpreters (INDEX/MATCH lookups and COUNTIFS/SUMIFS
# aggregations) that are appended verbatim to the generated file. These used to
# live in orchestration/shared.py where any substrate could import them. They
# now live inside the Python-only fence and are part of the Python simulator
# by definition. Other substrates must implement these in their own engine.
# =============================================================================

LOOKUP_AGG_INTERPRETER_BLOCK = '''

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
        r"=\\s*INDEX\\(\\s*(\\w+)!\\{\\{(\\w+)\\}\\}\\s*,"
        r"\\s*MATCH\\(\\s*(?:\\w+!)?\\{\\{(\\w+)\\}\\}\\s*,"
        r"\\s*(\\w+)!\\{\\{(\\w+)\\}\\}\\s*,\\s*0\\s*\\)\\s*\\)"
    )
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3), match.group(5))
    return (None, None, None, None)


def parse_countifs_formula(formula: str) -> tuple:
    """Parse =COUNTIFS(RelatedTable!{{LookupField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=COUNTIFS\\((\\w+)!\\{\\{(\\w+)\\}\\},\\s*\\w+!\\{\\{(\\w+)\\}\\}\\)"
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
    match = re.match(r"\\s*=\\s*COUNTIFS\\s*\\((.*)\\)\\s*$", formula, re.S)
    if not match:
        return (None, None)

    args = [a.strip() for a in _split_top_level_args(match.group(1))]
    if len(args) < 2 or len(args) % 2 != 0:
        raise ValueError(
            f"COUNTIFS takes (range, criteria) pairs, got {len(args)} argument(s): {formula}")

    table = None
    criteria = []
    for range_arg, criteria_arg in zip(args[0::2], args[1::2]):
        range_match = re.match(r"^(\\w+)!\\{\\{(\\w+)\\}\\}$", range_arg)
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
        if char in '"\\'':
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
    field_match = re.match(r"^(?:\\w+!)?\\{\\{(\\w+)\\}\\}$", arg)
    if field_match:
        return ('field', field_match.group(1))
    # TRUE and FALSE appear with or without call parentheses.
    bare = arg.upper().replace('()', '').strip()
    if bare == 'TRUE':
        return ('literal', True)
    if bare == 'FALSE':
        return ('literal', False)
    if len(arg) >= 2 and arg[0] == arg[-1] and arg[0] in '"\\'':
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
    pattern = r"=COUNTIFS\\((\\w+)!\\{\\{(\\w+)\\}\\},\\s*(TRUE|FALSE)\\(\\)\\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3) == 'TRUE')
    return (None, None, None)


def count_closure_rows(closure_rows: list, column: str, expected) -> int:
    """Count materialized closure rows whose column equals expected."""
    return sum(1 for row in closure_rows if row.get(column) == expected)


def parse_sumifs_formula(formula: str) -> tuple:
    """Parse =SUMIFS(RelatedTable!{{SumField}}, RelatedTable!{{CriteriaField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=SUMIFS\\((\\w+)!\\{\\{(\\w+)\\}\\},\\s*(\\w+)!\\{\\{(\\w+)\\}\\},\\s*\\w+!\\{\\{(\\w+)\\}\\}\\)"
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
'''


def build_dag_levels(calculated_fields: List[Dict], raw_field_names: Set[str]) -> List[List[Dict]]:
    """
    Build DAG (Directed Acyclic Graph) levels for calculated fields based on dependencies.

    This is the core algorithm that determines computation order. Fields are grouped
    into levels where each level only depends on fields from previous levels.

    Level 0: Raw fields (not returned, just used as the base dependency set)
    Level 1+: Calculated fields ordered by dependency depth

    ALGORITHM:
    1. Parse each formula to extract field dependencies
    2. Start with raw fields as "assigned" (available for use)
    3. Repeatedly find fields whose dependencies are all assigned
    4. Add those fields to the current level and mark them as assigned
    5. Repeat until all fields are assigned or a circular dependency is detected

    Args:
        calculated_fields: List of field definitions with 'name' and 'formula' keys
        raw_field_names: Set of raw field names that are available as inputs

    Returns:
        List of levels, where each level is a list of field definitions
        that can be computed in parallel (all dependencies satisfied)
    """
    # -------------------------------------------------------------------------
    # STEP 1: Parse all formulas and extract their field dependencies
    # -------------------------------------------------------------------------
    # field_deps maps field_name -> set of field names it depends on
    field_deps = {}
    for field in calculated_fields:
        formula = field.get('formula', '')
        try:
            # Parse the formula into an expression tree and extract referenced fields
            expr = parse_formula(formula)
            deps = get_field_dependencies(expr)
            # Normalize to snake_case for consistent comparison
            field_deps[field['name']] = set(to_snake_case(d) for d in deps)
        except Exception as e:
            print(f"Warning: Failed to parse formula for {field['name']}: {e}")
            field_deps[field['name']] = set()

    # -------------------------------------------------------------------------
    # STEP 2: Build levels using iterative dependency resolution
    # -------------------------------------------------------------------------
    levels = []

    # Start with raw fields as "assigned" - these are available without computation
    assigned = set(to_snake_case(name) for name in raw_field_names)

    # Track which fields still need to be placed in a level
    remaining = {f['name']: f for f in calculated_fields}

    while remaining:
        # Find all fields whose dependencies are fully satisfied
        # (i.e., all deps are in the 'assigned' set)
        current_level = []
        for name, field in list(remaining.items()):
            deps = field_deps.get(name, set())
            # Check if all dependencies are satisfied (deps is a subset of assigned)
            if deps <= assigned:
                current_level.append(field)

        if not current_level:
            # No progress possible - either circular dependency or missing field
            # Add remaining fields to a final level with a warning
            print(f"Warning: Could not resolve dependencies for: {list(remaining.keys())}")
            levels.append(list(remaining.values()))
            break

        # Add the current level to our result
        levels.append(current_level)

        # Mark all fields in this level as assigned (available for next level)
        for field in current_level:
            assigned.add(to_snake_case(field['name']))
            del remaining[field['name']]

    return levels


def generate_function_signature(entity_name: str, field_name: str, deps: List[str]) -> str:
    """
    Generate a Python function signature with entity-namespaced function name.

    NAMING CONVENTION:
      calc_<entity>_<field>(dep1, dep2, ...)

    Example:
      Entity: "LineItem", Field: "total", Dependencies: ["quantity", "unit_price"]
      Output: def calc_line_item_total(quantity, unit_price):

    This namespacing prevents collisions when multiple entities have
    fields with the same name (e.g., both Order and LineItem might have 'total').

    Args:
        entity_name: The entity name (e.g., "LineItem", "Order")
        field_name: The calculated field name (e.g., "total", "tax_amount")
        deps: List of field names this calculation depends on

    Returns:
        A Python function definition line (e.g., "def calc_line_item_total(quantity, unit_price):")
    """
    entity_snake = to_snake_case(entity_name)
    field_snake = to_snake_case(field_name)
    func_name = f"calc_{entity_snake}_{field_snake}"
    params = [to_snake_case(d) for d in deps]
    params_str = ", ".join(params) if params else ""
    return f"def {func_name}({params_str}):"


def generate_calc_function(entity_name: str, field: Dict) -> str:
    """
    Generate a complete Python calculation function for a single field.

    This function takes an ERB formula and produces a pure Python function
    that computes the field value from its dependencies.

    TRANSFORMATION PIPELINE:
      1. Parse the ERB formula into an expression tree
      2. Extract field dependencies from the expression tree
      3. Compile the expression tree to a Python expression
      4. Generate a function with the deps as parameters

    EXAMPLE:
      Input formula: "Quantity * UnitPrice"
      Output:
        def calc_line_item_total(quantity, unit_price):
            '''Formula: Quantity * UnitPrice'''
            return quantity * unit_price

    Args:
        entity_name: The entity this field belongs to (for namespacing)
        field: Field definition dict with 'name' and 'formula' keys

    Returns:
        Complete Python function definition as a string
    """
    name = field['name']
    formula = field.get('formula', '')
    description = field.get('Description', '')
    entity_snake = to_snake_case(entity_name)
    field_snake = to_snake_case(name)

    # -------------------------------------------------------------------------
    # Parse the formula and compile to Python
    # -------------------------------------------------------------------------
    try:
        expr = parse_formula(formula)           # Parse ERB formula
        deps = get_field_dependencies(expr)     # Extract field references
        python_expr = compile_to_python(expr)   # Convert to Python expression
    except Exception as e:
        # If parsing fails, generate a stub function that raises an error
        # This allows the generated file to still be syntactically valid
        return f'''
def calc_{entity_snake}_{field_snake}():
    """ERROR: Could not parse formula: {formula}
    Error: {e}
    """
    raise NotImplementedError("Formula parsing failed")
'''

    # -------------------------------------------------------------------------
    # Build the function definition
    # -------------------------------------------------------------------------
    lines = []

    # Function signature with dependencies as parameters
    sig = generate_function_signature(entity_name, name, deps)
    lines.append(sig)

    # Docstring preserves the original formula and description for documentation
    # (escape special characters that would break the docstring)
    formula_escaped = formula.replace('\\', '\\\\').replace('"""', "'''")
    # Prevent """" at end of docstring if formula ends with a quote
    if formula_escaped.endswith('"'):
        formula_escaped = formula_escaped + ' '

    # Build multi-line docstring if we have a description
    if description:
        desc_escaped = description.replace('\\', '\\\\').replace('"""', "'''")
        lines.append(f'    """')
        lines.append(f'    {desc_escaped}')
        lines.append(f'    ')
        lines.append(f'    Formula: {formula_escaped}')
        lines.append(f'    """')
    else:
        lines.append(f'    """Formula: {formula_escaped}"""')

    # The actual computation - just returns the compiled Python expression
    lines.append(f'    return {python_expr}')

    return '\n'.join(lines)


def generate_entity_compute_function(
    entity_name: str,
    calculated_fields: List[Dict],
    dag_levels: List[List[Dict]],
    string_fields: List[str],
    entity_description: str = ''
) -> str:
    """
    Generate a compute function that calculates ALL fields for a specific entity.

    This is the main entry point for computing an entity's calculated fields.
    It orchestrates calls to individual calc_* functions in the correct
    dependency order (by DAG level).

    GENERATED FUNCTION STRUCTURE:
        def compute_<entity>_fields(record: dict) -> dict:
            result = dict(record)  # Copy input to preserve original

            # Level 1 calculations
            result['field_a'] = calc_entity_field_a(result.get('raw_1'), ...)

            # Level 2 calculations (depends on Level 1)
            result['field_b'] = calc_entity_field_b(result.get('field_a'), ...)

            # ... more levels ...

            # Post-processing (e.g., empty string -> None)
            return result

    Args:
        entity_name: Name of the entity (e.g., "LineItem")
        calculated_fields: All calculated field definitions for this entity
        dag_levels: Fields organized by dependency level (from build_dag_levels)
        string_fields: List of string-type calculated fields (for post-processing)
        entity_description: Optional description of the entity from the rulebook

    Returns:
        Complete Python function definition as a string
    """
    entity_snake = to_snake_case(entity_name)

    lines = []
    lines.append(f'def compute_{entity_snake}_fields(record: dict) -> dict:')
    if entity_description:
        desc_escaped = entity_description.replace('\\', '\\\\').replace('"""', "'''")
        lines.append(f'    """')
        lines.append(f'    Compute all calculated fields for {entity_name}.')
        lines.append(f'    ')
        lines.append(f'    {desc_escaped}')
        lines.append(f'    """')
    else:
        lines.append(f'    """Compute all calculated fields for {entity_name}."""')

    # Start with a copy of the input record to avoid mutating the original
    lines.append('    result = dict(record)')
    lines.append('')

    # -------------------------------------------------------------------------
    # Generate calculation calls for each DAG level
    # -------------------------------------------------------------------------
    # Each level's calculations can safely reference results from previous levels
    for level_idx, level_fields in enumerate(dag_levels):
        lines.append(f'    # Level {level_idx + 1} calculations')
        for field in level_fields:
            name = field['name']
            snake_name = to_snake_case(name)

            # Re-parse formula to get dependencies for the function call.
            # A parse error here means the formula in the rulebook is invalid
            # — surface it with the entity + field so the user can fix it,
            # rather than silently emitting a no-arg call that will be wrong.
            formula_text = field.get('formula', '')
            try:
                expr = parse_formula(formula_text)
                deps = get_field_dependencies(expr)
            except Exception as e:
                raise ValueError(
                    f"Failed to parse formula for {entity_snake}.{snake_name}: "
                    f"{type(e).__name__}: {e}\n"
                    f"  formula: {formula_text!r}"
                ) from e

            # Generate the function call with arguments pulled from result dict
            func_name = f"calc_{entity_snake}_{snake_name}"
            if deps:
                # Pass each dependency value from the result dict
                args = [f"result.get('{to_snake_case(dep)}')" for dep in deps]
                args_str = ', '.join(args)
                lines.append(f"    result['{snake_name}'] = {func_name}({args_str})")
            else:
                # No dependencies - call with no arguments
                lines.append(f"    result['{snake_name}'] = {func_name}()")
        lines.append('')

    # -------------------------------------------------------------------------
    # Post-processing: normalize empty strings to None for string fields
    # -------------------------------------------------------------------------
    # This ensures consistent NULL handling across substrates
    if string_fields:
        lines.append('    # Convert empty strings to None for string fields')
        fields_list = ', '.join(f"'{f}'" for f in string_fields)
        lines.append(f"    for key in [{fields_list}]:")
        lines.append("        if result.get(key) == '':")
        lines.append('            result[key] = None')
        lines.append('')

    lines.append('    return result')

    return '\n'.join(lines)


def generate_dispatcher_function(entities_with_calcs: List[str]) -> str:
    """
    Generate a dispatcher function that routes to the correct entity's compute function.

    This is a convenience function that allows callers to compute fields for any
    entity type using a single entry point, without needing to know which specific
    compute_*_fields function to call.

    GENERATED FUNCTION:
        def compute_all_calculated_fields(record: dict, entity_name: str = None) -> dict:
            entity_lower = entity_name.lower().replace('-', '_')

            if entity_lower == 'line_item':
                return compute_line_item_fields(record)
            elif entity_lower == 'order':
                return compute_order_fields(record)
            # ... etc ...
            else:
                return dict(record)  # Unknown entity, pass through

    USAGE:
        result = compute_all_calculated_fields(raw_data, "LineItem")
        result = compute_all_calculated_fields(raw_data, "line_item")  # Also works

    Args:
        entities_with_calcs: List of entity names that have calculated fields

    Returns:
        Complete Python function definition as a string
    """
    lines = []
    lines.append('def compute_all_calculated_fields(record: dict, entity_name: str = None) -> dict:')
    lines.append('    """')
    lines.append('    Compute all calculated fields for a record.')
    lines.append('    ')
    lines.append('    This is the main entry point for computing calculated fields.')
    lines.append('    It routes to the appropriate entity-specific compute function.')
    lines.append('    ')
    lines.append('    Args:')
    lines.append('        record: The record dict with raw field values')
    lines.append('        entity_name: Entity name (snake_case or PascalCase)')
    lines.append('    ')
    lines.append('    Returns:')
    lines.append('        Record dict with calculated fields filled in')
    lines.append('    """')

    # Handle case where no entity name is provided
    lines.append('    if entity_name is None:')
    lines.append('        # No entity specified - return record unchanged')
    lines.append('        return dict(record)')
    lines.append('')

    # Normalize entity name to handle various input formats
    lines.append('    # Normalize to snake_case to support "LineItem", "line_item", "line-item"')
    lines.append("    entity_lower = entity_name.lower().replace('-', '_')")
    lines.append('')

    # Generate dispatch logic using if/elif chain
    if entities_with_calcs:
        known_entities = [to_snake_case(e) for e in entities_with_calcs]
        for i, entity in enumerate(entities_with_calcs):
            entity_snake = to_snake_case(entity)
            prefix = 'if' if i == 0 else 'elif'
            lines.append(f"    {prefix} entity_lower == '{entity_snake}':")
            lines.append(f"        return compute_{entity_snake}_fields(record)")

        # An unknown entity name is a bug in the caller — RAISE so the user
        # sees what was passed, what was expected, and which generated file
        # they're hitting. Silently returning the record unchanged hides the
        # mismatch between the rulebook used to generate this file and the
        # data being computed.
        lines.append('    else:')
        lines.append(
            "        raise KeyError("
            f"f\"compute_all_calculated_fields called with unknown entity {{entity_name!r}}. \""
            f"f\"Known entities in this generated erb_calc.py: {known_entities!r}. \""
            "f\"Check that the rulebook used to generate this file matches the data being computed.\")"
        )
    else:
        # No entities with calculated fields - just return record unchanged
        lines.append('    # No entities have calculated fields in this rulebook')
        lines.append('    return dict(record)')

    return '\n'.join(lines)


def generate_erb_calc(rulebook: Dict) -> str:
    """
    Generate the complete erb_calc.py content for ALL entities in the rulebook.

    This is the main code generation function that produces a complete Python
    module containing:
      1. Individual calc_* functions for each calculated field
      2. compute_*_fields functions for each entity
      3. A dispatcher function for convenience

    GENERATED FILE STRUCTURE:
        '''ERB Calculation Library (GENERATED)'''

        # ========== LINEITEM CALCULATIONS ==========
        # Level 1
        def calc_line_item_subtotal(quantity, unit_price): ...

        # Level 2
        def calc_line_item_total(subtotal, tax_rate): ...

        def compute_line_item_fields(record): ...

        # ========== ORDER CALCULATIONS ==========
        # ... similar structure ...

        # ========== DISPATCHER FUNCTION ==========
        def compute_all_calculated_fields(record, entity_name): ...

    Args:
        rulebook: The loaded ERB rulebook dictionary

    Returns:
        Complete Python module content as a string
    """
    lines = []

    # -------------------------------------------------------------------------
    # File Header
    # -------------------------------------------------------------------------
    lines.append('"""')
    lines.append('ERB Calculation Library (GENERATED - DO NOT EDIT)')
    lines.append('=================================================')
    lines.append(f'Generated from: effortless-rulebook/{get_rulebook_path().name}')
    lines.append('')
    lines.append('PYTHON SUBSTRATE ONLY. Importing this module from any other substrate')
    lines.append('is cheating. That substrate must execute the rulebook in its own native')
    lines.append('semantics (its $ENGINE). The whole point of ERB conformance is that each')
    lines.append('substrate computes calculated, lookup, and aggregation fields by')
    lines.append('interpreting/compiling the rulebook itself — not by calling out to a')
    lines.append('Python simulator. If a substrate cannot natively compute a field, it must')
    lines.append('leave it null and accept the 0 score for that field.')
    lines.append('')
    lines.append('This file contains:')
    lines.append('  - Generated calc_* functions for calculated (scalar) fields')
    lines.append('  - Generated compute_*_fields(record) dispatchers per entity')
    lines.append('  - The compute_all_calculated_fields(record, entity_name) entry point')
    lines.append('  - Hand-written compute_lookups() and compute_aggregations() — the')
    lines.append('    INDEX/MATCH and COUNTIFS/SUMIFS interpreters. These used to live in')
    lines.append('    orchestration/shared.py where any substrate could import them, which')
    lines.append('    let 7 substrates report 100% without executing anything native. They')
    lines.append('    now live inside the Python-only fence by design.')
    lines.append('"""')
    lines.append('')
    lines.append('import json')
    lines.append('import re')
    lines.append('from pathlib import Path')
    lines.append('from typing import Optional, Any')
    lines.append('')
    # Compiled date formulas call the shared helpers through this alias so the
    # calendar-month semantics have a single implementation.
    lines.append('from orchestration import formula_parser as _erb')
    lines.append('')
    lines.append('from orchestration.shared import (')
    lines.append('    to_snake_case,')
    lines.append('    get_entity_schema,')
    lines.append('    get_lookup_fields,')
    lines.append('    get_aggregation_fields,')
    lines.append(')')
    lines.append('')

    # -------------------------------------------------------------------------
    # Discover and process all entities
    # -------------------------------------------------------------------------
    entities = discover_entities(rulebook)
    entities_with_calcs = []  # Track which entities have calculated fields

    for entity_name in entities:
        schema = get_entity_schema(rulebook, entity_name)
        calculated_fields = get_calculated_fields(schema)

        # Skip entities that have no calculated fields
        if not calculated_fields:
            continue

        entities_with_calcs.append(entity_name)

        # Get entity description from rulebook
        entity_data = rulebook.get(entity_name, {})
        entity_description = entity_data.get('Description', '') if isinstance(entity_data, dict) else ''

        # Level-0 inputs for the DAG. Beyond raw columns this includes every
        # field the runtime resolves before scalar formulas run: relationships
        # come straight off the record, and compute_lookups /
        # compute_aggregations / compute_closures have already populated their
        # fields by the time compute_all_calculated_fields is called. Seeding
        # only raw fields deadlocks any calculated field built on a lookup.
        raw_fields = get_raw_fields(schema)
        raw_field_names = {f['name'] for f in raw_fields}
        raw_field_names |= {
            f['name'] for f in schema
            if f.get('type') in ('lookup', 'aggregation', 'closure', 'relationship')
        }

        # Identify string-type calculated fields for post-processing
        # (empty strings will be converted to None)
        string_fields = [
            to_snake_case(f['name'])
            for f in calculated_fields
            if f.get('datatype') == 'string'
        ]

        # Build dependency levels for this entity
        dag_levels = build_dag_levels(calculated_fields, raw_field_names)

        # ---------------------------------------------------------------------
        # Generate entity section header
        # ---------------------------------------------------------------------
        entity_snake = to_snake_case(entity_name)
        lines.append('')
        lines.append('# ' + '=' * 77)
        lines.append(f'# {entity_name.upper()} CALCULATIONS')
        if entity_description:
            lines.append(f'# {entity_description}')
        lines.append('# ' + '=' * 77)

        # ---------------------------------------------------------------------
        # Generate individual calculation functions, organized by DAG level
        # ---------------------------------------------------------------------
        for level_idx, level_fields in enumerate(dag_levels):
            lines.append('')
            lines.append(f'# Level {level_idx + 1}')

            for field in level_fields:
                lines.append('')
                lines.append(generate_calc_function(entity_name, field))

        # ---------------------------------------------------------------------
        # Generate the entity's compute function
        # ---------------------------------------------------------------------
        lines.append('')
        lines.append('')
        lines.append(generate_entity_compute_function(
            entity_name, calculated_fields, dag_levels, string_fields, entity_description
        ))

    # -------------------------------------------------------------------------
    # Generate dispatcher function (routes to correct entity compute function)
    # -------------------------------------------------------------------------
    lines.append('')
    lines.append('')
    lines.append('# ' + '=' * 77)
    lines.append('# DISPATCHER FUNCTION')
    lines.append('# ' + '=' * 77)
    lines.append('')
    lines.append(generate_dispatcher_function(entities_with_calcs))

    # -------------------------------------------------------------------------
    # Append the hand-written lookup/aggregation interpreter block.
    # This block used to live in orchestration/shared.py where any substrate
    # could import it. It now lives inside the Python-only fence by design.
    # -------------------------------------------------------------------------
    return '\n'.join(lines) + LOOKUP_AGG_INTERPRETER_BLOCK


def main():
    """
    Main entry point for the Python substrate injector.

    WORKFLOW:
    1. Load the ERB rulebook (effortless-rulebook.json)
    2. Discover all entities and their calculated fields
    3. Generate erb_calc.py with calculation functions
    4. Write the generated file to the substrate directory

    USAGE:
        python inject-into-python.py          # Generate erb_calc.py
        python inject-into-python.py --clean  # Remove generated files

    The generated erb_calc.py can then be imported by other Python code
    or by other substrates (English, YAML, etc.) that need to compute
    calculated fields.
    """
    # -------------------------------------------------------------------------
    # Configuration: files generated by this script
    # -------------------------------------------------------------------------
    GENERATED_FILES = [
        'python_only_erb_simulator.py',
        'erb_calc.py',  # legacy filename — clean it up if present
    ]

    # Handle --clean argument (removes generated files and exits)
    if handle_clean_arg(GENERATED_FILES, "Python substrate: Removes generated calculation library"):
        return

    # ERB_OUTPUT_DIR allows ssotme-proxy to redirect output into a project folder
    env_output = os.environ.get("ERB_OUTPUT_DIR")
    script_dir = Path(env_output).resolve() if env_output else Path(__file__).resolve().parent

    # -------------------------------------------------------------------------
    # Print banner and load rulebook
    # -------------------------------------------------------------------------
    print("=" * 70)
    print("Python Execution Substrate - Multi-Entity Formula Compiler")
    print("=" * 70)
    print()

    print("Loading rulebook...")
    try:
        rulebook = load_rulebook()
    except FileNotFoundError as e:
        print(f"ERROR: {e}")
        sys.exit(1)

    # -------------------------------------------------------------------------
    # Discover entities and display summary
    # -------------------------------------------------------------------------
    entities = discover_entities(rulebook)
    print(f"Discovered {len(entities)} entities: {', '.join(entities)}")
    print()

    # Show calculated fields per entity for user visibility
    total_fields = 0
    for entity_name in entities:
        schema = get_entity_schema(rulebook, entity_name)
        calculated_fields = get_calculated_fields(schema)
        if calculated_fields:
            print(f"  {entity_name}: {len(calculated_fields)} calculated fields")
            for field in calculated_fields:
                print(f"    - {field['name']}")
            total_fields += len(calculated_fields)

    print()
    print(f"Total: {total_fields} calculated fields to compile")
    print()
    print("-" * 70)
    print()

    # -------------------------------------------------------------------------
    # Generate and write erb_calc.py
    # -------------------------------------------------------------------------
    print("Generating python_only_erb_simulator.py...")
    erb_calc_content = generate_erb_calc(rulebook)

    erb_calc_path = script_dir / "python_only_erb_simulator.py"
    erb_calc_path.write_text(erb_calc_content, encoding='utf-8')
    print(f"Wrote: {erb_calc_path} ({len(erb_calc_content)} bytes)")

    # Remove the legacy filename if present from a prior build, so nothing
    # imports the un-fenced version by accident.
    legacy_path = script_dir / "erb_calc.py"
    if legacy_path.exists():
        legacy_path.unlink()
        print(f"Removed legacy file: {legacy_path}")

    print()
    print("=" * 70)
    print("Generation complete!")
    print("=" * 70)


# =============================================================================
# SCRIPT ENTRY POINT
# =============================================================================
if __name__ == "__main__":
    main()
