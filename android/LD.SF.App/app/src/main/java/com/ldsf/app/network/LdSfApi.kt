package com.ldsf.app.network

import com.ldsf.app.model.AppTask
import com.ldsf.app.model.AuthorizationResponse
import com.ldsf.app.model.AuthorizeRequest
import com.ldsf.app.model.ConfirmSentRequest
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface LdSfApi {
    @POST("api/ldsf/app/authorize")
    suspend fun authorize(@Body request: AuthorizeRequest): AuthorizationResponse

    @GET("api/ldsf/app/tasks/{identifier}")
    suspend fun getTasks(@Path("identifier") identifier: String): List<AppTask>

    @POST("api/ldsf/app/tasks/{taskId}/confirm-sent")
    suspend fun confirmSent(
        @Path("taskId") taskId: String,
        @Body request: ConfirmSentRequest
    )
}

