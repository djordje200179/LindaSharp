package com.djordjemilanovic.lindasharp

import com.djordjemilanovic.lindasharp.services.ActionsGrpcKt.ActionsCoroutineStub
import com.djordjemilanovic.lindasharp.services.ActionsOuterClass
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

    suspend fun tryGet(vararg pattern: Any?) =
        actionsStub.inp(pattern.toGrpcPattern())
            .takeIf(ActionsOuterClass.OptionalTuple::hasTuple)
            ?.tuple?.toLindaTuple()
    suspend fun tryQuery(vararg pattern: Any?) =
        actionsStub.rdp(pattern.toGrpcPattern())
            .takeIf(ActionsOuterClass.OptionalTuple::hasTuple)
            ?.tuple?.toLindaTuple()

    suspend fun isHealthy() = runCatching { healthStub.ping(Empty.getDefaultInstance()).value == "pong" }.getOrDefault(false)
}