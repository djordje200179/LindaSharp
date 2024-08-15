package com.djordjemilanovic.lindasharp.client;

import com.djordjemilanovic.lindasharp.services.ActionsOuterClass;
import com.google.protobuf.*;
import java.math.BigDecimal;
import java.math.BigInteger;
import java.util.Arrays;
import java.util.Collection;
import java.util.Map;
import java.util.stream.Collectors;

class MessageConversions {
	static Value elemToValue(Object elem) {
		return (switch (elem) {
			case null -> Value.newBuilder().setNullValue(NullValue.NULL_VALUE);
			case Boolean b -> Value.newBuilder().setBoolValue(b);
			case String str -> Value.newBuilder().setStringValue(str);

			// TODO: Simplify
			case Byte num -> Value.newBuilder().setNumberValue(num);
			case Short num -> Value.newBuilder().setNumberValue(num);
			case Integer num -> Value.newBuilder().setNumberValue(num);
			case Long num -> Value.newBuilder().setNumberValue(num);
			case Float num -> Value.newBuilder().setNumberValue(num);
			case Double num -> Value.newBuilder().setNumberValue(num);
			case BigInteger num -> Value.newBuilder().setNumberValue(num.doubleValue());
			case BigDecimal num -> Value.newBuilder().setNumberValue(num.doubleValue());

			case Collection<?> iterable -> Value.newBuilder().setListValue(ListValue.newBuilder().addAllValues(
				iterable.stream().map(MessageConversions::elemToValue).collect(Collectors.toList())
			));
			case Object[] array -> Value.newBuilder().setListValue(ListValue.newBuilder().addAllValues(
				Arrays.stream(array)
					.map(MessageConversions::elemToValue)
					.collect(Collectors.toList())
			));
			case Map<?, ?> map -> {
				var struct = Struct.newBuilder();

				for (var entry : map.entrySet())
					struct.putFields(entry.getKey().toString(), elemToValue(entry.getValue()));

				yield Value.newBuilder().setStructValue(struct);
			}
			default -> throw new IllegalArgumentException(String.format("Cannot parse %s", elem));
		}).build();
	}

	private static Object valueToElem(Value value) {
		return switch(value.getKindCase()) {
			case Value.KindCase.KIND_NOT_SET -> throw new UnsupportedOperationException();
			case Value.KindCase.NULL_VALUE -> null;
			case Value.KindCase.NUMBER_VALUE -> value.getNumberValue();
			case Value.KindCase.STRING_VALUE -> value.getStringValue();
			case Value.KindCase.BOOL_VALUE -> value.getBoolValue();
			case Value.KindCase.STRUCT_VALUE ->
				value.getStructValue().getFieldsMap().entrySet().stream().collect(Collectors.toMap(
					Map.Entry::getKey,
					e -> valueToElem(e.getValue())
				));
			case Value.KindCase.LIST_VALUE -> value.getListValue().getValuesList().stream().map(MessageConversions::valueToElem).toList();
		};
	}

	public static ActionsOuterClass.Tuple ToGrpcTuple(Object[] lindaTuple) {
		var grpcTuple = ActionsOuterClass.Tuple.newBuilder();

		grpcTuple.getFieldsList().addAll(
			Arrays.stream(lindaTuple).map(MessageConversions::elemToValue).toList()
		);

		return grpcTuple.build();
	}

	public static ActionsOuterClass.Pattern ToGrpcPattern(Object[] lindaPattern) {
		var grpcPattern = ActionsOuterClass.Pattern.newBuilder();

		for (var elem: lindaPattern)
			grpcPattern.addFields(MessageConversions.elemToValue(elem));

		return grpcPattern.build();
	}

	public static Object[] ToLindaTuple(ActionsOuterClass.Tuple grpcTuple) {
		return grpcTuple.getFieldsList().stream().map(MessageConversions::valueToElem).toArray();
	}
}
