"""
ERB SDK (GENERATED - DO NOT EDIT)
=================================
Generated from: effortless-rulebook/a1-effortless-init-sample-rulebook.json

A calc_<table>_<field>() function per calculated field, a
compute_<table>_fields(record) per table, and the table registry main.py
runs. Every formula is compiled; lookups and aggregations are compiled to
the specs in ERB_TABLES, which erb_runtime.erb_run computes over the whole
dataset. Nothing here parses a formula or reads the rulebook.
"""

import erb_runtime as _erb


# =============================================================================
# HELLOWHOS
# The smallest complete rulebook: an id, a display name, and a rule that derives a greeting from it.
# =============================================================================

# Level 1

def calc_hello_whos_introduction(name):
    """Formula: ="Hello " & {{Name}} & "!!!" """
    return ('Hello ' + str(name or "") + '!!!')


def compute_hello_whos_fields(record: dict) -> dict:
    """
    Compute all calculated fields for HelloWhos.
    
    The smallest complete rulebook: an id, a display name, and a rule that derives a greeting from it.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['introduction'] = calc_hello_whos_introduction(result.get('name'))
    except Exception as _field_exc:
        result['introduction'] = None
        result.setdefault('_erb_errors', {})['introduction'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['introduction']:
        if result.get(key) == '':
            result[key] = None

    return result


# =============================================================================
# DISPATCHER
# =============================================================================

def compute_all_calculated_fields(record: dict, entity_name: str) -> dict:
    """Compute every calculated field of one record of the named entity
    (PascalCase or snake_case). Lookups and aggregations read other rows,
    so they are computed by erb_run over the whole dataset, not here."""
    compute = _ERB_COMPUTE_BY_NAME.get(entity_name)
    if compute is None:
        raise KeyError(
            f"compute_all_calculated_fields called with unknown entity {entity_name!r}. "
            f"Known entities: {sorted(_ERB_COMPUTE_BY_NAME)!r}.")
    return compute(record)


_ERB_COMPUTE_BY_NAME = {
    'HelloWhos': compute_hello_whos_fields,
    'hello_whos': compute_hello_whos_fields,
}


# =============================================================================
# AGGREGATE SCALARS — formulas wrapped around aggregate calls
# =============================================================================


# calculated_field_count bounds the runner's passes over the dataset.
CALCULATED_FIELD_COUNT = 1

# ERB_TABLES is every table, in rulebook order.
ERB_TABLES = [
    {'name': 'HelloWhos', 'file': 'hello_whos', 'rulebook_rows': 3,
     'compute': compute_hello_whos_fields,
     'fields': ['hello_who_id', 'name', 'introduction'],
     'calculated': {'introduction'},
     'lookups': [],
     'aggregations': []},
]

# ERB_CLOSURES materializes each vw_<entity>_closure view aggregations read.
ERB_CLOSURES = [
]
