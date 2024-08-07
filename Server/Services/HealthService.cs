using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LindaSharp.Services;

namespace LindaSharp.Server.Services;

public class HealthService : Health.HealthBase {
	public override async Task<StringValue> Ping(Empty request, ServerCallContext context) {
		return new StringValue { Value = "pong" };
	}
}
