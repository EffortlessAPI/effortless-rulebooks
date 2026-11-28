# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 23685 |
| Passed | 23685 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 7s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 21468 | 21468 | 100.0% |
| Lookup (INDEX/MATCH) | 1897 | 1897 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 320 | 320 | 100.0% |

## Results by Entity

### business_rules

- Fields: 57/57 (100.0%)
- Computed columns: name

### business_rule_categories

- Fields: 30/30 (100.0%)
- Computed columns: name, rule_count

### glossary_categories

- Fields: 12/12 (100.0%)
- Computed columns: name, term_count

### glossary_terms

- Fields: 36/36 (100.0%)
- Computed columns: name

### roles

- Fields: 8/8 (100.0%)
- Computed columns: name, app_user_count

### audit_log_entries

- Fields: 21/21 (100.0%)
- Computed columns: name, is_override_action, entry_age_hours

### platform_naviation

- Fields: 880/880 (100.0%)
- Computed columns: name, package_is_active, package_is_licensed, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete, depth, full_path, handler_base_name

### jurisdictions

- Fields: 21/21 (100.0%)
- Computed columns: name, parent_jurisdiction_name, is_root_jurisdiction, child_jurisdiction_count, relative_path, rule_count, source_document_count

### jurisdiction_source_documents

- Fields: 20/20 (100.0%)
- Computed columns: name, jurisdiction_name, relative_path, rule_count

### jurisdiction_rules

- Fields: 30/30 (100.0%)
- Computed columns: name, route_path, relative_path, jurisdiction_name, jurisdiction_type, is_federal

### app_users

- Fields: 28/28 (100.0%)
- Computed columns: name_redacted, email_address_redacted, role_title, role_description

### site_branding

- Fields: 1/1 (100.0%)
- Computed columns: name

### reference_documents

- Fields: 8/8 (100.0%)
- Computed columns: name, is_appeal_board_decision

### state_machines

- Fields: 16/16 (100.0%)
- Computed columns: name, package_is_active, state_count, transition_rule_count

### machine_states

- Fields: 25/25 (100.0%)
- Computed columns: name

### state_transition_rules

- Fields: 104/104 (100.0%)
- Computed columns: name, from_state_key, to_state_key, is_forward_edge

### state_transitions

- Fields: 40/40 (100.0%)
- Computed columns: name, is_forward

### work_queue_items

- Fields: 20/20 (100.0%)
- Computed columns: name, due_in_days, is_overdue, urgency_bucket, is_urgent

### ai_models

- Fields: 12/12 (100.0%)
- Computed columns: name, pricing_version_count, turn_count, total_cost

### model_pricing_versions

- Fields: 12/12 (100.0%)
- Computed columns: name, ai_model_title, turn_count

### assistant_turns

- Fields: 50/50 (100.0%)
- Computed columns: name, total_tokens, billable_input_tokens, ai_model_title, input_price_per_m_tok, cached_input_price_per_m_tok, output_price_per_m_tok, input_cost, output_cost, total_cost

### platforms

- Fields: 1/1 (100.0%)
- Computed columns: name

### erb_packages

- Fields: 18/18 (100.0%)
- Computed columns: name, feature_count, shipped_feature_count

### erb_feature_statuses

- Fields: 10/10 (100.0%)
- Computed columns: name, feature_count

### erb_feature_categories

- Fields: 36/36 (100.0%)
- Computed columns: name, feature_count

### erb_features

- Fields: 600/600 (100.0%)
- Computed columns: name, route_path, relative_path, package_is_licensed, category_title, status_title, status_description, status_is_active

### erb_tables

- Fields: 1045/1045 (100.0%)
- Computed columns: name, package_is_active, package_is_licensed, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete

### erb_fields

- Fields: 18620/18620 (100.0%)
- Computed columns: name, table_package_is_active, is_calculated, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete

### api_endpoints

- Fields: 36/36 (100.0%)
- Computed columns: name

### subject_state_instances

- Fields: 33/33 (100.0%)
- Computed columns: name, is_current, has_complete_lineage

### violation_types

- Fields: 35/35 (100.0%)
- Computed columns: name, jurisdiction_label, traffic_school_point_cap, is_school_eligible_by_cap, count_of_citations

### drivers

- Fields: 48/48 (100.0%)
- Computed columns: name, full_name, home_jurisdiction_label, suspension_threshold, warning_threshold, count_of_citations, active_points, license_status

### citations

- Fields: 312/312 (100.0%)
- Computed columns: name, driver_label, violation_label, jurisdiction_label, base_fine_usd, violation_points, days_to_respond, days_to_pay_after_ruling, late_penalty_pct, days_late_to_collections, response_due_date, days_until_response_due, is_response_overdue, count_of_hearings, latest_hearing_outcome, contest_status, is_dismissed, is_guilty, amount_due_usd, payment_due_date, is_payment_late, collections_deadline, is_in_collections, payment_status, effective_points, citation_status

### hearings

- Fields: 10/10 (100.0%)
- Computed columns: name, citation_label

### payments

- Fields: 6/6 (100.0%)
- Computed columns: name, citation_label

### case_events

- Fields: 24/24 (100.0%)
- Computed columns: name, citation_label

### test_category

- Fields: 7/7 (100.0%)
- Computed columns: name

### test_surface

- Fields: 5/5 (100.0%)
- Computed columns: name

### test_technology

- Fields: 4/4 (100.0%)
- Computed columns: name

### test_case

- Fields: 388/388 (100.0%)
- Computed columns: name, expectation_count

### test_expectation

- Fields: 351/351 (100.0%)
- Computed columns: name

### screen_layouts

- Fields: 40/40 (100.0%)
- Computed columns: name

### screen_sections

- Fields: 88/88 (100.0%)
- Computed columns: name

### field_display_hints

- Fields: 485/485 (100.0%)
- Computed columns: name

### fee_schedules

- Fields: 21/21 (100.0%)
- Computed columns: name

### deadline_rules

- Fields: 9/9 (100.0%)
- Computed columns: name

### contest_grounds

- Fields: 14/14 (100.0%)
- Computed columns: name

### driver_license_points

- Fields: 3/3 (100.0%)
- Computed columns: name

### build_phases

- Fields: 5/5 (100.0%)
- Computed columns: name
