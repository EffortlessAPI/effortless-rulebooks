## ============================================================================
##  Causal Autoimmune Architecture Platform — single-container image.
## ============================================================================
##  One image = Postgres (bundled) + the seeded schema/data + the Node app.
##  `docker run` boots Postgres, runs postgres/init-db.sh to create & seed the
##  `causal_autoimmune` database, then starts the Express server which serves
##  BOTH the built React SPA and the /api inference surface on a single port.
##
##  This is a DEMO image: data lives inside the container (ephemeral). Mount a
##  volume at /var/lib/postgresql/data to persist it. See run-docker.sh.
## ----------------------------------------------------------------------------
##  Build context is the repo root. Two stages:
##    1) builder  — Node 20, `vite build` produces admin-app/dist
##    2) runtime  — postgres:18 + Node 20 + app + seed SQL + entrypoint
## ============================================================================

# ---- Stage 1: build the React client -------------------------------------
FROM node:20-bookworm-slim AS builder
WORKDIR /app/admin-app

# Install deps with a warm cache: copy manifests first.
COPY admin-app/package.json admin-app/package-lock.json ./
RUN npm ci

# Bring in the client source + the public explainer-dag assets and build.
# `vite build` (root: client) emits to ../dist and copies client/public/* in.
COPY admin-app/ ./
RUN npm run build


# ---- Stage 2: runtime (Postgres + Node + app) ----------------------------
# Postgres 18 (matches localhost / Postgres.app). NOT 16: this project's calc
# fields are a deep tree of nested STABLE functions where top-level keystone
# columns (is_clinically_actionable, predicted_value, lifecycle_state_key,
# deciding_gate) each re-derive shared sub-results. PG18's planner collapses
# those repeated calls; PG16 re-executes every one, making a single `SELECT *`
# of one prediction row take ~6.4s vs ~33ms on PG18. Measured, both engines,
# same schema+data. Keep this at 18 unless the calc DAG is flattened to not
# depend on the optimizer (see the calc functions in postgres/02-*.sql).
FROM postgres:18-bookworm AS runtime

# Postgres image is Debian bookworm; add Node 20 from NodeSource and the few
# tools the entrypoint needs. (psql already ships in the postgres image.)
RUN apt-get update \
 && apt-get install -y --no-install-recommends ca-certificates curl gnupg \
 && mkdir -p /etc/apt/keyrings \
 && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key \
      | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
 && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" \
      > /etc/apt/sources.list.d/nodesource.list \
 && apt-get update \
 && apt-get install -y --no-install-recommends nodejs \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# --- App: install RUNTIME deps only, then copy server + built client. -----
# (devDeps like vite/concurrently are not needed at runtime — the SPA is
#  already built in stage 1.)
COPY admin-app/package.json admin-app/package-lock.json ./admin-app/
RUN cd admin-app && npm ci --omit=dev

COPY admin-app/server   ./admin-app/server
COPY admin-app/tests    ./admin-app/tests
COPY --from=builder /app/admin-app/dist ./admin-app/dist

# --- Seed: the generated SQL + the init script that applies it. -----------
COPY postgres ./postgres

# --- Rulebook (read-only at runtime; init-db's reverse-sync is disabled). --
COPY effortless-rulebook ./effortless-rulebook

# --- Entrypoint that boots PG, seeds, then runs the app. ------------------
COPY docker-entrypoint.sh /usr/local/bin/app-entrypoint.sh
RUN chmod +x /usr/local/bin/app-entrypoint.sh

# init-db.sh (run as the `postgres` user) writes postgres/.applied-manifest.json
# back into the app dir, and the app runs as `postgres` too. Hand /app to that
# user so both can write where they expect.
RUN chown -R postgres:postgres /app

# The app serves UI + API here. PORT is read by server/index.js. We use 6348 to
# MATCH the dev UI port (start.sh's Vite client), so the URL is identical whether
# you run via ./start.sh (dev) or ./run-docker.sh (single-origin prod).
ENV PORT=6348
ENV NODE_ENV=production
# Inside the container the app talks to the bundled Postgres over the unix
# socket as the local `postgres` superuser — no password needed.
ENV DATABASE_URL=postgresql://postgres@localhost:5432/causal_autoimmune
# Don't let the container's seed write back into the (read-only) rulebook.
ENV SNAPSHOT_RULEBOOK=false

EXPOSE 6348

# The postgres image's own entrypoint expects to run as root then drops to the
# `postgres` user; our entrypoint does the same dance explicitly, so run as
# root and let app-entrypoint.sh manage privilege.
ENTRYPOINT ["/usr/local/bin/app-entrypoint.sh"]
