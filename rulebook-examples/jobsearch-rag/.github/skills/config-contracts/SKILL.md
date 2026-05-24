---
name: config-contracts
description: "TOML config structure and validation rules. Use when creating or modifying config files (global_rubric.toml, role_archetypes.toml, settings.toml), updating config validation, or loading config in the indexer."
---

# Config Contracts — TOML Structure & Validation Rules

## When This Skill Applies

When creating or modifying config files (`global_rubric.toml`, `role_archetypes.toml`,
`settings.toml`), updating config validation in `config.py`, or loading config in
the indexer.

---

## Config File Overview

Three TOML config files with full structure examples in [toml-schemas.md](references/toml-schemas.md):

- **`config/global_rubric.toml`** — 8 dimensions (`altitude`, `humane_culture`, etc.) with `description`, `signals_positive`, `signals_negative`
- **`config/role_archetypes.toml`** — `[[archetypes]]` array with `name`, `description`, `signals_positive`, `signals_negative`
- **`config/settings.toml`** — `[scoring]` section with weights, `base_salary`, thresholds

---

## Validation Rules

### `global_rubric.toml`

| Check | Error |
|-------|-------|
| File missing | ActionableError naming path + "create the file with at least one dimension" |
| Malformed TOML | ActionableError with syntax error details + file path |

### `role_archetypes.toml`

| Check | Error |
|-------|-------|
| File missing | ActionableError naming path + recovery steps |
| Malformed TOML | ActionableError with syntax error + file path |
| Empty archetypes list | ActionableError advising to add entries before search |
| Archetype without `signals_positive` | Graceful — embed description only |
| Archetype without `signals_negative` | Graceful — no negative document for this archetype |

### `settings.toml` — `negative_weight`

| Check | Error |
|-------|-------|
| Missing | Default to `0.4` |
| Out of range (< 0.0 or > 1.0) | ActionableError naming field + valid range |

### `settings.toml` — `culture_weight`

| Check | Error |
|-------|-------|
| Missing | Default to `0.2` |
| Out of range (< 0.0 or > 1.0) | ActionableError naming field + valid range |

### `settings.toml` — `global_rubric_path`

| Check | Error |
|-------|-------|
| Missing from settings | Default to `"config/global_rubric.toml"` |
| Path does not exist | ActionableError naming path + creation guidance |
