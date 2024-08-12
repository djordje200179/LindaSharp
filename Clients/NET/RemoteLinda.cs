using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using LindaSharp.Services;

namespace LindaSharp.Client;

public class RemoteLinda : IScriptEvalLinda, IDisposable {
	private readonly GrpcChannel channel;
	private readonly Actions.ActionsClient actionsClient;
	private readonly Scripts.ScriptsClient scriptsClient;
	private readonly Health.HealthClient healthClient;

	public RemoteLinda(string address) {
		channel = GrpcChannel.ForAddress(address);
		actionsClient = new Actions.ActionsClient(channel);
		healthClient = new Health.HealthClient(channel);
		scriptsClient = new Scripts.ScriptsClient(channel);
	}

	public async Task Put(params object[] tuple) {
		await actionsClient.OutAsync(tuple.ToGrpcTuple());
	}

	public async Task<object[]> Get(params object?[] pattern) => (await actionsClient.InAsync(pattern.ToGrpcPattern())).ToLindaTuple();
	public async Task<object[]> Query(params object?[] pattern) => (await actionsClient.RdAsync(pattern.ToGrpcPattern())).ToLindaTuple();


	public async Task<object[]?> TryGet(params object?[] pattern) => 
		(await actionsClient.InpAsync(pattern.ToGrpcPattern())).Tuple?.ToLindaTuple();

	public async Task<object[]?> TryQuery(params object?[] pattern) => 
		(await actionsClient.RdpAsync(pattern.ToGrpcPattern())).Tuple?.ToLindaTuple();

	public async Task RegisterScript(string key, string ironpythonCode) => await scriptsClient.RegisterAsync(new RegisterScriptRequest { 
		Key = key,
		Script = new Script {
			Type = Script.Types.Type.Ironpython,
			Code = ironpythonCode,
		}
	});

	public async Task InvokeScript(string key, object? parameter = null) => await scriptsClient.InvokeAsync(new InvokeScriptRequest { 
		Key = key, 
		Parameter = MessageConversions.ElemToValue(parameter)
	});


	public async Task EvalScript(string ironpythonCode) => await scriptsClient.EvalAsync(new EvalScriptRequest {
		Script = new Script {
			Type = Script.Types.Type.Ironpython,
			Code = ironpythonCode,
		}
	});

	public void Dispose() {
		GC.SuppressFinalize(this);

		channel.Dispose();
	}

	public async Task<bool> IsHealthy() {
		try {
			var result = await healthClient.PingAsync(new Empty(), deadline: DateTime.UtcNow.AddSeconds(1));
			return result.Value == "pong";
		} catch (Exception) {
			return false;
		}
	}
}
