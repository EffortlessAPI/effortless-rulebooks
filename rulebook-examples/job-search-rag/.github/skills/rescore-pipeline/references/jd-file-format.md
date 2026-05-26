# JD File Format — Parsing Rules & Edge Cases

## File Format

Files live in `output/jds/` with naming convention `{rank:03d}_{company_slug}_{title_slug}.md`:

```markdown
# Principal Engineer - Data Platform

**Company:** Target  
**Location:** Minneapolis, MN US 55445  
**Board:** ziprecruiter  
**URL:** https://www.ziprecruiter.com/c/Target/Job/...  

## Score

- **Rank:** #1
- **Final Score:** 0.75
- **Fit Score:** 0.82
- **Archetype Score:** 0.72
- **History Score:** 0.00
- **Comp Score:** 1.00

## Job Description

[full JD text here]
```

## Parsing Rules

| Field | Extraction |
|---|---|
| `title` | `# ` line (H1 heading) |
| `company` | `**Company:**` line, strip bold markers |
| `location` | `**Location:**` line, strip bold markers |
| `board` | `**Board:**` line, strip bold markers |
| `url` | `**URL:**` line, strip bold markers |
| `full_text` | Everything after `## Job Description` marker |
| `external_id` | Derived from URL (same logic as during search) |
| `comp_*` fields | **Re-parsed from `full_text`** by `comp_parser.py`, not from score header |

## Edge Cases

- Missing header field → skip file with warning (log filename)
- Missing `## Job Description` marker → skip file with warning
- Empty body after marker → skip file with warning
- File rank prefix (`001_`) is ignored — rescore produces new rankings
