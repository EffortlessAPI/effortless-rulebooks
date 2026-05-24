# TOML Config Schemas — Full Structure Reference

## `config/global_rubric.toml`

8 dimensions under `[global_rubric.<name>]`. Each dimension has:

```toml
[global_rubric.altitude]
description = """..."""            # Human reference only — NOT embedded
signals_positive = [               # Embedded into archetype synthesis (future)
  "architecture ownership",
  "platform strategy",
]
signals_negative = [               # Embedded into negative_signals collection
  "implement features",
  "execute tasks",
]
```

### Dimension Names

`altitude`, `humane_culture`, `domain_alignment`, `compensation`, `scope`,
`company_maturity`, `ethics`, `nd_compatibility`

### Special Cases

- `compensation` has `minimum_target` and `weight_*` fields — human reference only, never embedded
- `nd_compatibility` has no `signals_negative` in some configurations — skip gracefully
- A dimension with no `signals_negative` produces no document in the collection

---

## `config/role_archetypes.toml`

Array of tables under `[[archetypes]]`:

```toml
[[archetypes]]
name = "Staff Platform Architect"
description = """..."""           # Core identity — embedded as-is before signal synthesis
signals_positive = [              # Synthesized into archetype embedding text
  "platform strategy",
  "architecture ownership",
]
signals_negative = [              # Contributes to negative_signals collection
  "implement assigned features",
  "execute tickets",
]
```

### Three Archetypes

1. **Staff Platform Architect** — distributed systems, API governance, cross-team influence
2. **Principal Data Platform Engineer** — data pipelines, governance, lakehouse, streaming
3. **Developer Relations / Technical Evangelist** — engineering depth, community, SDKs

---

## `config/settings.toml` — Scoring Section

```toml
[scoring]
archetype_weight = 0.5
fit_weight = 0.3
history_weight = 0.2
comp_weight = 0.15
negative_weight = 0.4              # Penalty for negative signal matches
base_salary = 220000
disqualify_on_llm_flag = true
min_score_threshold = 0.45
```

---

## Loading Pattern in Indexer

```python
import tomllib
from pathlib import Path

def _load_global_rubric(path: str) -> dict:
    rubric_path = Path(path)
    if not rubric_path.exists():
        raise ActionableError(...)  # path + creation guidance
    with rubric_path.open("rb") as f:
        data = tomllib.load(f)
    return data.get("global_rubric", {})
```

The returned dict maps dimension names to dimension dicts. The indexer iterates
dimensions and extracts `signals_negative` lists — everything else is ignored
during embedding.
