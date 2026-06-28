// vite.config.js — React client dev server for the admin app.
//
//   Postgres  <-  Express (API, :6347)  <-  Vite (React client, :6348)
//
// Vite serves the React SPA and proxies every /api/* call through to the
// Express server, so the browser only ever talks to one origin (:6348) in dev.
// The client NEVER touches Postgres directly — it reads the inference surface
// the Express API exposes, which in turn reads vw_* views (project rule).
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const API_PORT = process.env.PORT || 6347;
const CLIENT_PORT = Number(process.env.CLIENT_PORT) || 6348;

export default defineConfig({
  root: 'client',
  plugins: [react()],
  server: {
    port: CLIENT_PORT,
    strictPort: true,
    proxy: {
      '/api': {
        target: `http://localhost:${API_PORT}`,
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: '../dist',
    emptyOutDir: true,
  },
});
