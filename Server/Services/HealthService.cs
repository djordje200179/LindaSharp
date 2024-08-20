using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LindaSharp.Services;

namespace LindaSharp.Server.Services;

public class HealthService(SharedLinda linda) : Health.HealthBase {
	public override Task<StringValue> Ping(Empty request, ServerCallContext context) {
		return Task.FromResult(new StringValue { Value = "pong" });
	}

	public override async Task<ScriptExecutionStatus> GetScriptExecutionStatus(Int32Value request, ServerCallContext context) {
		var status = await linda.GetScriptExecutionStatus(request.Value);
		return status.ToGrpcStatus();
	}
}
