package com.ldsf.app.model

data class AuthorizeRequest(
    val identifier: String,
    val appPublicKeyPem: String
)

data class AuthorizationResponse(
    val plain: AuthorizationPayload,
    val encrypted: EncryptedEnvelope?
)

data class AuthorizationPayload(
    val identifier: String,
    val subaccountId: String,
    val remainingUses: Int,
    val totalGranted: Int,
    val usedCount: Int,
    val serverTime: String
)

data class EncryptedEnvelope(
    val algorithm: String,
    val cipherText: String
)

data class AppTask(
    val id: String,
    val taskName: String,
    val batchSize: Int,
    val content1: String,
    val content2: String?,
    val status: String,
    val phoneNumbers: List<String>
)

data class ConfirmSentRequest(
    val phoneNumbers: List<String>,
    val content: String
)

