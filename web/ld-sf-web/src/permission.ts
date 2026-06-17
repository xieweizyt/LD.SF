import router from './router'
import { useUserStore } from '@/store/modules/user'

router.beforeEach((to) => {
  const userStore = useUserStore()
  if (to.path !== '/login' && !userStore.isLogin) {
    return '/login'
  }
  if (to.path === '/login' && userStore.isLogin) {
    return '/'
  }
  return true
})

