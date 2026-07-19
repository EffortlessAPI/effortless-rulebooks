# Generated Postgres projection

This directory is an output spoke.

Run:

```bash
../start.sh build
```

The `rulebooktopostgres` transpiler generates the SQL files and `init-db.sh` from:

```text
../effortless-rulebook/traveling-salesman-rulebook.json
```

Do not hand-maintain `00`–`05` SQL. `../start.sh db` drops and recreates `erb_traveling_salesman`, then loads the generated initializer with an explicit `DATABASE_URL`.
