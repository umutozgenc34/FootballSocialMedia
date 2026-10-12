import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    // Backend'deki Cors:Origins ayarı bu adresi bekliyor
    port: 5173,
    strictPort: true,
  },
});
