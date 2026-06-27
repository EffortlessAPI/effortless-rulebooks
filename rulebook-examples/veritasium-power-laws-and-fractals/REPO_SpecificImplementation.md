# Repository Implementation Details

This document provides a comprehensive technical overview of the Power Laws & Fractals repository—its architecture, multi-platform implementations, and operational testing methodology.

---

## 1. Repository Summary

### Purpose

This repository implements a **cross-platform validation system** for power-law and fractal mathematics. It takes a Single Source of Truth (SSoT) JSON file containing mathematical models of fractal/power-law systems and generates equivalent implementations across three execution environments:

- **Python** (data processing)
- **PostgreSQL** (database with calculated views)
- **Golang** (compiled binary)

Each platform must compute identical derived values from the same input data, and all results are validated against a canonical answer key.

### Core Concept: The Entity Rule Book (ERB)

The SSoT (`ssot/ERB_veritasium-power-laws-and-fractals.json`) follows the CMCC (Computed Measured Calculated Columns) pattern—a schema that clearly distinguishes:

| Field Type | Description | Example |
|------------|-------------|---------|
| **Raw** | Direct input values | `Measure`, `Iteration`, `SystemID` |
| **Lookup** | Values retrieved from parent tables | `BaseScale`, `ScaleFactor` |
| **Calculated** | Derived from formulas | `Scale = BaseScale × ScaleFactorPower` |
| **Aggregation** | Rolled up from child tables | `PointCount`, `MinLogScale` |

### Mathematical Domain

The repository models 7 systems from Veritasium's power-law video:

| System | Class | Theoretical Slope |
|--------|-------|-------------------|
| Sierpinski Triangle | Fractal | -1.585 |
| Koch Snowflake | Fractal | -0.262 |
| Zipf Word Frequencies | Power Law | -1.0 |
| Scale-Free Networks | Power Law | -2.5 |
| Sandpile Avalanches | Power Law | -1.0 |
| Earthquake Energies | Power Law | -1.0 |
| Forest Fire Sizes | Power Law | -1.3 |

Each system has 8 data points (iterations 0-7), where the log-log relationship between Scale and Measure should approximate a straight line with the theoretical slope.

---

## 2. Directory Structure

```
ERB_veritasium-power-laws-and-fractals/
├── ssot/                              # Source of Truth
│   └── ERB_veritasium-power-laws-and-fractals.json
│
├── test-data/                         # Generated test artifacts
│   ├── base-data.json                 # Iterations 0-3 (for platform init)
│   ├── test-input.json                # Iterations 4-7 (raw facts only)
│   └── answer-key.json                # All iterations with expected results
│
├── test-results/                      # Platform outputs
│   ├── python-results.json
│   ├── postgres-results.json
│   └── golang-results.json
│
├── python/                            # Python implementation
│   ├── run-tests.py
│   ├── rulebook-to-python.py
│   └── rulebook/                      # Generated models
│       ├── models.py
│       ├── data.py
│       └── utils.py
│
├── postgres/                          # PostgreSQL implementation
│   ├── run-tests.py
│   ├── 01-drop-and-create-tables.sql
│   ├── 02-create-functions.sql
│   └── 03-create-views.sql
│
├── golang/                            # Go implementation
│   ├── run-tests.go
│   ├── go.mod
│   └── pkg/rulebook/
│       ├── models.go
│       ├── data.go
│       └── utils.go
│
├── visualizer/                        # Reporting tools
│   ├── generate_report.py
│   ├── console_output.py
│   ├── compare.py
│   └── report.html
│
├── start.sh                           # Interactive launcher
├── orchestrator.py                    # Test orchestration
├── generate-test-data.py              # Data generation from SSoT
└── TESTING-PROTOCOL.md
```

---

## 3. Platform Architectures

### 3.1 Python Implementation

**Location:** `python/`

**Architecture:**
- Pure Python with dataclasses for type-safe models
- Lazy calculation pattern (values computed on first access)
- No external dependencies beyond standard library

**Key Components:**

```
python/
├── run-tests.py           # Test runner (entry point)
├── rulebook-to-python.py  # Code generator from SSoT
└── rulebook/
    ├── __init__.py
    ├── models.py          # System, Scale, SystemStats dataclasses
    ├── data.py            # Embedded data from SSoT
    └── utils.py           # Helper functions
```

**Calculation Flow:**

```python
# From models.py - Scale calculation chain
@dataclass
class Scale:
    # Raw fields
    scale_id: str
    system: str
    iteration: int
    measure: float
    
    # Cached computed values (lazy)
    _base_scale: Optional[float] = None
    _scale_factor: Optional[float] = None
    _scale_factor_power: Optional[float] = None
    _scale: Optional[float] = None
    _log_scale: Optional[float] = None
    _log_measure: Optional[float] = None

    def calculate_scale_factor_power(self) -> float:
        """Formula: =POWER(ScaleFactor, Iteration)"""
        if self._scale_factor_power is None:
            self._scale_factor_power = math.pow(self._scale_factor, self.iteration)
        return self._scale_factor_power
```

**Test Protocol:**
1. Load `base-data.json` (systems config + iterations 0-3)
2. Load `test-input.json` (raw facts for iterations 4-7)
3. Compute all derived values using lookup chain
4. Export to `test-results/python-results.json`
5. Validate against `answer-key.json`

---

### 3.2 PostgreSQL Implementation

**Location:** `postgres/`

**Architecture:**
- Normalized tables store only raw data
- SQL functions compute derived values
- Views present "raw + computed" as a unified interface
- Requires PostgreSQL (via Docker or native)

**Key Components:**

```
postgres/
├── run-tests.py                # Python wrapper for psql
├── 01-drop-and-create-tables.sql   # Schema definition
├── 02-create-functions.sql         # Calculation functions (~4000 lines)
├── 03-create-views.sql             # vw_scales, vw_systems, etc.
├── 04-create-policies.sql          # Row-level security (optional)
└── 05-insert-data.sql              # Seed data from SSoT
```

**Table Schema (normalized):**

```sql
-- systems: Only raw fields stored
CREATE TABLE systems (
  system_id                TEXT PRIMARY KEY,
  display_name             TEXT,
  class                    TEXT,
  base_scale               INTEGER,
  scale_factor             NUMERIC,
  measure_name             TEXT,
  fractal_dimension        NUMERIC,
  theoretical_log_log_slope NUMERIC
);

-- scales: Raw facts only (computed values via views)
CREATE TABLE scales (
  scale_id      TEXT PRIMARY KEY,
  "system"      TEXT,
  iteration     INTEGER,
  measure       NUMERIC,
  is_projected  BOOLEAN
);
```

**Computed Views:**

```sql
-- vw_scales: Raw + all calculated fields
CREATE VIEW vw_scales AS
SELECT 
  s.scale_id,
  s.system,
  s.iteration,
  s.measure,
  -- Lookups from parent system
  sys.base_scale,
  sys.scale_factor,
  -- Calculations
  POWER(sys.scale_factor, s.iteration) AS scale_factor_power,
  sys.base_scale * POWER(sys.scale_factor, s.iteration) AS scale,
  LOG(sys.base_scale * POWER(sys.scale_factor, s.iteration)) AS log_scale,
  LOG(s.measure) AS log_measure,
  s.is_projected
FROM scales s
JOIN systems sys ON s.system = sys.system_id;
```

**Test Protocol:**
1. Initialize schema (run SQL files in order)
2. Insert systems from `base-data.json`
3. Insert base scales (iterations 0-3)
4. Insert test scales (iterations 4-7, raw facts only)
5. Query `vw_scales` to retrieve PostgreSQL-computed values
6. Export to `test-results/postgres-results.json`
7. Validate against `answer-key.json`

---

### 3.3 Golang Implementation

**Location:** `golang/`

**Architecture:**
- Strongly typed structs with pointer-based lazy caching
- Module-based organization (`pkg/rulebook`)
- Single-file test runner with embedded visualization

**Key Components:**

```
golang/
├── run-tests.go          # Entry point (main)
├── go.mod                 # Module definition
└── pkg/rulebook/
    ├── models.go          # System, Scale structs
    ├── data.go            # JSON loaders and savers
    └── utils.go           # Validation helpers
```

**Type Definitions:**

```go
// Scale with lazy-cached computed values
type Scale struct {
    ScaleID     string  `json:"ScaleID"`
    System      string  `json:"System"`
    Iteration   int     `json:"Iteration"`
    Measure     float64 `json:"Measure"`
    IsProjected bool    `json:"IsProjected"`

    // Computed values (nil until calculated)
    baseScale        *float64
    scaleFactor      *float64
    scaleFactorPower *float64
    scale            *float64
    logScale         *float64
    logMeasure       *float64
}

// CalculateAllFields computes all derived values in dependency order
func (s *Scale) CalculateAllFields(systems SystemsMap) {
    s.CalculateBaseScale(systems)    // Lookup
    s.CalculateScaleFactor(systems)  // Lookup
    s.CalculateScaleFactorPower()    // math.Pow(ScaleFactor, Iteration)
    s.CalculateScale()               // BaseScale × ScaleFactorPower
    s.CalculateLogScale()            // math.Log10(Scale)
    s.CalculateLogMeasure()          // math.Log10(Measure)
}
```

**Test Protocol:**
1. Load `base-data.json` via `rulebook.LoadBaseData()`
2. Build systems map: `rulebook.BuildSystemsMap()`
3. Load `test-input.json` via `rulebook.LoadTestInput()`
4. Compute derived values: `scale.CalculateAllFields(systemsMap)`
5. Export to `test-results/golang-results.json`
6. Validate: `rulebook.ValidateAllScales(computed, answerKey)`

---

### 3.4 HTML Visualizer

**Location:** `visualizer/`

**Architecture:**
- Pure Python report generator (no web server)
- Outputs static HTML with embedded Chart.js
- Dark theme with color-coded validation status

**Key Components:**

```
visualizer/
├── generate_report.py     # HTML report generator
├── console_output.py      # Shared ASCII visualization library
├── compare.py             # Cross-platform comparison tool
├── report.html            # Generated output
└── index.html             # Static dashboard
```

**Report Features:**
- Platform status cards (Python, PostgreSQL, Go)
- Per-system tables with all 8 iterations
- Color coding:
  - 🟢 Green = Actual data (iterations 0-3)
  - 🟣 Purple = Projected/computed (iterations 4-7)
  - 🔴 Red = Validation failures
- Interactive log-log scatter plots with Chart.js
- Theoretical slope lines overlaid

---

## 4. Operational Methodology

### 4.1 The `start.sh` Interactive Launcher

The entry point is a bash script providing a menu-driven interface:

```
╔════════════════════════════════════════════════════════════╗
║     🔺 POWER LAWS & FRACTALS - Veritasium Edition         ║
╚════════════════════════════════════════════════════════════╝

  Run Tests:
  1)  🧪  Run ALL Platform Tests   (+ opens report)
  2)  🐍  Python Only
  3)  🐹  Go Only
  4)  🐘  PostgreSQL Only          (requires Docker)

  View:
  5)  📊  View Results Report      (opens in browser)

  Utilities:
  g)  🔄  Regenerate Test Data     (CANONICAL Python, 6dp)
  s)  📄  View SSoT JSON
  r)  📖  View README
  j)  📓  Jupyter Notebook

  q)  ❌  Quit
```

**Menu Actions:**

| Option | Action |
|--------|--------|
| `1` | Runs `orchestrator.py --all`, then opens HTML report |
| `2` | Runs `python/run-tests.py` directly |
| `3` | Runs `cd golang && go run .` |
| `4` | Runs `postgres/run-tests.py` (requires psql + Docker) |
| `5` | Generates and opens `visualizer/report.html` |
| `g` | Runs `generate-test-data.py` to rebuild test artifacts |

---

### 4.2 Test Data Generation

**Script:** `generate-test-data.py`

This script reads the SSoT and produces three JSON files:

```
SSoT (ERB_veritasium-power-laws-and-fractals.json)
                    │
                    ▼
    ┌───────────────┼───────────────┐
    │               │               │
    ▼               ▼               ▼
base-data.json  test-input.json  answer-key.json
(iters 0-3)     (iters 4-7)      (all iters)
(all values)    (raw facts only) (all values)
```

**Key Design Decisions:**
- All numeric values rounded to **6 decimal places** for cross-platform consistency
- Base data contains full computed values (platforms don't recompute these)
- Test input contains ONLY raw facts—platforms must compute derived values
- Answer key is the **canonical reference** that all platforms must match

**Data Split:**
- Iterations 0-3: "Actual" data (IsProjected = false)
- Iterations 4-7: "Projected" data (IsProjected = true, tested)

**Measure Generation:**
Uses the power-law relationship to generate synthetic measures:
```python
# log(Measure) = slope × log(Scale) + constant
log_measure = slope * log_scale + constant
measure = 10 ** log_measure
```

---

### 4.3 Test Orchestration

**Script:** `orchestrator.py`

The orchestrator is the central test coordinator:

```bash
# Run all platforms
./orchestrator.py --all

# Run specific platform
./orchestrator.py --platform python

# Regenerate test data first
./orchestrator.py --all --regenerate

# Generate HTML report after tests
./orchestrator.py --all --report
```

**Orchestration Flow:**

```
┌─────────────────────────────────────────────────────────────┐
│                    orchestrator.py                          │
│                                                             │
│  1. [Optional] Regenerate test data                         │
│     └── subprocess: python generate-test-data.py            │
│                                                             │
│  2. Run each platform's test runner                         │
│     ├── python:   subprocess: python python/run-tests.py    │
│     ├── postgres: subprocess: python postgres/run-tests.py  │
│     └── golang:   subprocess: go run . (from golang/)       │
│                                                             │
│  3. Validate each platform's results                        │
│     └── Compare test-results/{platform}-results.json        │
│         against test-data/answer-key.json                   │
│                                                             │
│  4. Print summary                                           │
│     ├── ✓ python: 28 scales validated                      │
│     ├── ✓ postgres: 28 scales validated                    │
│     └── ✓ golang: 28 scales validated                      │
│                                                             │
│  5. [Optional] Generate HTML report                         │
│     └── subprocess: python visualizer/generate_report.py    │
└─────────────────────────────────────────────────────────────┘
```

**Validation Logic:**

```python
# Tolerance for floating-point comparison
TOLERANCE = 0.0000015  # Allows for 6dp rounding boundary

# Fields validated for each scale
COMPUTED_FIELDS = [
    'BaseScale',       # Lookup from system
    'ScaleFactor',     # Lookup from system
    'ScaleFactorPower',# POWER(ScaleFactor, Iteration)
    'Scale',           # BaseScale × ScaleFactorPower
    'LogScale',        # LOG10(Scale)
    'LogMeasure'       # LOG10(Measure)
]
```

---

### 4.4 Console Visualization

**Library:** `visualizer/console_output.py`

All three platform runners use this shared library for consistent output:

```
================================================================================
  🐍 POWER LAWS & FRACTALS - Python Test Runner
================================================================================

All Computed Values (from Python):
  ● Green = Actual Data (iterations 0-3)
  ◌ Magenta = Projected/Computed (iterations 4-7)
────────────────────────────────────────────────────────────────────────────────

🔺 Sierpinski Triangle
  Theoretical slope: -1.585

  Iter       Measure           Scale     LogScale    LogMeasure        Type
  ──────────────────────────────────────────────────────────────────────────
     0      1.000000      1.00000000     0.00000       0.00000  ● actual
     1      3.000000      0.50000000    -0.30103       0.47712  ● actual
     2      9.000000      0.25000000    -0.60206       0.95424  ● actual
     3     27.000000      0.12500000    -0.90309       1.43136  ● actual
     4     82.500000      0.06250000    -1.20412       1.91645  ◌ projected
     5    246.200000      0.03125000    -1.50515       2.39129  ◌ projected
     6    711.000000      0.01562500    -1.80618       2.85187  ◌ projected
     7   2225.000000      0.00781250    -2.10721       3.34733  ◌ projected

  Log-Log Plot:
  log(Measure)
     3.35 ┤
        │                                        ◌
        │                                   ◌
        │                              ◌
        │                         ◌
        │                    ●
        │               ●
        │          ●
        │     ●
     0.00 ┤●··········································
         └──────────────────────────────────────────────
         -2.11                                      0.00
                          log(Scale)
  ● Actual   ◌ Projected   · Theoretical (slope=-1.585)
```

---

### 4.5 HTML Report Generation

**Script:** `visualizer/generate_report.py`

Produces a comprehensive dark-themed HTML report:

**Features:**
- Status banner (ALL PASSED or FAILURES DETECTED)
- Platform status cards (🐍 Python, 🐘 PostgreSQL, 🐹 Go)
- Per-system sections with:
  - Data tables showing all 8 iterations
  - Expected vs. actual values for each platform
  - Failed values highlighted in red
  - Interactive Chart.js log-log scatter plots
  - Failed points rendered as red X markers

**Sample output structure:**

```html
<div class="status-banner passed">
    ✓ ALL PLATFORMS PASSED
</div>

<div class="platform-grid">
    <div class="platform-card passed">
        <div class="platform-name">🐍 Python</div>
        <div class="platform-status passed">✓ PASSED</div>
        <div class="platform-counts">28 passed, 0 failed</div>
    </div>
    <!-- ... -->
</div>
```

---

## 5. The Calculation Chain

All three platforms must implement the same calculation chain:

```
┌─────────────────────────────────────────────────────────────┐
│                    RAW FACTS (Input)                        │
│   ScaleID, System, Iteration, Measure, IsProjected          │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    LOOKUP (From System)                     │
│   BaseScale ← systems[System].BaseScale                     │
│   ScaleFactor ← systems[System].ScaleFactor                 │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    CALCULATED                               │
│   ScaleFactorPower = ScaleFactor ^ Iteration                │
│   Scale = BaseScale × ScaleFactorPower                      │
│   LogScale = log₁₀(Scale)                                   │
│   LogMeasure = log₁₀(Measure)                               │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    OUTPUT (JSON)                            │
│   All values rounded to 6 decimal places                    │
└─────────────────────────────────────────────────────────────┘
```

---

## 6. Running the Tests

### Prerequisites

| Platform | Requirements |
|----------|-------------|
| Python | Python 3.8+ |
| PostgreSQL | `psql` CLI + Docker (or native PostgreSQL) |
| Go | Go 1.21+ |

### Quick Start

```bash
# Make executable
chmod +x start.sh

# Run interactive launcher
./start.sh

# Or run all tests directly
./orchestrator.py --all

# Or run individual platforms
python python/run-tests.py
cd golang && go run .
python postgres/run-tests.py  # Requires PostgreSQL
```

### PostgreSQL Setup (Docker)

```bash
# Start PostgreSQL in Docker
docker run -d -p 5432:5432 \
  -e POSTGRES_HOST_AUTH_METHOD=trust \
  --name power-laws-postgres \
  postgres

# Run tests
python postgres/run-tests.py
```

---

## 7. Key Files Reference

| File | Purpose |
|------|---------|
| `ssot/ERB_*.json` | Source of Truth - defines all data models and schemas |
| `generate-test-data.py` | Creates test artifacts from SSoT |
| `orchestrator.py` | Master test coordinator |
| `start.sh` | Interactive menu launcher |
| `python/run-tests.py` | Python platform test runner |
| `postgres/run-tests.py` | PostgreSQL platform test runner |
| `golang/run-tests.go` | Go platform test runner |
| `visualizer/generate_report.py` | HTML report generator |
| `visualizer/console_output.py` | Shared ASCII visualization |
| `test-data/answer-key.json` | Canonical expected results |

---

## 8. Validation Criteria

A platform **PASSES** when all projected scales (iterations 4-7) match the answer key within tolerance:

- **Tolerance:** 0.0000015 (accounts for 6 decimal place rounding)
- **Fields validated:** BaseScale, ScaleFactor, ScaleFactorPower, Scale, LogScale, LogMeasure
- **Total validated scales:** 28 (7 systems × 4 projected iterations)

A platform **FAILS** if:
- Any computed value differs beyond tolerance
- Results file is not generated
- Scale records are missing
- Execution times out (120s limit)

---

## Summary

This repository demonstrates a practical implementation of the "Single Source of Truth" pattern for mathematical models:

1. **One SSoT** → Multiple equivalent implementations
2. **Automated code generation** for each platform
3. **Unified test protocol** ensuring consistency
4. **Rich visualization** for validation and debugging
5. **Interactive launcher** for easy operation

The architecture ensures that the mathematical truth (power-law relationships, fractal dimensions) is defined once and computed identically across Python, PostgreSQL, and Go.


