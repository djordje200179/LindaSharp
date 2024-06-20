using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LindaSharp.Services;
using System.Net;
using LindaTuple = LindaSharp.Services.Tuple;
namespace LindaSharp.Server.Services;

public class ActionsService(SharedLinda linda) : Actions.ActionsBase {
	public override Task<LindaTuple> In(TuplePattern request, ServerCallContext context) {
		return base.In(request, context);
	}

	public override Task<OptionalTuple> Inp(TuplePattern request, ServerCallContext context) {
		return base.Inp(request, context);
	}

	public override Task<Empty> Out(LindaTuple request, ServerCallContext context) {
		linda.Out(request.Fields.ToArray());
		return Task.FromResult(new Empty());
	}

	public override Task<LindaTuple> Rd(TuplePattern request, ServerCallContext context) {
		return base.Rd(request, context);
	}

	public override Task<OptionalTuple> Rdp(TuplePattern request, ServerCallContext context) {
		return base.Rdp(request, context);
	}
}
