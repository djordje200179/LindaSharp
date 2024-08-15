using Google.Protobuf.WellKnownTypes;
using System.Numerics;
using GrpcTuple = LindaSharp.Services.Tuple;
using GrpcPattern = LindaSharp.Services.Pattern;

namespace LindaSharp.Client;

internal static class MessageConversions {
	internal static Value ElemToValue(object? elem) {
		return elem switch {
			null => Value.ForNull(),
			bool b => Value.ForBool(b),
			string str => Value.ForString(str),

			// TODO: Simplify
			sbyte num => Value.ForNumber(num),
			byte num => Value.ForNumber(num),
			short num => Value.ForNumber(num),
			ushort num => Value.ForNumber(num),
			int num => Value.ForNumber(num),
			uint num => Value.ForNumber(num),
			long num => Value.ForNumber(num),
			ulong num => Value.ForNumber(num),
			nint num => Value.ForNumber(num),
			nuint num => Value.ForNumber(num),
			float num => Value.ForNumber(num),
			double num => Value.ForNumber(num),
			decimal num => Value.ForNumber((double)num),
			BigInteger num => Value.ForNumber((double)num),

			IEnumerable<object> list => Value.ForList(list.Select(ElemToValue).ToArray()),
			IDictionary<string, object> dict => Value.ForStruct(dict.ToStruct()),
			_ => throw new ArgumentException($"Cannot parse {elem}")
		};
	}

	private static object? ValueToElem(Value value) {
		return value.KindCase switch {
			Value.KindOneofCase.None => throw new NotImplementedException(),
			Value.KindOneofCase.NullValue => null,
			Value.KindOneofCase.NumberValue => value.NumberValue,
			Value.KindOneofCase.StringValue => value.StringValue,
			Value.KindOneofCase.BoolValue => value.BoolValue,
			Value.KindOneofCase.StructValue => value.StructValue.ToDict(),
			Value.KindOneofCase.ListValue => value.ListValue.Values.Select(ValueToElem).ToList(),
			_ => throw new ArgumentException($"Unknown case {value.KindCase}")
		};
	}

	private static Struct ToStruct(this IDictionary<string, object> dict) {
		var s = new Struct();
		foreach (var (key, value) in dict)
			s.Fields.Add(key, ElemToValue(value));

		return s;
	}

	private static IDictionary<string, object> ToDict(this Struct s) {
		return s.Fields.ToDictionary(p => p.Key, p => ValueToElem(p.Value))!;
	}

	public static GrpcTuple ToGrpcTuple(this object[] lindaTuple) {
		var grpcTuple = new GrpcTuple();
		grpcTuple.Fields.AddRange(lindaTuple.Select(ElemToValue));

		return grpcTuple;
	}

	public static GrpcPattern ToGrpcPattern(this object?[] lindaPattern) {
		var grpcPattern = new GrpcPattern();
		grpcPattern.Fields.AddRange(lindaPattern.Select(ElemToValue));

		return grpcPattern;
	}

	public static object[] ToLindaTuple(this GrpcTuple grpcTuple) => grpcTuple.Fields.Select(ValueToElem).ToArray()!;
}