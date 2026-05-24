# Multi-stage Dockerfile for mark-acosta-demo (Vite React + Express API)
#
# Build: cpln image build --name mark-acosta-demo:v1 --dockerfile ./Dockerfile --push --org effortlessapi
# Run locally: docker build -t mark-acosta-demo:local . && docker run -p 4000:4000 -e DATABASE_URL=... mark-acosta-demo:local

# ── Stage 1: Build the Vite React frontend ───────────────────────────
FROM node:20-alpine AS web-build
WORKDIR /app/web
COPY web/package.json web/package-lock.json* ./
RUN npm install --no-audit --no-fund
COPY web/ ./
RUN npm run build

# ── Stage 2: Server dependencies (includes tsx — it IS the runtime here) ──
FROM node:20-alpine AS server-deps
WORKDIR /app/server
COPY server/package.json server/package-lock.json* ./
RUN npm install --no-audit --no-fund

# ── Stage 3: Runtime ─────────────────────────────────────────────────
FROM node:20-alpine AS runtime
WORKDIR /app

# Server dependencies + source
COPY --from=server-deps /app/server/node_modules ./server/node_modules
COPY server/package.json server/tsconfig.json ./server/
COPY server/src ./server/src

# Built web assets (served as static by the server in production)
COPY --from=web-build /app/web/dist ./web/dist

ENV NODE_ENV=production
ENV PORT=4000
ENV SERVE_STATIC=true
ENV STATIC_DIR=/app/web/dist

EXPOSE 4000

# tsx as the runtime — same code as dev, no extra build step.
# tsx wraps Node and handles TS natively; no experimental flags needed.
WORKDIR /app/server
CMD ["node", "--no-warnings", "node_modules/tsx/dist/cli.mjs", "src/index.ts"]
