<template>
  <div class="page">
    <el-row :gutter="16">
      <el-col :span="6">
        <el-card><span>分后台</span><strong>{{ subaccounts.length }}</strong></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><span>剩余次数</span><strong>{{ balance }}</strong></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><span>已发送</span><strong>{{ sent }}</strong></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><span>累计授权</span><strong>{{ totalGranted }}</strong></el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { getSubaccounts, SubaccountDto } from '@/api/ldsf'

const subaccounts = ref<SubaccountDto[]>([])
const balance = computed(() => subaccounts.value.reduce((sum, item) => sum + item.balance, 0))
const sent = computed(() => subaccounts.value.reduce((sum, item) => sum + item.sentCount, 0))
const totalGranted = computed(() => subaccounts.value.reduce((sum, item) => sum + item.totalGranted, 0))

onMounted(async () => {
  subaccounts.value = await getSubaccounts()
})
</script>

