import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const apiPort = process.env.VITE_API_PORT ?? '3032';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5175,
    strictPort: true,
    proxy: {
      '/api': {
        target: `http://localhost:${apiPort}`,
        changeOrigin: true,
      },
    },
  },
});
