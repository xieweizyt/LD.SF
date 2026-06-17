package com.ldsf.app.data

import android.content.Context
import com.ldsf.app.RsaKeys
import com.ldsf.app.model.AppTask
import com.ldsf.app.model.AuthorizationResponse
import com.ldsf.app.model.AuthorizeRequest
import com.ldsf.app.model.ConfirmSentRequest
import com.ldsf.app.network.LdSfApi

class LdSfRepository(
    private val api: LdSfApi
) {
    suspend fun authorize(context: Context, identifier: String): AuthorizationResponse {
        val publicKeyPem = RsaKeys.getOrCreatePublicKeyPem(context)
        return api.authorize(AuthorizeRequest(identifier, publicKeyPem))
    }

    suspend fun getTasks(identifier: String): List<AppTask> = api.getTasks(identifier)

    suspend fun confirmSent(taskId: String, phones: List<String>, content: String) {
        api.confirmSent(taskId, ConfirmSentRequest(phones, content))
    }
}

