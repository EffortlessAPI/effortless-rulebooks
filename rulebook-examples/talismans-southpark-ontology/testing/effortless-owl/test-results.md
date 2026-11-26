# Test Results: effortless-owl

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 31456 |
| Passed | 31456 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 10s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 13211 | 13211 | 100.0% |
| Lookup (INDEX/MATCH) | 15487 | 15487 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 2758 | 2758 | 100.0% |

## Results by Entity

### graphs

- Fields: 15/15 (100.0%)
- Computed columns: name, count_of_stated_facts, count_of_conclusions, count_of_violations, is_consistent, conclusions_message, count_of_facts_to_add, has_facts_to_add, next_fact_sort_order, max_activation_seq, next_activation_seq, max_listed_sort_order, next_listed_sort_order, max_sort_order, next_candidate_sort_order

### rules

- Fields: 10/10 (100.0%)
- Computed columns: name, count_of_firings

### firing_kinds

- Fields: 12/12 (100.0%)
- Computed columns: name, rule_enabled

### things

- Fields: 135/135 (100.0%)
- Computed columns: portrait_name, portrait_page, portrait_image, portrait_crop, portrait_width, portrait_height, name, node_key, count_of_children, count_of_grandchildren, count_as_place, count_as_subject, count_as_object, is_in_graph, node_kind

### portraits

- Fields: 13/13 (100.0%)
- Computed columns: name

### portrait_aliases

- Fields: 32/32 (100.0%)
- Computed columns: name

### properties

- Fields: 10/10 (100.0%)
- Computed columns: name

### property_axioms

- Fields: 4/4 (100.0%)
- Computed columns: name

### statements

- Fields: 13500/13500 (100.0%)
- Computed columns: subject_label, property_label, object_label, property_datatype, property_has_one_current_value, property_domain_class, property_range_class, literal_quote, literal_type_marker, built_in_source_text, user_source_text, next_fact_sort_order, is_literal, object_text, name, words, source, subject_property_key, latest_sibling_activation_seq, is_in_effect, listed_unit, in_effect_unit, listed_off_unit, is_next_to_add, count_of_inverse_firings, count_of_symmetric_firings, holds_before_chain, count_of_chain_firings, holds_before_typing, count_of_type_firings, holds, is_conclusion, holds_unit, conclusion_unit, parent_unit, grandparent_unit, place_unit, first_pre_chain_order_key, snap_position, first_fired_order_key, count_of_chosen_firings, subject_node_key, object_node_key, subject_node_kind, object_thing_node_kind, object_node_kind, object_value_node_label, count_of_violations_as_first, count_of_violations_as_second, is_in_contradiction

### firings

- Fields: 17205/17205 (100.0%)
- Computed columns: rule_enabled, kind_wording, kind_sort_order, target_name, target_subject_label, target_property_label, target_object_text, target_is_conclusion, target_subject_node_key, target_object_node_key, target_first_fired_order_key, first_words, second_words, first_is_in_effect, first_holds_before_chain, second_holds_before_chain, first_holds_before_typing, first_property, first_property_domain_class, first_property_range_class, first_sort_order, second_is_in_effect, first_snap_position, second_snap_position, inverse_fired_unit, symmetric_fired_unit, chain_fired_unit, type_fired_unit, fired_unit, pre_chain_fired_unit, pre_chain_order_key, round, order_key, is_chosen, chosen_unit, card_text, name

### disjoint_checks

- Fields: 336/336 (100.0%)
- Computed columns: rule_enabled, first_holds, second_holds, subject_label, object_label, violation_unit, alarm_text, name

### scenarios

- Fields: 21/21 (100.0%)
- Computed columns: name, graph_has_facts_to_add, is_available

### scenario_fact_changes

- Fields: 6/6 (100.0%)
- Computed columns: name

### scenario_rule_settings

- Fields: 10/10 (100.0%)
- Computed columns: name

### node_layouts

- Fields: 26/26 (100.0%)
- Computed columns: name

### layout_slots

- Fields: 8/8 (100.0%)
- Computed columns: name

### page_texts

- Fields: 102/102 (100.0%)
- Computed columns: name

### engines

- Fields: 3/3 (100.0%)
- Computed columns: name

### rulebook_tools

- Fields: 8/8 (100.0%)
- Computed columns: name
