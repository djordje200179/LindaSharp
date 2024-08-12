using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LindaSharp.Services;

namespace LindaSharp.Server.Services;

public class ScriptsService(SharedLinda linda) : Scripts.ScriptsBase {
	public override async Task<Empty> Register(RegisterScriptRequest request, ServerCallContext context) {
		await linda.RegisterScript(request.Key, request.Script.Code);
		return new Empty();
	}

	public override async Task<Empty> Invoke(InvokeScriptRequest request, ServerCallContext context) {
		await linda.InvokeScript(request.Key, MessageConversions.ValueToElem(request.Parameter));
		return new Empty();
	}

	public override async Task<Empty> Eval(EvalScriptRequest request, ServerCallContext context) {
		await linda.EvalScript(request.Script.Code);
		return new Empty();
	}
}
