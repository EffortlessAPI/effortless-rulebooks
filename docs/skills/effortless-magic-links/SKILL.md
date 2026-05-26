<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-magic-links/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-magic-links
description: >
  Use when the user wants to add passwordless email-code (magic-link) auth to
  any project backed by a Postgres database — not just bases.effortlessapi.com.
  Covers minting a tenant on magiclink.effortlessapi.com, storing the
  public key for JWT verification, wiring app-side `Authorization: Bearer`
  middleware, and (optionally) installing the `app.jwt_*()` SQL helpers so
  RLS policies can filter by the verified email. Triggers: "add magic links
  to this app", "secure this app with magic links", "passwordless auth on a
  postgres app", "wire JWT auth into this project".

  **Scope (load gate):** Loads only on explicit user request — applies to any Postgres app, not just Effortless-marked projects. Do not auto-load just because a project uses Postgres.
audience: general
---

# Magic Links → any Postgres app

> **Load-bearing axiom: Magic-links is a notary, not a referee.**
> It makes one claim per JWT: *"we sent code C to email E, the holder of C
> returned it, therefore E is verified."* Magic-links stores no end-users,
> no roles, no ownership. All user-shaped state lives in the **consuming
> app's** Postgres database.

If a feature wants magic-links to know what an end-user does in your app,
the feature belongs in your app — not in magic-links. See
`magiclink.effortlessapi.com/UNIFICATION-PLAN.md` for the full contract.

This skill is the **generic** flow: any project, any Postgres database. For
the bases-specific flow (auto-RLS templates, `auth.trusted_tenants`,
`/auth/generate-policy`), use the `effortless-bases` skill instead.

> Long-tail material — the recursion gotcha for `app.jwt_role()`, the
> multi-DB tenant-sharing pattern, the FastAPI middleware flavor, the full
> refresh flow, and the cheat sheet — lives in [REFERENCE.md](REFERENCE.md).
> The core flow (Steps 0–5, RLS, Common Mistakes) stays here.

## "AppUsers" / "Roles" / "Profiles" tables belong in the consuming app, NOT in `app.*`

Magic-links stores no users. Your app does. If the app is an ERB project,
that means `AppUsers` (or whatever you call it) is an **Airtable entity**,
regenerated as `public.app_users` + `vw_app_users`. The `app.jwt_role()`
helper reads from `vw_app_users`.

Do NOT create `app.app_users` (or `app.users`, `app.profiles`, etc.) by
hand in `01b-customize-schema.sql`. That mirror will drift from the
rulebook, never appear in views, and collide with the real entity once
someone adds it the right way.

The only legitimate hand-written tables in `auth` / `app` are things the
rulebook genuinely cannot model: `auth.trusted_tenants` (JWT public keys)
and the `app.jwt_*()` helper functions themselves.

---

## Default action when invoked

When the user says "add magic links to this project" / "wire up magic
links" / "integrate magic links" / "secure this app with magic links" or
debugs a broken magic-link login, **just do it** — don't ask "want me to
proceed?" before each step. The user invoking the skill **is** the
go-ahead.

Run this checklist top-to-bottom:

1. **Locate or mint the tenant.** Grep the project for an existing
   `TENANT_ID` / `MAGICLINK_TENANT_ID`. If found, verify it still exists
   on the magic-link service:
   ```bash
   curl -s "https://magiclink.effortlessapi.com/api/tenants/<id>" -w '\nHTTP %{http_code}\n'
   ```
   404 `tenant_not_found` → re-mint (the upstream may have been wiped, or
   the tenant was minted on a different magic-link instance). 200 with
   `public_key_pem` → reuse it; just confirm the in-app public key matches.
   No tenant in the project → mint a new one.
   To mint: run Step 0 (self-auth) then Step 1 (POST `/api/tenants`)
   below. **Do not put this in the user's lap as "you go run these
   curls."** Claude drives the curls; the user only reads a code from
   their inbox.
2. **Pick where the tenant config lives.** New project → env vars
   (`MAGICLINK_BASE_URL`, `MAGICLINK_TENANT_ID`, `MAGICLINK_PUBLIC_KEY_PEM`).
   Existing project that already hard-codes them → keep the same shape so
   you don't churn the diff.
3. **Wire (or fix) the server-side proxy + verifier.** Two routes —
   `POST /api/auth/request-code` → upstream `/api/tenants/<id>/send-code`,
   and `POST /api/auth/verify-code` → upstream
   `/api/tenants/<id>/verify-code` (note: upstream takes `code`, not
   `token`). Verify the returned JWT locally with RS256 against
   `public_key_pem`, with `tenant_id` claim pinned to your tenant.
4. **Wire (or check) the login UI** — two-step email → code, JWT stored
   in `localStorage` and sent as `Authorization: Bearer` on subsequent
   calls.
5. **Restart and smoke-test.** `curl` the local request-code endpoint;
   real `ok:true` means the upstream sent an email. (Heads-up:
   upstream `send-code` always returns `ok:true` even for non-existent
   tenants — see "Common mistakes". Always pair it with `GET
   /api/tenants/<id>` on first wire-up to confirm the tenant actually
   exists.)
6. **Persist tenant id + public-key location** in project memory or a
   short note in CLAUDE.md so future sessions don't re-mint.

If the project is on `bases.effortlessapi.com`, switch to the
`effortless-bases` skill before Step 3 — bases adds `auth.trusted_tenants`
registration and `app.jwt_*()` helpers that change the wiring.

---

## What "adding magic links" actually means

Three artifacts get added to your project:

1. **A tenant** on `magiclink.effortlessapi.com` (one per app). You
   receive `{tenant_id, public_key_pem}`. The private key never leaves
   magic-links.
2. **An auth middleware** in your app (or per request handler) that:
   - reads `Authorization: Bearer <jwt>`,
   - RS256-verifies it against the cached `public_key_pem`,
   - exposes `req.user.email` (and any `additional_claims`).
3. **A login UI** with two steps: collect email → call `send-code`, then
   collect 6-digit code → call `verify-code` → store the JWT.

**Optional but usually wanted:** SQL helpers + RLS so the *database* also
knows the verified email and can filter rows directly.

---

## API surface (the unified `/api/*` paths)

```
MAGICLINK = https://magiclink.effortlessapi.com
```

| Method | Path | Auth | Purpose |
|---|---|---|---|
| `POST` | `/auth/send-code` | open | Self-auth: send code to **developer's** email. |
| `POST` | `/auth/verify-code` | open | Self-auth: verify → JWT used to create tenants. |
| `POST` | `/api/tenants` | self-auth Bearer | Mint a tenant. Server generates keypair. Returns `{tenant_id, public_key_pem}`. |
| `GET`  | `/api/tenants/{id}` | open | Public info: `{tenant_id, public_key_pem, from_email, jwt_expires_in_seconds}`. |
| `PATCH`| `/api/tenants/{id}` | self-auth + ownership | Update `from_email` / `jwt_expires_in_seconds`. |
| `DELETE`| `/api/tenants/{id}` | self-auth + ownership | Remove tenant. |
| `POST` | `/api/tenants/{id}/send-code` | open | `{email, additional_claims?}` → `{ok:true}`. |
| `POST` | `/api/tenants/{id}/verify-code` | open | `{email, code, additional_claims?}` → `{ok, jwt, expires_in}`. |
| `POST` | `/api/tenants/{id}/refresh` | Bearer expired JWT | Multi-use within grace window → fresh JWT. (Details: [REFERENCE.md](REFERENCE.md#refresh-flow).) |

**`additional_claims`** are baked into the JWT verbatim. Reserved claims
**always win** (and you cannot override them): `email`, `iss`, `iat`,
`nbf`, `exp`, `sub`, `tenant_id`. No namespacing — your app owns its own
claim conventions (e.g. `role`, `app_user_id`).

**JWT details:** RS256, `iss = {magiclink_base}/{tenant_id}` (the **full
URL**, not the bare UUID), `tenant_id` = the bare UUID (separate claim),
`email` = verified email, `exp` per the tenant's `jwt_expires_in_seconds`
(default 3600s).

**Verifier gotcha:** look up your `auth.trusted_tenants` row by the
`tenant_id` claim — **not** by `iss`. Then pass `iss` as the expected
issuer to your JWT library. Confusing the two means your registry lookup
silently misses every token.

---

## Recipe: zero → secured app

### Step 0 — get a self-auth JWT (Claude drives this; user just reads a code)

**This is Claude's job, not the user's.** Whenever a tenant create/modify
operation is needed (`POST/PATCH/DELETE /api/tenants`), Claude obtains and
caches the self-auth JWT itself. The user's only involvement is reading
the 6-digit code from their inbox when asked.

The flow:

1. Claude POSTs `/auth/send-code` with the user's email (from
   `git config user.email`, the user's known email in context, or by
   asking once if neither is available).
2. Claude tells the user "I sent a code to <email> — paste it back."
3. User pastes the 6-digit code.
4. Claude POSTs `/auth/verify-code` and stores the returned token
   somewhere session-durable (e.g. `/tmp/magiclink-self-auth-<email>.json`
   with mode 0600). The response shape is `{ token, expires_in, user }`
   — the JWT field is `token`, **not** `jwt` (`/api/tenants/{id}/verify-code`
   uses `jwt`; `/auth/verify-code` uses `token` — easy to get wrong).
5. Claude reuses that cached token for every subsequent tenant op in the
   session, until `expires_in` lapses (default 3600s). Then re-runs Steps
   1–4.

```bash
MAGICLINK="https://magiclink.effortlessapi.com"
DEV_EMAIL="<the user's email>"

# Claude runs:
curl -sS "$MAGICLINK/auth/send-code" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$DEV_EMAIL\"}"
# → tell the user: "I sent a code to $DEV_EMAIL — paste it back."

# After user pastes CODE:
curl -sS "$MAGICLINK/auth/verify-code" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$DEV_EMAIL\",\"code\":\"$CODE\"}" \
  > /tmp/magiclink-self-auth-$DEV_EMAIL.json
chmod 600 /tmp/magiclink-self-auth-$DEV_EMAIL.json

SELF_JWT=$(jq -r .token /tmp/magiclink-self-auth-$DEV_EMAIL.json)
```

**Do not put this flow in the user's lap as "you go run these curls."**
The user shouldn't be juggling tokens; that's the agent's responsibility.

### Step 1 — mint the tenant

```bash
TENANT=$(curl -sS "$MAGICLINK/api/tenants" \
  -H "Authorization: Bearer $SELF_JWT" \
  -H "Content-Type: application/json" \
  -d '{"from_email":"noreply@yourapp.com","jwt_expires_in_seconds":3600}')

TENANT_ID=$(echo "$TENANT" | jq -r .tenant_id)
PUBLIC_KEY=$(echo "$TENANT" | jq -r .public_key_pem)
```

Persist `TENANT_ID` and `PUBLIC_KEY` in the project (config/env vars). The
`public_key_pem` is also fetchable any time from `GET /api/tenants/{id}`,
so caching it is a perf optimization, not a requirement.

**Suggested env vars:**
```
MAGICLINK_BASE_URL=https://magiclink.effortlessapi.com
MAGICLINK_TENANT_ID=<tenant_id>
MAGICLINK_PUBLIC_KEY_PEM=<public_key_pem multi-line>
```

### Step 2 — install the JWT verification middleware (Node / Express)

```js
import jwt from 'jsonwebtoken';

const PUBLIC_KEY = process.env.MAGICLINK_PUBLIC_KEY_PEM;
const TENANT_ID  = process.env.MAGICLINK_TENANT_ID;

export function requireAuth(req, res, next) {
  const auth = req.headers.authorization;
  if (!auth?.startsWith('Bearer ')) {
    return res.status(401).json({ error: 'missing_token' });
  }
  try {
    const token = auth.slice(7);
    // iss is the FULL URL form: https://magiclink.effortlessapi.com/<tenant_id>
    // Read it off the unverified payload, then pass it to jwt.verify so the
    // verifier checks the signature *and* matches the expected issuer.
    const unverified = jwt.decode(token);
    if (unverified?.tenant_id !== TENANT_ID) throw new Error('wrong_tenant');
    const decoded = jwt.verify(token, PUBLIC_KEY, {
      algorithms: ['RS256'],
      issuer: unverified.iss,
    });
    req.user = decoded; // { email, sub, iss, tenant_id, exp, ...additional_claims }
    next();
  } catch {
    return res.status(401).json({ error: 'invalid_token' });
  }
}
```

For Python / FastAPI flavor, see [REFERENCE.md → Python middleware](REFERENCE.md#python--fastapi-middleware).

### Step 3 — build the two-step login UI

```js
// 1. Email step
await fetch(`${MAGICLINK}/api/tenants/${TENANT_ID}/send-code`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email }),
});

// 2. Code step — optionally enrich with app-side claims
const { jwt } = await fetch(
  `${MAGICLINK}/api/tenants/${TENANT_ID}/verify-code`,
  {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email, code,
      additional_claims: { app_user_id, role }, // optional
    }),
  },
).then(r => r.json());

localStorage.setItem('jwt', jwt);
```

All subsequent API calls send `Authorization: Bearer ${jwt}`.

### Step 4 (optional) — push the verified email into Postgres

If you want **the database** to filter rows by the JWT email (RLS),
install the canonical `install-magic-links.sql` once (per `MAGIC_LINKS_REFACTOR.md`
§1), then per request hand the bearer token to `auth.set_jwt(token)`
inside a transaction. The helper RS256-verifies against
`auth.trusted_tenants` and writes `app.jwt_*` as transaction-local GUCs;
RLS policies read them via the `app.jwt_*()` helpers.

```sql
BEGIN;
SELECT auth.set_jwt($1);          -- $1 = the raw bearer token (TEXT)
SELECT * FROM documents;          -- RLS reads app.jwt_email()
COMMIT;
```

That's the entire wire-up — no `set_config` from app code, no manual
GUC threading, no second JWT-decode step. The token is verified once,
inside the database, against the registry. Connection-pool friendly:
the GUCs are transaction-local, so a checked-in connection cannot leak
identity to the next request.

#### Anti-pattern: v1 GUC-cache

If you see this shape in old code, it's the **v1 anti-pattern**. RLS
still fires, but the database trusts whatever the app put in the GUC
without verifying. Migrate it to the v2 shape above:

```sql
-- DO NOT WRITE THIS. Recognize it in legacy code, then migrate.
SELECT set_config('app.jwt_email',  $1, true);
SELECT set_config('app.jwt_claims', $2, true);
-- ... and policies that read current_setting('app.jwt_email', true) directly.
```

Migration recipe: install `install-magic-links.sql`, replace each Node
`requireAuth` middleware that called `set_config(...)` with `BEGIN; SELECT
auth.set_jwt($token); …; COMMIT;`, and rewrite RLS policies to call
`app.jwt_email()` instead of `current_setting(...)`. See `effortless-conventions/SKILL.md`
"v1 GUC-cache pattern" for the audit checklist.

### Step 5 — write RLS policies (skip if the DB doesn't need to filter)

Policies call the `app.jwt_*()` helpers — never `current_setting()`
directly. The helpers fail closed when no JWT has been set.

```sql
ALTER TABLE documents ENABLE ROW LEVEL SECURITY;
CREATE POLICY "deny_all" ON documents FOR ALL USING (false);

CREATE POLICY "owner_select" ON documents
  FOR SELECT TO magiclink_consumer
  USING (owner_email = app.jwt_email());

CREATE POLICY "owner_modify" ON documents
  FOR ALL TO magiclink_consumer
  USING (owner_email = app.jwt_email())
  WITH CHECK (owner_email = app.jwt_email());

-- Admin sees everything (role comes from the JWT's additional_claims).
CREATE POLICY "admin_select" ON documents
  FOR SELECT TO magiclink_consumer
  USING (app.has_role('admin'));
```

Smoke-test with a real token (no shortcuts via `SET LOCAL`):

```sql
BEGIN;
  SELECT auth.set_jwt('<paste a real bearer token>');
  SELECT app.jwt_email(), app.jwt_tenant_id(), app.has_role('admin');
  SET LOCAL ROLE app_anon;
  SELECT count(*) FROM documents;        -- filtered by RLS
COMMIT;
```

Without the surrounding `BEGIN`/`COMMIT` the GUCs evaporate between
statements and the test reads NULL — so always wrap.

> If `app.jwt_role()` (or any helper) reads an RLS-protected table, you
> can hit a recursion bug — `stack depth limit exceeded` on every
> authenticated request. The fix (mark the resolver `SECURITY DEFINER` +
> `SET row_security = off`) is in
> [REFERENCE.md → Recursion gotcha](REFERENCE.md#gotcha-recursion-when-the-role-resolver-reads-an-rls-protected-table).

## "It started up" is not "it works"

`200 /healthz` proves a process bound a port. It does not prove JWT
verification, RLS, role lookup, or view grants work. Before declaring an
auth/RLS change done, run the smoke test in Step 5 with a real bearer
token — the `auth.set_jwt` round-trip is what catches a missing
`auth.trusted_tenants` row, a wrong role grant, or a policy that
references a column that no longer exists.

---

## Common mistakes

- **Hand-decoding JWTs without verifying the signature.** Always RS256-verify
  against the saved `public_key_pem`. `atob(jwt.split('.')[1])` is a debug
  tool, not auth.
- **Treating magic-links as a user store.** It isn't. Your app's `users`
  table lives in your Postgres database, keyed by `email`.
- **Asking magic-links for the private key.** It will never give it to you.
  The only key the API ever returns is `public_key_pem`.
- **Reusing one tenant across unrelated apps.** One tenant per app —
  rotation, ownership, and audit live at the tenant boundary.
- **Forgetting to set `iss` on the verifier.** Without an `issuer` option,
  a JWT minted for *any* tenant the same magic-links instance hosts would
  pass — which is a cross-tenant auth bug. Pass the **decoded `iss`** (the
  URL form), not the bare tenant UUID.
- **Looking up `auth.trusted_tenants` by `iss`.** The registry's primary
  key is the bare UUID; `iss` is a URL. Use the `tenant_id` claim for the
  lookup.
- **Putting bases-side knowledge into magic-links via a custom endpoint.**
  Use `additional_claims` instead — they bake straight into the JWT, and
  reserved claims always win.
- **Setting `app.jwt_email` from app code at all.** That's the v1
  GUC-cache anti-pattern — the database trusts whatever you wrote.
  Hand the raw token to `SELECT auth.set_jwt($1)` inside a transaction
  and let the helper validate + populate the GUCs. It's transaction-local
  by construction, so a pooled connection cannot leak identity to the
  next request.

---

## See also

- [REFERENCE.md](REFERENCE.md) — long-tail material kept out of the core: Python/FastAPI middleware, the role-resolver recursion gotcha, refresh flow, multi-database tenant sharing, full cheat sheet.
- `effortless-bases` — switch to this skill if the project's database lives on `bases.effortlessapi.com`. Bases-specific endpoints (`/auth/generate-policy`, `/auth/apply-privileges-template`) and pre-installed `app.jwt_*()` helpers replace much of Steps 4–5 here.
- `effortless-orchestrator` — if this is an ERB project, `AppUsers` belongs in Airtable, not in `app.app_users` by hand.
- `effortless-sql` — for `*b-customize-*.sql` placement of `auth.trusted_tenants` and `app.jwt_*()` helpers in ERB projects.

---

## Magic-links refactor (v0.2)

> See [../../MAGIC_LINKS_REFACTOR.md](../../MAGIC_LINKS_REFACTOR.md) §2 + §3 for the canonical v0.2 magic-links contract.

REFERENCE.md does NOT inline the install SQL — it points at `https://magiclink.effortlessapi.com/install-magic-links/v1.sql` and documents the contract surface (`auth.set_jwt`, `app.jwt_email`, `app.jwt_tenant_id`, `app.jwt_claims`, `app.has_role`).

DO-NOT: never put `tenant_id` / `public_key_pem` in the rulebook. Never create `ERBmagiclinks` or `MagicLinkIntegration` tables. Auth lives in `auth.trusted_tenants` only.

RLS template idiom (§3): `USING (owned_by_email_address = app.jwt_email())` — never `current_setting(...)` directly.

## LOCALHOST MODE — read before doing anything

Before any magic-links / bases work, check whether `MAGICLINK_BASE_URL` is set to a `http://localhost:*` URL (or the operator said "use the local dev stack" / "localhost"). If so, follow [MAGIC_LINKS_REFACTOR.md §13](../../MAGIC_LINKS_REFACTOR.md#13-localhost-mode--opt-in-via-env-vars) — production URLs become localhost URLs, the magic-link email loop is replaced by debug code `424242`, and bases registration goes against the local bases server.

Operator quick-ref in localhost mode:
- magiclink server: `http://localhost:4787` (admin UI at `/`)
- bases server:     `http://localhost:4788` (admin UI at `/`)
- unified dash:     `http://localhost:4789`
- env file:         `magic-links-refactor/test-env/dev/.env` — `source` it before any curl recipe; gives you `OWNER_JWT`, `MAGICLINK_BASE_URL`, `BASES_BASE_URL`, etc.

Up-check (run before assuming the stack is live):
```
curl -fsS http://localhost:4787/install-magic-links/v1.sql >/dev/null && echo "magiclink up" || echo "magiclink DOWN"
curl -fsS http://localhost:4788/health >/dev/null && echo "bases up" || echo "bases DOWN"
```
If down, run `bash magic-links-refactor/test-env/scripts/dev-stack-up.sh`.
