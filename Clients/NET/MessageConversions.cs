using Google.Protobuf.WellKnownTypes;
using System.Numerics;
using GrpcTuple = LindaSharp.Services.Tuple;
using GrpcPattern = LindaSharp.Services.Pattern;
using GrpcScriptExecutionStatus = LindaSharp.Services.ScriptExecutionStatus;
using GrpcScript = LindaSharp.Services.Script;
using System.Collections;
using static LindaSharp.IScriptEvalLinda;
using static LindaSharp.IScriptEvalLinda.ScriptExecutionStatus;

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

			IDictionary dict => Value.ForStruct(dict.ToStruct()),
			IEnumerable list => Value.ForList(list.Cast<object>().Select(ElemToValue).ToArray()),
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
			Value.KindOneofCase.StructValue => value.StructValue.Fields.ToDictionary(p => p.Key, p => ValueToElem(p.Value))!,
			Value.KindOneofCase.ListValue => value.ListValue.Values.Select(ValueToElem).ToList(),
			_ => throw new ArgumentException($"Unknown case {value.KindCase}")
		};
	}

	private static Struct ToStruct(this IDictionary dict) {
		var s = new Struct();

		foreach (DictionaryEntry entry in dict)
			s.Fields.Add(entry.Key.ToString(), ElemToValue(entry.Value));

		return s;
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

	public static ScriptExecutionStatus ToLindaStatus(this GrpcScriptExecutionStatus status) {
		return status.StatusCase switch {
			GrpcScriptExecutionStatus.StatusOneofCase.NotFound => new ScriptExecutionStatus(ExecutionState.NotFound),
			GrpcScriptExecutionStatus.StatusOneofCase.Ok => new ScriptExecutionStatus(ExecutionState.Finished),
			GrpcScriptExecutionStatus.StatusOneofCase.Exception => new ScriptExecutionStatus(
				ExecutionState.Exception,
				new Exception(status.Exception.Message) {
					Source = status.Exception.Source,
					// TODO: Embed other fields
				}),
			_ => throw new ArgumentException(nameof(status.StatusCase))
		};
	}

	public static GrpcScript ToGrpcScript(this Script script) {
		return new GrpcScript {
			Code = script.Code,
			Type = script.Type switch {
				Script.Language.IronPython => GrpcScript.Types.Type.Ironpython,
				Script.Language.CSharp => GrpcScript.Types.Type.CSharp,
				_ => throw new ArgumentException(nameof(script.Type))
			}
		};
	}
}