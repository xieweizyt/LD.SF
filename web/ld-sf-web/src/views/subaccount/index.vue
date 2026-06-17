<template>
  <div class="page">
    <el-card>
      <template #header>创建分后台</template>
      <el-form :inline="true" :model="form">
        <el-form-item label="名称"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="初始次数"><el-input-number v-model="form.initialBalance" :min="0" /></el-form-item>
        <el-form-item label="授权标识"><el-input v-model="form.identifier" placeholder="留空自动生成" /></el-form-item>
        <el-form-item><el-button type="primary" @click="submit">创建</el-button></el-form-item>
      </el-form>
    </el-card>

    <el-card>
      <template #header>分后台列表</template>
      <el-table :data="list" border>
        <el-table-column prop="name" label="名称" />
        <el-table-column prop="publicId" label="分后台ID" />
        <el-table-column prop="authorizationIdentifier" label="授权标识" />
        <el-table-column prop="balance" label="剩余次数" />
        <el-table-column prop="sentCount" label="已发送" />
        <el-table-column prop="totalGranted" label="累计授权" />
        <el-table-column label="增次" width="260">
          <template #default="{ row }">
            <el-input-number v-model="grantMap[row.authorizationIdentifier]" :min="1" size="small" />
            <el-button size="small" @click="grant(row.authorizationIdentifier)">增加</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { createSubaccount, getSubaccounts, grantUses, SubaccountDto } from '@/api/ldsf'

const list = ref<SubaccountDto[]>([])
const grantMap = reactive<Record<string, number>>({})
const form = reactive({ name: '', initialBalance: 100, identifier: '' })

async function load() {
  list.value = await getSubaccounts()
  list.value.forEach((item) => {
    if (item.authorizationIdentifier && !grantMap[item.authorizationIdentifier]) grantMap[item.authorizationIdentifier] = 100
  })
}

async function submit() {
  await createSubaccount(form)
  ElMessage.success('创建成功')
  form.name = ''
  form.initialBalance = 100
  form.identifier = ''
  await load()
}

async function grant(identifier: string) {
  await grantUses(identifier, { uses: grantMap[identifier], reason: '平台增加次数' })
  ElMessage.success('增加成功')
  await load()
}

onMounted(load)
</script>

