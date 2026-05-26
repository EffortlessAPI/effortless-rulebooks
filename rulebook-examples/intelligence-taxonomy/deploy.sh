#!/usr/bin/env bash
# Deploy intelligence-taxonomy end-to-end:
#   bases-api auth → find-or-create base → /build trigger → wipe cloned schema →
#   apply our rulebook SQL → cpln secret + identity + policy → image build & push →
#   workload apply → print URL.
#
# Re-runnable. If the JWT/base/secret/identity/policy already exist, this is a
# fast incremental redeploy.
#
# Flags:
#   --schema-only      Stop after applying SQL to the bases base.
#   --build-only       Build + push the image; skip workload apply.
#   --reseed           TRUNCATE the rulebook tables before re-applying 05-insert-data.sql.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# ── Config (override via env) ───────────────────────────────────────────────
DEV_EMAIL="${DEV_EMAIL:-eejai42@gmail.com}"
BASES_API="${BASES_API_URL:-https://bases-api.effortlessapi.com}"
BASES="${BASES_URL:-https://bases.effortlessapi.com}"

PROJECT_NAME="${PROJECT_NAME:-intelligence-taxonomy}"
DISPLAY_NAME="${DISPLAY_NAME:-Intelligence Taxonomy Demo}"

CPLN_ORG="${CPLN_ORG:-effortlessapi}"
CPLN_GVC="${CPLN_GVC:-effortlessapi}"
CPLN_WORKLOAD="${CPLN_WORKLOAD:-$PROJECT_NAME}"
CPLN_IMAGE="${CPLN_IMAGE:-$PROJECT_NAME}"
CPLN_SECRET="${CPLN_SECRET:-$PROJECT_NAME}"
CPLN_IDENTITY="${CPLN_IDENTITY:-$PROJECT_NAME}"
CPLN_POLICY="${CPLN_POLICY:-${PROJECT_NAME}-secret-access}"

JWT_CACHE="/tmp/bases-api-jwt-${DEV_EMAIL}.txt"
MODE="${1:-full}"

note() { printf '\033[36m[deploy]\033[0m %s\n' "$*" >&2; }
warn() { printf '\033[33m[deploy]\033[0m %s\n' "$*" >&2; }
fail() { printf '\033[31m[deploy]\033[0m %s\n' "$*" >&2; exit 1; }

require_cmd() { command -v "$1" >/dev/null || fail "missing: $1"; }
for c in curl python3 psql cpln; do require_cmd "$c"; done

py_field() { python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('$1','') or '')"; }
jwt_expired() {
  local jwt="$1"
  [ -z "$jwt" ] && return 0
  python3 - "$jwt" <<'PY'
import sys, json, base64, time
parts = sys.argv[1].split('.')
if len(parts) < 2: sys.exit(0)
pl = parts[1] + '=' * (-len(parts[1]) % 4)
try:
    exp = json.loads(base64.urlsafe_b64decode(pl)).get('exp', 0)
except Exception:
    sys.exit(0)
sys.exit(0 if int(time.time()) + 30 >= int(exp) else 1)
PY
}

# ── 1. Auth against bases-api ──────────────────────────────────────────────
get_jwt() {
  if [ -f "$JWT_CACHE" ]; then
    local cached
    cached="$(<"$JWT_CACHE")"
    if [ -n "$cached" ] && ! jwt_expired "$cached"; then
      echo "$cached"
      return
    fi
    warn "cached JWT expired or missing — re-authenticating"
  fi

  note "emailing 6-digit code to $DEV_EMAIL"
  curl -sf -X POST "$BASES_API/auth/email-auth" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$DEV_EMAIL\"}" >/dev/null || fail "email-auth failed"

  printf '\033[36m[deploy]\033[0m enter 6-digit code: ' >&2
  read -r CODE
  [[ "$CODE" =~ ^[0-9]{6}$ ]] || fail "expected 6 digits"

  local resp jwt
  resp=$(curl -sS -X POST "$BASES_API/auth/magic-link" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$DEV_EMAIL\",\"code\":\"$CODE\"}")
  jwt=$(echo "$resp" | py_field jwt)
  [ -n "$jwt" ] || fail "magic-link failed: $resp"
  echo "$jwt" > "$JWT_CACHE"
  echo "$jwt"
}

# ── 2. Find or create the base ─────────────────────────────────────────────
find_base() {
  local jwt="$1"
  curl -sf "$BASES_API/api/bases" -H "Authorization: Bearer $jwt" \
    | python3 -c "
import sys, json
target = '''$DISPLAY_NAME'''
for b in json.load(sys.stdin) or []:
    if b.get('display_name') == target:
        print(b.get('base_id',''))
        break
"
}

ensure_base() {
  local jwt="$1"
  local base_id
  base_id=$(find_base "$jwt")
  if [ -n "$base_id" ]; then
    note "reusing existing base $base_id"
    echo "$base_id"
    return
  fi

  note "creating base \"$DISPLAY_NAME\" via clone (deployed bases doesn't support source=empty yet)"
  local all source_id resp
  all=$(curl -sf "$BASES_API/api/bases" -H "Authorization: Bearer $jwt")
  source_id=$(echo "$all" | python3 -c "import sys,json; d=json.load(sys.stdin) or []; print(d[0]['base_id'] if d else '')")
  [ -n "$source_id" ] || fail "no source base to clone from. Create one via the dashboard first."

  resp=$(curl -sS -X POST "$BASES/bases/register" \
    -H "Authorization: Bearer $jwt" -H "Content-Type: application/json" \
    -d "{\"sourceBaseId\":\"$source_id\",\"displayName\":\"$DISPLAY_NAME\",\"createERBTables\":true}")
  base_id=$(echo "$resp" | python3 -c "import sys,json; print(json.load(sys.stdin).get('base',{}).get('base_id',''))")
  [ -n "$base_id" ] || fail "register failed: $resp"
  note "created base $base_id"
  echo "$base_id"
}

# ── 3. Trigger build → DB + credentials ────────────────────────────────────
# JSON helpers — `dict.get(key, default)` returns the value if the key exists
# even when that value is JSON null, so we use `or ''` to coerce null → ''.
# Without this, `print(None)` emits the literal string "None" and bash treats
# it as a non-empty value, which is how we ended up with `psql None` last time.
py_get() {
  # Usage: echo "$json" | py_get key1 [key2 ...]   (chained nested .get)
  python3 -c "
import sys, json
d = json.load(sys.stdin)
for k in sys.argv[1:]:
    if not isinstance(d, dict):
        d = None
        break
    d = d.get(k)
print(d if d is not None else '')
" "$@"
}

trigger_build_and_get_creds() {
  local jwt="$1" base_id="$2"
  note "triggering /build/$base_id (provisions Postgres + returns credentials)"
  local resp
  resp=$(curl -sS -X POST "$BASES/build/$base_id" \
    -H "Authorization: Bearer $jwt" -H "Content-Type: application/json" -d '{}')
  local admin_conn
  admin_conn=$(echo "$resp" | py_get credentials admin connectionString)
  if [ -z "$admin_conn" ]; then
    note "no creds in build response — fetching via /auth/credentials"
    local c
    c=$(curl -sS "$BASES/bases/$base_id/auth/credentials" -H "Authorization: Bearer $jwt")
    local user pw db
    user=$(echo "$c" | py_get admin username)
    pw=$(echo "$c"   | py_get admin password)
    db=$(curl -sS "$BASES/bases/$base_id/details" -H "Authorization: Bearer $jwt" \
         | py_get database_name)
    if [ -z "$user" ] || [ -z "$pw" ] || [ -z "$db" ]; then
      warn "build response was: $(echo "$resp" | head -c 400)"
      warn "credentials response was: $(echo "$c" | head -c 400)"
      fail "could not assemble admin connection string (user='$user' pw_set=$([ -n "$pw" ] && echo y || echo n) db='$db')"
    fi
    admin_conn="postgresql://$user:$pw@bases.effortlessapi.com:5432/$db"
  fi
  # Sanity-check: must look like a real postgresql:// URL, otherwise we'd
  # send something like "None" or "" to psql and get a baffling error.
  case "$admin_conn" in
    postgresql://*@*/*) : ;;
    *)
      warn "build response was: $(echo "$resp" | head -c 400)"
      fail "admin connection string looks invalid: '$admin_conn'" ;;
  esac
  echo "$admin_conn"
}

# ── 4. Wipe cloned schema, apply our SQL ───────────────────────────────────
clean_cloned_schema() {
  local conn="$1"
  note "dropping any leftover tables from the source-base clone"
  # List public schema tables NOT in our rulebook + drop them.
  local our='capabilities|intelligences|assessments|erb_versions|erb_customizations'
  local doomed
  doomed=$(psql "$conn" -tAc \
    "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'" \
    | grep -vE "^($our)$" || true)
  for t in $doomed; do
    note "  DROP TABLE $t CASCADE"
    psql "$conn" -c "DROP TABLE IF EXISTS \"$t\" CASCADE;" >/dev/null
  done
}

apply_schema() {
  local conn="$1"
  local files=(
    01-drop-and-create-tables.sql 01b-customize-schema.sql
    02-create-functions.sql       02b-customize-functions.sql
    03-create-views.sql           03b-customize-views.sql
    05-insert-data.sql            05b-customize-data.sql
  )
  for f in "${files[@]}"; do
    [ -f "postgres/$f" ] || continue
    note "psql -f postgres/$f"
    psql "$conn" -v ON_ERROR_STOP=1 -f "postgres/$f" >/dev/null
  done
}

reseed() {
  local conn="$1"
  note "TRUNCATE + re-seed"
  psql "$conn" -c "TRUNCATE assessments, intelligences, capabilities CASCADE;" >/dev/null
  psql "$conn" -v ON_ERROR_STOP=1 -f postgres/05-insert-data.sql >/dev/null
}

# ── 5. cpln secret / identity / policy ─────────────────────────────────────
upsert_secret() {
  local conn="$1"
  if cpln secret get "$CPLN_SECRET" --org "$CPLN_ORG" >/dev/null 2>&1; then
    note "deleting existing cpln secret $CPLN_SECRET"
    cpln secret delete "$CPLN_SECRET" --org "$CPLN_ORG" >/dev/null
  fi
  note "creating cpln dictionary secret $CPLN_SECRET"
  cpln secret create-dictionary --name "$CPLN_SECRET" --org "$CPLN_ORG" \
    --entry "DATABASE_URL=$conn" >/dev/null
}

ensure_identity() {
  if cpln identity get "$CPLN_IDENTITY" --gvc "$CPLN_GVC" --org "$CPLN_ORG" >/dev/null 2>&1; then
    note "identity $CPLN_IDENTITY exists"
  else
    note "creating identity $CPLN_IDENTITY"
    cpln identity create --name "$CPLN_IDENTITY" --gvc "$CPLN_GVC" --org "$CPLN_ORG" >/dev/null
  fi
}

ensure_policy() {
  local tmp
  tmp=$(mktemp).yaml
  cat > "$tmp" <<EOF
kind: policy
name: $CPLN_POLICY
description: Allow $CPLN_IDENTITY to reveal $CPLN_SECRET
targetKind: secret
targetLinks:
  - /org/$CPLN_ORG/secret/$CPLN_SECRET
bindings:
  - permissions: [reveal, view, use]
    principalLinks:
      - /org/$CPLN_ORG/gvc/$CPLN_GVC/identity/$CPLN_IDENTITY
EOF
  note "cpln apply $CPLN_POLICY"
  cpln apply -f "$tmp" --org "$CPLN_ORG" >/dev/null
  rm -f "$tmp"
}

# ── 6. Image build + workload apply ────────────────────────────────────────
build_and_apply() {
  local tag image
  tag="$(date +%Y%m%d%H%M%S)"
  image="${CPLN_IMAGE}:${tag}"
  note "cpln image build → $image"
  cpln image build --name "$image" --dockerfile ./Dockerfile --push --org "$CPLN_ORG" >/dev/null

  local tmpl=".cpln/cpln-workload.yaml"
  local deploy_file=".cpln/cpln-workload-deploy.yaml"
  cp "$tmpl" "$deploy_file"
  sed -i.bak \
    -e "s|WORKLOAD_NAME|${CPLN_WORKLOAD}|g" \
    -e "s|IMAGE_NAME_TAG|${image}|g" \
    -e "s|ORG_NAME|${CPLN_ORG}|g" \
    -e "s|DESCRIPTION|${DISPLAY_NAME}|g" \
    -e "s|PROJECT_TAG|demo|g" \
    -e "s|CONTAINER_NAME|app|g" \
    -e "s|SECRET_NAME|${CPLN_SECRET}|g" \
    -e "s|CPLN_GVC|${CPLN_GVC}|g" \
    "$deploy_file"
  rm -f "${deploy_file}.bak"

  note "cpln apply workload"
  cpln apply -f "$deploy_file" --gvc "$CPLN_GVC" --org "$CPLN_ORG" >/dev/null
  rm -f "$deploy_file"
}

print_url() {
  local url=""
  for _ in 1 2 3 4 5 6; do
    url=$(cpln workload get "$CPLN_WORKLOAD" --gvc "$CPLN_GVC" --org "$CPLN_ORG" -o json 2>/dev/null \
      | python3 -c "import sys,json; print(json.load(sys.stdin).get('status',{}).get('endpoint','') or '')")
    [ -n "$url" ] && break
    sleep 3
  done
  echo
  echo "================================================================="
  if [ -n "$url" ]; then
    echo "  Live URL: $url"
  else
    echo "  URL not yet available — check the cpln console:"
  fi
  echo "  Console:  https://console.cpln.io/console/org/${CPLN_ORG}/gvc/${CPLN_GVC}/workload/${CPLN_WORKLOAD}/-info"
  echo "================================================================="
}

# ── Main ───────────────────────────────────────────────────────────────────
JWT=$(get_jwt)
BASE_ID=$(ensure_base "$JWT")
CONN=$(trigger_build_and_get_creds "$JWT" "$BASE_ID")
[ -n "$CONN" ] || fail "no admin connection string"

clean_cloned_schema "$CONN"
apply_schema       "$CONN"
[ "$MODE" = "--reseed" ] && reseed "$CONN"

if [ "$MODE" = "--schema-only" ]; then
  note "--schema-only complete"
  exit 0
fi

upsert_secret    "$CONN"
ensure_identity
ensure_policy
build_and_apply
[ "$MODE" = "--build-only" ] && exit 0
print_url
