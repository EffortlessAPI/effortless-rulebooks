# Test Results: explain-dag

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 23685 |
| Passed | 20504 |
| Failed | 3181 |
| Score | 86.6% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 20372 | 21468 | 94.9% |
| Lookup (INDEX/MATCH) | 28 | 1897 | 1.5% |
| Aggregation (COUNTIFS/SUMIFS) | 104 | 320 | 32.5% |

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

- Fields: 7/21 (33.3%)
- Computed columns: name, is_override_action, entry_age_hours

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| audit-0001-overdue | name | audit-c-0001-2026-05-22t00-00- | audit-c-0001--status_change |
| audit-0001-overdue | entry_age_hours | 4561 | None |
| audit-0002-hearing-req | name | audit-c-0002-2026-05-25t14-30- | audit-c-0002--status_change |
| audit-0002-hearing-req | entry_age_hours | 4475 | None |
| audit-0002-scheduled | name | audit-c-0002-2026-05-26t09-10- | audit-c-0002--status_change |
| audit-0002-scheduled | entry_age_hours | 4456 | None |
| audit-0003-dismissed | name | audit-c-0003-2026-04-15t15-00- | audit-c-0003--review-decision |
| audit-0003-dismissed | entry_age_hours | 5434 | None |
| audit-0003-notowed-override | name | audit-c-0003-2026-04-15t15-05- | audit-c-0003--override |
| audit-0003-notowed-override | entry_age_hours | 5434 | None |
| audit-0004-late | name | audit-c-0004-2026-03-01t00-00- | audit-c-0004--status_change |
| audit-0004-late | entry_age_hours | 6529 | None |
| audit-0005-paid | name | audit-c-0005-2026-05-18t19-22- | audit-c-0005--status_change |
| audit-0005-paid | entry_age_hours | 4638 | None |

### platform_naviation

- Fields: 514/880 (58.4%)
- Computed columns: name, package_is_active, package_is_licensed, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete, depth, full_path, handler_base_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| nav-admin | package_is_active | True | None |
| nav-admin | package_is_licensed | True | None |
| nav-admin | admin_can_create | True | None |
| nav-admin | admin_can_read | True | None |
| nav-admin | admin_can_update | True | None |
| nav-admin | admin_can_delete | True | None |
| nav-admin-audit-log | package_is_active | True | None |
| nav-admin-audit-log | package_is_licensed | True | None |
| nav-admin-audit-log | admin_can_create | True | None |
| nav-admin-audit-log | admin_can_read | True | None |
| nav-admin-audit-log | admin_can_update | True | None |
| nav-admin-audit-log | admin_can_delete | True | None |
| nav-admin-audit-log | manager_can_read | True | None |
| nav-admin-audit-log | depth | 1 | None |
| nav-admin-branding | package_is_active | True | None |
| nav-admin-branding | package_is_licensed | True | None |
| nav-admin-branding | admin_can_create | True | None |
| nav-admin-branding | admin_can_read | True | None |
| nav-admin-branding | admin_can_update | True | None |
| nav-admin-branding | admin_can_delete | True | None |
| ... | ... | (346 more) | ... |

### jurisdictions

- Fields: 12/21 (57.1%)
- Computed columns: name, parent_jurisdiction_name, is_root_jurisdiction, child_jurisdiction_count, relative_path, rule_count, source_document_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| j-ca-la | is_root_jurisdiction | 1 | True |
| j-ca-la | rule_count | 2 | None |
| j-ca-la | source_document_count | 2 | None |
| j-ny-nyc | is_root_jurisdiction | 1 | True |
| j-ny-nyc | rule_count | 1 | None |
| j-ny-nyc | source_document_count | 1 | None |
| j-tx-aus | is_root_jurisdiction | 1 | True |
| j-tx-aus | rule_count | 2 | None |
| j-tx-aus | source_document_count | 2 | None |

### jurisdiction_source_documents

- Fields: 10/20 (50.0%)
- Computed columns: name, jurisdiction_name, relative_path, rule_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| doc-ca-traffic-school | jurisdiction_name | ca-us | None |
| doc-ca-traffic-school | rule_count | 1 | None |
| doc-ca-vc-deadlines | jurisdiction_name | ca-us | None |
| doc-ca-vc-deadlines | rule_count | 1 | None |
| doc-ny-vtl-points | jurisdiction_name | ny-us | None |
| doc-ny-vtl-points | rule_count | 1 | None |
| doc-tx-aus-appeal-2025-114 | jurisdiction_name | tx-us | None |
| doc-tx-aus-appeal-2025-114 | rule_count | 1 | None |
| doc-tx-transportation-code | jurisdiction_name | tx-us | None |
| doc-tx-transportation-code | rule_count | 1 | None |

### jurisdiction_rules

- Fields: 20/30 (66.7%)
- Computed columns: name, route_path, relative_path, jurisdiction_name, jurisdiction_type, is_federal

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| jr-ca-respond-21 | jurisdiction_name | ca-us | None |
| jr-ca-respond-21 | jurisdiction_type | county | None |
| jr-ca-school-1pt | jurisdiction_name | ca-us | None |
| jr-ca-school-1pt | jurisdiction_type | county | None |
| jr-ny-suspend-11 | jurisdiction_name | ny-us | None |
| jr-ny-suspend-11 | jurisdiction_type | city | None |
| jr-tx-radar-dismiss | jurisdiction_name | tx-us | None |
| jr-tx-radar-dismiss | jurisdiction_type | county | None |
| jr-tx-respond-14 | jurisdiction_name | tx-us | None |
| jr-tx-respond-14 | jurisdiction_type | county | None |

### app_users

- Fields: 14/28 (50.0%)
- Computed columns: name_redacted, email_address_redacted, role_title, role_description

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| admin-ej-ssot-me | role_title | System Administrator | None |
| admin-ej-ssot-me | role_description | System administrator — full ac | None |
| admin-example | role_title | System Administrator | None |
| admin-example | role_description | System administrator — full ac | None |
| eejai42-gmail | role_title | System Administrator | None |
| eejai42-gmail | role_description | System administrator — full ac | None |
| external-llm-example | role_title | External LLM (3rd Party) | None |
| external-llm-example | role_description | A 3rd-party / external LLM int | None |
| manager-example | role_title | Manager / Team Lead | None |
| manager-example | role_description | Manager / team lead — reviews  | None |
| manager-niko-ssot-me | role_title | System Administrator | None |
| manager-niko-ssot-me | role_description | System administrator — full ac | None |
| representative-example | role_title | Representative | None |
| representative-example | role_description | Line agent — owns cases/record | None |

### site_branding

- Fields: 1/1 (100.0%)
- Computed columns: name

### reference_documents

- Fields: 8/8 (100.0%)
- Computed columns: name, is_appeal_board_decision

### state_machines

- Fields: 12/16 (75.0%)
- Computed columns: name, package_is_active, state_count, transition_rule_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| citation-lifecycle | package_is_active | True | None |
| contest-track | package_is_active | True | None |
| license-track | package_is_active | True | None |
| payment-track | package_is_active | True | None |

### machine_states

- Fields: 25/25 (100.0%)
- Computed columns: name

### state_transition_rules

- Fields: 52/104 (50.0%)
- Computed columns: name, from_state_key, to_state_key, is_forward_edge

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| citation-lifecycle--adjudicated->closed | from_state_key | Adjudicated | None |
| citation-lifecycle--adjudicated->closed | to_state_key | Closed | None |
| citation-lifecycle--incontest->adjudicated | from_state_key | InContest | None |
| citation-lifecycle--incontest->adjudicated | to_state_key | Adjudicated | None |
| citation-lifecycle--incontest->dismissed | from_state_key | InContest | None |
| citation-lifecycle--incontest->dismissed | to_state_key | dismissed | None |
| citation-lifecycle--issued->adjudicated | from_state_key | Issued | None |
| citation-lifecycle--issued->adjudicated | to_state_key | Adjudicated | None |
| citation-lifecycle--issued->responded | from_state_key | Issued | None |
| citation-lifecycle--issued->responded | to_state_key | Responded | None |
| citation-lifecycle--responded->incontest | from_state_key | Responded | None |
| citation-lifecycle--responded->incontest | to_state_key | InContest | None |
| contest-track--heard->dismissed | from_state_key | Heard | None |
| contest-track--heard->dismissed | to_state_key | dismissed | None |
| contest-track--heard->reduced | from_state_key | Heard | None |
| contest-track--heard->reduced | to_state_key | reduced | None |
| contest-track--heard->upheld | from_state_key | Heard | None |
| contest-track--heard->upheld | to_state_key | upheld | None |
| contest-track--hearingrequested->scheduled | from_state_key | HearingRequested | None |
| contest-track--hearingrequested->scheduled | to_state_key | Scheduled | None |
| ... | ... | (32 more) | ... |

### state_transitions

- Fields: 40/40 (100.0%)
- Computed columns: name, is_forward

### work_queue_items

- Fields: 4/20 (20.0%)
- Computed columns: name, due_in_days, is_overdue, urgency_bucket, is_urgent

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| wq-citations-tc-2026-0001 | due_in_days | -189 | None |
| wq-citations-tc-2026-0001 | is_overdue | True | False |
| wq-citations-tc-2026-0001 | urgency_bucket | urgent | follow-up |
| wq-citations-tc-2026-0001 | is_urgent | True | False |
| wq-citations-tc-2026-0002 | due_in_days | -160 | None |
| wq-citations-tc-2026-0002 | is_overdue | True | False |
| wq-citations-tc-2026-0002 | urgency_bucket | urgent | follow-up |
| wq-citations-tc-2026-0002 | is_urgent | True | False |
| wq-citations-tc-2026-0004 | due_in_days | -226 | None |
| wq-citations-tc-2026-0004 | is_overdue | True | False |
| wq-citations-tc-2026-0004 | urgency_bucket | urgent | follow-up |
| wq-citations-tc-2026-0004 | is_urgent | True | False |
| wq-citations-tc-2026-0006 | due_in_days | -156 | None |
| wq-citations-tc-2026-0006 | is_overdue | True | False |
| wq-citations-tc-2026-0006 | urgency_bucket | urgent | follow-up |
| wq-citations-tc-2026-0006 | is_urgent | True | False |

### ai_models

- Fields: 4/12 (33.3%)
- Computed columns: name, pricing_version_count, turn_count, total_cost

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| erb-internal-classifier | pricing_version_count | 1 | None |
| erb-internal-classifier | turn_count | 1 | None |
| gpt-4o | pricing_version_count | 1 | None |
| gpt-4o | turn_count | 1 | None |
| gpt-4o | total_cost | 0.007575 | None |
| gpt-4o-mini | pricing_version_count | 2 | None |
| gpt-4o-mini | turn_count | 3 | None |
| gpt-4o-mini | total_cost | 0.000696 | None |

### model_pricing_versions

- Fields: 5/12 (41.7%)
- Computed columns: name, ai_model_title, turn_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| erb-internal-classifier-2026-01 | ai_model_title | ERB Internal Classifier | None |
| erb-internal-classifier-2026-01 | turn_count | 1 | None |
| gpt-4o-2024-08 | ai_model_title | GPT-4o | None |
| gpt-4o-2024-08 | turn_count | 1 | None |
| gpt-4o-mini-2024-07 | ai_model_title | GPT-4o mini | None |
| gpt-4o-mini-2025-07 | ai_model_title | GPT-4o mini | None |
| gpt-4o-mini-2025-07 | turn_count | 3 | None |

### assistant_turns

- Fields: 11/50 (22.0%)
- Computed columns: name, total_tokens, billable_input_tokens, ai_model_title, input_price_per_m_tok, cached_input_price_per_m_tok, output_price_per_m_tok, input_cost, output_cost, total_cost

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| turn-0002-deadline | total_tokens | 1990 | None |
| turn-0002-deadline | billable_input_tokens | 650 | None |
| turn-0002-deadline | ai_model_title | GPT-4o mini | None |
| turn-0002-deadline | input_price_per_m_tok | 0.15 | None |
| turn-0002-deadline | cached_input_price_per_m_tok | 0.075 | None |
| turn-0002-deadline | output_price_per_m_tok | 0.6 | None |
| turn-0002-deadline | input_cost | 0.0001875 | None |
| turn-0002-deadline | output_cost | 8.4e-05 | None |
| turn-0002-deadline | total_cost | 0.0002715 | None |
| turn-0003-dismiss-basis | total_tokens | 3320 | None |
| turn-0003-dismiss-basis | billable_input_tokens | 1200 | None |
| turn-0003-dismiss-basis | ai_model_title | GPT-4o | None |
| turn-0003-dismiss-basis | input_price_per_m_tok | 2.5 | None |
| turn-0003-dismiss-basis | cached_input_price_per_m_tok | 1.25 | None |
| turn-0003-dismiss-basis | output_price_per_m_tok | 10 | None |
| turn-0003-dismiss-basis | input_cost | 0.005375 | None |
| turn-0003-dismiss-basis | output_cost | 0.0022 | None |
| turn-0003-dismiss-basis | total_cost | 0.007575 | None |
| turn-0004-penalty | total_tokens | 1860 | None |
| turn-0004-penalty | billable_input_tokens | 600 | None |
| ... | ... | (19 more) | ... |

### platforms

- Fields: 1/1 (100.0%)
- Computed columns: name

### erb_packages

- Fields: 6/18 (33.3%)
- Computed columns: name, feature_count, shipped_feature_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| access-control | feature_count | 6 | None |
| access-control | shipped_feature_count | 55 | None |
| assistant | feature_count | 6 | None |
| assistant | shipped_feature_count | 55 | None |
| citations | feature_count | 6 | None |
| citations | shipped_feature_count | 55 | None |
| core | feature_count | 6 | None |
| core | shipped_feature_count | 55 | None |
| platform-meta | feature_count | 6 | None |
| platform-meta | shipped_feature_count | 55 | None |
| state-machines | feature_count | 6 | None |
| state-machines | shipped_feature_count | 55 | None |

### erb_feature_statuses

- Fields: 10/10 (100.0%)
- Computed columns: name, feature_count

### erb_feature_categories

- Fields: 36/36 (100.0%)
- Computed columns: name, feature_count

### erb_features

- Fields: 89/600 (14.8%)
- Computed columns: name, route_path, relative_path, package_is_licensed, category_title, status_title, status_description, status_is_active

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ai-assistant | route_path | /assistant | None |
| ai-assistant | relative_path | /assistant | None |
| ai-assistant | package_is_licensed | True | None |
| ai-assistant | category_title | AI Assistant | None |
| ai-assistant | status_title | In Progress | None |
| ai-assistant | status_description | Actively being built. | None |
| ai-assistant | status_is_active | True | None |
| as-cost-dashboard | route_path | /assistant/costs | None |
| as-cost-dashboard | relative_path | /assistant/costs | None |
| as-cost-dashboard | package_is_licensed | True | None |
| as-cost-dashboard | category_title | Dashboards | None |
| as-cost-dashboard | status_title | Shipped | None |
| as-cost-dashboard | status_description | Delivered and live. | None |
| as-cost-dashboard | status_is_active | True | None |
| as-cost-telemetry | route_path | /assistant/costs | None |
| as-cost-telemetry | relative_path | /assistant/costs | None |
| as-cost-telemetry | package_is_licensed | True | None |
| as-cost-telemetry | category_title | Data Model | None |
| as-cost-telemetry | status_title | Shipped | None |
| as-cost-telemetry | status_description | Delivered and live. | None |
| ... | ... | (491 more) | ... |

### erb_tables

- Fields: 371/1045 (35.5%)
- Computed columns: name, package_is_active, package_is_licensed, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| APIEndpoints | package_is_active | True | None |
| APIEndpoints | package_is_licensed | True | None |
| APIEndpoints | admin_can_create | True | None |
| APIEndpoints | admin_can_read | True | None |
| APIEndpoints | admin_can_update | True | None |
| APIEndpoints | admin_can_delete | True | None |
| APIEndpoints | manager_can_create | True | None |
| APIEndpoints | manager_can_read | True | None |
| APIEndpoints | manager_can_update | True | None |
| APIEndpoints | manager_can_delete | True | None |
| APIEndpoints | representative_can_read | True | None |
| APIEndpoints | external_llm_can_read | True | None |
| AiModels | package_is_active | True | None |
| AiModels | package_is_licensed | True | None |
| AiModels | admin_can_create | True | None |
| AiModels | admin_can_read | True | None |
| AiModels | admin_can_update | True | None |
| AiModels | admin_can_delete | True | None |
| AiModels | manager_can_create | True | None |
| AiModels | manager_can_read | True | None |
| ... | ... | (654 more) | ... |

### erb_fields

- Fields: 17640/18620 (94.7%)
- Computed columns: name, table_package_is_active, is_calculated, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| APIEndpoints.APIEndpointId | table_package_is_active | True | None |
| APIEndpoints.CreatedAt | table_package_is_active | True | None |
| APIEndpoints.CreatedBy | table_package_is_active | True | None |
| APIEndpoints.Description | table_package_is_active | True | None |
| APIEndpoints.EndpointType | table_package_is_active | True | None |
| APIEndpoints.HttpMethod | table_package_is_active | True | None |
| APIEndpoints.ModifiedAt | table_package_is_active | True | None |
| APIEndpoints.ModifiedBy | table_package_is_active | True | None |
| APIEndpoints.ModifiedByModel | table_package_is_active | True | None |
| APIEndpoints.Name | table_package_is_active | True | None |
| APIEndpoints.Path | table_package_is_active | True | None |
| APIEndpoints.RoleVisibility | table_package_is_active | True | None |
| APIEndpoints.Status | table_package_is_active | True | None |
| APIEndpoints.SubjectTableName | table_package_is_active | True | None |
| APIEndpoints.Title | table_package_is_active | True | None |
| APIEndpoints.TriggersStateMachine | table_package_is_active | True | None |
| APIEndpoints.WhereClause | table_package_is_active | True | None |
| AiModels.AiModelId | table_package_is_active | True | None |
| AiModels.AssistantTurns | table_package_is_active | True | None |
| AiModels.ContextWindowTokens | table_package_is_active | True | None |
| ... | ... | (960 more) | ... |

### api_endpoints

- Fields: 36/36 (100.0%)
- Computed columns: name

### subject_state_instances

- Fields: 25/33 (75.8%)
- Computed columns: name, is_current, has_complete_lineage

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| d1234567--license-track--pointsassessed | is_current | True | None |
| ssi-citations-tc-2026-0002-incontest-3 | is_current | True | None |
| ssi-citations-tc-2026-0003-closed-2 | is_current | True | None |
| ssi-citations-tc-2026-0003-heard-1 | is_current | True | None |
| ssi-citations-tc-2026-0004-late-1 | is_current | True | None |
| ssi-citations-tc-2026-0005-paid-1 | is_current | True | None |
| tc-2026-0001--payment-track--paid | is_current | True | None |
| tc-2026-0002--contest-track--scheduled | is_current | True | None |

### violation_types

- Fields: 18/35 (51.4%)
- Computed columns: name, jurisdiction_label, traffic_school_point_cap, is_school_eligible_by_cap, count_of_citations

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| vt-cvc-21453a | jurisdiction_label | Los Angeles County, California | None |
| vt-cvc-21453a | traffic_school_point_cap | 1 | None |
| vt-cvc-21453a | is_school_eligible_by_cap | True | False |
| vt-cvc-22350 | jurisdiction_label | Los Angeles County, California | None |
| vt-cvc-22350 | traffic_school_point_cap | 1 | None |
| vt-cvc-22350 | is_school_eligible_by_cap | True | False |
| vt-cvc-23123 | jurisdiction_label | Los Angeles County, California | None |
| vt-cvc-23123 | traffic_school_point_cap | 1 | None |
| vt-cvc-23123 | is_school_eligible_by_cap | True | False |
| vt-ttc-544007 | jurisdiction_label | Travis County, Texas | None |
| vt-ttc-544007 | traffic_school_point_cap | 2 | None |
| vt-ttc-544007 | is_school_eligible_by_cap | True | False |
| vt-ttc-545351 | jurisdiction_label | Travis County, Texas | None |
| vt-ttc-545351 | traffic_school_point_cap | 2 | None |
| vt-ttc-545351 | is_school_eligible_by_cap | True | False |
| vt-vtl-1111d | jurisdiction_label | New York City, New York | None |
| vt-vtl-1180b | jurisdiction_label | New York City, New York | None |

### drivers

- Fields: 30/48 (62.5%)
- Computed columns: name, full_name, home_jurisdiction_label, suspension_threshold, warning_threshold, count_of_citations, active_points, license_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| d-d1234567 | home_jurisdiction_label | Los Angeles County, California | None |
| d-d1234567 | suspension_threshold | 4 | None |
| d-d1234567 | warning_threshold | 3 | None |
| d-d1357924 | home_jurisdiction_label | Travis County, Texas | None |
| d-d1357924 | suspension_threshold | 6 | None |
| d-d1357924 | warning_threshold | 4 | None |
| d-d2468013 | home_jurisdiction_label | Los Angeles County, California | None |
| d-d2468013 | suspension_threshold | 4 | None |
| d-d2468013 | warning_threshold | 3 | None |
| d-d7654321 | home_jurisdiction_label | Travis County, Texas | None |
| d-d7654321 | suspension_threshold | 6 | None |
| d-d7654321 | warning_threshold | 4 | None |
| d-n0099887 | home_jurisdiction_label | New York City, New York | None |
| d-n0099887 | suspension_threshold | 11 | None |
| d-n0099887 | warning_threshold | 6 | None |
| d-n5544332 | home_jurisdiction_label | New York City, New York | None |
| d-n5544332 | suspension_threshold | 11 | None |
| d-n5544332 | warning_threshold | 6 | None |

### citations

- Fields: 114/312 (36.5%)
- Computed columns: name, driver_label, violation_label, jurisdiction_label, base_fine_usd, violation_points, days_to_respond, days_to_pay_after_ruling, late_penalty_pct, days_late_to_collections, response_due_date, days_until_response_due, is_response_overdue, count_of_hearings, latest_hearing_outcome, contest_status, is_dismissed, is_guilty, amount_due_usd, payment_due_date, is_payment_late, collections_deadline, is_in_collections, payment_status, effective_points, citation_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| c-0001 | driver_label | Doe, Jane | None |
| c-0001 | violation_label | Unsafe speed for conditions (b | None |
| c-0001 | jurisdiction_label | Los Angeles County, California | None |
| c-0001 | base_fine_usd | 238.0 | None |
| c-0001 | violation_points | 1 | None |
| c-0001 | days_to_respond | 21 | None |
| c-0001 | days_to_pay_after_ruling | 30 | None |
| c-0001 | late_penalty_pct | 0.25 | None |
| c-0001 | days_late_to_collections | 60 | None |
| c-0001 | response_due_date | 2026-05-22 | None |
| c-0001 | days_until_response_due | -15 | None |
| c-0001 | is_response_overdue | True | False |
| c-0001 | is_guilty | True | False |
| c-0001 | amount_due_usd | 238.0 | None |
| c-0001 | payment_due_date | 2026-06-21 | None |
| c-0001 | collections_deadline | 2026-08-20 | None |
| c-0001 | payment_status | Due | Pending |
| c-0001 | effective_points | 1 | 0 |
| c-0001 | citation_status | Adjudicated | Issued |
| c-0002 | driver_label | Doe, Jane | None |
| ... | ... | (178 more) | ... |

### hearings

- Fields: 5/10 (50.0%)
- Computed columns: name, citation_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| h-0001 | citation_label | TC-2026-0002 | None |
| h-0002 | citation_label | TC-2026-0003 | None |
| h-0003 | citation_label | TC-2026-0007 | None |
| h-0004 | citation_label | TC-2026-0010 | None |
| h-0005 | citation_label | TC-2026-0012 | None |

### payments

- Fields: 3/6 (50.0%)
- Computed columns: name, citation_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| p-0001 | citation_label | TC-2026-0005 | None |
| p-0002 | citation_label | TC-2026-0009 | None |
| p-0003 | citation_label | TC-2026-0001 | None |

### case_events

- Fields: 12/24 (50.0%)
- Computed columns: name, citation_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ev-0001 | citation_label | TC-2026-0001 | None |
| ev-0002 | citation_label | TC-2026-0002 | None |
| ev-0003 | citation_label | TC-2026-0002 | None |
| ev-0004 | citation_label | TC-2026-0002 | None |
| ev-0005 | citation_label | TC-2026-0003 | None |
| ev-0006 | citation_label | TC-2026-0003 | None |
| ev-0007 | citation_label | TC-2026-0005 | None |
| ev-0008 | citation_label | TC-2026-0007 | None |
| ev-0009 | citation_label | TC-2026-0007 | None |
| ev-0010 | citation_label | TC-2026-0009 | None |
| ev-0011 | citation_label | TC-2026-0010 | None |
| ev-0012 | citation_label | TC-2026-0012 | None |

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

- Fields: 194/388 (50.0%)
- Computed columns: name, expectation_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tc-endpoint-assistant-ask | expectation_count | 351 | None |
| tc-endpoint-case-timeline-report | expectation_count | 351 | None |
| tc-endpoint-citation-advance-state | expectation_count | 351 | None |
| tc-endpoint-citation-contest | expectation_count | 351 | None |
| tc-endpoint-citation-dismiss | expectation_count | 351 | None |
| tc-endpoint-citation-import | expectation_count | 351 | None |
| tc-endpoint-citation-ocr | expectation_count | 351 | None |
| tc-endpoint-citation-request-hearing | expectation_count | 351 | None |
| tc-endpoint-citation-respond | expectation_count | 351 | None |
| tc-endpoint-citation-withdraw-contest | expectation_count | 351 | None |
| tc-endpoint-collections-sweep | expectation_count | 351 | None |
| tc-endpoint-config-drift-check | expectation_count | 351 | None |
| tc-endpoint-config-save | expectation_count | 351 | None |
| tc-endpoint-cost-rollup | expectation_count | 351 | None |
| tc-endpoint-deadline-report | expectation_count | 351 | None |
| tc-endpoint-determination-issue | expectation_count | 351 | None |
| tc-endpoint-determination-letter-generate | expectation_count | 351 | None |
| tc-endpoint-driver-lookup | expectation_count | 351 | None |
| tc-endpoint-feature-toggle-license | expectation_count | 351 | None |
| tc-endpoint-hearing-prep | expectation_count | 351 | None |
| ... | ... | (174 more) | ... |

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
