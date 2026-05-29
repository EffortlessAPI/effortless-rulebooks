# Rails Substrate Tools

Four focused, single-purpose tools for generating Rails artifacts from an Effortless rulebook.

## Overview

Each tool generates one artifact type and can be run independently:

```
rulebook.json
    ↓
    ├─→ rulebook-to-rails-models.py      → app/models/*.rb
    ├─→ rulebook-to-rails-migrations.py  → db/migrate/*.rb
    ├─→ rulebook-to-rails-views.py       → db/views/*.sql
    └─→ rulebook-to-rails-seeds.py       → db/seeds/*.rb + db/seeds.rb
```

## Tools

### 1. `rulebook-to-rails-models.py`

**Generates:** Rails model classes in `app/models/`

**Purpose:** Create Active Record models that read from PostgreSQL views where all computed fields are materialized.

**Required parameters:**
- `ERB_RULEBOOK_PATH` — path to the rulebook JSON

**Optional parameters:**
- `ERB_OUTPUT_DIR` — output directory (default: current directory)

**What it generates:**
- One model file per entity (table)
- Models are read-only (tied to views)
- Includes basic validations for non-nullable raw fields
- Proper primary key configuration

**Example:**
```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
ERB_OUTPUT_DIR=. \
python rulebook-to-rails-models.py
```

**Output:**
```
app/models/customers.rb
app/models/orders.rb
```

---

### 2. `rulebook-to-rails-migrations.py`

**Generates:** Rails migrations in `db/migrate/`

**Purpose:** Generate schema migrations, with smart delta detection (add/drop columns only if needed).

**Required parameters:**
- `ERB_RULEBOOK_PATH` — path to the rulebook JSON
- `ERB_OUTPUT_DIR` — output directory

**Optional parameters (for schema comparison):**
- `ERB_PGDUMP_PATH` — path to a PostgreSQL schema dump (`.sql` file)
  - OR `ERB_DATABASE_URL` — live Postgres connection string (requires `psycopg2`)

**Behavior:**

1. **If no schema provided:** Generates full `create_table` migration (for new tables)
2. **If schema provided:** Generates smart migrations:
   - `add_column` for new fields in rulebook
   - `remove_column` for fields not in rulebook
   - Skips columns that already match

**Example (full create_table):**
```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
ERB_OUTPUT_DIR=. \
python rulebook-to-rails-migrations.py
```

**Example (smart migrations from pgdump):**
```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
ERB_OUTPUT_DIR=. \
ERB_PGDUMP_PATH=../postgres/schema.sql \
python rulebook-to-rails-migrations.py
```

**Example (smart migrations from live database):**
```bash
ERB_DATABASE_URL=postgresql://user:pass@localhost/erb_myapp \
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
ERB_OUTPUT_DIR=. \
python rulebook-to-rails-migrations.py
```

**Output:**
```
db/migrate/1716993456000_customers.rb
db/migrate/1716993457000_update_orders.rb
```

---

### 3. `rulebook-to-rails-views.py`

**Generates:** PostgreSQL view definitions in `db/views/`

**Purpose:** Create views that materialize all calculated, lookup, and aggregation fields.

**Required parameters:**
- `ERB_RULEBOOK_PATH` — path to the rulebook JSON
- `ERB_OUTPUT_DIR` — output directory

**What it generates:**
- One `.sql` file per entity (table)
- `CREATE OR REPLACE VIEW vw_<entity>` statements
- All raw columns + computed columns (calculated by SQL functions)
- Rails models read from these views (the view IS the contract)

**Example:**
```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
ERB_OUTPUT_DIR=. \
python rulebook-to-rails-views.py
```

**Output:**
```
db/views/customers.sql    # SELECT id, name, full_name (calculated), ...
db/views/orders.sql       # SELECT id, order_date, total (aggregated), ...
```

**Formula conversion:**
- Excel `&` (concatenation) → PostgreSQL `||`
- Excel `LEFT(str, n)` → PostgreSQL `SUBSTRING(str FROM 1 FOR n)`
- Excel `RIGHT(str, n)` → PostgreSQL `SUBSTRING(str FROM LENGTH(str) - n + 1)`
- Field references `{{FieldName}}` → column names `"field_name"`

---

### 4. `rulebook-to-rails-seeds.py`

**Generates:** Rails seed data in `db/seeds/` + `db/seeds.rb`

**Purpose:** Populate the database with seed data from the rulebook.

**Required parameters:**
- `ERB_RULEBOOK_PATH` — path to the rulebook JSON
- `ERB_OUTPUT_DIR` — output directory

**What it generates:**
- One `.rb` file per entity that has data
- `Model.create()` calls for each row in the rulebook
- Only seeds raw fields (skips calculated fields)
- Main `db/seeds.rb` that loads all seed files

**Example:**
```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
ERB_OUTPUT_DIR=. \
python rulebook-to-rails-seeds.py
```

**Output:**
```
db/seeds/customers.rb
db/seeds/orders.rb
db/seeds.rb           # Loader
```

**In Rails console:**
```bash
rails db:seed
# Loads all entities from the rulebook into the database
```

---

## Philosophy

All tools follow **Effortless doctrine:**

1. **Views are the contract** — Computed values are materialized in PostgreSQL views, never re-derived in Ruby
2. **One tool, one artifact** — Each tool does one thing well, with clear inputs and outputs
3. **Optional schema comparison** — Migrations can be smart (add/drop only) or full (create_table)
4. **Deterministic and idempotent** — Running tools multiple times produces the same output

---

## Integration Pattern (Future)

Each tool can be registered as a separate `effortless.json` transpiler:

```json
{
  "Name": "rulebook-to-rails-models",
  "CommandLine": "http://localhost:4242/rulebook-to-rails-models -i ../effortless-rulebook/rulebook.json",
  "RelativePath": "/rails"
},
{
  "Name": "rulebook-to-rails-migrations",
  "CommandLine": "http://localhost:4242/rulebook-to-rails-migrations -i ../effortless-rulebook/rulebook.json -p pgdump=../postgres/schema.sql",
  "RelativePath": "/rails"
},
{
  "Name": "rulebook-to-rails-views",
  "CommandLine": "http://localhost:4242/rulebook-to-rails-views -i ../effortless-rulebook/rulebook.json",
  "RelativePath": "/rails"
},
{
  "Name": "rulebook-to-rails-seeds",
  "CommandLine": "http://localhost:4242/rulebook-to-rails-seeds -i ../effortless-rulebook/rulebook.json",
  "RelativePath": "/rails"
}
```

Then `effortless build` would run all four tools in order.

---

## Testing

Each tool can be tested independently:

```bash
cd rails

# Test models
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
python rulebook-to-rails-models.py
ls app/models/

# Test migrations (full)
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
python rulebook-to-rails-migrations.py
ls db/migrate/

# Test views
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
python rulebook-to-rails-views.py
ls db/views/

# Test seeds
ERB_RULEBOOK_PATH=../effortless-rulebook/rulebook.json \
python rulebook-to-rails-seeds.py
ls db/seeds/
```
