using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LindaSharp.Services;
using GrpcTuple = LindaSharp.Services.Tuple;
using GrpcOptionalTuple = LindaSharp.Services.OptionalTuple;
using GrpcPattern = LindaSharp.Services.Pattern;

namespace LindaSharp.Server.Services;

public class ActionsService(SharedLinda linda) : Actions.ActionsBase {
	public override async Task<Empty> Out(GrpcTuple request, ServerCallContext context) {
		await linda.Put(request.ToLindaTuple());
		return new Empty();
	}
	
	public override async Task<GrpcTuple> In(GrpcPattern request, ServerCallContext context) {
		return (await linda.Get(request.ToLindaPattern())).ToGrpcTuple();
	}

	public override async Task<GrpcTuple> Rd(GrpcPattern request, ServerCallContext context) {
		return (await linda.Query(request.ToLindaPattern())).ToGrpcTuple();
	}

	public override async Task<GrpcOptionalTuple> Inp(GrpcPattern request, ServerCallContext context) {
		return new GrpcOptionalTuple {
			Tuple = (await linda.Get(request.ToLindaPattern()))?.ToGrpcTuple()
		};
	}

	public override async Task<GrpcOptionalTuple> Rdp(GrpcPattern request, ServerCallContext context) {
		return new GrpcOptionalTuple {
			Tuple = (await linda.Query(request.ToLindaPattern()))?.ToGrpcTuple()
		};
	}

	public override async Task<Empty> RegisterScript(RegisterScriptRequest request, ServerCallContext context) {
		await linda.RegisterScript(request.Key, request.IronPythonCode);
		return new Empty();
	}

	public override async Task<Empty> InvokeScript(InvokeScriptRequest request, ServerCallContext context) {
		await linda.InvokeScript(request.Key, MessageConversions.ValueToElem(request.Parameter));
		return new Empty();
	}

	public override async Task<Empty> EvalScript(EvalScriptRequest request, ServerCallContext context) {
		await linda.EvalScript(request.IronPythonCode);
		return new Empty();
	}
}