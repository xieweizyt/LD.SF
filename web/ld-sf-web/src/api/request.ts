import axios from 'axios'
import { ElMessage } from 'element-plus'

export const backboneService = axios.create({
  baseURL: import.meta.env.VITE_API_PREFIX || '',
  timeout: 30000,
})

backboneService.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const message = error?.response?.data?.error?.message || error?.response?.data?.message || error.message || '请求失败'
    ElMessage.error(message)
    return Promise.reject(error)
  },
)

