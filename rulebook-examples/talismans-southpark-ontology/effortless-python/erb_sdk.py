"""
ERB SDK (GENERATED - DO NOT EDIT)
=================================
Generated from: effortless-rulebook/talisman-s-south-park-ontology-explainer-rulebook.json

A calc_<table>_<field>() function per calculated field, a
compute_<table>_fields(record) per table, and the table registry main.py
runs. Every formula is compiled; lookups and aggregations are compiled to
the specs in ERB_TABLES, which erb_runtime.erb_run computes over the whole
dataset. Nothing here parses a formula or reads the rulebook.
"""

import erb_runtime as _erb

# The ERB build parameters this SDK was generated under
# (docs/ERB-BUILD-PARAMETERS.md). The runtime refuses to run unconfigured.
ERB_BUILD_PARAMETERS = {'erbDateDiff': 'calendar', 'erbTimezone': 'UTC', 'erbDateTimeText': 'iso8601', 'erbBlankLogic': 'coerce', 'erbWholeNumber': 'by-field-type'}
_erb.configure(ERB_BUILD_PARAMETERS)


# =============================================================================
# __META__
# Project-level metadata. Transpiler-ignored name/value rows.
# =============================================================================

# Level 1

def calc___meta___name(meta_key):
    """
    Order 1. Mirrors MetaKey so the row is addressable by Name.
    
    Formula: ={{MetaKey}}
    """
    return meta_key


def compute___meta___fields(record: dict) -> dict:
    """
    Compute all calculated fields for __meta__.
    
    Project-level metadata. Transpiler-ignored name/value rows.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc___meta___name(result.get('meta_key'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# GRAPHS
# The explainer page as one graph: its wording variables and its totals.
# =============================================================================

# Level 1

def calc_graphs_name(label):
    """
    Order 1. Display name of the graph.
    
    Formula: ={{Label}}
    """
    return label

def calc_graphs_is_consistent(count_of_violations):
    """
    Order 19. True when no enabled disjointness rule is violated.
    
    Formula: ={{CountOfViolations}} = 0
    """
    return _erb.erb_eq(count_of_violations, 0)

def calc_graphs_conclusions_message(count_of_conclusions, no_conclusions_text):
    """
    Order 19. The empty-state message for the conclusions panel, blank when there are conclusions.
    
    Formula: =IF({{CountOfConclusions}} = 0, {{NoConclusionsText}}, "")
    """
    return (no_conclusions_text if _erb.erb_bool3(_erb.erb_eq(count_of_conclusions, 0)) else '')

def calc_graphs_has_facts_to_add(count_of_facts_to_add):
    """
    Order 6. True while some listed fact can still be added.
    
    Formula: ={{CountOfFactsToAdd}} > 0
    """
    return _erb.erb_cmp(count_of_facts_to_add, '>', 0)

def calc_graphs_next_activation_seq(max_activation_seq):
    """
    Order 2. The activation number the next switched-on fact takes.
    
    Formula: ={{MaxActivationSeq}} + 1
    """
    return _erb.erb_integer(_erb.erb_add(max_activation_seq, 1))

def calc_graphs_next_listed_sort_order(max_listed_sort_order):
    """
    Order 3. The list position a newly added fact takes.
    
    Formula: ={{MaxListedSortOrder}} + 1
    """
    return _erb.erb_integer(_erb.erb_add(max_listed_sort_order, 1))


def compute_graphs_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Graphs.
    
    The explainer page as one graph: its wording variables and its totals.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_graphs_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_consistent'] = calc_graphs_is_consistent(result.get('count_of_violations'))
    except Exception as _field_exc:
        result['is_consistent'] = None
        result.setdefault('_erb_errors', {})['is_consistent'] = str(_field_exc)
    try:
        result['conclusions_message'] = calc_graphs_conclusions_message(result.get('count_of_conclusions'), result.get('no_conclusions_text'))
    except Exception as _field_exc:
        result['conclusions_message'] = None
        result.setdefault('_erb_errors', {})['conclusions_message'] = str(_field_exc)
    try:
        result['has_facts_to_add'] = calc_graphs_has_facts_to_add(result.get('count_of_facts_to_add'))
    except Exception as _field_exc:
        result['has_facts_to_add'] = None
        result.setdefault('_erb_errors', {})['has_facts_to_add'] = str(_field_exc)
    try:
        result['next_activation_seq'] = calc_graphs_next_activation_seq(result.get('max_activation_seq'))
    except Exception as _field_exc:
        result['next_activation_seq'] = None
        result.setdefault('_erb_errors', {})['next_activation_seq'] = str(_field_exc)
    try:
        result['next_listed_sort_order'] = calc_graphs_next_listed_sort_order(result.get('max_listed_sort_order'))
    except Exception as _field_exc:
        result['next_listed_sort_order'] = None
        result.setdefault('_erb_errors', {})['next_listed_sort_order'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'conclusions_message']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# RULES
# The five rule switches on the page.
# =============================================================================

# Level 1

def calc_rules_name(label):
    """
    Order 1. The rule's name.
    
    Formula: ={{Label}}
    """
    return label


def compute_rules_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Rules.
    
    The five rule switches on the page.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_rules_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# FIRINGKINDS
# The ways a rule can apply. Domain and range belong to one rule but are worded apart.
# =============================================================================

# Level 1

def calc_firing_kinds_name(wording):
    """
    Order 1. The kind's wording.
    
    Formula: ={{Wording}}
    """
    return wording


def compute_firing_kinds_fields(record: dict) -> dict:
    """
    Compute all calculated fields for FiringKinds.
    
    The ways a rule can apply. Domain and range belong to one rule but are worded apart.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_firing_kinds_name(result.get('wording'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# THINGS
# Every individual and class the page can draw.
# =============================================================================

# Level 1

def calc_things_name(label):
    """
    Order 1. The thing's name.
    
    Formula: ={{Label}}
    """
    return label

def calc_things_is_in_graph(count_as_subject, count_as_object):
    """
    Order 18. True when the graph draws this thing.
    
    Formula: =OR({{CountAsSubject}} > 0, {{CountAsObject}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(count_as_subject, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(count_as_object, '>', 0)))

def calc_things_node_kind(kind, count_as_place):
    """
    Order 18. How the graph draws the thing: a class, a place someone attends, or a person.
    
    Formula: =IF({{Kind}} = "class", "class", IF({{CountAsPlace}} > 0, "place", "person"))
    """
    return ('class' if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'class')) else ('place' if _erb.erb_bool3(_erb.erb_cmp(count_as_place, '>', 0)) else 'person'))


def compute_things_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Things.
    
    Every individual and class the page can draw.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_things_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_in_graph'] = calc_things_is_in_graph(result.get('count_as_subject'), result.get('count_as_object'))
    except Exception as _field_exc:
        result['is_in_graph'] = None
        result.setdefault('_erb_errors', {})['is_in_graph'] = str(_field_exc)
    try:
        result['node_kind'] = calc_things_node_kind(result.get('kind'), result.get('count_as_place'))
    except Exception as _field_exc:
        result['node_kind'] = None
        result.setdefault('_erb_errors', {})['node_kind'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'node_kind']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROPERTIES
# The ten properties and the wiring every rule reads.
# =============================================================================

# Level 1

def calc_properties_name(label):
    """
    Order 1. The property's wording.
    
    Formula: ={{Label}}
    """
    return label


def compute_properties_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Properties.
    
    The ten properties and the wiring every rule reads.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_properties_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STATEMENTS
# Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold.
# =============================================================================

# Level 1

def calc_statements_is_literal(object_thing):
    """
    Order 1. True when the object is a value rather than a thing.
    
    Formula: ={{ObjectThing}} = "" 
    """
    return (object_thing is None or object_thing == "")

def calc_statements_object_text(object_thing, object_value, object_label):
    """
    Order 2. The object's name or the value.
    
    Formula: =IF({{ObjectThing}} = "", {{ObjectValue}}, {{ObjectLabel}})
    """
    return (object_value if _erb.erb_bool3((object_thing is None or object_thing == "")) else object_label)

def calc_statements_words(subject_label, property_label, object_thing, literal_quote, object_value, literal_type_marker, property_datatype, object_label):
    """
    Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form.
    
    Formula: =CONCAT({{SubjectLabel}}, " ", {{PropertyLabel}}, " ", IF({{ObjectThing}} = "", CONCAT({{LiteralQuote}}, {{ObjectValue}}, {{LiteralQuote}}, {{LiteralTypeMarker}}, {{PropertyDatatype}}), {{ObjectLabel}}))
    """
    return (_erb.erb_text(subject_label) + ' ' + _erb.erb_text(property_label) + ' ' + _erb.erb_text(((_erb.erb_text(literal_quote) + _erb.erb_text(object_value) + _erb.erb_text(literal_quote) + _erb.erb_text(literal_type_marker) + _erb.erb_text(property_datatype)) if _erb.erb_bool3((object_thing is None or object_thing == "")) else object_label)))

def calc_statements_source(is_built_in, built_in_source_text, user_source_text):
    """
    Order 2. Where a listed fact came from.
    
    Formula: =IF({{IsBuiltIn}}, {{BuiltInSourceText}}, {{UserSourceText}})
    """
    return (built_in_source_text if (is_built_in is True) else user_source_text)

def calc_statements_subject_property_key(subject, property):
    """
    Order 1. Groups the values one subject has for one property.
    
    Formula: =CONCAT({{Subject}}, "|", {{Property}})
    """
    return (_erb.erb_text(subject) + '|' + _erb.erb_text(property))

def calc_statements_is_in_effect(is_stated, property_has_one_current_value, activation_seq, latest_sibling_activation_seq):
    """
    Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line.
    
    Formula: =AND({{IsStated}}, OR(NOT({{PropertyHasOneCurrentValue}}), {{ActivationSeq}} = {{LatestSiblingActivationSeq}}))
    """
    return _erb.erb_and((is_stated is True), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(property_has_one_current_value))), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(activation_seq), latest_sibling_activation_seq)))))

def calc_statements_listed_unit(is_listed):
    """
    Order 1. 1 when the fact is in the fact list.
    
    Formula: =IF({{IsListed}}, 1, 0)
    """
    return _erb.erb_integer((1 if (is_listed is True) else 0))

# Level 2

def calc_statements_name(subject_label, property_label, object_text):
    """
    Order 3. The fact in words.
    
    Formula: =CONCAT({{SubjectLabel}}, " ", {{PropertyLabel}}, " ", {{ObjectText}})
    """
    return (_erb.erb_text(subject_label) + ' ' + _erb.erb_text(property_label) + ' ' + _erb.erb_text(object_text))

def calc_statements_in_effect_unit(is_in_effect):
    """
    Order 4. 1 when the fact is in effect.
    
    Formula: =IF({{IsInEffect}}, 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(is_in_effect) else 0))

def calc_statements_listed_off_unit(is_listed, is_in_effect):
    """
    Order 4. 1 when the fact is listed but not in effect.
    
    Formula: =IF(AND({{IsListed}}, NOT({{IsInEffect}})), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and((is_listed is True), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_in_effect))))) else 0))

def calc_statements_holds_before_chain(is_in_effect, count_of_inverse_firings, count_of_symmetric_firings):
    """
    Order 7. Holds from facts in effect plus the inverse and symmetric rules.
    
    Formula: =OR({{IsInEffect}}, {{CountOfInverseFirings}} > 0, {{CountOfSymmetricFirings}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(is_in_effect), _erb.erb_bool3(_erb.erb_cmp(count_of_inverse_firings, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(count_of_symmetric_firings, '>', 0)))

# Level 3

def calc_statements_is_next_to_add(listed_off_unit, sort_order, next_fact_sort_order):
    """
    Order 7. The fact the Add next fact button switches on.
    
    Formula: =AND({{ListedOffUnit}} = 1, {{SortOrder}} = {{NextFactSortOrder}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(listed_off_unit, 1)), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(sort_order), next_fact_sort_order)))

def calc_statements_holds_before_typing(holds_before_chain, count_of_chain_firings):
    """
    Order 11. Holds before the domain and range rules apply.
    
    Formula: =OR({{HoldsBeforeChain}}, {{CountOfChainFirings}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(holds_before_chain), _erb.erb_bool3(_erb.erb_cmp(count_of_chain_firings, '>', 0)))

# Level 4

def calc_statements_holds(holds_before_typing, count_of_type_firings):
    """
    Order 15. True when in effect or concluded by an enabled rule; the graph draws it.
    
    Formula: =OR({{HoldsBeforeTyping}}, {{CountOfTypeFirings}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(holds_before_typing), _erb.erb_bool3(_erb.erb_cmp(count_of_type_firings, '>', 0)))

# Level 5

def calc_statements_is_conclusion(holds, is_in_effect):
    """
    Order 16. Holds only because a rule concluded it. Drawn dashed.
    
    Formula: =AND({{Holds}}, NOT({{IsInEffect}}))
    """
    return _erb.erb_and(_erb.erb_bool3(holds), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_in_effect))))

def calc_statements_holds_unit(holds):
    """
    Order 16. 1 when the fact holds.
    
    Formula: =IF({{Holds}}, 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(holds) else 0))

def calc_statements_parent_unit(holds, property):
    """
    Order 16. 1 for a holding parent link.
    
    Formula: =IF(AND({{Holds}}, {{Property}} = "isParentOf"), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(holds), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(property), 'isParentOf')))) else 0))

def calc_statements_grandparent_unit(holds, property):
    """
    Order 16. 1 for a holding grandparent link.
    
    Formula: =IF(AND({{Holds}}, {{Property}} = "isGrandparentOf"), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(holds), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(property), 'isGrandparentOf')))) else 0))

def calc_statements_place_unit(holds, property):
    """
    Order 16. 1 for a holding attends link.
    
    Formula: =IF(AND({{Holds}}, {{Property}} = "attends"), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(holds), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(property), 'attends')))) else 0))

# Level 6

def calc_statements_conclusion_unit(is_conclusion):
    """
    Order 17. 1 when the fact is a conclusion.
    
    Formula: =IF({{IsConclusion}}, 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(is_conclusion) else 0))


def compute_statements_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Statements.
    
    Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['is_literal'] = calc_statements_is_literal(result.get('object_thing'))
    except Exception as _field_exc:
        result['is_literal'] = None
        result.setdefault('_erb_errors', {})['is_literal'] = str(_field_exc)
    try:
        result['object_text'] = calc_statements_object_text(result.get('object_thing'), result.get('object_value'), result.get('object_label'))
    except Exception as _field_exc:
        result['object_text'] = None
        result.setdefault('_erb_errors', {})['object_text'] = str(_field_exc)
    try:
        result['words'] = calc_statements_words(result.get('subject_label'), result.get('property_label'), result.get('object_thing'), result.get('literal_quote'), result.get('object_value'), result.get('literal_type_marker'), result.get('property_datatype'), result.get('object_label'))
    except Exception as _field_exc:
        result['words'] = None
        result.setdefault('_erb_errors', {})['words'] = str(_field_exc)
    try:
        result['source'] = calc_statements_source(result.get('is_built_in'), result.get('built_in_source_text'), result.get('user_source_text'))
    except Exception as _field_exc:
        result['source'] = None
        result.setdefault('_erb_errors', {})['source'] = str(_field_exc)
    try:
        result['subject_property_key'] = calc_statements_subject_property_key(result.get('subject'), result.get('property'))
    except Exception as _field_exc:
        result['subject_property_key'] = None
        result.setdefault('_erb_errors', {})['subject_property_key'] = str(_field_exc)
    try:
        result['is_in_effect'] = calc_statements_is_in_effect(result.get('is_stated'), result.get('property_has_one_current_value'), result.get('activation_seq'), result.get('latest_sibling_activation_seq'))
    except Exception as _field_exc:
        result['is_in_effect'] = None
        result.setdefault('_erb_errors', {})['is_in_effect'] = str(_field_exc)
    try:
        result['listed_unit'] = calc_statements_listed_unit(result.get('is_listed'))
    except Exception as _field_exc:
        result['listed_unit'] = None
        result.setdefault('_erb_errors', {})['listed_unit'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['name'] = calc_statements_name(result.get('subject_label'), result.get('property_label'), result.get('object_text'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['in_effect_unit'] = calc_statements_in_effect_unit(result.get('is_in_effect'))
    except Exception as _field_exc:
        result['in_effect_unit'] = None
        result.setdefault('_erb_errors', {})['in_effect_unit'] = str(_field_exc)
    try:
        result['listed_off_unit'] = calc_statements_listed_off_unit(result.get('is_listed'), result.get('is_in_effect'))
    except Exception as _field_exc:
        result['listed_off_unit'] = None
        result.setdefault('_erb_errors', {})['listed_off_unit'] = str(_field_exc)
    try:
        result['holds_before_chain'] = calc_statements_holds_before_chain(result.get('is_in_effect'), result.get('count_of_inverse_firings'), result.get('count_of_symmetric_firings'))
    except Exception as _field_exc:
        result['holds_before_chain'] = None
        result.setdefault('_erb_errors', {})['holds_before_chain'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_next_to_add'] = calc_statements_is_next_to_add(result.get('listed_off_unit'), result.get('sort_order'), result.get('next_fact_sort_order'))
    except Exception as _field_exc:
        result['is_next_to_add'] = None
        result.setdefault('_erb_errors', {})['is_next_to_add'] = str(_field_exc)
    try:
        result['holds_before_typing'] = calc_statements_holds_before_typing(result.get('holds_before_chain'), result.get('count_of_chain_firings'))
    except Exception as _field_exc:
        result['holds_before_typing'] = None
        result.setdefault('_erb_errors', {})['holds_before_typing'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['holds'] = calc_statements_holds(result.get('holds_before_typing'), result.get('count_of_type_firings'))
    except Exception as _field_exc:
        result['holds'] = None
        result.setdefault('_erb_errors', {})['holds'] = str(_field_exc)

    # Level 5 calculations
    try:
        result['is_conclusion'] = calc_statements_is_conclusion(result.get('holds'), result.get('is_in_effect'))
    except Exception as _field_exc:
        result['is_conclusion'] = None
        result.setdefault('_erb_errors', {})['is_conclusion'] = str(_field_exc)
    try:
        result['holds_unit'] = calc_statements_holds_unit(result.get('holds'))
    except Exception as _field_exc:
        result['holds_unit'] = None
        result.setdefault('_erb_errors', {})['holds_unit'] = str(_field_exc)
    try:
        result['parent_unit'] = calc_statements_parent_unit(result.get('holds'), result.get('property'))
    except Exception as _field_exc:
        result['parent_unit'] = None
        result.setdefault('_erb_errors', {})['parent_unit'] = str(_field_exc)
    try:
        result['grandparent_unit'] = calc_statements_grandparent_unit(result.get('holds'), result.get('property'))
    except Exception as _field_exc:
        result['grandparent_unit'] = None
        result.setdefault('_erb_errors', {})['grandparent_unit'] = str(_field_exc)
    try:
        result['place_unit'] = calc_statements_place_unit(result.get('holds'), result.get('property'))
    except Exception as _field_exc:
        result['place_unit'] = None
        result.setdefault('_erb_errors', {})['place_unit'] = str(_field_exc)

    # Level 6 calculations
    try:
        result['conclusion_unit'] = calc_statements_conclusion_unit(result.get('is_conclusion'))
    except Exception as _field_exc:
        result['conclusion_unit'] = None
        result.setdefault('_erb_errors', {})['conclusion_unit'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['object_text', 'name', 'words', 'source', 'subject_property_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# FIRINGS
# Every way an enabled rule could conclude a candidate fact from the facts that hold.
# =============================================================================

# Level 1

def calc_firings_inverse_fired_unit(rule_enabled, kind, first_is_in_effect):
    """
    Order 5. 1 when the inverse rule fires here.
    
    Formula: =IF(AND({{RuleEnabled}}, {{Kind}} = "inverse", {{FirstIsInEffect}}), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(rule_enabled), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'inverse')), _erb.erb_bool3(first_is_in_effect))) else 0))

def calc_firings_symmetric_fired_unit(rule_enabled, kind, first_is_in_effect):
    """
    Order 5. 1 when the symmetric rule fires here.
    
    Formula: =IF(AND({{RuleEnabled}}, {{Kind}} = "symmetric", {{FirstIsInEffect}}), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(rule_enabled), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'symmetric')), _erb.erb_bool3(first_is_in_effect))) else 0))

def calc_firings_chain_fired_unit(rule_enabled, kind, first_holds_before_chain, second_holds_before_chain):
    """
    Order 9. 1 when the property chain fires here.
    
    Formula: =IF(AND({{RuleEnabled}}, {{Kind}} = "chain", {{FirstHoldsBeforeChain}}, {{SecondHoldsBeforeChain}}), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(rule_enabled), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'chain')), _erb.erb_bool3(first_holds_before_chain), _erb.erb_bool3(second_holds_before_chain))) else 0))

def calc_firings_type_fired_unit(rule_enabled, kind, first_holds_before_typing):
    """
    Order 13. 1 when the domain or range rule fires here.
    
    Formula: =IF(AND({{RuleEnabled}}, OR({{Kind}} = "domain", {{Kind}} = "range"), {{FirstHoldsBeforeTyping}}), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(rule_enabled), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'domain')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'range')))), _erb.erb_bool3(first_holds_before_typing))) else 0))

def calc_firings_order_key(first_sort_order, kind_sort_order, second, second_sort_order):
    """
    Order 2. The page reasoner's order: by premise, then rule kind, then second premise.
    
    Formula: =({{FirstSortOrder}} * 100000) + ({{KindSortOrder}} * 1000) + IF({{Second}} = "", 0, {{SecondSortOrder}})
    """
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add(_erb.erb_mul(first_sort_order, 100000), _erb.erb_mul(kind_sort_order, 1000)), (0 if _erb.erb_bool3((second is None or second == "")) else second_sort_order)))

def calc_firings_name(firing_id):
    """
    Order 1. The application's key.
    
    Formula: ={{FiringId}}
    """
    return firing_id

# Level 2

def calc_firings_fired_unit(inverse_fired_unit, symmetric_fired_unit, chain_fired_unit, type_fired_unit):
    """
    Order 14. 1 when this application fires.
    
    Formula: ={{InverseFiredUnit}} + {{SymmetricFiredUnit}} + {{ChainFiredUnit}} + {{TypeFiredUnit}}
    """
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add(_erb.erb_add(inverse_fired_unit, symmetric_fired_unit), chain_fired_unit), type_fired_unit))

# Level 3

def calc_firings_is_chosen(fired_unit, target_is_conclusion, order_key, target_first_fired_order_key):
    """
    Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order.
    
    Formula: =AND({{FiredUnit}} = 1, {{TargetIsConclusion}}, {{OrderKey}} = {{TargetFirstFiredOrderKey}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(fired_unit, 1)), _erb.erb_bool3(target_is_conclusion), _erb.erb_bool3(_erb.erb_eq(order_key, target_first_fired_order_key)))

# Level 4

def calc_firings_chosen_unit(is_chosen):
    """
    Order 19. 1 when this application is shown.
    
    Formula: =IF({{IsChosen}}, 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(is_chosen) else 0))

def calc_firings_card_text(is_chosen, kind_wording, first_words, kind, second_words, first_property, first_property_domain_class, first_property_range_class):
    """
    Order 19. The conclusion card's provenance line, in the page's wording.
    
    Formula: =IF({{IsChosen}}, CONCAT("Worked out by the ", {{KindWording}}, " from “", {{FirstWords}}, "”", IF({{Kind}} = "chain", CONCAT(" and “", {{SecondWords}}, "”"), IF({{Kind}} = "domain", CONCAT(" and “", {{FirstProperty}}, " has domain ", {{FirstPropertyDomainClass}}, "”"), IF({{Kind}} = "range", CONCAT(" and “", {{FirstProperty}}, " has range ", {{FirstPropertyRangeClass}}, "”"), ""))), "."), "")
    """
    return (('Worked out by the ' + _erb.erb_text(kind_wording) + ' from “' + _erb.erb_text(first_words) + '”' + _erb.erb_text(((' and “' + _erb.erb_text(second_words) + '”') if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'chain')) else ((' and “' + _erb.erb_text(first_property) + ' has domain ' + _erb.erb_text(first_property_domain_class) + '”') if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'domain')) else ((' and “' + _erb.erb_text(first_property) + ' has range ' + _erb.erb_text(first_property_range_class) + '”') if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(kind), 'range')) else '')))) + '.') if _erb.erb_bool3(is_chosen) else '')


def compute_firings_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Firings.
    
    Every way an enabled rule could conclude a candidate fact from the facts that hold.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['inverse_fired_unit'] = calc_firings_inverse_fired_unit(result.get('rule_enabled'), result.get('kind'), result.get('first_is_in_effect'))
    except Exception as _field_exc:
        result['inverse_fired_unit'] = None
        result.setdefault('_erb_errors', {})['inverse_fired_unit'] = str(_field_exc)
    try:
        result['symmetric_fired_unit'] = calc_firings_symmetric_fired_unit(result.get('rule_enabled'), result.get('kind'), result.get('first_is_in_effect'))
    except Exception as _field_exc:
        result['symmetric_fired_unit'] = None
        result.setdefault('_erb_errors', {})['symmetric_fired_unit'] = str(_field_exc)
    try:
        result['chain_fired_unit'] = calc_firings_chain_fired_unit(result.get('rule_enabled'), result.get('kind'), result.get('first_holds_before_chain'), result.get('second_holds_before_chain'))
    except Exception as _field_exc:
        result['chain_fired_unit'] = None
        result.setdefault('_erb_errors', {})['chain_fired_unit'] = str(_field_exc)
    try:
        result['type_fired_unit'] = calc_firings_type_fired_unit(result.get('rule_enabled'), result.get('kind'), result.get('first_holds_before_typing'))
    except Exception as _field_exc:
        result['type_fired_unit'] = None
        result.setdefault('_erb_errors', {})['type_fired_unit'] = str(_field_exc)
    try:
        result['order_key'] = calc_firings_order_key(result.get('first_sort_order'), result.get('kind_sort_order'), result.get('second'), result.get('second_sort_order'))
    except Exception as _field_exc:
        result['order_key'] = None
        result.setdefault('_erb_errors', {})['order_key'] = str(_field_exc)
    try:
        result['name'] = calc_firings_name(result.get('firing_id'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['fired_unit'] = calc_firings_fired_unit(result.get('inverse_fired_unit'), result.get('symmetric_fired_unit'), result.get('chain_fired_unit'), result.get('type_fired_unit'))
    except Exception as _field_exc:
        result['fired_unit'] = None
        result.setdefault('_erb_errors', {})['fired_unit'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_chosen'] = calc_firings_is_chosen(result.get('fired_unit'), result.get('target_is_conclusion'), result.get('order_key'), result.get('target_first_fired_order_key'))
    except Exception as _field_exc:
        result['is_chosen'] = None
        result.setdefault('_erb_errors', {})['is_chosen'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['chosen_unit'] = calc_firings_chosen_unit(result.get('is_chosen'))
    except Exception as _field_exc:
        result['chosen_unit'] = None
        result.setdefault('_erb_errors', {})['chosen_unit'] = str(_field_exc)
    try:
        result['card_text'] = calc_firings_card_text(result.get('is_chosen'), result.get('kind_wording'), result.get('first_words'), result.get('kind'), result.get('second_words'), result.get('first_property'), result.get('first_property_domain_class'), result.get('first_property_range_class'))
    except Exception as _field_exc:
        result['card_text'] = None
        result.setdefault('_erb_errors', {})['card_text'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['card_text', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# DISJOINTCHECKS
# Every pair the disjointness rule watches: the same two people linked by two disjoint properties.
# =============================================================================

# Level 1

def calc_disjoint_checks_violation_unit(rule_enabled, first_holds, second_holds):
    """
    Order 17. 1 when both links hold.
    
    Formula: =IF(AND({{RuleEnabled}}, {{FirstHolds}}, {{SecondHolds}}), 1, 0)
    """
    return _erb.erb_integer((1 if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(rule_enabled), _erb.erb_bool3(first_holds), _erb.erb_bool3(second_holds))) else 0))

def calc_disjoint_checks_name(disjoint_check_id):
    """
    Order 1. The check's key.
    
    Formula: ={{DisjointCheckId}}
    """
    return disjoint_check_id

# Level 2

def calc_disjoint_checks_alarm_text(violation_unit, subject_label, object_label):
    """
    Order 18. The page's contradiction alert, blank when there is none.
    
    Formula: =IF({{ViolationUnit}} = 1, CONCAT("Contradiction found. ", {{SubjectLabel}}, " is recorded as both parent and spouse of ", {{ObjectLabel}}, ". Check those facts and the selected disjoint rule."), "")
    """
    return (('Contradiction found. ' + _erb.erb_text(subject_label) + ' is recorded as both parent and spouse of ' + _erb.erb_text(object_label) + '. Check those facts and the selected disjoint rule.') if _erb.erb_bool3(_erb.erb_eq(violation_unit, 1)) else '')


def compute_disjoint_checks_fields(record: dict) -> dict:
    """
    Compute all calculated fields for DisjointChecks.
    
    Every pair the disjointness rule watches: the same two people linked by two disjoint properties.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['violation_unit'] = calc_disjoint_checks_violation_unit(result.get('rule_enabled'), result.get('first_holds'), result.get('second_holds'))
    except Exception as _field_exc:
        result['violation_unit'] = None
        result.setdefault('_erb_errors', {})['violation_unit'] = str(_field_exc)
    try:
        result['name'] = calc_disjoint_checks_name(result.get('disjoint_check_id'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['alarm_text'] = calc_disjoint_checks_alarm_text(result.get('violation_unit'), result.get('subject_label'), result.get('object_label'))
    except Exception as _field_exc:
        result['alarm_text'] = None
        result.setdefault('_erb_errors', {})['alarm_text'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['alarm_text', 'name']:
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
    '__meta__': compute___meta___fields,
    'Graphs': compute_graphs_fields,
    'graphs': compute_graphs_fields,
    'Rules': compute_rules_fields,
    'rules': compute_rules_fields,
    'FiringKinds': compute_firing_kinds_fields,
    'firing_kinds': compute_firing_kinds_fields,
    'Things': compute_things_fields,
    'things': compute_things_fields,
    'Properties': compute_properties_fields,
    'properties': compute_properties_fields,
    'Statements': compute_statements_fields,
    'statements': compute_statements_fields,
    'Firings': compute_firings_fields,
    'firings': compute_firings_fields,
    'DisjointChecks': compute_disjoint_checks_fields,
    'disjoint_checks': compute_disjoint_checks_fields,
}


# =============================================================================
# AGGREGATE SCALARS — formulas wrapped around aggregate calls
# =============================================================================


# calculated_field_count bounds the runner's passes over the dataset.
CALCULATED_FIELD_COUNT = 46

# ERB_TABLES is every table, in rulebook order.
ERB_TABLES = [
    {'name': '__meta__', 'file': '__meta__', 'rulebook_rows': 7,
     'compute': compute___meta___fields,
     'fields': ['meta_key', 'name', 'value_type', 'string_value', 'json_value'],
     'datatypes': {'name': 'string'},
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Graphs', 'file': 'graphs', 'rulebook_rows': 1,
     'compute': compute_graphs_fields,
     'fields': ['graph_id', 'label', 'literal_quote', 'literal_type_marker', 'built_in_source_text', 'user_source_text', 'no_conclusions_text', 'name', 'count_of_stated_facts', 'count_of_conclusions', 'count_of_violations', 'is_consistent', 'conclusions_message', 'count_of_facts_to_add', 'has_facts_to_add', 'next_fact_sort_order', 'max_activation_seq', 'next_activation_seq', 'max_listed_sort_order', 'next_listed_sort_order'],
     'datatypes': {'name': 'string', 'count_of_stated_facts': 'integer', 'count_of_conclusions': 'integer', 'count_of_violations': 'integer', 'is_consistent': 'boolean', 'conclusions_message': 'string', 'count_of_facts_to_add': 'integer', 'has_facts_to_add': 'boolean', 'next_fact_sort_order': 'integer', 'max_activation_seq': 'integer', 'next_activation_seq': 'integer', 'max_listed_sort_order': 'integer', 'next_listed_sort_order': 'integer'},
     'calculated': {'conclusions_message', 'is_consistent', 'has_facts_to_add', 'name', 'next_listed_sort_order', 'next_activation_seq'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_of_stated_facts', 'op': 'SUM', 'table': 'statements', 'target': 'in_effect_unit', 'criteria': [('graph', 'raw_field', 'graph_id')], 'suffix': None},
        {'field': 'count_of_conclusions', 'op': 'SUM', 'table': 'statements', 'target': 'conclusion_unit', 'criteria': [('graph', 'raw_field', 'graph_id')], 'suffix': None},
        {'field': 'count_of_violations', 'op': 'SUM', 'table': 'disjoint_checks', 'target': 'violation_unit', 'criteria': [('graph', 'raw_field', 'graph_id')], 'suffix': None},
        {'field': 'count_of_facts_to_add', 'op': 'SUM', 'table': 'statements', 'target': 'listed_off_unit', 'criteria': [('graph', 'raw_field', 'graph_id')], 'suffix': None},
        {'field': 'next_fact_sort_order', 'op': 'MIN', 'table': 'statements', 'target': 'sort_order', 'criteria': [('graph', 'raw_field', 'graph_id'), ('listed_off_unit', 'literal', 1)], 'suffix': None, 'empty': 0},
        {'field': 'max_activation_seq', 'op': 'MAX', 'table': 'statements', 'target': 'activation_seq', 'criteria': [('graph', 'raw_field', 'graph_id')], 'suffix': None, 'empty': 0},
        {'field': 'max_listed_sort_order', 'op': 'MAX', 'table': 'statements', 'target': 'sort_order', 'criteria': [('graph', 'raw_field', 'graph_id'), ('listed_unit', 'literal', 1)], 'suffix': None, 'empty': 0},]},
    {'name': 'Rules', 'file': 'rules', 'rulebook_rows': 5,
     'compute': compute_rules_fields,
     'fields': ['rule_id', 'label', 'explanation', 'is_enabled', 'sort_order', 'name', 'count_of_firings'],
     'datatypes': {'name': 'string', 'count_of_firings': 'integer'},
     'calculated': {'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_of_firings', 'op': 'SUM', 'table': 'firings', 'target': 'fired_unit', 'criteria': [('rule', 'raw_field', 'rule_id')], 'suffix': None},]},
    {'name': 'FiringKinds', 'file': 'firing_kinds', 'rulebook_rows': 5,
     'compute': compute_firing_kinds_fields,
     'fields': ['firing_kind_id', 'rule', 'wording', 'sort_order', 'name', 'rule_enabled'],
     'datatypes': {'name': 'string', 'rule_enabled': 'boolean'},
     'calculated': {'name'},
     'lookups': [
        {'field': 'rule_enabled', 'target': 'rules', 'return': 'is_enabled', 'key': 'rule', 'match': 'rule_id'},],
     'aggregations': []},
    {'name': 'Things', 'file': 'things', 'rulebook_rows': 9,
     'compute': compute_things_fields,
     'fields': ['thing_id', 'label', 'kind', 'sort_order', 'portrait_name', 'portrait_page', 'portrait_image', 'portrait_crop', 'name', 'count_of_children', 'count_of_grandchildren', 'count_as_place', 'count_as_subject', 'count_as_object', 'is_in_graph', 'node_kind'],
     'datatypes': {'name': 'string', 'count_of_children': 'integer', 'count_of_grandchildren': 'integer', 'count_as_place': 'integer', 'count_as_subject': 'integer', 'count_as_object': 'integer', 'is_in_graph': 'boolean', 'node_kind': 'string'},
     'calculated': {'node_kind', 'name', 'is_in_graph'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_of_children', 'op': 'SUM', 'table': 'statements', 'target': 'parent_unit', 'criteria': [('subject', 'raw_field', 'thing_id')], 'suffix': None},
        {'field': 'count_of_grandchildren', 'op': 'SUM', 'table': 'statements', 'target': 'grandparent_unit', 'criteria': [('subject', 'raw_field', 'thing_id')], 'suffix': None},
        {'field': 'count_as_place', 'op': 'SUM', 'table': 'statements', 'target': 'place_unit', 'criteria': [('object_thing', 'raw_field', 'thing_id')], 'suffix': None},
        {'field': 'count_as_subject', 'op': 'SUM', 'table': 'statements', 'target': 'holds_unit', 'criteria': [('subject', 'raw_field', 'thing_id')], 'suffix': None},
        {'field': 'count_as_object', 'op': 'SUM', 'table': 'statements', 'target': 'holds_unit', 'criteria': [('object_thing', 'raw_field', 'thing_id')], 'suffix': None},]},
    {'name': 'Properties', 'file': 'properties', 'rulebook_rows': 10,
     'compute': compute_properties_fields,
     'fields': ['property_id', 'label', 'property_kind', 'datatype', 'is_user_addable', 'has_one_current_value', 'inverse_of', 'is_symmetric', 'chain_first', 'chain_second', 'domain_class', 'range_class', 'disjoint_with', 'sort_order', 'name'],
     'datatypes': {'name': 'string'},
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Statements', 'file': 'statements', 'rulebook_rows': 270,
     'compute': compute_statements_fields,
     'fields': ['statement_id', 'graph', 'subject', 'property', 'object_thing', 'object_value', 'is_listed', 'is_built_in', 'is_detail', 'is_stated', 'activation_seq', 'sort_order', 'subject_label', 'property_label', 'object_label', 'property_datatype', 'property_has_one_current_value', 'property_domain_class', 'property_range_class', 'literal_quote', 'literal_type_marker', 'built_in_source_text', 'user_source_text', 'next_fact_sort_order', 'is_literal', 'object_text', 'name', 'words', 'source', 'subject_property_key', 'latest_sibling_activation_seq', 'is_in_effect', 'listed_unit', 'in_effect_unit', 'listed_off_unit', 'is_next_to_add', 'count_of_inverse_firings', 'count_of_symmetric_firings', 'holds_before_chain', 'count_of_chain_firings', 'holds_before_typing', 'count_of_type_firings', 'holds', 'is_conclusion', 'holds_unit', 'conclusion_unit', 'parent_unit', 'grandparent_unit', 'place_unit', 'first_fired_order_key', 'count_of_chosen_firings'],
     'datatypes': {'subject_label': 'string', 'property_label': 'string', 'object_label': 'string', 'property_datatype': 'string', 'property_has_one_current_value': 'boolean', 'property_domain_class': 'string', 'property_range_class': 'string', 'literal_quote': 'string', 'literal_type_marker': 'string', 'built_in_source_text': 'string', 'user_source_text': 'string', 'next_fact_sort_order': 'integer', 'is_literal': 'boolean', 'object_text': 'string', 'name': 'string', 'words': 'string', 'source': 'string', 'subject_property_key': 'string', 'latest_sibling_activation_seq': 'integer', 'is_in_effect': 'boolean', 'listed_unit': 'integer', 'in_effect_unit': 'integer', 'listed_off_unit': 'integer', 'is_next_to_add': 'boolean', 'count_of_inverse_firings': 'integer', 'count_of_symmetric_firings': 'integer', 'holds_before_chain': 'boolean', 'count_of_chain_firings': 'integer', 'holds_before_typing': 'boolean', 'count_of_type_firings': 'integer', 'holds': 'boolean', 'is_conclusion': 'boolean', 'holds_unit': 'integer', 'conclusion_unit': 'integer', 'parent_unit': 'integer', 'grandparent_unit': 'integer', 'place_unit': 'integer', 'first_fired_order_key': 'integer', 'count_of_chosen_firings': 'integer'},
     'calculated': {'words', 'subject_property_key', 'is_in_effect', 'in_effect_unit', 'holds_before_typing', 'source', 'is_next_to_add', 'is_conclusion', 'listed_unit', 'name', 'holds', 'place_unit', 'grandparent_unit', 'conclusion_unit', 'holds_before_chain', 'listed_off_unit', 'object_text', 'is_literal', 'parent_unit', 'holds_unit'},
     'lookups': [
        {'field': 'subject_label', 'target': 'things', 'return': 'label', 'key': 'subject', 'match': 'thing_id'},
        {'field': 'property_label', 'target': 'properties', 'return': 'label', 'key': 'property', 'match': 'property_id'},
        {'field': 'object_label', 'target': 'things', 'return': 'label', 'key': 'object_thing', 'match': 'thing_id'},
        {'field': 'property_datatype', 'target': 'properties', 'return': 'datatype', 'key': 'property', 'match': 'property_id'},
        {'field': 'property_has_one_current_value', 'target': 'properties', 'return': 'has_one_current_value', 'key': 'property', 'match': 'property_id'},
        {'field': 'property_domain_class', 'target': 'properties', 'return': 'domain_class', 'key': 'property', 'match': 'property_id'},
        {'field': 'property_range_class', 'target': 'properties', 'return': 'range_class', 'key': 'property', 'match': 'property_id'},
        {'field': 'literal_quote', 'target': 'graphs', 'return': 'literal_quote', 'key': 'graph', 'match': 'graph_id'},
        {'field': 'literal_type_marker', 'target': 'graphs', 'return': 'literal_type_marker', 'key': 'graph', 'match': 'graph_id'},
        {'field': 'built_in_source_text', 'target': 'graphs', 'return': 'built_in_source_text', 'key': 'graph', 'match': 'graph_id'},
        {'field': 'user_source_text', 'target': 'graphs', 'return': 'user_source_text', 'key': 'graph', 'match': 'graph_id'},
        {'field': 'next_fact_sort_order', 'target': 'graphs', 'return': 'next_fact_sort_order', 'key': 'graph', 'match': 'graph_id'},],
     'aggregations': [
        {'field': 'latest_sibling_activation_seq', 'op': 'MAX', 'table': 'statements', 'target': 'activation_seq', 'criteria': [('subject_property_key', 'field', 'subject_property_key')], 'suffix': None, 'empty': 0},
        {'field': 'count_of_inverse_firings', 'op': 'SUM', 'table': 'firings', 'target': 'inverse_fired_unit', 'criteria': [('target', 'raw_field', 'statement_id')], 'suffix': None},
        {'field': 'count_of_symmetric_firings', 'op': 'SUM', 'table': 'firings', 'target': 'symmetric_fired_unit', 'criteria': [('target', 'raw_field', 'statement_id')], 'suffix': None},
        {'field': 'count_of_chain_firings', 'op': 'SUM', 'table': 'firings', 'target': 'chain_fired_unit', 'criteria': [('target', 'raw_field', 'statement_id')], 'suffix': None},
        {'field': 'count_of_type_firings', 'op': 'SUM', 'table': 'firings', 'target': 'type_fired_unit', 'criteria': [('target', 'raw_field', 'statement_id')], 'suffix': None},
        {'field': 'first_fired_order_key', 'op': 'MIN', 'table': 'firings', 'target': 'order_key', 'criteria': [('target', 'raw_field', 'statement_id'), ('fired_unit', 'literal', 1)], 'suffix': None, 'empty': 0},
        {'field': 'count_of_chosen_firings', 'op': 'SUM', 'table': 'firings', 'target': 'chosen_unit', 'criteria': [('target', 'raw_field', 'statement_id')], 'suffix': None},]},
    {'name': 'Firings', 'file': 'firings', 'rulebook_rows': 465,
     'compute': compute_firings_fields,
     'fields': ['firing_id', 'kind', 'rule', 'target', 'first', 'second', 'rule_enabled', 'kind_wording', 'kind_sort_order', 'target_name', 'target_subject_label', 'target_property_label', 'target_object_text', 'target_is_conclusion', 'target_first_fired_order_key', 'first_words', 'second_words', 'first_is_in_effect', 'first_holds_before_chain', 'second_holds_before_chain', 'first_holds_before_typing', 'first_property', 'first_property_domain_class', 'first_property_range_class', 'first_sort_order', 'second_sort_order', 'inverse_fired_unit', 'symmetric_fired_unit', 'chain_fired_unit', 'type_fired_unit', 'fired_unit', 'order_key', 'is_chosen', 'chosen_unit', 'card_text', 'name'],
     'datatypes': {'rule_enabled': 'boolean', 'kind_wording': 'string', 'kind_sort_order': 'integer', 'target_name': 'string', 'target_subject_label': 'string', 'target_property_label': 'string', 'target_object_text': 'string', 'target_is_conclusion': 'boolean', 'target_first_fired_order_key': 'integer', 'first_words': 'string', 'second_words': 'string', 'first_is_in_effect': 'boolean', 'first_holds_before_chain': 'boolean', 'second_holds_before_chain': 'boolean', 'first_holds_before_typing': 'boolean', 'first_property': 'string', 'first_property_domain_class': 'string', 'first_property_range_class': 'string', 'first_sort_order': 'integer', 'second_sort_order': 'integer', 'inverse_fired_unit': 'integer', 'symmetric_fired_unit': 'integer', 'chain_fired_unit': 'integer', 'type_fired_unit': 'integer', 'fired_unit': 'integer', 'order_key': 'integer', 'is_chosen': 'boolean', 'chosen_unit': 'integer', 'card_text': 'string', 'name': 'string'},
     'calculated': {'card_text', 'order_key', 'chain_fired_unit', 'is_chosen', 'type_fired_unit', 'fired_unit', 'symmetric_fired_unit', 'chosen_unit', 'name', 'inverse_fired_unit'},
     'lookups': [
        {'field': 'rule_enabled', 'target': 'rules', 'return': 'is_enabled', 'key': 'rule', 'match': 'rule_id'},
        {'field': 'kind_wording', 'target': 'firing_kinds', 'return': 'wording', 'key': 'kind', 'match': 'firing_kind_id'},
        {'field': 'kind_sort_order', 'target': 'firing_kinds', 'return': 'sort_order', 'key': 'kind', 'match': 'firing_kind_id'},
        {'field': 'target_name', 'target': 'statements', 'return': 'name', 'key': 'target', 'match': 'statement_id'},
        {'field': 'target_subject_label', 'target': 'statements', 'return': 'subject_label', 'key': 'target', 'match': 'statement_id'},
        {'field': 'target_property_label', 'target': 'statements', 'return': 'property_label', 'key': 'target', 'match': 'statement_id'},
        {'field': 'target_object_text', 'target': 'statements', 'return': 'object_text', 'key': 'target', 'match': 'statement_id'},
        {'field': 'target_is_conclusion', 'target': 'statements', 'return': 'is_conclusion', 'key': 'target', 'match': 'statement_id'},
        {'field': 'target_first_fired_order_key', 'target': 'statements', 'return': 'first_fired_order_key', 'key': 'target', 'match': 'statement_id'},
        {'field': 'first_words', 'target': 'statements', 'return': 'words', 'key': 'first', 'match': 'statement_id'},
        {'field': 'second_words', 'target': 'statements', 'return': 'words', 'key': 'second', 'match': 'statement_id'},
        {'field': 'first_is_in_effect', 'target': 'statements', 'return': 'is_in_effect', 'key': 'first', 'match': 'statement_id'},
        {'field': 'first_holds_before_chain', 'target': 'statements', 'return': 'holds_before_chain', 'key': 'first', 'match': 'statement_id'},
        {'field': 'second_holds_before_chain', 'target': 'statements', 'return': 'holds_before_chain', 'key': 'second', 'match': 'statement_id'},
        {'field': 'first_holds_before_typing', 'target': 'statements', 'return': 'holds_before_typing', 'key': 'first', 'match': 'statement_id'},
        {'field': 'first_property', 'target': 'statements', 'return': 'property', 'key': 'first', 'match': 'statement_id'},
        {'field': 'first_property_domain_class', 'target': 'statements', 'return': 'property_domain_class', 'key': 'first', 'match': 'statement_id'},
        {'field': 'first_property_range_class', 'target': 'statements', 'return': 'property_range_class', 'key': 'first', 'match': 'statement_id'},
        {'field': 'first_sort_order', 'target': 'statements', 'return': 'sort_order', 'key': 'first', 'match': 'statement_id'},
        {'field': 'second_sort_order', 'target': 'statements', 'return': 'sort_order', 'key': 'second', 'match': 'statement_id'},],
     'aggregations': []},
    {'name': 'DisjointChecks', 'file': 'disjoint_checks', 'rulebook_rows': 42,
     'compute': compute_disjoint_checks_fields,
     'fields': ['disjoint_check_id', 'graph', 'rule', 'first', 'second', 'rule_enabled', 'first_holds', 'second_holds', 'subject_label', 'object_label', 'violation_unit', 'alarm_text', 'name'],
     'datatypes': {'rule_enabled': 'boolean', 'first_holds': 'boolean', 'second_holds': 'boolean', 'subject_label': 'string', 'object_label': 'string', 'violation_unit': 'integer', 'alarm_text': 'string', 'name': 'string'},
     'calculated': {'violation_unit', 'name', 'alarm_text'},
     'lookups': [
        {'field': 'rule_enabled', 'target': 'rules', 'return': 'is_enabled', 'key': 'rule', 'match': 'rule_id'},
        {'field': 'first_holds', 'target': 'statements', 'return': 'holds', 'key': 'first', 'match': 'statement_id'},
        {'field': 'second_holds', 'target': 'statements', 'return': 'holds', 'key': 'second', 'match': 'statement_id'},
        {'field': 'subject_label', 'target': 'statements', 'return': 'subject_label', 'key': 'first', 'match': 'statement_id'},
        {'field': 'object_label', 'target': 'statements', 'return': 'object_label', 'key': 'first', 'match': 'statement_id'},],
     'aggregations': []},
]

# ERB_CLOSURES materializes each vw_<entity>_closure view aggregations read.
ERB_CLOSURES = [
]
