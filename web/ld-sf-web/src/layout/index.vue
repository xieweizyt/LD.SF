<template>
  <div class="app-layout">
    <aside class="sidebar">
      <h1>LD.SF</h1>
      <router-link to="/dashboard">总览</router-link>
      <router-link to="/subaccounts">分后台管理</router-link>
      <router-link to="/tasks">任务管理</router-link>
      <router-link to="/ledger">次数流水</router-link>
    </aside>
    <section class="main">
      <header class="main-header">
        <div>
          <strong>{{ title }}</strong>
          <span>{{ userStore.displayName }} · {{ userStore.role }}</span>
        </div>
        <el-button @click="logout">退出</el-button>
      </header>
      <router-view />
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useUserStore } from '@/store/modules/user'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const title = computed(() => route.meta.title || 'LD.SF')

function logout() {
  userStore.logout()
  router.push('/login')
}
</script>

