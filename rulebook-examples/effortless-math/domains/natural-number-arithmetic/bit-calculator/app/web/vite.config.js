import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const SERVER_PORT = process.env.SERVER_PORT || "3040";

export default defineConfig({
  plugins: [react()],
  server: {
    port: Number(process.env.WEB_PORT) || 5180,
    proxy: {
      "/api": `http://localhost:${SERVER_PORT}`,
      "/healthz": `http://localhost:${SERVER_PORT}`,
    },
  },
});
