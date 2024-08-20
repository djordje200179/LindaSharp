using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LindaSharp.Services;

namespace LindaSharp.Server.Services;

public class ScriptsService(IScriptEvalLinda linda) : Scripts.ScriptsBase {
	public override async Task<Empty> Register(RegisterScriptRequest request, ServerCallContext context) {
		await linda.RegisterScript(request.Key, request.Script.Code);
		return new Empty();
	}

	public override async Task<EvalScriptResponse> Invoke(InvokeScriptRequest request, ServerCallContext context) {
		var id = await linda.InvokeScript(request.Key, MessageConversions.ValueToElem(request.Parameter));

		return new EvalScriptResponse {
			TaskId = id
		};
	}

	public override async Task<EvalScriptResponse> Eval(EvalScriptRequest request, ServerCallContext context) {
		var id = await linda.EvalScript(request.Script.Code);
		
		return new EvalScriptResponse {
			TaskId = id
		};
	}
}
