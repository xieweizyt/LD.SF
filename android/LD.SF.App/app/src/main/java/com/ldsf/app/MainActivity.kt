package com.ldsf.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.viewModels
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.ldsf.app.model.AppTask
import com.ldsf.app.ui.LdSfViewModel

class MainActivity : ComponentActivity() {
    private val viewModel: LdSfViewModel by viewModels()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MaterialTheme {
                val state by viewModel.state.collectAsState()
                LdSfScreen(
                    state = state,
                    onIdentifierChange = viewModel::setIdentifier,
                    onAuthorize = viewModel::authorize,
                    onLoadTasks = viewModel::loadTasks,
                    onOpenSms = { task ->
                        viewModel.buildSmsIntent(task)?.let { startActivity(it) }
                    },
                    onConfirm = viewModel::confirmSent
                )
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun LdSfScreen(
    state: com.ldsf.app.ui.LdSfUiState,
    onIdentifierChange: (String) -> Unit,
    onAuthorize: () -> Unit,
    onLoadTasks: () -> Unit,
    onOpenSms: (AppTask) -> Unit,
    onConfirm: (AppTask) -> Unit,
) {
    Scaffold(
        topBar = {
            TopAppBar(title = { Text("LD.SF") })
        }
    ) { padding ->
        LazyColumn(
            modifier = Modifier
                .padding(padding)
                .fillMaxSize()
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            item {
                OutlinedTextField(
                    value = state.identifier,
                    onValueChange = onIdentifierChange,
                    modifier = Modifier.fillMaxWidth(),
                    label = { Text("授权标识") },
                    singleLine = true
                )
            }
            item {
                Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    Button(onClick = onAuthorize, enabled = !state.loading) {
                        Text("获取次数")
                    }
                    OutlinedButton(onClick = onLoadTasks, enabled = !state.loading && state.identifier.isNotBlank()) {
                        Text("获取任务")
                    }
                }
            }
            item {
                AssistChip(onClick = {}, label = { Text(state.message) })
            }
            item {
                ElevatedCard(Modifier.fillMaxWidth()) {
                    Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(6.dp)) {
                        Text("剩余次数：${state.remainingUses}")
                        Text("累计授权：${state.totalGranted}")
                        Text("累计使用：${state.usedCount}")
                    }
                }
            }
            items(state.tasks, key = { it.id }) { task ->
                TaskCard(task = task, onOpenSms = onOpenSms, onConfirm = onConfirm)
            }
        }
    }
}

@Composable
private fun TaskCard(
    task: AppTask,
    onOpenSms: (AppTask) -> Unit,
    onConfirm: (AppTask) -> Unit,
) {
    ElevatedCard(Modifier.fillMaxWidth()) {
        Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
            Text(task.taskName, style = MaterialTheme.typography.titleMedium)
            Text("待发送号码：${task.phoneNumbers.size}，批量：${task.batchSize}，状态：${task.status}")
            Text(task.content1)
            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                Button(onClick = { onOpenSms(task) }) {
                    Text("打开系统短信")
                }
                OutlinedButton(onClick = { onConfirm(task) }) {
                    Text("确认本批已发送")
                }
            }
        }
    }
}
