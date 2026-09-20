import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// The role experiences run on :5175 in dev; the API is the same Express server the
// admin console uses (:8099). One origin in the browser, identical paths in prod.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5175,
    proxy: { "/api": { target: process.env.API_URL || "http://localhost:8099", changeOrigin: true } },
  },
});
