import { defineConfig } from "vite";

// In dev the UI runs on :5174 and the API on :8099; proxy /api so the browser
// sees one origin and the fetch paths are identical in dev and prod.
export default defineConfig({
  server: {
    port: 5174,
    proxy: {
      "/api": {
        target: process.env.API_URL || "http://localhost:8099",
        changeOrigin: true,
      },
    },
  },
});
