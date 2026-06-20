<template>
  <div class="page task-page">
    <section class="table-panel">
      <div class="table-toolbar">
        <el-form :inline="true" class="toolbar-form">
          <el-form-item label="分后台">
            <el-select
              v-model="selectedSubaccountId"
              filterable
              placeholder="请选择分后台"
              class="toolbar-select"
            >
              <el-option v-for="item in subaccounts" :key="item.id" :label="item.name" :value="item.id" />
            </el-select>
          </el-form-item>
        </el-form>
        <div class="toolbar-actions">
          <el-button @click="loadTasks">刷新</el-button>
          <el-button type="primary" :disabled="!selectedSubaccountId" @click="openCreateDialog">新增任务</el-button>
        </div>
      </div>

      <el-table v-loading="loading" :data="tasks" border stripe class="data-table">
        <el-table-column type="index" label="序号" width="70" align="center" />
        <el-table-column prop="taskName" label="任务名称" min-width="180" show-overflow-tooltip />
        <el-table-column prop="totalNumbers" label="号码数" width="100" align="center" />
        <el-table-column prop="sentCount" label="已发送" width="100" align="center" />
        <el-table-column prop="batchSize" label="批量" width="90" align="center" />
        <el-table-column label="状态" width="110" align="center">
          <template #default="{ row }">
            <el-tag :type="statusType(row.status)" effect="light">{{ statusText(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="短信内容" min-width="240" show-overflow-tooltip>
          <template #default="{ row }">
            {{ row.content1 }}
          </template>
        </el-table-column>
        <el-table-column label="创建时间" width="180">
          <template #default="{ row }">{{ parseTime(row.creationTime) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right" align="center">
          <template #default="{ row }">
            <el-button type="danger" size="small" @click="remove(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </section>

    <el-dialog v-model="dialogVisible" title="新增任务" width="720px" destroy-on-close>
      <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="分后台">
          <el-select v-model="selectedSubaccountId" filterable disabled class="form-control">
            <el-option v-for="item in subaccounts" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="任务名称" prop="taskName">
          <el-input v-model="form.taskName" maxlength="100" show-word-limit />
        </el-form-item>
        <el-form-item label="批量大小" prop="batchSize">
          <el-input-number v-model="form.batchSize" :min="1" :max="200" />
        </el-form-item>
        <el-form-item label="短信内容1" prop="content1">
          <el-input v-model="form.content1" type="textarea" :rows="4" maxlength="500" show-word-limit />
        </el-form-item>
        <el-form-item label="短信内容2">
          <el-input v-model="form.content2" type="textarea" :rows="3" maxlength="500" show-word-limit />
        </el-form-item>
        <el-form-item label="手机号" prop="phoneText">
          <el-input
            v-model="form.phoneText"
            type="textarea"
            :rows="8"
            placeholder="每行一个手机号，也支持空格、逗号、分号分隔"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage, ElMessageBox } from 'element-plus'
import { createTask, deleteTask, getSubaccounts, getTasks, SmsTaskDto, SubaccountDto } from '@/api/ldsf'
import { parsePhones, parseTime } from '@/utils'

const subaccounts = ref<SubaccountDto[]>([])
const selectedSubaccountId = ref('')
const tasks = ref<SmsTaskDto[]>([])
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const formRef = ref<FormInstance>()

const form = reactive({
  taskName: '',
  batchSize: 1,
  content1: '',
  content2: '',
  phoneText: '',
})

const rules: FormRules = {
  taskName: [{ required: true, message: '请输入任务名称', trigger: 'blur' }],
  batchSize: [{ required: true, message: '请输入批量大小', trigger: 'change' }],
  content1: [{ required: true, message: '请输入短信内容', trigger: 'blur' }],
  phoneText: [{ required: true, message: '请输入手机号', trigger: 'blur' }],
}

async function loadSubaccounts() {
  subaccounts.value = await getSubaccounts()
  selectedSubaccountId.value ||= subaccounts.value[0]?.id || ''
}

async function loadTasks() {
  if (!selectedSubaccountId.value) {
    tasks.value = []
    return
  }

  loading.value = true
  try {
    tasks.value = await getTasks(selectedSubaccountId.value)
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  resetForm()
  dialogVisible.value = true
}

async function submit() {
  if (!selectedSubaccountId.value) return
  await formRef.value?.validate()
  const phoneNumbers = parsePhones(form.phoneText)
  if (!phoneNumbers.length) {
    ElMessage.warning('请输入有效手机号')
    return
  }

  submitting.value = true
  try {
    await createTask(selectedSubaccountId.value, {
      taskName: form.taskName.trim(),
      batchSize: form.batchSize,
      content1: form.content1.trim(),
      content2: form.content2.trim() || undefined,
      phoneNumbers,
    })
    ElMessage.success('创建成功')
    dialogVisible.value = false
    await loadTasks()
  } finally {
    submitting.value = false
  }
}

async function remove(taskId: string) {
  await ElMessageBox.confirm('确认删除该任务吗？', '删除确认', { type: 'warning' })
  await deleteTask(selectedSubaccountId.value, taskId)
  ElMessage.success('删除成功')
  await loadTasks()
}

function resetForm() {
  form.taskName = ''
  form.batchSize = 1
  form.content1 = ''
  form.content2 = ''
  form.phoneText = ''
  formRef.value?.clearValidate()
}

function statusText(status: string) {
  const map: Record<string, string> = {
    Pending: '待发送',
    Processing: '处理中',
    Completed: '已完成',
    Failed: '失败',
    Cancelled: '已取消',
  }
  return map[status] || status
}

function statusType(status: string) {
  const map: Record<string, 'success' | 'warning' | 'info' | 'danger'> = {
    Pending: 'info',
    Processing: 'warning',
    Completed: 'success',
    Failed: 'danger',
    Cancelled: 'info',
  }
  return map[status] || 'info'
}

watch(selectedSubaccountId, loadTasks)
onMounted(async () => {
  await loadSubaccounts()
  await loadTasks()
})
</script>
