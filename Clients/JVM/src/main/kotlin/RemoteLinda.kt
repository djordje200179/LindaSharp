package com.djordjemilanovic.lindasharp

import com.djordjemilanovic.lindasharp.services.ActionsGrpcKt.ActionsCoroutineStub
import com.djordjemilanovic.lindasharp.services.HealthGrpcKt.HealthCoroutineStub
import com.djordjemilanovic.lindasharp.services.ScriptsGrpcKt.ScriptsCoroutineStub
import com.google.protobuf.Empty
import io.grpc.ManagedChannelBuilder

class RemoteLinda(hostname: String, port: Int) {
    private val channel = ManagedChannelBuilder.forAddress(hostname, port).usePlaintext().build()
    private val actionsStub = ActionsCoroutineStub(channel)
    private val healthStub = HealthCoroutineStub(channel)
    private val scriptsStub = ScriptsCoroutineStub(channel)

    suspend fun put(vararg tuple: Any) = actionsStub.out(tuple.toGrpcTuple())
    suspend fun get(vararg pattern: Any?) = actionsStub.`in`(pattern.toGrpcPattern()).toLindaTuple()
    suspend fun query(vararg pattern: Any?) = actionsStub.rd(pattern.toGrpcPattern()).toLindaTuple()

    suspend fun tryGet(vararg pattern: Any?) : Array<Any>? {
        val response = actionsStub.inp(pattern.toGrpcPattern())
        return if(response.hasTuple()) response.tuple.toLindaTuple() else null
    }
    suspend fun tryQuery(vararg pattern: Any?) : Array<Any>? {
        val response = actionsStub.rdp(pattern.toGrpcPattern())
        return if(response.hasTuple()) response.tuple.toLindaTuple() else null
    }

    suspend fun isHealthy() =
        try {
            healthStub.ping(Empty.getDefaultInstance()).value == "pong"
        } catch (_: Exception) {
            false
        }
}