package com.djordjemilanovic.lindasharp

import com.google.protobuf.ListValue
import com.google.protobuf.NullValue
import com.google.protobuf.Struct
import com.google.protobuf.Value
import com.djordjemilanovic.lindasharp.services.ActionsOuterClass

fun Any?.toValue() : Value {
    val builder = Value.newBuilder()

    when (this) {
        null -> builder.nullValue = (NullValue.NULL_VALUE)
        is Boolean -> builder.boolValue = this
        is String -> builder.stringValue = this
        is Number -> builder.numberValue = this.toDouble()
        is Char -> builder.stringValue = this.toString()
        is Collection<*> -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is Map<*, *> -> {
            val struct = Struct.newBuilder()
            for ((key, value) in this)
                struct.putFields(key.toString(), value.toValue())

            builder.structValue = struct.build()
        }

        is Array<*> -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is BooleanArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is ByteArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is CharArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is DoubleArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is FloatArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is IntArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is LongArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()
        is ShortArray -> builder.listValue = ListValue.newBuilder().addAllValues(this.map { it.toValue() }).build()

        else -> throw IllegalArgumentException("Cannot convert $this")
    }

    return builder.build()
}

fun Value.toAny() : Any =
    when (this.kindCase) {
        Value.KindCase.BOOL_VALUE -> this.boolValue
        Value.KindCase.NUMBER_VALUE -> this.numberValue
        Value.KindCase.STRING_VALUE -> this.stringValue
        Value.KindCase.STRUCT_VALUE -> this.structValue.fieldsMap.mapValues { it.value.toAny() }
        Value.KindCase.LIST_VALUE -> this.listValue.valuesList.map { it.toAny() }
        Value.KindCase.KIND_NOT_SET, Value.KindCase.NULL_VALUE -> throw IllegalArgumentException("Invalid $this")
        else -> throw IllegalArgumentException("Cannot convert $this")
    }

fun Array<out Any?>.toGrpcPattern() = ActionsOuterClass.Pattern.newBuilder().addAllFields(this.map { it.toValue() }).build()
fun Array<out Any>.toGrpcTuple() = ActionsOuterClass.Tuple.newBuilder().addAllFields(this.map { it.toValue() }).build()
fun ActionsOuterClass.Tuple.toLindaTuple() = this.fieldsList.map { it.toAny() }.toTypedArray()