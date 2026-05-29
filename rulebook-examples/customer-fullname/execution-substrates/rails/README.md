# Rails Substrate

This substrate generates a full Rails application from the Effortless rulebook.

## What It Generates

- **Models** (`app/models/`) — Active Record models that read from views
- **Migrations** (`db/migrate/`) — Schema migrations (smart comparison with current DB)
- **Views** (`db/views/`) — PostgreSQL views that materialize computed fields
- **Seeds** (`db/seeds/`) — Test data from the rulebook

## Philosophy

The generated Rails app follows Effortless doctrine:

1. **Views are the contract** — All calculated, lookup, and aggregation fields are materialized in PostgreSQL views
2. **Models read from views** — Active Record models use `default_scope` / explicit `.from()` to read from `vw_<entity>`
3. **Never re-derive computed values** — If a field is computed in the rulebook, it's computed in SQL, not Ruby

## Usage

### Generate from scratch

```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/customer-fullname-rulebook.json \
ERB_OUTPUT_DIR=. \
python inject-rails-models.py
```

### Generate with schema comparison (pgdump)

```bash
ERB_RULEBOOK_PATH=../effortless-rulebook/customer-fullname-rulebook.json \
ERB_OUTPUT_DIR=. \
ERB_PGDUMP_PATH=../postgres/schema.sql \
python inject-rails-models.py
```

The transpiler will:
1. Parse the current schema from the pgdump
2. Compare it to the rulebook
3. Generate migrations to add missing columns, drop obsolete ones, etc.

### Generate with schema comparison (live database)

```bash
ERB_DATABASE_URL=postgresql://user:pass@localhost/erb_customer_fullname \
ERB_RULEBOOK_PATH=../effortless-rulebook/customer-fullname-rulebook.json \
ERB_OUTPUT_DIR=. \
python inject-rails-models.py
```

The transpiler will:
1. Query the live Postgres database
2. Extract current schema
3. Generate migrations to sync it to the rulebook

## File Structure

```
rails/
├── app/models/           # Generated Rails models (read-only views)
├── db/
│   ├── migrate/          # Generated migrations
│   ├── views/            # Generated view SQL
│   └── seeds/            # Generated seed data
├── db/seeds.rb           # Main seeds loader
├── inject-rails-models.py # The transpiler (this script)
└── README.md             # This file
```

## Integration with Effortless Build

Register this transpiler in the project's `effortless.json`:

```bash
cd rulebook-examples/customer-fullname/rails
effortless -install http://localhost:4242/rulebook-to-rails \
    -i ../effortless-rulebook/customer-fullname-rulebook.json
```

Then `effortless build` will regenerate Rails code on every rebuild.

## Smart Migrations

When a pgdump or live database is provided, the transpiler generates only the migrations needed to sync the schema:

- **Add columns** that are in the rulebook but not in the DB
- **Drop columns** that are in the DB but not in the rulebook
- **Skip** columns that are already up to date

This means you can safely run `effortless build` repeatedly without creating duplicate migrations.

## View-Based Models

Generated models use PostgreSQL views:

```ruby
class Customer < ApplicationRecord
  self.table_name = 'vw_customer'
  self.primary_key = 'customer_id'

  def readonly?
    true
  end
end
```

When you query `Customer.all`, you get:
- Raw columns (first_name, last_name)
- Computed columns (full_name, initials) — all calculated by the view

You never query the raw table directly. The view IS the interface.

## Testing

To test the generated Rails app:

```bash
# Install gems
bundle install

# Run migrations
rails db:migrate

# Load seeds
rails db:seed

# Test the models
rails console
> Customer.all
> Customer.first.full_name
```

All computed fields are already populated by the view.
