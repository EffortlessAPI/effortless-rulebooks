import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const SERVER_PORT = process.env.SERVER_PORT || "3032";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5175,
    proxy: {
      "/api": `http://localhost:${SERVER_PORT}`,
      "/healthz": `http://localhost:${SERVER_PORT}`,
    },
  },
});
