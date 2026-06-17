<template>
  <div class="page">
    <el-card>
      <template #header>创建任务</template>
      <el-form :model="form" label-width="100px">
        <el-form-item label="分后台">
          <el-select v-model="selectedSubaccountId" filterable>
            <el-option v-for="item in subaccounts" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="任务名称"><el-input v-model="form.taskName" /></el-form-item>
        <el-form-item label="批量大小"><el-input-number v-model="form.batchSize" :min="1" :max="200" /></el-form-item>
        <el-form-item label="短信内容1"><el-input v-model="form.content1" type="textarea" /></el-form-item>
        <el-form-item label="短信内容2"><el-input v-model="form.content2" type="textarea" /></el-form-item>
        <el-form-item label="手机号"><el-input v-model="form.phoneText" type="textarea" :rows="8" /></el-form-item>
        <el-form-item><el-button type="primary" @click="submit">创建任务</el-button></el-form-item>
      </el-form>
    </el-card>

    <el-card>
      <template #header>任务列表</template>
      <el-table :data="tasks" border>
        <el-table-column prop="taskName" label="任务名称" />
        <el-table-column prop="totalNumbers" label="号码数" />
        <el-table-column prop="sentCount" label="已发送" />
        <el-table-column prop="batchSize" label="批量" />
        <el-table-column prop="status" label="状态" />
        <el-table-column label="操作">
          <template #default="{ row }">
            <el-button type="danger" size="small" @click="remove(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { createTask, deleteTask, getSubaccounts, getTasks, SmsTaskDto, SubaccountDto } from '@/api/ldsf'
import { parsePhones } from '@/utils'

const subaccounts = ref<SubaccountDto[]>([])
const selectedSubaccountId = ref('')
const tasks = ref<SmsTaskDto[]>([])
const form = reactive({ taskName: '', batchSize: 1, content1: '', content2: '', phoneText: '' })

async function loadSubaccounts() {
  subaccounts.value = await getSubaccounts()
  selectedSubaccountId.value ||= subaccounts.value[0]?.id || ''
}

async function loadTasks() {
  if (!selectedSubaccountId.value) return
  tasks.value = await getTasks(selectedSubaccountId.value)
}

async function submit() {
  await createTask(selectedSubaccountId.value, {
    taskName: form.taskName,
    batchSize: form.batchSize,
    content1: form.content1,
    content2: form.content2,
    phoneNumbers: parsePhones(form.phoneText),
  })
  ElMessage.success('创建成功')
  form.taskName = ''
  form.batchSize = 1
  form.content1 = ''
  form.content2 = ''
  form.phoneText = ''
  await loadTasks()
}

async function remove(taskId: string) {
  await deleteTask(selectedSubaccountId.value, taskId)
  ElMessage.success('删除成功')
  await loadTasks()
}

watch(selectedSubaccountId, loadTasks)
onMounted(async () => {
  await loadSubaccounts()
  await loadTasks()
})
</script>

