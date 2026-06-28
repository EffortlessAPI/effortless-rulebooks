# Embedding Synthesis — Code Patterns & Field Reference

## Archetype Embedding Synthesis

Synthesized from three TOML fields, not raw description:

```python
def build_archetype_embedding_text(archetype: dict) -> str:
    parts = [archetype["description"]]
    if signals := archetype.get("signals_positive"):
        parts.append("Strong indicators: " + ", ".join(signals))
    if signals := archetype.get("signals_negative"):
        parts.append("Misaligned indicators: " + ", ".join(signals))
    return "\n\n".join(parts)
```

## Global Positive Signal Synthesis

One document per rubric dimension that has `signals_positive`:

```python
def build_positive_embedding_texts(global_rubric: dict) -> list[tuple[str, str, str]]:
    texts = []  # (id, text, source_dimension)
    for dim in global_rubric.get("dimensions", []):
        dim_name = dim.get("name", "unknown")
        if signals := dim.get("signals_positive"):
            text = f"{dim_name}: " + ", ".join(signals)
            texts.append((f"pos-{slugify(dim_name)}", text, dim_name))
    return texts
```

## Negative Signal Synthesis

One document per source:

```python
def build_negative_embedding_texts(global_rubric: dict, archetypes: list[dict]) -> list[str]:
    texts = []
    for dim_name, dim in global_rubric.items():
        if signals := dim.get("signals_negative"):
            texts.append(f"{dim_name}: " + ", ".join(signals))
    for arch in archetypes:
        if signals := arch.get("signals_negative"):
            texts.append(f"{arch['name']} misaligned: " + ", ".join(signals))
    return texts
```

## What Gets Embedded vs. What Doesn't

| Field | Embedded? | Collection | Why |
|---|---|---|---|
| `description` | Yes | `role_archetypes` | Core semantic content |
| `signals_positive` | Yes | `role_archetypes` + `global_positive_signals` | Strengthens archetype/culture discrimination |
| `signals_negative` | Yes | `negative_signals` | Provides continuous penalty signal |
| `minimum_target` | **No** | — | Numeric metadata — adds noise to cosine similarity |
| `weight_*` fields | **No** | — | Scoring config — not semantic content |
| `description` in rubric | **No** | — | Human reference only |

## Non-Semantic Scores (Not From ChromaDB)

- **`comp_score`** — regex-parsed from JD text by `comp_parser.py`; relative to `base_salary`
- **Disqualifier** — binary LLM gate via Ollama; augmented by past "no" rejection reasons
