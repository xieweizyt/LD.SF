<template>
  <div class="page">
    <el-card>
      <template #header>次数流水</template>
      <el-table :data="list" border>
        <el-table-column prop="identifier" label="授权标识" />
        <el-table-column prop="type" label="类型" />
        <el-table-column prop="count" label="次数" />
        <el-table-column prop="taskId" label="任务ID" />
        <el-table-column prop="note" label="说明" />
        <el-table-column label="时间">
          <template #default="{ row }">{{ parseTime(row.creationTime) }}</template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getLedgers, UsageLedgerDto } from '@/api/ldsf'
import { parseTime } from '@/utils'

const list = ref<UsageLedgerDto[]>([])
onMounted(async () => {
  list.value = await getLedgers()
})
</script>

