import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/theme-chalk/src/index.scss'
import App from './App.vue'
import router from './router'
import pinia from './store'
import './permission'
import '@/styles/index.scss'

const app = createApp(App)
app.use(pinia)
app.use(router)
app.use(ElementPlus)
app.mount('#app')

