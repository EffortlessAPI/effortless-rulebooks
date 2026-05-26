# Derived SDK — Generated from Effortless Rulebook

This directory contains auto-generated building blocks for a Vite + React
frontend (and optionally a Node/Express backend). Drop these files into your
project and import what you need.

> **Re-running `effortless build` regenerates all files marked `Always`.**
> Files you customize won't be overwritten.

## Tables in this rulebook

| Table | Hook | API path | View |
|-------|------|----------|------|
| Users | `useUsers()` | `/api/users` | `vw_users` |
| Businesses | `useBusinesses()` | `/api/businesses` | `vw_businesses` |
| BeneficialOwners | `useBeneficialOwners()` | `/api/beneficial_owners` | `vw_beneficial_owners` |
| Contacts | `useContacts()` | `/api/contacts` | `vw_contacts` |
| Accounts | `useAccounts()` | `/api/accounts` | `vw_accounts` |
| Loans | `useLoans()` | `/api/loans` | `vw_loans` |
| Covenants | `useCovenants()` | `/api/covenants` | `vw_covenants` |
| RiskRatingHistory | `useRiskRatingHistory()` | `/api/risk_rating_history` | `vw_risk_rating_history` |
| Documents | `useDocuments()` | `/api/documents` | `vw_documents` |
| Interactions | `useInteractions()` | `/api/interactions` | `vw_interactions` |

## Frontend SDK (`src/derived-sdk/`)

### Installation

Copy `src/derived-sdk/` into your Vite + React project's `src/` directory.
No additional dependencies required — uses plain `fetch` and React hooks.

### `api.js` — HTTP client

A thin `fetch` wrapper with per-table CRUD methods:

```js
import { api } from './derived-sdk';

// List all users
const items = await api.users.list();

// Get one by ID
const item = await api.users.get(id);

// Create
await api.users.create({ name: 'New item' });

// Update
await api.users.update(id, { name: 'Updated' });

// Delete
await api.users.remove(id);
```

### `schema.js` — Table/field metadata

Exports the full schema so your components can introspect fields, types,
and relationships at runtime:

```js
import { schema } from './derived-sdk';

schema.users.fields      // all field definitions
schema.users.rawFields   // writable field names
schema.users.primaryKey  // 'id'
schema.users.nameField   // display field
```

### `hooks/use<Table>.js` — React hooks

Each table gets 5 hooks:

| Hook | Returns | Purpose |
|------|---------|---------|
| `use<Plural>()` | `{ data, loading, error, refresh }` | List all records |
| `use<Singular>(id)` | `{ data, loading, error }` | Get one record |
| `useCreate<Singular>()` | `{ mutate, loading, error }` | Create a record |
| `useUpdate<Singular>()` | `{ mutate, loading, error }` | Update a record |
| `useDelete<Singular>()` | `{ mutate, loading, error }` | Delete a record |

**Example — list page:**

```jsx
import { useUsers } from './derived-sdk';

function UsersList() {
  const { data, loading, error } = useUsers();
  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error.message}</p>;
  return (
    <ul>
      {data.map(item => <li key={item.id}>{item.name}</li>)}
    </ul>
  );
}
```

**Example — create form:**

```jsx
import { useCreateUser } from './derived-sdk';

function CreateUserForm() {
  const { mutate, loading, error } = useCreateUser();
  const [name, setName] = useState('');

  async function handleSubmit(e) {
    e.preventDefault();
    await mutate({ name });
  }

  return (
    <form onSubmit={handleSubmit}>
      <input value={name} onChange={e => setName(e.target.value)} />
      <button disabled={loading}>Create</button>
      {error && <p>{error.message}</p>}
    </form>
  );
}
```

## Backend SDK (`backend/derived-sdk/`)

Pre-built Express routes that read from `vw_*` views and write to base tables.

### Setup

```bash
npm install express pg cors
export DATABASE_URL=postgresql://postgres@localhost:5432/effortless_banking
```

### Usage

Create a `server.js` that mounts the generated routes:

```js
import express from 'express';
import cors from 'cors';
import routes from './derived-sdk/routes/index.js';

const app = express();
app.use(cors());
app.use(express.json());
app.use('/api', routes);

app.listen(3001, () => console.log(`Running on :3001`));
```

### What each route does

| Method | Path | Action |
|--------|------|--------|
| GET | `/api/<table>` | `SELECT * FROM vw_<table>` |
| GET | `/api/<table>/:id` | `SELECT * FROM vw_<table> WHERE <pk> = $1` |
| POST | `/api/<table>` | `INSERT INTO <table> (raw fields)` |
| PUT | `/api/<table>/:id` | `UPDATE <table> SET ... WHERE <pk> = $1` |
| DELETE | `/api/<table>/:id` | `DELETE FROM <table> WHERE <pk> = $1` |

> Reads go through `vw_*` views (includes all calculated/lookup fields).
> Writes target base tables using only raw fields.

## Generated files

| File | Regenerated? | Purpose |
|------|:---:|---------|
| `src/derived-sdk/api.js` | Yes | HTTP client with per-table CRUD |
| `src/derived-sdk/schema.js` | Yes | Table/field metadata |
| `src/derived-sdk/index.js` | Yes | Barrel re-export |
| `src/derived-sdk/hooks/use*.js` | Yes | React hooks (one per table) |
| `backend/derived-sdk/db.js` | Yes | pg Pool connection |
| `backend/derived-sdk/schema.js` | Yes | Same metadata (backend copy) |
| `backend/derived-sdk/routes/*.js` | Yes | Express CRUD routes |
| `backend/derived-sdk/routes/index.js` | Yes | Route aggregator |
| `derived-sdk-README.md` | No | This file |

