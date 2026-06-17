<template>
  <main class="login-page">
    <el-card class="login-card">
      <h1>LD.SF 平台</h1>
      <el-form :model="form" label-position="top" @submit.prevent>
        <el-form-item label="账号">
          <el-input v-model="form.userName" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="form.password" type="password" show-password />
        </el-form-item>
        <el-button type="primary" :loading="loading" @click="submit">登录</el-button>
      </el-form>
    </el-card>
  </main>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/store/modules/user'

const router = useRouter()
const userStore = useUserStore()
const loading = ref(false)
const form = reactive({ userName: 'admin', password: 'admin123' })

async function submit() {
  loading.value = true
  try {
    await userStore.login(form.userName, form.password)
    router.push('/')
  } finally {
    loading.value = false
  }
}
</script>

