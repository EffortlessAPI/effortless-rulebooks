#!/usr/bin/env python3
"""Add an `admin.owl` route to RoutingAndNavigation so the OWL conformance page
appears in the left nav under Admin (next to Explainer DAG). Raw fields only — the
build re-derives Name/Depth/FullPath/*CanRead/etc. Idempotent.
"""
import json, collections
RB = 'effortless-rulebook/effortless-rulebook.json'
rb = json.load(open(RB), object_pairs_hook=collections.OrderedDict)
data = rb['RoutingAndNavigation']['data']

ROW = collections.OrderedDict([
    ('RoutingAndNavigationId', 'nav-admin-owl'),
    ('DisplayName', 'OWL Conformance'),
    ('Route', '/admin/owl'),
    ('Description', 'Cross-substrate conformance: OWL-RL closure == Postgres closure (inference kind #7).'),
    ('SortOrder', 95),
    ('ParentRouteKey', 'admin'),
    ('RouteKey', 'admin.owl'),
    ('NavLevel', 'sub'),
    ('RoleVisibility', 'admin'),
    ('IsDynamic', False),
    ('PinToTop', False),
    ('AdminCRUD', 'CRUD'),
    ('IntakeClinicianCRUD', ''),
    ('DiagnosingDoctorCRUD', ''),
    ('ExternalLlmCRUD', ''),
    ('CreatedBy', 'build-seed'),
    ('ModifiedBy', 'build-seed'),
    ('IconHint', 'git-merge'),
])

# Replace if present (idempotent), else insert right after admin.explainer.
data[:] = [r for r in data if r.get('RouteKey') != 'admin.owl']
idx = next((i for i, r in enumerate(data) if r.get('RouteKey') == 'admin.explainer'), len(data) - 1)
data.insert(idx + 1, ROW)

with open(RB, 'w') as f:
    json.dump(rb, f, indent=2, ensure_ascii=False)
    f.write('\n')
print(f"admin.owl route added (after admin.explainer). RoutingAndNavigation rows: {len(data)}")
