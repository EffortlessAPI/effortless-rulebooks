import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// In dev, the React app runs on :5173 and proxies /api -> the Express backend
// on :8088, so the frontend always talks to the same-origin /api path whether
// it is running under Vite (dev) or served as static files by Express (prod).
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      "/api": {
        target: process.env.BACKEND_URL || "http://localhost:8088",
        changeOrigin: true,
      },
    },
  },
});
