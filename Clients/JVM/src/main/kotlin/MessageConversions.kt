package com.djordjemilanovic.lindasharp

import com.google.protobuf.ListValue
import com.google.protobuf.NullValue
import com.google.protobuf.Struct
import com.google.protobuf.Value
import com.djordjemilanovic.lindasharp.services.ActionsOuterClass

fun Any?.toValue(): Value = Value.newBuilder().also {
    when (this) {
        null -> it.nullValue = (NullValue.NULL_VALUE)
        is Boolean -> it.boolValue = this
        is String -> it.stringValue = this
        is Number -> it.numberValue = toDouble()
        is Char -> it.stringValue = toString()
        is Collection<*> -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is Map<*, *> -> it.structValue = entries.fold(Struct.newBuilder()) { builder, (key, value) ->
            builder.putFields(key.toString(), value.toValue())
        }.build()

        is Array<*> -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is BooleanArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is ByteArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is CharArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is DoubleArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is FloatArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is IntArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is LongArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()
        is ShortArray -> it.listValue = ListValue.newBuilder().addAllValues(map(Any?::toValue)).build()

        else -> throw IllegalArgumentException("Cannot convert $this")
    }
}.build()

fun Value.toAny() : Any =
    when (kindCase) {
        Value.KindCase.BOOL_VALUE -> boolValue
        Value.KindCase.NUMBER_VALUE -> numberValue
        Value.KindCase.STRING_VALUE -> stringValue
        Value.KindCase.STRUCT_VALUE -> structValue.fieldsMap.mapValues { it.value.toAny() }
        Value.KindCase.LIST_VALUE -> listValue.valuesList.map(Value::toAny)
        Value.KindCase.KIND_NOT_SET, Value.KindCase.NULL_VALUE -> throw IllegalArgumentException("Invalid $this")
        else -> throw IllegalArgumentException("Cannot convert $this")
    }

fun Array<out Any?>.toGrpcPattern() = ActionsOuterClass.Pattern.newBuilder().addAllFields(map(Any?::toValue)).build()
fun Array<out Any>.toGrpcTuple() = ActionsOuterClass.Tuple.newBuilder().addAllFields(map(Any?::toValue)).build()
fun ActionsOuterClass.Tuple.toLindaTuple() = fieldsList.map(Value::toAny).toTypedArray()