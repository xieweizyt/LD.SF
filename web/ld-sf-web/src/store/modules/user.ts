import { defineStore } from 'pinia'
import { login } from '@/api/ldsf'

export const useUserStore = defineStore('user', {
  state: () => ({
    userId: '',
    userName: '',
    displayName: '',
    role: '',
  }),
  getters: {
    isLogin: (state) => !!state.userId,
  },
  actions: {
    async login(userName: string, password: string) {
      const data = await login({ userName, password })
      this.userId = data.userId
      this.userName = data.userName
      this.displayName = data.displayName
      this.role = data.role
    },
    logout() {
      this.$reset()
    },
  },
})

