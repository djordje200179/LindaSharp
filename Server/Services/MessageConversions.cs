using Google.Protobuf.WellKnownTypes;
using LindaSharp.Server.Types;
using System.Numerics;
using GrpcTuple = LindaSharp.Services.Tuple;
using GrpcPattern = LindaSharp.Services.Pattern;
using GrpcScriptExecutionStatus = LindaSharp.Services.ScriptExecutionStatus;
using static LindaSharp.IScriptEvalLinda;
using static LindaSharp.IScriptEvalLinda.ScriptExecutionStatus;

namespace LindaSharp.Server.Services;

internal static class MessageConversions {
	private static Value ElemToValue(object? elem) {
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

			ComparableList list => Value.ForList(list.Select(ElemToValue).ToArray()),
			ComparableDictionary dict => Value.ForStruct(dict.ToStruct()),
			_ => throw new ArgumentException($"Cannot parse {elem}")
		};
	}

	internal static object? ValueToElem(Value value) {
		return value.KindCase switch {
			Value.KindOneofCase.None => throw new NotImplementedException(),
			Value.KindOneofCase.NullValue => null,
			Value.KindOneofCase.NumberValue => new BigInteger(value.NumberValue),
			Value.KindOneofCase.StringValue => value.StringValue,
			Value.KindOneofCase.BoolValue => value.BoolValue,
			Value.KindOneofCase.StructValue => value.StructValue.ToDict(),
			Value.KindOneofCase.ListValue => new ComparableList(value.ListValue.Values.Select(ValueToElem)),
			_ => throw new ArgumentException($"Unknown case {value.KindCase}")
		};
	}

	private static Struct ToStruct(this ComparableDictionary dict) {
		var s = new Struct();
		foreach (var (key, value) in dict)
			s.Fields.Add(key, ElemToValue(value));

		return s;
	}

	private static ComparableDictionary ToDict(this Struct s) {
		var dict = new ComparableDictionary();
		foreach (var (key, value) in s.Fields)
			dict.Add(key, ValueToElem(value));

		return dict;
	}

	public static GrpcTuple ToGrpcTuple(this object[] lindaTuple) {
		var grpcTuple = new GrpcTuple();
		grpcTuple.Fields.AddRange(lindaTuple.Select(ElemToValue));

		return grpcTuple;
	}

	public static object[] ToLindaTuple(this GrpcTuple grpcTuple) => grpcTuple.Fields.Select(ValueToElem).ToArray()!;
	public static object?[] ToLindaPattern(this GrpcPattern grpcPattern) => grpcPattern.Fields.Select(ValueToElem).ToArray();

	public static GrpcScriptExecutionStatus ToGrpcStatus(this ScriptExecutionStatus status) {
		return status switch {
			ScriptExecutionStatus(ExecutionState.NotFound, _) => new GrpcScriptExecutionStatus { NotFound = new Empty() },
			ScriptExecutionStatus(ExecutionState.Finished, _) => new GrpcScriptExecutionStatus { Ok = new Empty() },
			ScriptExecutionStatus(ExecutionState.Exception, var exception) when exception is not null =>
				new GrpcScriptExecutionStatus {
					Exception = new GrpcScriptExecutionStatus.Types.Exception {
						Message = exception.Message,
						Source = exception.Source,
						StackTrace = exception.StackTrace,
						Type = exception.GetType().FullName,
					}
				},
			_ => throw new ArgumentException("invalid status")
		};
	}
}