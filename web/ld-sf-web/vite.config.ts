import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

function resolve(dir: string) {
  return path.join(__dirname, dir)
}

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())

  return {
    base: env.VITE_APP_PATH_BASE || '/',
    plugins: [vue()],
    resolve: {
      alias: {
        '@': resolve('src'),
      },
      extensions: ['.mjs', '.js', '.ts', '.jsx', '.tsx', '.json', '.vue'],
    },
    server: {
      host: '0.0.0.0',
      port: 8080,
      open: true,
      cors: true,
      proxy: {
        '/api': {
          target: env.VITE_API_BASE || 'http://localhost:5000',
          changeOrigin: true,
        },
      },
    },
    build: {
      outDir: 'dist',
      sourcemap: mode !== 'production',
    },
  }
})
