"""
ERB SDK (GENERATED - DO NOT EDIT)
=================================
Generated from: effortless-rulebook/south-park-ontology-in-sdlaf-rulebook.json

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
# REASONING
# The one world the rules reason over, and the switch of each of her five rules. Every table links here, so each rule's switch is one lookup away from the field it gates, and every count of the world rolls up here.
# =============================================================================

# Level 1

def calc_reasoning_name(label):
    """
    Order 1. The world's name.
    
    Formula: ={{Label}}
    """
    return label

def calc_reasoning_count_of_stated_facts(count_of_stated_parenthoods, count_of_stated_marriages, count_of_stated_friendships, count_of_stated_individual_facts, count_of_current_grade_levels, count_of_current_roles):
    """
    Order 4. Every stated fact in effect, wherever it is held.
    
    Formula: ={{CountOfStatedParenthoods}} + {{CountOfStatedMarriages}} + {{CountOfStatedFriendships}} + {{CountOfStatedIndividualFacts}} + {{CountOfCurrentGradeLevels}} + {{CountOfCurrentRoles}}
    """
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add(_erb.erb_add(_erb.erb_add(_erb.erb_add(count_of_stated_parenthoods, count_of_stated_marriages), count_of_stated_friendships), count_of_stated_individual_facts), count_of_current_grade_levels), count_of_current_roles))

def calc_reasoning_count_of_conclusions(count_of_child_of_conclusions, count_of_mirrored_marriages, count_of_mirrored_friendships, count_of_grandparent_conclusions, count_of_person_conclusions, count_of_school_conclusions):
    """
    Order 5. Every fact the rules conclude that no one stated.
    
    Formula: ={{CountOfChildOfConclusions}} + {{CountOfMirroredMarriages}} + {{CountOfMirroredFriendships}} + {{CountOfGrandparentConclusions}} + {{CountOfPersonConclusions}} + {{CountOfSchoolConclusions}}
    """
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add(_erb.erb_add(_erb.erb_add(_erb.erb_add(count_of_child_of_conclusions, count_of_mirrored_marriages), count_of_mirrored_friendships), count_of_grandparent_conclusions), count_of_person_conclusions), count_of_school_conclusions))

def calc_reasoning_is_consistent(count_of_contradictions):
    """
    Order 5. No contradiction the disjoint rule can see.
    
    Formula: ={{CountOfContradictions}} = 0
    """
    return _erb.erb_eq(count_of_contradictions, 0)

# Level 2

def calc_reasoning_summary(count_of_stated_facts, count_of_conclusions, count_of_contradictions):
    """
    Order 6. The world in one line.
    
    Formula: =CONCAT({{CountOfStatedFacts}}, " stated facts, ", {{CountOfConclusions}}, " conclusions, ", {{CountOfContradictions}}, " contradictions")
    """
    return (_erb.erb_text(count_of_stated_facts) + ' stated facts, ' + _erb.erb_text(count_of_conclusions) + ' conclusions, ' + _erb.erb_text(count_of_contradictions) + ' contradictions')


def compute_reasoning_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Reasoning.
    
    The one world the rules reason over, and the switch of each of her five rules. Every table links here, so each rule's switch is one lookup away from the field it gates, and every count of the world rolls up here.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_reasoning_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['count_of_stated_facts'] = calc_reasoning_count_of_stated_facts(result.get('count_of_stated_parenthoods'), result.get('count_of_stated_marriages'), result.get('count_of_stated_friendships'), result.get('count_of_stated_individual_facts'), result.get('count_of_current_grade_levels'), result.get('count_of_current_roles'))
    except Exception as _field_exc:
        result['count_of_stated_facts'] = None
        result.setdefault('_erb_errors', {})['count_of_stated_facts'] = str(_field_exc)
    try:
        result['count_of_conclusions'] = calc_reasoning_count_of_conclusions(result.get('count_of_child_of_conclusions'), result.get('count_of_mirrored_marriages'), result.get('count_of_mirrored_friendships'), result.get('count_of_grandparent_conclusions'), result.get('count_of_person_conclusions'), result.get('count_of_school_conclusions'))
    except Exception as _field_exc:
        result['count_of_conclusions'] = None
        result.setdefault('_erb_errors', {})['count_of_conclusions'] = str(_field_exc)
    try:
        result['is_consistent'] = calc_reasoning_is_consistent(result.get('count_of_contradictions'))
    except Exception as _field_exc:
        result['is_consistent'] = None
        result.setdefault('_erb_errors', {})['is_consistent'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['summary'] = calc_reasoning_summary(result.get('count_of_stated_facts'), result.get('count_of_conclusions'), result.get('count_of_contradictions'))
    except Exception as _field_exc:
        result['summary'] = None
        result.setdefault('_erb_errors', {})['summary'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'summary']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PORTRAITS
# The picture of each individual, linked from its Fandom page (never downloaded).
# =============================================================================

# Level 1

def calc_portraits_name(display_name):
    """
    Order 1. The portrait's name.
    
    Formula: ={{DisplayName}}
    """
    return display_name


def compute_portraits_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Portraits.
    
    The picture of each individual, linked from its Fandom page (never downloaded).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_portraits_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# INDIVIDUALS
# Every individual of the world (owl:Thing). What is said about one individual alone is a column of its row: its age, the place it attends, whether it is asserted to be a Person or a School. Its current grade level and role come from their records. Class membership is a formula, not a table, because it is concluded: no one stated that South Park Elementary is a School.
# =============================================================================

# Level 1

def calc_individuals_name(label):
    """
    Order 1. The individual's name.
    
    Formula: ={{Label}}
    """
    return label

def calc_individuals_has_age(age):
    """
    Order 1. An age is stated.
    
    Formula: ={{Age}} <> "" 
    """
    return (not (age is None or age == ""))

def calc_individuals_has_attends(attends):
    """
    Order 1. A place attended is stated.
    
    Formula: ={{Attends}} <> "" 
    """
    return (not (attends is None or attends == ""))

def calc_individuals_has_grade_level(count_of_grade_levels_on):
    """
    Order 2. A grade level is in effect.
    
    Formula: ={{CountOfGradeLevelsOn}} > 0
    """
    return _erb.erb_cmp(count_of_grade_levels_on, '>', 0)

def calc_individuals_has_role(count_of_roles_on):
    """
    Order 2. A role is in effect.
    
    Formula: ={{CountOfRolesOn}} > 0
    """
    return _erb.erb_cmp(count_of_roles_on, '>', 0)

def calc_individuals_is_school_by_range(domain_range_is_on, attended_by_count):
    """
    Order 2. The range rule makes this individual a School: someone attends it.
    
    Formula: =AND({{DomainRangeIsOn}}, {{AttendedByCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(domain_range_is_on), _erb.erb_bool3(_erb.erb_cmp(attended_by_count, '>', 0)))

def calc_individuals_spouse_count(stated_spouse_count, mirrored_spouse_count):
    """
    Order 4. Spouses: isSpouseOf facts about this individual, stated or mirrored.
    
    Formula: ={{StatedSpouseCount}} + {{MirroredSpouseCount}}
    """
    return _erb.erb_integer(_erb.erb_add(stated_spouse_count, mirrored_spouse_count))

def calc_individuals_friend_count(stated_friend_count, mirrored_friend_count):
    """
    Order 4. Friends: isFriendOf facts about this individual, stated or mirrored.
    
    Formula: ={{StatedFriendCount}} + {{MirroredFriendCount}}
    """
    return _erb.erb_integer(_erb.erb_add(stated_friend_count, mirrored_friend_count))

# Level 2

def calc_individuals_stated_fact_count(is_asserted_person, is_asserted_school, has_age, has_attends):
    """
    Order 2. Stated facts held on this row: is a Person, is a School, has age, attends.
    
    Formula: =IF({{IsAssertedPerson}}, 1, 0) + IF({{IsAssertedSchool}}, 1, 0) + IF({{HasAge}}, 1, 0) + IF({{HasAttends}}, 1, 0)
    """
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add(_erb.erb_add((1 if (is_asserted_person is True) else 0), (1 if (is_asserted_school is True) else 0)), (1 if _erb.erb_bool3(has_age) else 0)), (1 if _erb.erb_bool3(has_attends) else 0)))

def calc_individuals_current_grade_level_record(has_grade_level, individual_id, latest_grade_level_seq):
    """
    Order 3. Key of the grade level record in effect.
    
    Formula: =IF({{HasGradeLevel}}, CONCAT({{IndividualId}}, "|grade|", {{LatestGradeLevelSeq}}), "")
    """
    return ((_erb.erb_text(individual_id) + '|grade|' + _erb.erb_text(latest_grade_level_seq)) if _erb.erb_bool3(has_grade_level) else '')

def calc_individuals_current_role_record(has_role, individual_id, latest_role_seq):
    """
    Order 3. Key of the role record in effect.
    
    Formula: =IF({{HasRole}}, CONCAT({{IndividualId}}, "|role|", {{LatestRoleSeq}}), "")
    """
    return ((_erb.erb_text(individual_id) + '|role|' + _erb.erb_text(latest_role_seq)) if _erb.erb_bool3(has_role) else '')

def calc_individuals_has_person_evidence(has_attends, has_grade_level, has_role):
    """
    Order 3. Something is stated that only a Person can have: a place attended, a grade level or a role (the domain of attends, hasGradeLevel and hasRole).
    
    Formula: =OR({{HasAttends}}, {{HasGradeLevel}}, {{HasRole}})
    """
    return _erb.erb_or(_erb.erb_bool3(has_attends), _erb.erb_bool3(has_grade_level), _erb.erb_bool3(has_role))

def calc_individuals_is_school(is_asserted_school, is_school_by_range):
    """
    Order 3. A School, asserted or concluded. Open world: true, or not known.
    
    Formula: =IF(OR({{IsAssertedSchool}}, {{IsSchoolByRange}}), TRUE())
    """
    return (True if _erb.erb_bool3(_erb.erb_or((is_asserted_school is True), _erb.erb_bool3(is_school_by_range))) else None)

def calc_individuals_is_school_concluded(is_school_by_range, is_asserted_school):
    """
    Order 3. A School only because the range rule says so.
    
    Formula: =IF(AND({{IsSchoolByRange}}, NOT({{IsAssertedSchool}})), TRUE())
    """
    return (True if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_school_by_range), _erb.erb_bool3(_erb.erb_not((is_asserted_school is True))))) else None)

# Level 3

def calc_individuals_is_person_by_domain(domain_range_is_on, has_person_evidence):
    """
    Order 4. The domain rule makes this individual a Person.
    
    Formula: =AND({{DomainRangeIsOn}}, {{HasPersonEvidence}})
    """
    return _erb.erb_and(_erb.erb_bool3(domain_range_is_on), _erb.erb_bool3(has_person_evidence))

# Level 4

def calc_individuals_is_person(is_asserted_person, is_person_by_domain):
    """
    Order 5. A Person, asserted or concluded. Open world: true, or not known.
    
    Formula: =IF(OR({{IsAssertedPerson}}, {{IsPersonByDomain}}), TRUE())
    """
    return (True if _erb.erb_bool3(_erb.erb_or((is_asserted_person is True), _erb.erb_bool3(is_person_by_domain))) else None)

def calc_individuals_is_person_concluded(is_person_by_domain, is_asserted_person):
    """
    Order 5. A Person only because the domain rule says so.
    
    Formula: =IF(AND({{IsPersonByDomain}}, NOT({{IsAssertedPerson}})), TRUE())
    """
    return (True if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_person_by_domain), _erb.erb_bool3(_erb.erb_not((is_asserted_person is True))))) else None)


def compute_individuals_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Individuals.
    
    Every individual of the world (owl:Thing). What is said about one individual alone is a column of its row: its age, the place it attends, whether it is asserted to be a Person or a School. Its current grade level and role come from their records. Class membership is a formula, not a table, because it is concluded: no one stated that South Park Elementary is a School.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_individuals_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_age'] = calc_individuals_has_age(result.get('age'))
    except Exception as _field_exc:
        result['has_age'] = None
        result.setdefault('_erb_errors', {})['has_age'] = str(_field_exc)
    try:
        result['has_attends'] = calc_individuals_has_attends(result.get('attends'))
    except Exception as _field_exc:
        result['has_attends'] = None
        result.setdefault('_erb_errors', {})['has_attends'] = str(_field_exc)
    try:
        result['has_grade_level'] = calc_individuals_has_grade_level(result.get('count_of_grade_levels_on'))
    except Exception as _field_exc:
        result['has_grade_level'] = None
        result.setdefault('_erb_errors', {})['has_grade_level'] = str(_field_exc)
    try:
        result['has_role'] = calc_individuals_has_role(result.get('count_of_roles_on'))
    except Exception as _field_exc:
        result['has_role'] = None
        result.setdefault('_erb_errors', {})['has_role'] = str(_field_exc)
    try:
        result['is_school_by_range'] = calc_individuals_is_school_by_range(result.get('domain_range_is_on'), result.get('attended_by_count'))
    except Exception as _field_exc:
        result['is_school_by_range'] = None
        result.setdefault('_erb_errors', {})['is_school_by_range'] = str(_field_exc)
    try:
        result['spouse_count'] = calc_individuals_spouse_count(result.get('stated_spouse_count'), result.get('mirrored_spouse_count'))
    except Exception as _field_exc:
        result['spouse_count'] = None
        result.setdefault('_erb_errors', {})['spouse_count'] = str(_field_exc)
    try:
        result['friend_count'] = calc_individuals_friend_count(result.get('stated_friend_count'), result.get('mirrored_friend_count'))
    except Exception as _field_exc:
        result['friend_count'] = None
        result.setdefault('_erb_errors', {})['friend_count'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['stated_fact_count'] = calc_individuals_stated_fact_count(result.get('is_asserted_person'), result.get('is_asserted_school'), result.get('has_age'), result.get('has_attends'))
    except Exception as _field_exc:
        result['stated_fact_count'] = None
        result.setdefault('_erb_errors', {})['stated_fact_count'] = str(_field_exc)
    try:
        result['current_grade_level_record'] = calc_individuals_current_grade_level_record(result.get('has_grade_level'), result.get('individual_id'), result.get('latest_grade_level_seq'))
    except Exception as _field_exc:
        result['current_grade_level_record'] = None
        result.setdefault('_erb_errors', {})['current_grade_level_record'] = str(_field_exc)
    try:
        result['current_role_record'] = calc_individuals_current_role_record(result.get('has_role'), result.get('individual_id'), result.get('latest_role_seq'))
    except Exception as _field_exc:
        result['current_role_record'] = None
        result.setdefault('_erb_errors', {})['current_role_record'] = str(_field_exc)
    try:
        result['has_person_evidence'] = calc_individuals_has_person_evidence(result.get('has_attends'), result.get('has_grade_level'), result.get('has_role'))
    except Exception as _field_exc:
        result['has_person_evidence'] = None
        result.setdefault('_erb_errors', {})['has_person_evidence'] = str(_field_exc)
    try:
        result['is_school'] = calc_individuals_is_school(result.get('is_asserted_school'), result.get('is_school_by_range'))
    except Exception as _field_exc:
        result['is_school'] = None
        result.setdefault('_erb_errors', {})['is_school'] = str(_field_exc)
    try:
        result['is_school_concluded'] = calc_individuals_is_school_concluded(result.get('is_school_by_range'), result.get('is_asserted_school'))
    except Exception as _field_exc:
        result['is_school_concluded'] = None
        result.setdefault('_erb_errors', {})['is_school_concluded'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_person_by_domain'] = calc_individuals_is_person_by_domain(result.get('domain_range_is_on'), result.get('has_person_evidence'))
    except Exception as _field_exc:
        result['is_person_by_domain'] = None
        result.setdefault('_erb_errors', {})['is_person_by_domain'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['is_person'] = calc_individuals_is_person(result.get('is_asserted_person'), result.get('is_person_by_domain'))
    except Exception as _field_exc:
        result['is_person'] = None
        result.setdefault('_erb_errors', {})['is_person'] = str(_field_exc)
    try:
        result['is_person_concluded'] = calc_individuals_is_person_concluded(result.get('is_person_by_domain'), result.get('is_asserted_person'))
    except Exception as _field_exc:
        result['is_person_concluded'] = None
        result.setdefault('_erb_errors', {})['is_person_concluded'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'current_grade_level_record', 'current_role_record']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PARENTHOODS
# One row per stated isParentOf fact. A person has many children and a child more than one parent, so the link is its own entity with two links to Individuals. The inverse rule's isChildOf is not stored: it is this row read from the child's end.
# =============================================================================

# Level 1

def calc_parenthoods_name(parent_label, child_label):
    """
    Order 2. The stated fact in words.
    
    Formula: =CONCAT({{ParentLabel}}, " is parent of ", {{ChildLabel}})
    """
    return (_erb.erb_text(parent_label) + ' is parent of ' + _erb.erb_text(child_label))

def calc_parenthoods_child_of_holds(inverse_is_on):
    """
    Order 2. The inverse rule concludes the child is child of the parent. Open world: true, or not known.
    
    Formula: =IF({{InverseIsOn}}, TRUE())
    """
    return (True if _erb.erb_bool3(inverse_is_on) else None)

def calc_parenthoods_child_of_sentence(child_label, parent_label):
    """
    Order 2. The inverse fact in words.
    
    Formula: =CONCAT({{ChildLabel}}, " is child of ", {{ParentLabel}})
    """
    return (_erb.erb_text(child_label) + ' is child of ' + _erb.erb_text(parent_label))

def calc_parenthoods_is_contradiction(disjoint_is_on, spouse_as_stated_count, spouse_as_mirrored_count):
    """
    Order 4. The disjoint rule: the parent is also the child's spouse. Open world: true, or not known.
    
    Formula: =IF(AND({{DisjointIsOn}}, {{SpouseAsStatedCount}} + {{SpouseAsMirroredCount}} > 0), TRUE())
    """
    return (True if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(disjoint_is_on), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_add(spouse_as_stated_count, spouse_as_mirrored_count), '>', 0)))) else None)


def compute_parenthoods_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Parenthoods.
    
    One row per stated isParentOf fact. A person has many children and a child more than one parent, so the link is its own entity with two links to Individuals. The inverse rule's isChildOf is not stored: it is this row read from the child's end.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_parenthoods_name(result.get('parent_label'), result.get('child_label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['child_of_holds'] = calc_parenthoods_child_of_holds(result.get('inverse_is_on'))
    except Exception as _field_exc:
        result['child_of_holds'] = None
        result.setdefault('_erb_errors', {})['child_of_holds'] = str(_field_exc)
    try:
        result['child_of_sentence'] = calc_parenthoods_child_of_sentence(result.get('child_label'), result.get('parent_label'))
    except Exception as _field_exc:
        result['child_of_sentence'] = None
        result.setdefault('_erb_errors', {})['child_of_sentence'] = str(_field_exc)
    try:
        result['is_contradiction'] = calc_parenthoods_is_contradiction(result.get('disjoint_is_on'), result.get('spouse_as_stated_count'), result.get('spouse_as_mirrored_count'))
    except Exception as _field_exc:
        result['is_contradiction'] = None
        result.setdefault('_erb_errors', {})['is_contradiction'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'child_of_sentence']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# MARRIAGES
# One row per stated isSpouseOf fact. A marriage has no direction: the symmetric rule is the same row counted from its second partner's end.
# =============================================================================

# Level 1

def calc_marriages_name(partner_a_label, partner_b_label):
    """
    Order 2. The stated fact in words.
    
    Formula: =CONCAT({{PartnerALabel}}, " is spouse of ", {{PartnerBLabel}})
    """
    return (_erb.erb_text(partner_a_label) + ' is spouse of ' + _erb.erb_text(partner_b_label))

def calc_marriages_mirror_holds(symmetric_is_on):
    """
    Order 2. The symmetric rule concludes the second partner is spouse of the first. Open world: true, or not known.
    
    Formula: =IF({{SymmetricIsOn}}, TRUE())
    """
    return (True if _erb.erb_bool3(symmetric_is_on) else None)

def calc_marriages_mirror_sentence(partner_b_label, partner_a_label):
    """
    Order 2. The mirrored fact in words.
    
    Formula: =CONCAT({{PartnerBLabel}}, " is spouse of ", {{PartnerALabel}})
    """
    return (_erb.erb_text(partner_b_label) + ' is spouse of ' + _erb.erb_text(partner_a_label))


def compute_marriages_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Marriages.
    
    One row per stated isSpouseOf fact. A marriage has no direction: the symmetric rule is the same row counted from its second partner's end.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_marriages_name(result.get('partner_a_label'), result.get('partner_b_label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['mirror_holds'] = calc_marriages_mirror_holds(result.get('symmetric_is_on'))
    except Exception as _field_exc:
        result['mirror_holds'] = None
        result.setdefault('_erb_errors', {})['mirror_holds'] = str(_field_exc)
    try:
        result['mirror_sentence'] = calc_marriages_mirror_sentence(result.get('partner_b_label'), result.get('partner_a_label'))
    except Exception as _field_exc:
        result['mirror_sentence'] = None
        result.setdefault('_erb_errors', {})['mirror_sentence'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'mirror_sentence']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# FRIENDSHIPS
# One row per stated isFriendOf fact, read both ways by the symmetric rule, like a marriage.
# =============================================================================

# Level 1

def calc_friendships_name(friend_a_label, friend_b_label):
    """
    Order 2. The stated fact in words.
    
    Formula: =CONCAT({{FriendALabel}}, " is friend of ", {{FriendBLabel}})
    """
    return (_erb.erb_text(friend_a_label) + ' is friend of ' + _erb.erb_text(friend_b_label))

def calc_friendships_mirror_holds(symmetric_is_on):
    """
    Order 2. The symmetric rule concludes the second friend is friend of the first. Open world: true, or not known.
    
    Formula: =IF({{SymmetricIsOn}}, TRUE())
    """
    return (True if _erb.erb_bool3(symmetric_is_on) else None)

def calc_friendships_mirror_sentence(friend_b_label, friend_a_label):
    """
    Order 2. The mirrored fact in words.
    
    Formula: =CONCAT({{FriendBLabel}}, " is friend of ", {{FriendALabel}})
    """
    return (_erb.erb_text(friend_b_label) + ' is friend of ' + _erb.erb_text(friend_a_label))


def compute_friendships_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Friendships.
    
    One row per stated isFriendOf fact, read both ways by the symmetric rule, like a marriage.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_friendships_name(result.get('friend_a_label'), result.get('friend_b_label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['mirror_holds'] = calc_friendships_mirror_holds(result.get('symmetric_is_on'))
    except Exception as _field_exc:
        result['mirror_holds'] = None
        result.setdefault('_erb_errors', {})['mirror_holds'] = str(_field_exc)
    try:
        result['mirror_sentence'] = calc_friendships_mirror_sentence(result.get('friend_b_label'), result.get('friend_a_label'))
    except Exception as _field_exc:
        result['mirror_sentence'] = None
        result.setdefault('_erb_errors', {})['mirror_sentence'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'mirror_sentence']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# GRANDPARENTHOODS
# The property chain isParentOf then isParentOf, as rows: every ordered pair of parenthoods is a candidate, and it holds when the first one's child is the second one's parent. A formula cannot add a row, so the candidates are data (every pair, decided by nothing about the family) and whether one holds is a formula.
# =============================================================================

# Level 1

def calc_grandparenthoods_links(upper_child, lower_parent):
    """
    Order 2. The upper link's child is the lower link's parent: the two links make a chain.
    
    Formula: ={{UpperChild}} = {{LowerParent}}
    """
    return _erb.erb_eq(upper_child, lower_parent)

# Level 2

def calc_grandparenthoods_name(links, grandparent_label, grandchild_label, upper_link, lower_link):
    """
    Order 4. The concluded fact in words, or why this pair concludes nothing.
    
    Formula: =IF({{Links}}, CONCAT({{GrandparentLabel}}, " is grandparent of ", {{GrandchildLabel}}), CONCAT("(", {{UpperLink}}, " does not lead to ", {{LowerLink}}, ")"))
    """
    return ((_erb.erb_text(grandparent_label) + ' is grandparent of ' + _erb.erb_text(grandchild_label)) if _erb.erb_bool3(links) else ('(' + _erb.erb_text(upper_link) + ' does not lead to ' + _erb.erb_text(lower_link) + ')'))

def calc_grandparenthoods_holds(links, chain_is_on):
    """
    Order 3. The property chain concludes isGrandparentOf. Open world: true, or not known.
    
    Formula: =IF(AND({{Links}}, {{ChainIsOn}}), TRUE())
    """
    return (True if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(links), _erb.erb_bool3(chain_is_on))) else None)


def compute_grandparenthoods_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Grandparenthoods.
    
    The property chain isParentOf then isParentOf, as rows: every ordered pair of parenthoods is a candidate, and it holds when the first one's child is the second one's parent. A formula cannot add a row, so the candidates are data (every pair, decided by nothing about the family) and whether one holds is a formula.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['links'] = calc_grandparenthoods_links(result.get('upper_child'), result.get('lower_parent'))
    except Exception as _field_exc:
        result['links'] = None
        result.setdefault('_erb_errors', {})['links'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['name'] = calc_grandparenthoods_name(result.get('links'), result.get('grandparent_label'), result.get('grandchild_label'), result.get('upper_link'), result.get('lower_link'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['holds'] = calc_grandparenthoods_holds(result.get('links'), result.get('chain_is_on'))
    except Exception as _field_exc:
        result['holds'] = None
        result.setdefault('_erb_errors', {})['holds'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# GRADELEVELRECORDS
# Every grade level stated for an individual, kept when a later one replaces it: her page keeps one current grade, the latest switched on. The current value is time (which was stated last), not a flag.
# =============================================================================

# Level 1

def calc_grade_level_records_name(individual_label, grade_level):
    """
    Order 2. The fact in words.
    
    Formula: =CONCAT({{IndividualLabel}}, " has grade level ", {{GradeLevel}})
    """
    return (_erb.erb_text(individual_label) + ' has grade level ' + _erb.erb_text(grade_level))

def calc_grade_level_records_is_current(is_on, stated_seq, individual_latest_seq):
    """
    Order 3. This value is the one in effect: switched on, and the latest switched on.
    
    Formula: =AND({{IsOn}}, {{StatedSeq}} = {{IndividualLatestSeq}})
    """
    return _erb.erb_and((is_on is True), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(stated_seq), individual_latest_seq)))

def calc_grade_level_records_key_is_canonical(grade_level_record_id, individual, stated_seq):
    """
    Order 1. The key is individual|grade|StatedSeq, the key the individual's current value is looked up by.
    
    Formula: ={{GradeLevelRecordId}} = CONCAT({{Individual}}, "|grade|", {{StatedSeq}})
    """
    return _erb.erb_eq(_erb.erb_nullif(grade_level_record_id), (_erb.erb_text(individual) + '|grade|' + _erb.erb_text(stated_seq)))


def compute_grade_level_records_fields(record: dict) -> dict:
    """
    Compute all calculated fields for GradeLevelRecords.
    
    Every grade level stated for an individual, kept when a later one replaces it: her page keeps one current grade, the latest switched on. The current value is time (which was stated last), not a flag.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_grade_level_records_name(result.get('individual_label'), result.get('grade_level'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_current'] = calc_grade_level_records_is_current(result.get('is_on'), result.get('stated_seq'), result.get('individual_latest_seq'))
    except Exception as _field_exc:
        result['is_current'] = None
        result.setdefault('_erb_errors', {})['is_current'] = str(_field_exc)
    try:
        result['key_is_canonical'] = calc_grade_level_records_key_is_canonical(result.get('grade_level_record_id'), result.get('individual'), result.get('stated_seq'))
    except Exception as _field_exc:
        result['key_is_canonical'] = None
        result.setdefault('_erb_errors', {})['key_is_canonical'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLERECORDS
# Every role stated for an individual; the latest one switched on is in effect, like a grade level.
# =============================================================================

# Level 1

def calc_role_records_name(individual_label, role):
    """
    Order 2. The fact in words.
    
    Formula: =CONCAT({{IndividualLabel}}, " has role ", {{Role}})
    """
    return (_erb.erb_text(individual_label) + ' has role ' + _erb.erb_text(role))

def calc_role_records_is_current(is_on, stated_seq, individual_latest_seq):
    """
    Order 3. This value is the one in effect: switched on, and the latest switched on.
    
    Formula: =AND({{IsOn}}, {{StatedSeq}} = {{IndividualLatestSeq}})
    """
    return _erb.erb_and((is_on is True), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(stated_seq), individual_latest_seq)))

def calc_role_records_key_is_canonical(role_record_id, individual, stated_seq):
    """
    Order 1. The key is individual|role|StatedSeq, the key the individual's current value is looked up by.
    
    Formula: ={{RoleRecordId}} = CONCAT({{Individual}}, "|role|", {{StatedSeq}})
    """
    return _erb.erb_eq(_erb.erb_nullif(role_record_id), (_erb.erb_text(individual) + '|role|' + _erb.erb_text(stated_seq)))


def compute_role_records_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleRecords.
    
    Every role stated for an individual; the latest one switched on is in effect, like a grade level.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_role_records_name(result.get('individual_label'), result.get('role'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_current'] = calc_role_records_is_current(result.get('is_on'), result.get('stated_seq'), result.get('individual_latest_seq'))
    except Exception as _field_exc:
        result['is_current'] = None
        result.setdefault('_erb_errors', {})['is_current'] = str(_field_exc)
    try:
        result['key_is_canonical'] = calc_role_records_key_is_canonical(result.get('role_record_id'), result.get('individual'), result.get('stated_seq'))
    except Exception as _field_exc:
        result['key_is_canonical'] = None
        result.setdefault('_erb_errors', {})['key_is_canonical'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
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
    'Reasoning': compute_reasoning_fields,
    'reasoning': compute_reasoning_fields,
    'Portraits': compute_portraits_fields,
    'portraits': compute_portraits_fields,
    'Individuals': compute_individuals_fields,
    'individuals': compute_individuals_fields,
    'Parenthoods': compute_parenthoods_fields,
    'parenthoods': compute_parenthoods_fields,
    'Marriages': compute_marriages_fields,
    'marriages': compute_marriages_fields,
    'Friendships': compute_friendships_fields,
    'friendships': compute_friendships_fields,
    'Grandparenthoods': compute_grandparenthoods_fields,
    'grandparenthoods': compute_grandparenthoods_fields,
    'GradeLevelRecords': compute_grade_level_records_fields,
    'grade_level_records': compute_grade_level_records_fields,
    'RoleRecords': compute_role_records_fields,
    'role_records': compute_role_records_fields,
}


# =============================================================================
# AGGREGATE SCALARS — formulas wrapped around aggregate calls
# =============================================================================


# calculated_field_count bounds the runner's passes over the dataset.
CALCULATED_FIELD_COUNT = 43

# ERB_TABLES is every table, in rulebook order.
ERB_TABLES = [
    {'name': '__meta__', 'file': '__meta__', 'rulebook_rows': 8,
     'compute': compute___meta___fields,
     'fields': ['meta_key', 'name', 'value_type', 'string_value', 'json_value'],
     'datatypes': {'name': 'string'},
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Reasoning', 'file': 'reasoning', 'rulebook_rows': 1,
     'compute': compute_reasoning_fields,
     'fields': ['reasoning_id', 'name', 'label', 'inverse_is_on', 'symmetric_is_on', 'chain_is_on', 'domain_range_is_on', 'disjoint_is_on', 'count_of_individuals', 'count_of_people', 'count_of_schools', 'count_of_stated_parenthoods', 'count_of_stated_marriages', 'count_of_stated_friendships', 'count_of_stated_individual_facts', 'count_of_current_grade_levels', 'count_of_current_roles', 'count_of_stated_facts', 'count_of_child_of_conclusions', 'count_of_mirrored_marriages', 'count_of_mirrored_friendships', 'count_of_grandparent_conclusions', 'count_of_person_conclusions', 'count_of_school_conclusions', 'count_of_conclusions', 'count_of_contradictions', 'is_consistent', 'summary'],
     'datatypes': {'name': 'string', 'count_of_individuals': 'integer', 'count_of_people': 'integer', 'count_of_schools': 'integer', 'count_of_stated_parenthoods': 'integer', 'count_of_stated_marriages': 'integer', 'count_of_stated_friendships': 'integer', 'count_of_stated_individual_facts': 'integer', 'count_of_current_grade_levels': 'integer', 'count_of_current_roles': 'integer', 'count_of_stated_facts': 'integer', 'count_of_child_of_conclusions': 'integer', 'count_of_mirrored_marriages': 'integer', 'count_of_mirrored_friendships': 'integer', 'count_of_grandparent_conclusions': 'integer', 'count_of_person_conclusions': 'integer', 'count_of_school_conclusions': 'integer', 'count_of_conclusions': 'integer', 'count_of_contradictions': 'integer', 'is_consistent': 'boolean', 'summary': 'string'},
     'calculated': {'is_consistent', 'summary', 'count_of_conclusions', 'count_of_stated_facts', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_of_individuals', 'op': 'COUNTIFS', 'table': 'individuals', 'criteria': [('reasoning', 'raw_field', 'reasoning_id')]},
        {'field': 'count_of_people', 'op': 'COUNTIFS', 'table': 'individuals', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_person', 'literal', True)]},
        {'field': 'count_of_schools', 'op': 'COUNTIFS', 'table': 'individuals', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_school', 'literal', True)]},
        {'field': 'count_of_stated_parenthoods', 'op': 'COUNTIFS', 'table': 'parenthoods', 'criteria': [('reasoning', 'raw_field', 'reasoning_id')]},
        {'field': 'count_of_stated_marriages', 'op': 'COUNTIFS', 'table': 'marriages', 'criteria': [('reasoning', 'raw_field', 'reasoning_id')]},
        {'field': 'count_of_stated_friendships', 'op': 'COUNTIFS', 'table': 'friendships', 'criteria': [('reasoning', 'raw_field', 'reasoning_id')]},
        {'field': 'count_of_stated_individual_facts', 'op': 'SUM', 'table': 'individuals', 'target': 'stated_fact_count', 'criteria': [('reasoning', 'raw_field', 'reasoning_id')], 'suffix': None},
        {'field': 'count_of_current_grade_levels', 'op': 'COUNTIFS', 'table': 'grade_level_records', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_current', 'literal', True)]},
        {'field': 'count_of_current_roles', 'op': 'COUNTIFS', 'table': 'role_records', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_current', 'literal', True)]},
        {'field': 'count_of_child_of_conclusions', 'op': 'COUNTIFS', 'table': 'parenthoods', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('child_of_holds', 'literal', True)]},
        {'field': 'count_of_mirrored_marriages', 'op': 'COUNTIFS', 'table': 'marriages', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('mirror_holds', 'literal', True)]},
        {'field': 'count_of_mirrored_friendships', 'op': 'COUNTIFS', 'table': 'friendships', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('mirror_holds', 'literal', True)]},
        {'field': 'count_of_grandparent_conclusions', 'op': 'COUNTIFS', 'table': 'grandparenthoods', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('holds', 'literal', True)]},
        {'field': 'count_of_person_conclusions', 'op': 'COUNTIFS', 'table': 'individuals', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_person_concluded', 'literal', True)]},
        {'field': 'count_of_school_conclusions', 'op': 'COUNTIFS', 'table': 'individuals', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_school_concluded', 'literal', True)]},
        {'field': 'count_of_contradictions', 'op': 'COUNTIFS', 'table': 'parenthoods', 'criteria': [('reasoning', 'raw_field', 'reasoning_id'), ('is_contradiction', 'literal', True)]},]},
    {'name': 'Portraits', 'file': 'portraits', 'rulebook_rows': 7,
     'compute': compute_portraits_fields,
     'fields': ['portrait_id', 'display_name', 'page', 'image', 'crop', 'width', 'height', 'sort_order', 'name'],
     'datatypes': {'name': 'string'},
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Individuals', 'file': 'individuals', 'rulebook_rows': 7,
     'compute': compute_individuals_fields,
     'fields': ['individual_id', 'name', 'label', 'sort_order', 'reasoning', 'portrait', 'portrait_image', 'symmetric_is_on', 'domain_range_is_on', 'is_asserted_person', 'is_asserted_school', 'age', 'attends', 'attends_label', 'has_age', 'has_attends', 'stated_fact_count', 'count_of_grade_levels_on', 'latest_grade_level_seq', 'has_grade_level', 'current_grade_level_record', 'grade_level', 'count_of_roles_on', 'latest_role_seq', 'has_role', 'current_role_record', 'role', 'has_person_evidence', 'is_person_by_domain', 'is_person', 'is_person_concluded', 'attended_by_count', 'is_school_by_range', 'is_school', 'is_school_concluded', 'child_count', 'parent_count', 'child_of_fact_count', 'grandchild_count', 'grandparent_count', 'descendant_count', 'ancestor_count', 'stated_spouse_count', 'mirrored_spouse_count', 'spouse_count', 'stated_friend_count', 'mirrored_friend_count', 'friend_count'],
     'datatypes': {'name': 'string', 'portrait_image': 'string', 'symmetric_is_on': 'boolean', 'domain_range_is_on': 'boolean', 'attends_label': 'string', 'has_age': 'boolean', 'has_attends': 'boolean', 'stated_fact_count': 'integer', 'count_of_grade_levels_on': 'integer', 'latest_grade_level_seq': 'integer', 'has_grade_level': 'boolean', 'current_grade_level_record': 'string', 'grade_level': 'integer', 'count_of_roles_on': 'integer', 'latest_role_seq': 'integer', 'has_role': 'boolean', 'current_role_record': 'string', 'role': 'string', 'has_person_evidence': 'boolean', 'is_person_by_domain': 'boolean', 'is_person': 'boolean', 'is_person_concluded': 'boolean', 'attended_by_count': 'integer', 'is_school_by_range': 'boolean', 'is_school': 'boolean', 'is_school_concluded': 'boolean', 'child_count': 'integer', 'parent_count': 'integer', 'child_of_fact_count': 'integer', 'grandchild_count': 'integer', 'grandparent_count': 'integer', 'descendant_count': 'integer', 'ancestor_count': 'integer', 'stated_spouse_count': 'integer', 'mirrored_spouse_count': 'integer', 'spouse_count': 'integer', 'stated_friend_count': 'integer', 'mirrored_friend_count': 'integer', 'friend_count': 'integer'},
     'calculated': {'has_age', 'has_attends', 'friend_count', 'is_person_by_domain', 'is_school_by_range', 'is_school_concluded', 'has_role', 'spouse_count', 'name', 'has_grade_level', 'stated_fact_count', 'is_person', 'is_school', 'current_role_record', 'is_person_concluded', 'has_person_evidence', 'current_grade_level_record'},
     'lookups': [
        {'field': 'portrait_image', 'target': 'portraits', 'return': 'image', 'key': 'portrait', 'match': 'portrait_id'},
        {'field': 'symmetric_is_on', 'target': 'reasoning', 'return': 'symmetric_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},
        {'field': 'domain_range_is_on', 'target': 'reasoning', 'return': 'domain_range_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},
        {'field': 'attends_label', 'target': 'individuals', 'return': 'label', 'key': 'attends', 'match': 'individual_id'},
        {'field': 'grade_level', 'target': 'grade_level_records', 'return': 'grade_level', 'key': 'current_grade_level_record', 'match': 'grade_level_record_id'},
        {'field': 'role', 'target': 'role_records', 'return': 'role', 'key': 'current_role_record', 'match': 'role_record_id'},],
     'aggregations': [
        {'field': 'count_of_grade_levels_on', 'op': 'COUNTIFS', 'table': 'grade_level_records', 'criteria': [('individual', 'raw_field', 'individual_id'), ('is_on', 'literal', True)]},
        {'field': 'latest_grade_level_seq', 'op': 'MAX', 'table': 'grade_level_records', 'target': 'stated_seq', 'criteria': [('individual', 'raw_field', 'individual_id'), ('is_on', 'literal', True)], 'suffix': None, 'empty': 0},
        {'field': 'count_of_roles_on', 'op': 'COUNTIFS', 'table': 'role_records', 'criteria': [('individual', 'raw_field', 'individual_id'), ('is_on', 'literal', True)]},
        {'field': 'latest_role_seq', 'op': 'MAX', 'table': 'role_records', 'target': 'stated_seq', 'criteria': [('individual', 'raw_field', 'individual_id'), ('is_on', 'literal', True)], 'suffix': None, 'empty': 0},
        {'field': 'attended_by_count', 'op': 'COUNTIFS', 'table': 'individuals', 'criteria': [('attends', 'raw_field', 'individual_id')]},
        {'field': 'child_count', 'op': 'COUNTIFS', 'table': 'parenthoods', 'criteria': [('parent', 'raw_field', 'individual_id')]},
        {'field': 'parent_count', 'op': 'COUNTIFS', 'table': 'parenthoods', 'criteria': [('child', 'raw_field', 'individual_id')]},
        {'field': 'child_of_fact_count', 'op': 'COUNTIFS', 'table': 'parenthoods', 'criteria': [('child', 'raw_field', 'individual_id'), ('child_of_holds', 'literal', True)]},
        {'field': 'grandchild_count', 'op': 'COUNTIFS', 'table': 'grandparenthoods', 'criteria': [('grandparent', 'raw_field', 'individual_id'), ('holds', 'literal', True)]},
        {'field': 'grandparent_count', 'op': 'COUNTIFS', 'table': 'grandparenthoods', 'criteria': [('grandchild', 'raw_field', 'individual_id'), ('holds', 'literal', True)]},
        {'field': 'descendant_count', 'op': 'COUNTIFS', 'table': 'vw_parenthoods_closure', 'criteria': [('from_id', 'raw_field', 'individual_id')]},
        {'field': 'ancestor_count', 'op': 'COUNTIFS', 'table': 'vw_parenthoods_closure', 'criteria': [('to_id', 'raw_field', 'individual_id')]},
        {'field': 'stated_spouse_count', 'op': 'COUNTIFS', 'table': 'marriages', 'criteria': [('partner_a', 'raw_field', 'individual_id')]},
        {'field': 'mirrored_spouse_count', 'op': 'COUNTIFS', 'table': 'marriages', 'criteria': [('partner_b', 'raw_field', 'individual_id'), ('mirror_holds', 'literal', True)]},
        {'field': 'stated_friend_count', 'op': 'COUNTIFS', 'table': 'friendships', 'criteria': [('friend_a', 'raw_field', 'individual_id')]},
        {'field': 'mirrored_friend_count', 'op': 'COUNTIFS', 'table': 'friendships', 'criteria': [('friend_b', 'raw_field', 'individual_id'), ('mirror_holds', 'literal', True)]},]},
    {'name': 'Parenthoods', 'file': 'parenthoods', 'rulebook_rows': 3,
     'compute': compute_parenthoods_fields,
     'fields': ['parenthood_id', 'name', 'reasoning', 'parent', 'child', 'parent_label', 'child_label', 'inverse_is_on', 'symmetric_is_on', 'disjoint_is_on', 'child_of_holds', 'child_of_sentence', 'spouse_as_stated_count', 'spouse_as_mirrored_count', 'is_contradiction', 'ancestry_closure'],
     'datatypes': {'name': 'string', 'parent_label': 'string', 'child_label': 'string', 'inverse_is_on': 'boolean', 'symmetric_is_on': 'boolean', 'disjoint_is_on': 'boolean', 'child_of_holds': 'boolean', 'child_of_sentence': 'string', 'spouse_as_stated_count': 'integer', 'spouse_as_mirrored_count': 'integer', 'is_contradiction': 'boolean'},
     'calculated': {'child_of_sentence', 'child_of_holds', 'is_contradiction', 'name'},
     'lookups': [
        {'field': 'parent_label', 'target': 'individuals', 'return': 'label', 'key': 'parent', 'match': 'individual_id'},
        {'field': 'child_label', 'target': 'individuals', 'return': 'label', 'key': 'child', 'match': 'individual_id'},
        {'field': 'inverse_is_on', 'target': 'reasoning', 'return': 'inverse_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},
        {'field': 'symmetric_is_on', 'target': 'reasoning', 'return': 'symmetric_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},
        {'field': 'disjoint_is_on', 'target': 'reasoning', 'return': 'disjoint_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},],
     'aggregations': [
        {'field': 'spouse_as_stated_count', 'op': 'COUNTIFS', 'table': 'marriages', 'criteria': [('partner_a', 'raw_field', 'parent'), ('partner_b', 'raw_field', 'child')]},
        {'field': 'spouse_as_mirrored_count', 'op': 'COUNTIFS', 'table': 'marriages', 'criteria': [('partner_a', 'raw_field', 'child'), ('partner_b', 'raw_field', 'parent'), ('mirror_holds', 'literal', True)]},]},
    {'name': 'Marriages', 'file': 'marriages', 'rulebook_rows': 1,
     'compute': compute_marriages_fields,
     'fields': ['marriage_id', 'name', 'reasoning', 'partner_a', 'partner_b', 'partner_a_label', 'partner_b_label', 'symmetric_is_on', 'mirror_holds', 'mirror_sentence'],
     'datatypes': {'name': 'string', 'partner_a_label': 'string', 'partner_b_label': 'string', 'symmetric_is_on': 'boolean', 'mirror_holds': 'boolean', 'mirror_sentence': 'string'},
     'calculated': {'mirror_holds', 'name', 'mirror_sentence'},
     'lookups': [
        {'field': 'partner_a_label', 'target': 'individuals', 'return': 'label', 'key': 'partner_a', 'match': 'individual_id'},
        {'field': 'partner_b_label', 'target': 'individuals', 'return': 'label', 'key': 'partner_b', 'match': 'individual_id'},
        {'field': 'symmetric_is_on', 'target': 'reasoning', 'return': 'symmetric_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},],
     'aggregations': []},
    {'name': 'Friendships', 'file': 'friendships', 'rulebook_rows': 1,
     'compute': compute_friendships_fields,
     'fields': ['friendship_id', 'name', 'reasoning', 'friend_a', 'friend_b', 'friend_a_label', 'friend_b_label', 'symmetric_is_on', 'mirror_holds', 'mirror_sentence'],
     'datatypes': {'name': 'string', 'friend_a_label': 'string', 'friend_b_label': 'string', 'symmetric_is_on': 'boolean', 'mirror_holds': 'boolean', 'mirror_sentence': 'string'},
     'calculated': {'mirror_holds', 'name', 'mirror_sentence'},
     'lookups': [
        {'field': 'friend_a_label', 'target': 'individuals', 'return': 'label', 'key': 'friend_a', 'match': 'individual_id'},
        {'field': 'friend_b_label', 'target': 'individuals', 'return': 'label', 'key': 'friend_b', 'match': 'individual_id'},
        {'field': 'symmetric_is_on', 'target': 'reasoning', 'return': 'symmetric_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},],
     'aggregations': []},
    {'name': 'Grandparenthoods', 'file': 'grandparenthoods', 'rulebook_rows': 9,
     'compute': compute_grandparenthoods_fields,
     'fields': ['grandparenthood_id', 'name', 'reasoning', 'upper_link', 'lower_link', 'grandparent', 'upper_child', 'lower_parent', 'grandchild', 'grandparent_label', 'grandchild_label', 'links', 'chain_is_on', 'holds'],
     'datatypes': {'name': 'string', 'grandparent': 'string', 'upper_child': 'string', 'lower_parent': 'string', 'grandchild': 'string', 'grandparent_label': 'string', 'grandchild_label': 'string', 'links': 'boolean', 'chain_is_on': 'boolean', 'holds': 'boolean'},
     'calculated': {'links', 'holds', 'name'},
     'lookups': [
        {'field': 'grandparent', 'target': 'parenthoods', 'return': 'parent', 'key': 'upper_link', 'match': 'parenthood_id'},
        {'field': 'upper_child', 'target': 'parenthoods', 'return': 'child', 'key': 'upper_link', 'match': 'parenthood_id'},
        {'field': 'lower_parent', 'target': 'parenthoods', 'return': 'parent', 'key': 'lower_link', 'match': 'parenthood_id'},
        {'field': 'grandchild', 'target': 'parenthoods', 'return': 'child', 'key': 'lower_link', 'match': 'parenthood_id'},
        {'field': 'grandparent_label', 'target': 'parenthoods', 'return': 'parent_label', 'key': 'upper_link', 'match': 'parenthood_id'},
        {'field': 'grandchild_label', 'target': 'parenthoods', 'return': 'child_label', 'key': 'lower_link', 'match': 'parenthood_id'},
        {'field': 'chain_is_on', 'target': 'reasoning', 'return': 'chain_is_on', 'key': 'reasoning', 'match': 'reasoning_id'},],
     'aggregations': []},
    {'name': 'GradeLevelRecords', 'file': 'grade_level_records', 'rulebook_rows': 1,
     'compute': compute_grade_level_records_fields,
     'fields': ['grade_level_record_id', 'name', 'reasoning', 'individual', 'individual_label', 'grade_level', 'stated_seq', 'is_on', 'individual_latest_seq', 'is_current', 'key_is_canonical'],
     'datatypes': {'name': 'string', 'individual_label': 'string', 'individual_latest_seq': 'integer', 'is_current': 'boolean', 'key_is_canonical': 'boolean'},
     'calculated': {'is_current', 'name', 'key_is_canonical'},
     'lookups': [
        {'field': 'individual_label', 'target': 'individuals', 'return': 'label', 'key': 'individual', 'match': 'individual_id'},
        {'field': 'individual_latest_seq', 'target': 'individuals', 'return': 'latest_grade_level_seq', 'key': 'individual', 'match': 'individual_id'},],
     'aggregations': []},
    {'name': 'RoleRecords', 'file': 'role_records', 'rulebook_rows': 1,
     'compute': compute_role_records_fields,
     'fields': ['role_record_id', 'name', 'reasoning', 'individual', 'individual_label', 'role', 'stated_seq', 'is_on', 'individual_latest_seq', 'is_current', 'key_is_canonical'],
     'datatypes': {'name': 'string', 'individual_label': 'string', 'individual_latest_seq': 'integer', 'is_current': 'boolean', 'key_is_canonical': 'boolean'},
     'calculated': {'is_current', 'name', 'key_is_canonical'},
     'lookups': [
        {'field': 'individual_label', 'target': 'individuals', 'return': 'label', 'key': 'individual', 'match': 'individual_id'},
        {'field': 'individual_latest_seq', 'target': 'individuals', 'return': 'latest_role_seq', 'key': 'individual', 'match': 'individual_id'},],
     'aggregations': []},
]

# ERB_CLOSURES materializes each vw_<entity>_closure view aggregations read.
ERB_CLOSURES = [
    {'view': 'vw_parenthoods_closure', 'source': 'parenthoods', 'from': 'parent', 'to': 'child', 'filter': None},
]
