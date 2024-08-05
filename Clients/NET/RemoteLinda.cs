using LindaSharp.Server;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LindaSharp.Client;

public class RemoteLinda(string host, ushort port) : IScriptEvalLinda, IDisposable {
	private readonly HttpClient actionsHttpClient = new() {
		BaseAddress = new Uri($"http://{host}:{port}/api/actions/"),
		Timeout = Timeout.InfiniteTimeSpan
	};
	private readonly HttpClient healthHttpClient = new() {
		BaseAddress = new Uri($"http://{host}:{port}/api/health/"),
		Timeout = TimeSpan.FromSeconds(1)
	};

	private readonly CancellationTokenSource cancellationTokenSource = new();
	private static readonly JsonSerializerOptions serializationOptions = new() {
		Converters = {
			new TupleJsonSerializer()
		},
		WriteIndented = true
	};

	private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request) {
		try {
			var response = await actionsHttpClient.SendAsync(request, cancellationTokenSource.Token);
			ObjectDisposedException.ThrowIf(response.StatusCode == HttpStatusCode.InternalServerError, this);

			return response;
		} catch (TaskCanceledException) {
			throw new ObjectDisposedException(nameof(RemoteLinda));
		}
	}

	private async Task<object[]> WaitTuple(object?[] pattern, bool delete) {
		using var request = new HttpRequestMessage(
			delete ? HttpMethod.Delete : HttpMethod.Get,
			delete ? "in" : "rd"
		) {
			Content = JsonContent.Create(pattern, options: serializationOptions)
		};

		using var response = await SendRequest(request);

		return await response.Content.ReadFromJsonAsync<object[]>(serializationOptions);
	}

	private async Task<object[]?> TryGetTuple(object?[] pattern, bool delete) {
		using var request = new HttpRequestMessage(
			delete ? HttpMethod.Delete : HttpMethod.Get, 
			delete ? "inp" : "rdp"
		) {
			Content = JsonContent.Create(pattern, options: serializationOptions)
		};

		using var response = await SendRequest(request);

		if (response.StatusCode == HttpStatusCode.NotFound)
			return null;

		return await response.Content.ReadFromJsonAsync<object[]>(serializationOptions);
	}

	public async Task Put(params object[] tuple) {
		var request = new HttpRequestMessage(HttpMethod.Post, "out") {
			Content = JsonContent.Create(tuple, options: serializationOptions)
		};

		using var response = await SendRequest(request);
		response.EnsureSuccessStatusCode();
	}

	public Task<object[]> Get(params object?[] pattern) => WaitTuple(pattern, true);
	public Task<object[]> Query(params object?[] pattern) => WaitTuple(pattern, false);


	public Task<object[]?> TryGet(params object?[] pattern) => TryGetTuple(pattern, true);
	public Task<object[]?> TryQuery(params object?[] pattern) => TryGetTuple(pattern, false);

	public async Task RegisterScript(string key, string ironpythonCode) {
		var request = new HttpRequestMessage(HttpMethod.Put, $"eval/{key}") {
			Content = new StringContent(ironpythonCode, MediaTypeHeaderValue.Parse("text/ironpython"))
		};

		using var response = await SendRequest(request);
		response.EnsureSuccessStatusCode();
	}

	public async Task InvokeScript(string key, object? parameter = null) {
		var request = new HttpRequestMessage(HttpMethod.Post, $"eval/{key}") {
			Content = JsonContent.Create(parameter, options: serializationOptions)
		};

		using var response = await SendRequest(request);
		response.EnsureSuccessStatusCode();
	}


	public async Task EvalScript(string ironpythonCode) {
		var request = new HttpRequestMessage(HttpMethod.Post, "eval") {
			Content = new StringContent(ironpythonCode, MediaTypeHeaderValue.Parse("text/ironpython"))
		};

		using var response = await SendRequest(request);
		response.EnsureSuccessStatusCode();
	}

	public void Dispose() {
		cancellationTokenSource.Cancel();
		cancellationTokenSource.Dispose();
		actionsHttpClient.Dispose();
		healthHttpClient.Dispose();
	}

	public async Task<bool> IsHealthy() {
		try {
			using var response = await healthHttpClient.GetAsync("ping", cancellationTokenSource.Token);
			return response.IsSuccessStatusCode;
		} catch (Exception) {
			return false;
		}
	}
}
