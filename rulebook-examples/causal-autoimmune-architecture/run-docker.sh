#!/usr/bin/env bash
# ============================================================================
#  run-docker.sh — clone, then run the whole platform as ONE container.
# ============================================================================
#  Builds a self-contained image (bundled Postgres + seeded schema/data + the
#  Node app that serves the React UI and the /api inference surface on a single
#  port) and runs it. After this finishes, open the printed URL.
#
#      git clone <repo> && cd <repo>
#      ./run-docker.sh
#      # → open http://localhost:6348   (same UI port as dev / ./start.sh)
#
#  Flags:
#      ./run-docker.sh              rebuild a fresh image + run, foreground logs
#      ./run-docker.sh --rebuild    (default; kept for compatibility / clarity)
#      ./run-docker.sh --no-rebuild reuse the existing image, skip the build
#      ./run-docker.sh --persist    keep DB data across runs (named volume)
#      ./run-docker.sh --detach     run in the background, print the URL, exit
#      PORT=8080 ./run-docker.sh    publish on a different host port
#
#  NOTE: rebuild is the DEFAULT. The image bakes in the app + seed SQL at build
#  time, so a stale image silently runs OLD code (this is exactly the "Docker is
#  30 min behind" symptom). We rebuild every run so the container always matches
#  the working tree; pass --no-rebuild for a fast re-run when nothing changed.
#
#  Stop a detached run:  docker rm -f causal-autoimmune
# ============================================================================
set -euo pipefail

cd "$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

IMAGE="causal-autoimmune"
NAME="causal-autoimmune"
HOST_PORT="${PORT:-6348}"   # host port to publish; container serves UI+API on :6348 (matches dev)
CONTAINER_PORT=6348

# Rebuild by default: the image bakes in app + seed at build time, so reusing a
# stale image silently runs old code. --no-rebuild opts out for a fast re-run.
REBUILD=true
PERSIST=false
DETACH=false
for arg in "$@"; do
  case "$arg" in
    --rebuild) REBUILD=true ;;
    --no-rebuild|--reuse) REBUILD=false ;;
    --persist) PERSIST=true ;;
    --detach|-d) DETACH=true ;;
    -h|--help)
      sed -n '2,30p' "$0"; exit 0 ;;
    *) echo "unknown flag: $arg (try --help)" >&2; exit 2 ;;
  esac
done

if ! command -v docker >/dev/null 2>&1; then
  echo "docker is not installed or not on PATH. Install Docker Desktop / Engine first." >&2
  exit 1
fi

# --- Build the image. Rebuild by default (so the container matches the working
#     tree); --no-rebuild reuses an existing image. If none exists yet we must
#     build regardless of the flag. -----------------------------------------
if $REBUILD || [ -z "$(docker images -q "$IMAGE" 2>/dev/null)" ]; then
  echo "[run-docker] building image '$IMAGE' (Docker layer cache makes re-builds fast)…"
  docker build -t "$IMAGE" .
else
  echo "[run-docker] --no-rebuild: reusing existing image '$IMAGE' (may be stale!)."
fi

# --- Replace any previous container with the same name. ----------------------
if [ -n "$(docker ps -aq -f name="^${NAME}$")" ]; then
  echo "[run-docker] removing previous container '$NAME'…"
  docker rm -f "$NAME" >/dev/null
fi

# --- Optional persistence: keep Postgres data in a named volume. -------------
VOLUME_ARGS=()
if $PERSIST; then
  echo "[run-docker] --persist: DB data kept in volume 'causal-autoimmune-pgdata'."
  VOLUME_ARGS=(-v causal-autoimmune-pgdata:/var/lib/postgresql/data)
fi

echo "[run-docker] open  ->  http://localhost:${HOST_PORT}"

# NB: "${VOLUME_ARGS[@]+...}" guards against macOS's stock Bash 3.2, where
# expanding an EMPTY array under `set -u` errors as "unbound variable". This
# form expands to nothing when the array is empty and to the flags otherwise.
if $DETACH; then
  docker run -d --name "$NAME" \
    -p "${HOST_PORT}:${CONTAINER_PORT}" \
    "${VOLUME_ARGS[@]+"${VOLUME_ARGS[@]}"}" \
    "$IMAGE"
  echo "[run-docker] running in background as container '$NAME'."
  echo "[run-docker] logs:  docker logs -f $NAME"
  echo "[run-docker] stop:  docker rm -f $NAME"
else
  # Foreground: Ctrl-C stops & removes the container.
  exec docker run --rm --name "$NAME" \
    -p "${HOST_PORT}:${CONTAINER_PORT}" \
    "${VOLUME_ARGS[@]+"${VOLUME_ARGS[@]}"}" \
    "$IMAGE"
fi
