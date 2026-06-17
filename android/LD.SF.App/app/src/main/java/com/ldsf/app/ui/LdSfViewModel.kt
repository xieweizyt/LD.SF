package com.ldsf.app.ui

import android.app.Application
import android.content.Intent
import android.net.Uri
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.ldsf.app.data.LdSfRepository
import com.ldsf.app.model.AppTask
import com.ldsf.app.network.NetworkModule
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

data class LdSfUiState(
    val identifier: String = "",
    val remainingUses: Int = 0,
    val totalGranted: Int = 0,
    val usedCount: Int = 0,
    val tasks: List<AppTask> = emptyList(),
    val loading: Boolean = false,
    val message: String = "请输入授权标识",
)

class LdSfViewModel(application: Application) : AndroidViewModel(application) {
    private val repository = LdSfRepository(NetworkModule.api)
    private val _state = MutableStateFlow(LdSfUiState())
    val state: StateFlow<LdSfUiState> = _state

    fun setIdentifier(value: String) {
        _state.update { it.copy(identifier = value) }
    }

    fun authorize() {
        val identifier = state.value.identifier.trim()
        if (identifier.isBlank()) {
            _state.update { it.copy(message = "请输入授权标识") }
            return
        }

        viewModelScope.launch {
            runCatching {
                _state.update { it.copy(loading = true, message = "正在获取授权次数") }
                repository.authorize(getApplication(), identifier)
            }.onSuccess { result ->
                _state.update {
                    it.copy(
                        loading = false,
                        identifier = result.plain.identifier,
                        remainingUses = result.plain.remainingUses,
                        totalGranted = result.plain.totalGranted,
                        usedCount = result.plain.usedCount,
                        message = "授权成功，RSA：${result.encrypted?.algorithm ?: "未返回加密信封"}"
                    )
                }
            }.onFailure { error ->
                _state.update { it.copy(loading = false, message = error.message ?: "授权失败") }
            }
        }
    }

    fun loadTasks() {
        val identifier = state.value.identifier.trim()
        if (identifier.isBlank()) return
        viewModelScope.launch {
            runCatching {
                _state.update { it.copy(loading = true, message = "正在获取任务") }
                repository.getTasks(identifier)
            }.onSuccess { tasks ->
                _state.update { it.copy(loading = false, tasks = tasks, message = "任务数：${tasks.size}") }
            }.onFailure { error ->
                _state.update { it.copy(loading = false, message = error.message ?: "获取任务失败") }
            }
        }
    }

    fun buildSmsIntent(task: AppTask): Intent? {
        val phones = nextBatch(task)
        if (phones.isEmpty()) {
            _state.update { it.copy(message = "没有剩余次数或待发送号码") }
            return null
        }

        return Intent(Intent.ACTION_SENDTO, Uri.parse("smsto:${phones.joinToString(";")}")).apply {
            putExtra("sms_body", task.content1)
        }
    }

    fun confirmSent(task: AppTask) {
        val phones = nextBatch(task)
        if (phones.isEmpty()) return

        viewModelScope.launch {
            runCatching {
                _state.update { it.copy(loading = true, message = "正在确认发送") }
                repository.confirmSent(task.id, phones, task.content1)
            }.onSuccess {
                _state.update {
                    it.copy(
                        loading = false,
                        remainingUses = (it.remainingUses - phones.size).coerceAtLeast(0),
                        message = "已确认 ${phones.size} 条"
                    )
                }
                authorize()
                loadTasks()
            }.onFailure { error ->
                _state.update { it.copy(loading = false, message = error.message ?: "确认失败") }
            }
        }
    }

    private fun nextBatch(task: AppTask): List<String> {
        val allowed = minOf(task.batchSize, state.value.remainingUses, task.phoneNumbers.size)
        return task.phoneNumbers.take(allowed)
    }
}

