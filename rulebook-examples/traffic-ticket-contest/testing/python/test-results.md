# Test Results: python

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 23685 |
| Passed | 17450 |
| Failed | 6235 |
| Score | 73.7% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 17297 | 21468 | 80.6% |
| Lookup (INDEX/MATCH) | 42 | 1897 | 2.2% |
| Aggregation (COUNTIFS/SUMIFS) | 111 | 320 | 34.7% |

## Results by Entity

### business_rules

- Fields: 0/57 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| TT-DETERMINATION-01 | name | tt-determination-01 | None |
| TT-DETERMINATION-02 | name | tt-determination-02 | None |
| TT-STATE-01 | name | tt-state-01 | None |
| rule-r1 | name | r1 | None |
| rule-r10 | name | r10 | None |
| rule-r11 | name | r11 | None |
| rule-r12 | name | r12 | None |
| rule-r13 | name | r13 | None |
| rule-r2 | name | r2 | None |
| rule-r20 | name | r20 | None |
| rule-r21 | name | r21 | None |
| rule-r22 | name | r22 | None |
| rule-r23 | name | r23 | None |
| rule-r3 | name | r3 | None |
| rule-r30 | name | r30 | None |
| rule-r31 | name | r31 | None |
| rule-r40 | name | r40 | None |
| rule-r50 | name | r50 | None |
| rule-r51 | name | r51 | None |
| rule-r60 | name | r60 | None |
| ... | ... | (37 more) | ... |

### business_rule_categories

- Fields: 15/30 (50.0%)
- Computed columns: name, rule_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assistant | name | assistant | None |
| audit | name | audit | None |
| auth | name | auth | None |
| citation-lifecycle | name | citation-lifecycle | None |
| contest | name | contest | None |
| glossary | name | glossary | None |
| intake | name | intake | None |
| license | name | license | None |
| licensing | name | licensing | None |
| navigation | name | navigation | None |
| payment | name | payment | None |
| platform | name | platform | None |
| rbac | name | rbac | None |
| rules-engine | name | rules-engine | None |
| workqueue | name | workqueue | None |

### glossary_categories

- Fields: 6/12 (50.0%)
- Computed columns: name, term_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| analytics | name | analytics | None |
| legal | name | legal | None |
| lifecycle | name | lifecycle | None |
| money | name | money | None |
| people | name | people | None |
| system | name | system | None |

### glossary_terms

- Fields: 0/36 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| term-adjudication | name | adjudication | None |
| term-assistant | name | portal-assistant | None |
| term-business-rule | name | business-rule | None |
| term-citation | name | citation | None |
| term-citation-status | name | citation-status | None |
| term-collections | name | collections | None |
| term-contest-ground | name | contest-ground | None |
| term-contest-status | name | contest-status | None |
| term-cost-telemetry | name | cost-telemetry | None |
| term-deadline-rule | name | deadline-rule | None |
| term-default-judgment | name | default-judgment | None |
| term-determination | name | determination | None |
| term-determination-letter | name | determination-letter | None |
| term-driver | name | driver | None |
| term-driver-portal | name | driver-portal | None |
| term-explainer-dag | name | explainer-dag | None |
| term-fee-schedule | name | fee-schedule | None |
| term-fine | name | fine | None |
| term-hearing | name | hearing | None |
| term-jurisdiction | name | jurisdiction | None |
| ... | ... | (16 more) | ... |

### roles

- Fields: 4/8 (50.0%)
- Computed columns: name, app_user_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| admin | name | admin | None |
| external-llm | name | external-llm | None |
| manager | name | manager | None |
| representative | name | representative | None |

### audit_log_entries

- Fields: 6/21 (28.6%)
- Computed columns: name, is_override_action, entry_age_hours

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| audit-0001-overdue | name | audit-c-0001-2026-05-22t00-00- | None |
| audit-0001-overdue | entry_age_hours | 4561 | None |
| audit-0002-hearing-req | name | audit-c-0002-2026-05-25t14-30- | None |
| audit-0002-hearing-req | entry_age_hours | 4475 | None |
| audit-0002-scheduled | name | audit-c-0002-2026-05-26t09-10- | None |
| audit-0002-scheduled | entry_age_hours | 4456 | None |
| audit-0003-dismissed | name | audit-c-0003-2026-04-15t15-00- | None |
| audit-0003-dismissed | entry_age_hours | 5434 | None |
| audit-0003-notowed-override | name | audit-c-0003-2026-04-15t15-05- | None |
| audit-0003-notowed-override | is_override_action | True | None |
| audit-0003-notowed-override | entry_age_hours | 5434 | None |
| audit-0004-late | name | audit-c-0004-2026-03-01t00-00- | None |
| audit-0004-late | entry_age_hours | 6529 | None |
| audit-0005-paid | name | audit-c-0005-2026-05-18t19-22- | None |
| audit-0005-paid | entry_age_hours | 4638 | None |

### platform_naviation

- Fields: 394/880 (44.8%)
- Computed columns: name, package_is_active, package_is_licensed, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete, depth, full_path, handler_base_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| nav-admin | name | admin | None |
| nav-admin | package_is_active | True | None |
| nav-admin | package_is_licensed | True | None |
| nav-admin | admin_can_create | True | None |
| nav-admin | admin_can_read | True | None |
| nav-admin | admin_can_update | True | None |
| nav-admin | admin_can_delete | True | None |
| nav-admin | full_path | /admin | None |
| nav-admin | handler_base_name | admin | None |
| nav-admin-audit-log | name | audit-log | None |
| nav-admin-audit-log | package_is_active | True | None |
| nav-admin-audit-log | package_is_licensed | True | None |
| nav-admin-audit-log | admin_can_create | True | None |
| nav-admin-audit-log | admin_can_read | True | None |
| nav-admin-audit-log | admin_can_update | True | None |
| nav-admin-audit-log | admin_can_delete | True | None |
| nav-admin-audit-log | manager_can_read | True | None |
| nav-admin-audit-log | depth | 1 | None |
| nav-admin-audit-log | full_path | /admin/audit-log | None |
| nav-admin-audit-log | handler_base_name | admin audit log | None |
| ... | ... | (466 more) | ... |

### jurisdictions

- Fields: 6/21 (28.6%)
- Computed columns: name, parent_jurisdiction_name, is_root_jurisdiction, child_jurisdiction_count, relative_path, rule_count, source_document_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| j-ca-la | name | ca-us | None |
| j-ca-la | is_root_jurisdiction | 1 | None |
| j-ca-la | relative_path | /library/jurisdictions/j-ca-la | None |
| j-ca-la | rule_count | 2 | None |
| j-ca-la | source_document_count | 2 | None |
| j-ny-nyc | name | ny-us | None |
| j-ny-nyc | is_root_jurisdiction | 1 | None |
| j-ny-nyc | relative_path | /library/jurisdictions/j-ny-ny | None |
| j-ny-nyc | rule_count | 1 | None |
| j-ny-nyc | source_document_count | 1 | None |
| j-tx-aus | name | tx-us | None |
| j-tx-aus | is_root_jurisdiction | 1 | None |
| j-tx-aus | relative_path | /library/jurisdictions/j-tx-au | None |
| j-tx-aus | rule_count | 2 | None |
| j-tx-aus | source_document_count | 2 | None |

### jurisdiction_source_documents

- Fields: 0/20 (0.0%)
- Computed columns: name, jurisdiction_name, relative_path, rule_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| doc-ca-traffic-school | name | los-angeles-county-traffic-sch | None |
| doc-ca-traffic-school | jurisdiction_name | ca-us | None |
| doc-ca-traffic-school | relative_path | /library/jurisdiction-docs/doc | None |
| doc-ca-traffic-school | rule_count | 1 | None |
| doc-ca-vc-deadlines | name | california-vehicle-code-—-arra | None |
| doc-ca-vc-deadlines | jurisdiction_name | ca-us | None |
| doc-ca-vc-deadlines | relative_path | /library/jurisdiction-docs/doc | None |
| doc-ca-vc-deadlines | rule_count | 1 | None |
| doc-ny-vtl-points | name | new-york-vehicle-&-traffic-law | None |
| doc-ny-vtl-points | jurisdiction_name | ny-us | None |
| doc-ny-vtl-points | relative_path | /library/jurisdiction-docs/doc | None |
| doc-ny-vtl-points | rule_count | 1 | None |
| doc-tx-aus-appeal-2025-114 | name | travis-county-appeal-board-dec | None |
| doc-tx-aus-appeal-2025-114 | jurisdiction_name | tx-us | None |
| doc-tx-aus-appeal-2025-114 | relative_path | /library/jurisdiction-docs/doc | None |
| doc-tx-aus-appeal-2025-114 | rule_count | 1 | None |
| doc-tx-transportation-code | name | texas-transportation-code-—-sp | None |
| doc-tx-transportation-code | jurisdiction_name | tx-us | None |
| doc-tx-transportation-code | relative_path | /library/jurisdiction-docs/doc | None |
| doc-tx-transportation-code | rule_count | 1 | None |

### jurisdiction_rules

- Fields: 10/30 (33.3%)
- Computed columns: name, route_path, relative_path, jurisdiction_name, jurisdiction_type, is_federal

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| jr-ca-respond-21 | name | ca-vc-§40519 | None |
| jr-ca-respond-21 | relative_path | /library/jurisdiction-rules/jr | None |
| jr-ca-respond-21 | jurisdiction_name | ca-us | None |
| jr-ca-respond-21 | jurisdiction_type | county | None |
| jr-ca-school-1pt | name | ca-la-ts-1 | None |
| jr-ca-school-1pt | relative_path | /library/jurisdiction-rules/jr | None |
| jr-ca-school-1pt | jurisdiction_name | ca-us | None |
| jr-ca-school-1pt | jurisdiction_type | county | None |
| jr-ny-suspend-11 | name | ny-vtl-§510 | None |
| jr-ny-suspend-11 | relative_path | /library/jurisdiction-rules/jr | None |
| jr-ny-suspend-11 | jurisdiction_name | ny-us | None |
| jr-ny-suspend-11 | jurisdiction_type | city | None |
| jr-tx-radar-dismiss | name | tx-aus-ab-2025-114 | None |
| jr-tx-radar-dismiss | relative_path | /library/jurisdiction-rules/jr | None |
| jr-tx-radar-dismiss | jurisdiction_name | tx-us | None |
| jr-tx-radar-dismiss | jurisdiction_type | county | None |
| jr-tx-respond-14 | name | tx-tc-§543 | None |
| jr-tx-respond-14 | relative_path | /library/jurisdiction-rules/jr | None |
| jr-tx-respond-14 | jurisdiction_name | tx-us | None |
| jr-tx-respond-14 | jurisdiction_type | county | None |

### app_users

- Fields: 28/28 (100.0%)
- Computed columns: name_redacted, email_address_redacted, role_title, role_description

### site_branding

- Fields: 0/1 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| site-branding | name | site-branding | None |

### reference_documents

- Fields: 2/8 (25.0%)
- Computed columns: name, is_appeal_board_decision

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ref-ca-is-001 | name | ref-ca-is-001 | None |
| ref-ny-is-006 | name | ref-ny-is-006 | None |
| ref-tx-ab-2024-088 | name | ref-tx-ab-2024-088 | None |
| ref-tx-ab-2024-088 | is_appeal_board_decision | True | None |
| ref-tx-ab-2025-114 | name | ref-tx-ab-2025-114 | None |
| ref-tx-ab-2025-114 | is_appeal_board_decision | True | None |

### state_machines

- Fields: 8/16 (50.0%)
- Computed columns: name, package_is_active, state_count, transition_rule_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| citation-lifecycle | name | citation-lifecycle | None |
| citation-lifecycle | package_is_active | True | None |
| contest-track | name | contest-track | None |
| contest-track | package_is_active | True | None |
| license-track | name | license-track | None |
| license-track | package_is_active | True | None |
| payment-track | name | payment-track | None |
| payment-track | package_is_active | True | None |

### machine_states

- Fields: 0/25 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| citation-lifecycle--adjudicated | name | citation-lifecycle--adjudicate | None |
| citation-lifecycle--closed | name | citation-lifecycle--closed | None |
| citation-lifecycle--dismissed | name | citation-lifecycle--dismissed | None |
| citation-lifecycle--incontest | name | citation-lifecycle--incontest | None |
| citation-lifecycle--issued | name | citation-lifecycle--issued | None |
| citation-lifecycle--responded | name | citation-lifecycle--responded | None |
| contest-track--dismissed | name | contest-track--dismissed | None |
| contest-track--heard | name | contest-track--heard | None |
| contest-track--hearingrequested | name | contest-track--hearingrequeste | None |
| contest-track--notcontested | name | contest-track--notcontested | None |
| contest-track--reduced | name | contest-track--reduced | None |
| contest-track--scheduled | name | contest-track--scheduled | None |
| contest-track--upheld | name | contest-track--upheld | None |
| contest-track--withdrawn | name | contest-track--withdrawn | None |
| license-track--pointsassessed | name | license-track--pointsassessed | None |
| license-track--reinstated | name | license-track--reinstated | None |
| license-track--suspended | name | license-track--suspended | None |
| license-track--valid | name | license-track--valid | None |
| license-track--warning | name | license-track--warning | None |
| payment-track--collections | name | payment-track--collections | None |
| ... | ... | (5 more) | ... |

### state_transition_rules

- Fields: 0/104 (0.0%)
- Computed columns: name, from_state_key, to_state_key, is_forward_edge

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| citation-lifecycle--adjudicated->closed | name | citation-lifecycle--adjudicate | None |
| citation-lifecycle--adjudicated->closed | from_state_key | Adjudicated | None |
| citation-lifecycle--adjudicated->closed | to_state_key | Closed | None |
| citation-lifecycle--adjudicated->closed | is_forward_edge | True | None |
| citation-lifecycle--incontest->adjudicated | name | citation-lifecycle--incontest- | None |
| citation-lifecycle--incontest->adjudicated | from_state_key | InContest | None |
| citation-lifecycle--incontest->adjudicated | to_state_key | Adjudicated | None |
| citation-lifecycle--incontest->adjudicated | is_forward_edge | True | None |
| citation-lifecycle--incontest->dismissed | name | citation-lifecycle--incontest- | None |
| citation-lifecycle--incontest->dismissed | from_state_key | InContest | None |
| citation-lifecycle--incontest->dismissed | to_state_key | dismissed | None |
| citation-lifecycle--incontest->dismissed | is_forward_edge | True | None |
| citation-lifecycle--issued->adjudicated | name | citation-lifecycle--issued->ad | None |
| citation-lifecycle--issued->adjudicated | from_state_key | Issued | None |
| citation-lifecycle--issued->adjudicated | to_state_key | Adjudicated | None |
| citation-lifecycle--issued->adjudicated | is_forward_edge | True | None |
| citation-lifecycle--issued->responded | name | citation-lifecycle--issued->re | None |
| citation-lifecycle--issued->responded | from_state_key | Issued | None |
| citation-lifecycle--issued->responded | to_state_key | Responded | None |
| citation-lifecycle--issued->responded | is_forward_edge | True | None |
| ... | ... | (84 more) | ... |

### state_transitions

- Fields: 0/40 (0.0%)
- Computed columns: name, is_forward

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| contest-track--tc-2026-0002--hearingrequested->scheduled | name | contest-track--tc-2026-0002--h | None |
| contest-track--tc-2026-0002--hearingrequested->scheduled | is_forward | True | None |
| contest-track--tc-2026-0002--notcontested->hearingrequested | name | contest-track--tc-2026-0002--n | None |
| contest-track--tc-2026-0002--notcontested->hearingrequested | is_forward | True | None |
| license-track--d1234567--valid->pointsassessed | name | license-track--d1234567--valid | None |
| license-track--d1234567--valid->pointsassessed | is_forward | True | None |
| payment-track--tc-2026-0001--due->paid | name | payment-track--tc-2026-0001--d | None |
| payment-track--tc-2026-0001--due->paid | is_forward | True | None |
| st-0002-cl-incontest | name | st-0002-cl-incontest | None |
| st-0002-cl-incontest | is_forward | True | None |
| st-0002-cl-issued | name | st-0002-cl-issued | None |
| st-0002-cl-issued | is_forward | True | None |
| st-0002-cl-responded | name | st-0002-cl-responded | None |
| st-0002-cl-responded | is_forward | True | None |
| st-0002-ct-requested | name | st-0002-ct-requested | None |
| st-0002-ct-requested | is_forward | True | None |
| st-0002-ct-scheduled | name | st-0002-ct-scheduled | None |
| st-0002-ct-scheduled | is_forward | True | None |
| st-0003-cl-closed | name | st-0003-cl-closed | None |
| st-0003-cl-closed | is_forward | True | None |
| ... | ... | (20 more) | ... |

### work_queue_items

- Fields: 0/20 (0.0%)
- Computed columns: name, due_in_days, is_overdue, urgency_bucket, is_urgent

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| wq-citations-tc-2026-0001 | name | wq-citations-tc-2026-0001 | None |
| wq-citations-tc-2026-0001 | due_in_days | -189 | None |
| wq-citations-tc-2026-0001 | is_overdue | True | None |
| wq-citations-tc-2026-0001 | urgency_bucket | urgent | None |
| wq-citations-tc-2026-0001 | is_urgent | True | None |
| wq-citations-tc-2026-0002 | name | wq-citations-tc-2026-0002 | None |
| wq-citations-tc-2026-0002 | due_in_days | -160 | None |
| wq-citations-tc-2026-0002 | is_overdue | True | None |
| wq-citations-tc-2026-0002 | urgency_bucket | urgent | None |
| wq-citations-tc-2026-0002 | is_urgent | True | None |
| wq-citations-tc-2026-0004 | name | wq-citations-tc-2026-0004 | None |
| wq-citations-tc-2026-0004 | due_in_days | -226 | None |
| wq-citations-tc-2026-0004 | is_overdue | True | None |
| wq-citations-tc-2026-0004 | urgency_bucket | urgent | None |
| wq-citations-tc-2026-0004 | is_urgent | True | None |
| wq-citations-tc-2026-0006 | name | wq-citations-tc-2026-0006 | None |
| wq-citations-tc-2026-0006 | due_in_days | -156 | None |
| wq-citations-tc-2026-0006 | is_overdue | True | None |
| wq-citations-tc-2026-0006 | urgency_bucket | urgent | None |
| wq-citations-tc-2026-0006 | is_urgent | True | None |

### ai_models

- Fields: 11/12 (91.7%)
- Computed columns: name, pricing_version_count, turn_count, total_cost

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gpt-4o-mini | total_cost | 0.000696 | 0.0002715, 0.0002685, 0.000156 |

### model_pricing_versions

- Fields: 1/12 (8.3%)
- Computed columns: name, ai_model_title, turn_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| erb-internal-classifier-2026-01 | name | erb-internal-classifier-2026-0 | None |
| erb-internal-classifier-2026-01 | ai_model_title | ERB Internal Classifier | None |
| erb-internal-classifier-2026-01 | turn_count | 1 | None |
| gpt-4o-2024-08 | name | gpt-4o-2024-08 | None |
| gpt-4o-2024-08 | ai_model_title | GPT-4o | None |
| gpt-4o-2024-08 | turn_count | 1 | None |
| gpt-4o-mini-2024-07 | name | gpt-4o-mini-2024-07 | None |
| gpt-4o-mini-2024-07 | ai_model_title | GPT-4o mini | None |
| gpt-4o-mini-2025-07 | name | gpt-4o-mini-2025-07 | None |
| gpt-4o-mini-2025-07 | ai_model_title | GPT-4o mini | None |
| gpt-4o-mini-2025-07 | turn_count | 3 | None |

### assistant_turns

- Fields: 6/50 (12.0%)
- Computed columns: name, total_tokens, billable_input_tokens, ai_model_title, input_price_per_m_tok, cached_input_price_per_m_tok, output_price_per_m_tok, input_cost, output_cost, total_cost

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| turn-0002-deadline | name | turn-0002-deadline | None |
| turn-0002-deadline | total_tokens | 1990 | None |
| turn-0002-deadline | billable_input_tokens | 650 | None |
| turn-0002-deadline | ai_model_title | GPT-4o mini | None |
| turn-0002-deadline | input_price_per_m_tok | 0.15 | None |
| turn-0002-deadline | cached_input_price_per_m_tok | 0.075 | None |
| turn-0002-deadline | output_price_per_m_tok | 0.6 | None |
| turn-0002-deadline | input_cost | 0.0001875 | None |
| turn-0002-deadline | output_cost | 8.4e-05 | None |
| turn-0002-deadline | total_cost | 0.0002715 | None |
| turn-0003-dismiss-basis | name | turn-0003-dismiss-basis | None |
| turn-0003-dismiss-basis | total_tokens | 3320 | None |
| turn-0003-dismiss-basis | billable_input_tokens | 1200 | None |
| turn-0003-dismiss-basis | ai_model_title | GPT-4o | None |
| turn-0003-dismiss-basis | input_price_per_m_tok | 2.5 | None |
| turn-0003-dismiss-basis | cached_input_price_per_m_tok | 1.25 | None |
| turn-0003-dismiss-basis | output_price_per_m_tok | 10 | None |
| turn-0003-dismiss-basis | input_cost | 0.005375 | None |
| turn-0003-dismiss-basis | output_cost | 0.0022 | None |
| turn-0003-dismiss-basis | total_cost | 0.007575 | None |
| ... | ... | (24 more) | ... |

### platforms

- Fields: 0/1 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ticket-portal | name | ticket-portal | None |

### erb_packages

- Fields: 0/18 (0.0%)
- Computed columns: name, feature_count, shipped_feature_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| access-control | name | access-control | None |
| access-control | feature_count | 6 | None |
| access-control | shipped_feature_count | 55 | None |
| assistant | name | assistant | None |
| assistant | feature_count | 6 | None |
| assistant | shipped_feature_count | 55 | None |
| citations | name | citations | None |
| citations | feature_count | 6 | None |
| citations | shipped_feature_count | 55 | None |
| core | name | core | None |
| core | feature_count | 6 | None |
| core | shipped_feature_count | 55 | None |
| platform-meta | name | platform-meta | None |
| platform-meta | feature_count | 6 | None |
| platform-meta | shipped_feature_count | 55 | None |
| state-machines | name | state-machines | None |
| state-machines | feature_count | 6 | None |
| state-machines | shipped_feature_count | 55 | None |

### erb_feature_statuses

- Fields: 5/10 (50.0%)
- Computed columns: name, feature_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| deferred | name | deferred | None |
| designed | name | designed | None |
| in-progress | name | in-progress | None |
| planned | name | planned | None |
| shipped | name | shipped | None |

### erb_feature_categories

- Fields: 18/36 (50.0%)
- Computed columns: name, feature_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assistant | name | assistant | None |
| audit | name | audit | None |
| auth | name | auth | None |
| dashboard | name | dashboard | None |
| data-model | name | data-model | None |
| devops | name | devops | None |
| hearings | name | hearings | None |
| intake | name | intake | None |
| library | name | library | None |
| license | name | license | None |
| licensing | name | licensing | None |
| navigation | name | navigation | None |
| payments | name | payments | None |
| permissions | name | permissions | None |
| platform | name | platform | None |
| rules-engine | name | rules-engine | None |
| state-machine | name | state-machine | None |
| workqueue | name | workqueue | None |

### erb_features

- Fields: 14/600 (2.3%)
- Computed columns: name, route_path, relative_path, package_is_licensed, category_title, status_title, status_description, status_is_active

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ai-assistant | name | ai-assistant | None |
| ai-assistant | route_path | /assistant | None |
| ai-assistant | relative_path | /assistant | None |
| ai-assistant | package_is_licensed | True | None |
| ai-assistant | category_title | AI Assistant | None |
| ai-assistant | status_title | In Progress | None |
| ai-assistant | status_description | Actively being built. | None |
| ai-assistant | status_is_active | True | None |
| as-cost-dashboard | name | as-cost-dashboard | None |
| as-cost-dashboard | route_path | /assistant/costs | None |
| as-cost-dashboard | relative_path | /assistant/costs | None |
| as-cost-dashboard | package_is_licensed | True | None |
| as-cost-dashboard | category_title | Dashboards | None |
| as-cost-dashboard | status_title | Shipped | None |
| as-cost-dashboard | status_description | Delivered and live. | None |
| as-cost-dashboard | status_is_active | True | None |
| as-cost-telemetry | name | as-cost-telemetry | None |
| as-cost-telemetry | route_path | /assistant/costs | None |
| as-cost-telemetry | relative_path | /assistant/costs | None |
| as-cost-telemetry | package_is_licensed | True | None |
| ... | ... | (566 more) | ... |

### erb_tables

- Fields: 316/1045 (30.2%)
- Computed columns: name, package_is_active, package_is_licensed, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| APIEndpoints | name | APIEndpoints | None |
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
| AiModels | name | AiModels | None |
| AiModels | package_is_active | True | None |
| AiModels | package_is_licensed | True | None |
| AiModels | admin_can_create | True | None |
| AiModels | admin_can_read | True | None |
| AiModels | admin_can_update | True | None |
| AiModels | admin_can_delete | True | None |
| ... | ... | (709 more) | ... |

### erb_fields

- Fields: 16451/18620 (88.4%)
- Computed columns: name, table_package_is_active, is_calculated, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, manager_can_create, manager_can_read, manager_can_update, manager_can_delete, representative_can_create, representative_can_read, representative_can_update, representative_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| APIEndpoints.APIEndpointId | name | APIEndpoints.APIEndpointId | None |
| APIEndpoints.APIEndpointId | table_package_is_active | True | None |
| APIEndpoints.CreatedAt | name | APIEndpoints.CreatedAt | None |
| APIEndpoints.CreatedAt | table_package_is_active | True | None |
| APIEndpoints.CreatedBy | name | APIEndpoints.CreatedBy | None |
| APIEndpoints.CreatedBy | table_package_is_active | True | None |
| APIEndpoints.Description | name | APIEndpoints.Description | None |
| APIEndpoints.Description | table_package_is_active | True | None |
| APIEndpoints.EndpointType | name | APIEndpoints.EndpointType | None |
| APIEndpoints.EndpointType | table_package_is_active | True | None |
| APIEndpoints.HttpMethod | name | APIEndpoints.HttpMethod | None |
| APIEndpoints.HttpMethod | table_package_is_active | True | None |
| APIEndpoints.ModifiedAt | name | APIEndpoints.ModifiedAt | None |
| APIEndpoints.ModifiedAt | table_package_is_active | True | None |
| APIEndpoints.ModifiedBy | name | APIEndpoints.ModifiedBy | None |
| APIEndpoints.ModifiedBy | table_package_is_active | True | None |
| APIEndpoints.ModifiedByModel | name | APIEndpoints.ModifiedByModel | None |
| APIEndpoints.ModifiedByModel | table_package_is_active | True | None |
| APIEndpoints.Name | name | APIEndpoints.Name | None |
| APIEndpoints.Name | table_package_is_active | True | None |
| ... | ... | (2149 more) | ... |

### api_endpoints

- Fields: 36/36 (100.0%)
- Computed columns: name

### subject_state_instances

- Fields: 3/33 (9.1%)
- Computed columns: name, is_current, has_complete_lineage

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| d1234567--license-track--pointsassessed | name | d1234567--license-track--point | None |
| d1234567--license-track--pointsassessed | is_current | True | None |
| d1234567--license-track--pointsassessed | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0002-incontest-3 | name | ssi-citations-tc-2026-0002-inc | None |
| ssi-citations-tc-2026-0002-incontest-3 | is_current | True | None |
| ssi-citations-tc-2026-0002-incontest-3 | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0002-issued-1 | name | ssi-citations-tc-2026-0002-iss | None |
| ssi-citations-tc-2026-0002-issued-1 | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0002-responded-2 | name | ssi-citations-tc-2026-0002-res | None |
| ssi-citations-tc-2026-0002-responded-2 | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0003-closed-2 | name | ssi-citations-tc-2026-0003-clo | None |
| ssi-citations-tc-2026-0003-closed-2 | is_current | True | None |
| ssi-citations-tc-2026-0003-closed-2 | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0003-heard-1 | name | ssi-citations-tc-2026-0003-hea | None |
| ssi-citations-tc-2026-0003-heard-1 | is_current | True | None |
| ssi-citations-tc-2026-0003-heard-1 | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0003-issued-1 | name | ssi-citations-tc-2026-0003-iss | None |
| ssi-citations-tc-2026-0003-issued-1 | has_complete_lineage | True | None |
| ssi-citations-tc-2026-0004-late-1 | name | ssi-citations-tc-2026-0004-lat | None |
| ssi-citations-tc-2026-0004-late-1 | is_current | True | None |
| ... | ... | (10 more) | ... |

### violation_types

- Fields: 11/35 (31.4%)
- Computed columns: name, jurisdiction_label, traffic_school_point_cap, is_school_eligible_by_cap, count_of_citations

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| vt-cvc-21453a | name | cvc-21453a | None |
| vt-cvc-21453a | jurisdiction_label | Los Angeles County, California | None |
| vt-cvc-21453a | traffic_school_point_cap | 1 | None |
| vt-cvc-21453a | is_school_eligible_by_cap | True | None |
| vt-cvc-22350 | name | cvc-22350 | None |
| vt-cvc-22350 | jurisdiction_label | Los Angeles County, California | None |
| vt-cvc-22350 | traffic_school_point_cap | 1 | None |
| vt-cvc-22350 | is_school_eligible_by_cap | True | None |
| vt-cvc-23123 | name | cvc-23123 | None |
| vt-cvc-23123 | jurisdiction_label | Los Angeles County, California | None |
| vt-cvc-23123 | traffic_school_point_cap | 1 | None |
| vt-cvc-23123 | is_school_eligible_by_cap | True | None |
| vt-ttc-544007 | name | ttc-544007 | None |
| vt-ttc-544007 | jurisdiction_label | Travis County, Texas | None |
| vt-ttc-544007 | traffic_school_point_cap | 2 | None |
| vt-ttc-544007 | is_school_eligible_by_cap | True | None |
| vt-ttc-545351 | name | ttc-545351 | None |
| vt-ttc-545351 | jurisdiction_label | Travis County, Texas | None |
| vt-ttc-545351 | traffic_school_point_cap | 2 | None |
| vt-ttc-545351 | is_school_eligible_by_cap | True | None |
| ... | ... | (4 more) | ... |

### drivers

- Fields: 12/48 (25.0%)
- Computed columns: name, full_name, home_jurisdiction_label, suspension_threshold, warning_threshold, count_of_citations, active_points, license_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| d-d1234567 | name | d1234567 | None |
| d-d1234567 | full_name | Doe, Jane | None |
| d-d1234567 | home_jurisdiction_label | Los Angeles County, California | None |
| d-d1234567 | suspension_threshold | 4 | None |
| d-d1234567 | warning_threshold | 3 | None |
| d-d1234567 | license_status | Valid | None |
| d-d1357924 | name | d1357924 | None |
| d-d1357924 | full_name | Whitfield, James | None |
| d-d1357924 | home_jurisdiction_label | Travis County, Texas | None |
| d-d1357924 | suspension_threshold | 6 | None |
| d-d1357924 | warning_threshold | 4 | None |
| d-d1357924 | license_status | Valid | None |
| d-d2468013 | name | d2468013 | None |
| d-d2468013 | full_name | Gonzalez, Maria | None |
| d-d2468013 | home_jurisdiction_label | Los Angeles County, California | None |
| d-d2468013 | suspension_threshold | 4 | None |
| d-d2468013 | warning_threshold | 3 | None |
| d-d2468013 | license_status | Valid | None |
| d-d7654321 | name | d7654321 | None |
| d-d7654321 | full_name | Mendez, Carlos | None |
| ... | ... | (16 more) | ... |

### citations

- Fields: 87/312 (27.9%)
- Computed columns: name, driver_label, violation_label, jurisdiction_label, base_fine_usd, violation_points, days_to_respond, days_to_pay_after_ruling, late_penalty_pct, days_late_to_collections, response_due_date, days_until_response_due, is_response_overdue, count_of_hearings, latest_hearing_outcome, contest_status, is_dismissed, is_guilty, amount_due_usd, payment_due_date, is_payment_late, collections_deadline, is_in_collections, payment_status, effective_points, citation_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| c-0001 | name | tc-2026-0001 | None |
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
| c-0001 | is_response_overdue | True | None |
| c-0001 | contest_status | NotContested | None |
| c-0001 | is_guilty | True | None |
| c-0001 | amount_due_usd | 238.0 | None |
| c-0001 | payment_due_date | 2026-06-21 | None |
| c-0001 | collections_deadline | 2026-08-20 | None |
| c-0001 | payment_status | Due | None |
| c-0001 | effective_points | 1 | None |
| ... | ... | (205 more) | ... |

### hearings

- Fields: 0/10 (0.0%)
- Computed columns: name, citation_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| h-0001 | name | h-2026-0002 | None |
| h-0001 | citation_label | TC-2026-0002 | None |
| h-0002 | name | h-2026-0003 | None |
| h-0002 | citation_label | TC-2026-0003 | None |
| h-0003 | name | hrg-2026-0003 | None |
| h-0003 | citation_label | TC-2026-0007 | None |
| h-0004 | name | hrg-2026-0004 | None |
| h-0004 | citation_label | TC-2026-0010 | None |
| h-0005 | name | hrg-2026-0005 | None |
| h-0005 | citation_label | TC-2026-0012 | None |

### payments

- Fields: 0/6 (0.0%)
- Computed columns: name, citation_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| p-0001 | name | pmt-2026-0005 | None |
| p-0001 | citation_label | TC-2026-0005 | None |
| p-0002 | name | pmt-2026-0009 | None |
| p-0002 | citation_label | TC-2026-0009 | None |
| p-0003 | name | pmt-2026-0001 | None |
| p-0003 | citation_label | TC-2026-0001 | None |

### case_events

- Fields: 0/24 (0.0%)
- Computed columns: name, citation_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ev-0001 | name | ev-2026-0001 | None |
| ev-0001 | citation_label | TC-2026-0001 | None |
| ev-0002 | name | ev-2026-0002 | None |
| ev-0002 | citation_label | TC-2026-0002 | None |
| ev-0003 | name | ev-2026-0003 | None |
| ev-0003 | citation_label | TC-2026-0002 | None |
| ev-0004 | name | ev-2026-0004 | None |
| ev-0004 | citation_label | TC-2026-0002 | None |
| ev-0005 | name | ev-2026-0005 | None |
| ev-0005 | citation_label | TC-2026-0003 | None |
| ev-0006 | name | ev-2026-0006 | None |
| ev-0006 | citation_label | TC-2026-0003 | None |
| ev-0007 | name | ev-2026-0007 | None |
| ev-0007 | citation_label | TC-2026-0005 | None |
| ev-0008 | name | ev-2026-0008 | None |
| ev-0008 | citation_label | TC-2026-0007 | None |
| ev-0009 | name | ev-2026-0009 | None |
| ev-0009 | citation_label | TC-2026-0007 | None |
| ev-0010 | name | ev-2026-0010 | None |
| ev-0010 | citation_label | TC-2026-0009 | None |
| ... | ... | (4 more) | ... |

### test_category

- Fields: 0/7 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| api | name | api | None |
| calc | name | calc | None |
| data-integrity | name | data-integrity | None |
| lifecycle | name | lifecycle | None |
| permissions | name | permissions | None |
| rules | name | rules | None |
| smoke | name | smoke | None |

### test_surface

- Fields: 0/5 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| api | name | api | None |
| db | name | db | None |
| rules | name | rules | None |
| ui | name | ui | None |
| view | name | view | None |

### test_technology

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| manual | name | manual | None |
| playwright | name | playwright | None |
| sql | name | sql | None |
| vitest | name | vitest | None |

### test_case

- Fields: 0/388 (0.0%)
- Computed columns: name, expectation_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tc-endpoint-assistant-ask | name | tc-endpoint-assistant-ask | None |
| tc-endpoint-assistant-ask | expectation_count | 351 | None |
| tc-endpoint-case-timeline-report | name | tc-endpoint-case-timeline-repo | None |
| tc-endpoint-case-timeline-report | expectation_count | 351 | None |
| tc-endpoint-citation-advance-state | name | tc-endpoint-citation-advance-s | None |
| tc-endpoint-citation-advance-state | expectation_count | 351 | None |
| tc-endpoint-citation-contest | name | tc-endpoint-citation-contest | None |
| tc-endpoint-citation-contest | expectation_count | 351 | None |
| tc-endpoint-citation-dismiss | name | tc-endpoint-citation-dismiss | None |
| tc-endpoint-citation-dismiss | expectation_count | 351 | None |
| tc-endpoint-citation-import | name | tc-endpoint-citation-import | None |
| tc-endpoint-citation-import | expectation_count | 351 | None |
| tc-endpoint-citation-ocr | name | tc-endpoint-citation-ocr | None |
| tc-endpoint-citation-ocr | expectation_count | 351 | None |
| tc-endpoint-citation-request-hearing | name | tc-endpoint-citation-request-h | None |
| tc-endpoint-citation-request-hearing | expectation_count | 351 | None |
| tc-endpoint-citation-respond | name | tc-endpoint-citation-respond | None |
| tc-endpoint-citation-respond | expectation_count | 351 | None |
| tc-endpoint-citation-withdraw-contest | name | tc-endpoint-citation-withdraw- | None |
| tc-endpoint-citation-withdraw-contest | expectation_count | 351 | None |
| ... | ... | (368 more) | ... |

### test_expectation

- Fields: 0/351 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tc-endpoint-assistant-ask--exp1 | name | tc-endpoint-assistant-ask--exp | None |
| tc-endpoint-assistant-ask--exp2 | name | tc-endpoint-assistant-ask--exp | None |
| tc-endpoint-case-timeline-report--exp1 | name | tc-endpoint-case-timeline-repo | None |
| tc-endpoint-case-timeline-report--exp2 | name | tc-endpoint-case-timeline-repo | None |
| tc-endpoint-citation-advance-state--exp1 | name | tc-endpoint-citation-advance-s | None |
| tc-endpoint-citation-advance-state--exp2 | name | tc-endpoint-citation-advance-s | None |
| tc-endpoint-citation-advance-state--exp3 | name | tc-endpoint-citation-advance-s | None |
| tc-endpoint-citation-contest--exp1 | name | tc-endpoint-citation-contest-- | None |
| tc-endpoint-citation-contest--exp2 | name | tc-endpoint-citation-contest-- | None |
| tc-endpoint-citation-contest--exp3 | name | tc-endpoint-citation-contest-- | None |
| tc-endpoint-citation-dismiss--exp1 | name | tc-endpoint-citation-dismiss-- | None |
| tc-endpoint-citation-dismiss--exp2 | name | tc-endpoint-citation-dismiss-- | None |
| tc-endpoint-citation-dismiss--exp3 | name | tc-endpoint-citation-dismiss-- | None |
| tc-endpoint-citation-import--exp1 | name | tc-endpoint-citation-import--e | None |
| tc-endpoint-citation-import--exp2 | name | tc-endpoint-citation-import--e | None |
| tc-endpoint-citation-import--exp3 | name | tc-endpoint-citation-import--e | None |
| tc-endpoint-citation-ocr--exp1 | name | tc-endpoint-citation-ocr--exp1 | None |
| tc-endpoint-citation-ocr--exp2 | name | tc-endpoint-citation-ocr--exp2 | None |
| tc-endpoint-citation-request-hearing--exp1 | name | tc-endpoint-citation-request-h | None |
| tc-endpoint-citation-request-hearing--exp2 | name | tc-endpoint-citation-request-h | None |
| ... | ... | (331 more) | ... |

### screen_layouts

- Fields: 0/40 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sl-admin | name | sl-admin | None |
| sl-admin.audit-log | name | sl-admin.audit-log | None |
| sl-admin.branding | name | sl-admin.branding | None |
| sl-admin.build-pipeline | name | sl-admin.build-pipeline | None |
| sl-admin.customizations | name | sl-admin.customizations | None |
| sl-admin.endpoints | name | sl-admin.endpoints | None |
| sl-admin.explainer-dag | name | sl-admin.explainer-dag | None |
| sl-admin.feature-detail | name | sl-admin.feature-detail | None |
| sl-admin.features | name | sl-admin.features | None |
| sl-admin.fields | name | sl-admin.fields | None |
| sl-admin.navigation | name | sl-admin.navigation | None |
| sl-admin.packages | name | sl-admin.packages | None |
| sl-admin.permissions | name | sl-admin.permissions | None |
| sl-admin.roles | name | sl-admin.roles | None |
| sl-admin.route-designer | name | sl-admin.route-designer | None |
| sl-admin.state-machines | name | sl-admin.state-machines | None |
| sl-admin.tables | name | sl-admin.tables | None |
| sl-admin.users | name | sl-admin.users | None |
| sl-admin.versions | name | sl-admin.versions | None |
| sl-assistant | name | sl-assistant | None |
| ... | ... | (20 more) | ... |

### screen_sections

- Fields: 0/88 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| admin--grid | name | admin--grid | None |
| admin--toolbar | name | admin--toolbar | None |
| admin.audit-log--grid | name | admin.audit-log--grid | None |
| admin.audit-log--toolbar | name | admin.audit-log--toolbar | None |
| admin.branding--grid | name | admin.branding--grid | None |
| admin.branding--toolbar | name | admin.branding--toolbar | None |
| admin.build-pipeline--grid | name | admin.build-pipeline--grid | None |
| admin.build-pipeline--toolbar | name | admin.build-pipeline--toolbar | None |
| admin.customizations--grid | name | admin.customizations--grid | None |
| admin.customizations--toolbar | name | admin.customizations--toolbar | None |
| admin.endpoints--grid | name | admin.endpoints--grid | None |
| admin.endpoints--toolbar | name | admin.endpoints--toolbar | None |
| admin.explainer-dag--grid | name | admin.explainer-dag--grid | None |
| admin.explainer-dag--toolbar | name | admin.explainer-dag--toolbar | None |
| admin.feature-detail--actions | name | admin.feature-detail--actions | None |
| admin.feature-detail--fields | name | admin.feature-detail--fields | None |
| admin.feature-detail--summary | name | admin.feature-detail--summary | None |
| admin.feature-detail--timeline | name | admin.feature-detail--timeline | None |
| admin.features--grid | name | admin.features--grid | None |
| admin.features--toolbar | name | admin.features--toolbar | None |
| ... | ... | (68 more) | ... |

### field_display_hints

- Fields: 0/485 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| APIEndpoints.APIEndpointId | name | APIEndpoints.APIEndpointId | None |
| APIEndpoints.Description | name | APIEndpoints.Description | None |
| APIEndpoints.EndpointType | name | APIEndpoints.EndpointType | None |
| APIEndpoints.HttpMethod | name | APIEndpoints.HttpMethod | None |
| APIEndpoints.Name | name | APIEndpoints.Name | None |
| APIEndpoints.Path | name | APIEndpoints.Path | None |
| APIEndpoints.RoleVisibility | name | APIEndpoints.RoleVisibility | None |
| APIEndpoints.Status | name | APIEndpoints.Status | None |
| APIEndpoints.SubjectTableName | name | APIEndpoints.SubjectTableName | None |
| APIEndpoints.Title | name | APIEndpoints.Title | None |
| APIEndpoints.TriggersStateMachine | name | APIEndpoints.TriggersStateMach | None |
| APIEndpoints.WhereClause | name | APIEndpoints.WhereClause | None |
| AiModels.AiModelId | name | AiModels.AiModelId | None |
| AiModels.AssistantTurns | name | AiModels.AssistantTurns | None |
| AiModels.ContextWindowTokens | name | AiModels.ContextWindowTokens | None |
| AiModels.Description | name | AiModels.Description | None |
| AiModels.IsActive | name | AiModels.IsActive | None |
| AiModels.IsInternal | name | AiModels.IsInternal | None |
| AiModels.ModelPricingVersions | name | AiModels.ModelPricingVersions | None |
| AiModels.Name | name | AiModels.Name | None |
| ... | ... | (465 more) | ... |

### fee_schedules

- Fields: 0/21 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| j-ca-la--vt-cvc-21453a | name | j-ca-la--vt-cvc-21453a | None |
| j-ca-la--vt-cvc-22350 | name | j-ca-la--vt-cvc-22350 | None |
| j-ca-la--vt-cvc-23123 | name | j-ca-la--vt-cvc-23123 | None |
| j-ca-la--vt-ttc-544007 | name | j-ca-la--vt-ttc-544007 | None |
| j-ca-la--vt-ttc-545351 | name | j-ca-la--vt-ttc-545351 | None |
| j-ca-la--vt-vtl-1111d | name | j-ca-la--vt-vtl-1111d | None |
| j-ca-la--vt-vtl-1180b | name | j-ca-la--vt-vtl-1180b | None |
| j-ny-nyc--vt-cvc-21453a | name | j-ny-nyc--vt-cvc-21453a | None |
| j-ny-nyc--vt-cvc-22350 | name | j-ny-nyc--vt-cvc-22350 | None |
| j-ny-nyc--vt-cvc-23123 | name | j-ny-nyc--vt-cvc-23123 | None |
| j-ny-nyc--vt-ttc-544007 | name | j-ny-nyc--vt-ttc-544007 | None |
| j-ny-nyc--vt-ttc-545351 | name | j-ny-nyc--vt-ttc-545351 | None |
| j-ny-nyc--vt-vtl-1111d | name | j-ny-nyc--vt-vtl-1111d | None |
| j-ny-nyc--vt-vtl-1180b | name | j-ny-nyc--vt-vtl-1180b | None |
| j-tx-aus--vt-cvc-21453a | name | j-tx-aus--vt-cvc-21453a | None |
| j-tx-aus--vt-cvc-22350 | name | j-tx-aus--vt-cvc-22350 | None |
| j-tx-aus--vt-cvc-23123 | name | j-tx-aus--vt-cvc-23123 | None |
| j-tx-aus--vt-ttc-544007 | name | j-tx-aus--vt-ttc-544007 | None |
| j-tx-aus--vt-ttc-545351 | name | j-tx-aus--vt-ttc-545351 | None |
| j-tx-aus--vt-vtl-1111d | name | j-tx-aus--vt-vtl-1111d | None |
| ... | ... | (1 more) | ... |

### deadline_rules

- Fields: 0/9 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| j-ca-la--appeal | name | j-ca-la--appeal | None |
| j-ca-la--contest | name | j-ca-la--contest | None |
| j-ca-la--payment | name | j-ca-la--payment | None |
| j-ny-nyc--appeal | name | j-ny-nyc--appeal | None |
| j-ny-nyc--contest | name | j-ny-nyc--contest | None |
| j-ny-nyc--payment | name | j-ny-nyc--payment | None |
| j-tx-aus--appeal | name | j-tx-aus--appeal | None |
| j-tx-aus--contest | name | j-tx-aus--contest | None |
| j-tx-aus--payment | name | j-tx-aus--payment | None |

### contest_grounds

- Fields: 0/14 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| vt-cvc-21453a--malfunction | name | vt-cvc-21453a--malfunction | None |
| vt-cvc-21453a--procedural | name | vt-cvc-21453a--procedural | None |
| vt-cvc-22350--procedural | name | vt-cvc-22350--procedural | None |
| vt-cvc-22350--signage | name | vt-cvc-22350--signage | None |
| vt-cvc-23123--emergency | name | vt-cvc-23123--emergency | None |
| vt-cvc-23123--procedural | name | vt-cvc-23123--procedural | None |
| vt-ttc-544007--mistaken-id | name | vt-ttc-544007--mistaken-id | None |
| vt-ttc-544007--procedural | name | vt-ttc-544007--procedural | None |
| vt-ttc-545351--procedural | name | vt-ttc-545351--procedural | None |
| vt-ttc-545351--speedometer | name | vt-ttc-545351--speedometer | None |
| vt-vtl-1111d--obstruction | name | vt-vtl-1111d--obstruction | None |
| vt-vtl-1111d--procedural | name | vt-vtl-1111d--procedural | None |
| vt-vtl-1180b--calibration | name | vt-vtl-1180b--calibration | None |
| vt-vtl-1180b--procedural | name | vt-vtl-1180b--procedural | None |

### driver_license_points

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| c-0001--d-d1234567 | name | c-0001--d-d1234567 | None |
| c-0003--d-d7654321 | name | c-0003--d-d7654321 | None |
| c-0009--d-n5544332 | name | c-0009--d-n5544332 | None |

### build_phases

- Fields: 0/5 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| phase-1 | name | phase-1 | None |
| phase-2 | name | phase-2 | None |
| phase-3 | name | phase-3 | None |
| phase-4 | name | phase-4 | None |
| phase-5 | name | phase-5 | None |
