using LindaSharp.Server;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LindaSharp.Client;

public class RemoteLinda : ILinda {
	private readonly HttpClient actionsHttpClient, healthHttpClient;
	private readonly CancellationTokenSource cancellationTokenSource = new();
	private static readonly JsonSerializerOptions serializationOptions = new() {
		Converters = {
				new TupleJsonSerializer()
			},
		WriteIndented = true
	};

	public RemoteLinda(string host, ushort port) {
		actionsHttpClient = new() {
			BaseAddress = new Uri($"http://{host}:{port}/actions/"),
			Timeout = Timeout.InfiniteTimeSpan
		};

		healthHttpClient = new() {
			BaseAddress = new Uri($"http://{host}:{port}/health/"),
			Timeout = TimeSpan.FromSeconds(1)
		};
	}

	private HttpResponseMessage SendRequest(HttpRequestMessage request) {
		HttpResponseMessage response;
		try {
			using var requestTask = actionsHttpClient.SendAsync(request, cancellationTokenSource.Token);
			requestTask.Wait();
			response = requestTask.Result;
		} catch (TaskCanceledException) {
			throw new ObjectDisposedException(nameof(RemoteLinda));
		}

		if (response.StatusCode == HttpStatusCode.InternalServerError)
			throw new ObjectDisposedException(nameof(ILinda));

		return response;
	}

	private static object[] ReadTuple(HttpResponseMessage response) {
		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>(serializationOptions));
		decodingTask.Wait();
		return decodingTask.Result!;
	}

	private object[] WaitTuple(object?[] tuplePattern, bool delete) {
		var method = delete ? HttpMethod.Delete : HttpMethod.Get;
		var path = delete ? "in" : "rd";
		var request = new HttpRequestMessage(method, path) {
			Content = JsonContent.Create(tuplePattern, options: serializationOptions)
		};

		using var response = SendRequest(request);

		return ReadTuple(response);
	}

	private bool TryGetTuple(object?[] tuplePattern, bool delete, [MaybeNullWhen(false)] out object[] tuple) {
		var method = delete ? HttpMethod.Delete : HttpMethod.Get;
		var path = delete ? "inp" : "rdp";
		var request = new HttpRequestMessage(method, path) {
			Content = JsonContent.Create(tuplePattern, options: serializationOptions)
		};

		using var response = SendRequest(request);

		if (response.StatusCode == HttpStatusCode.NotFound) {
			tuple = null;
			return false;
		} else {
			tuple = ReadTuple(response);
			return true;
		}
	}

	public void Out(object[] tuple) {
		var request = new HttpRequestMessage(HttpMethod.Post, "out") {
			Content = JsonContent.Create(tuple, options: serializationOptions)
		};

		using var response = SendRequest(request);
		response.EnsureSuccessStatusCode();
	}

	public object[] In(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, true);
	}

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		return TryGetTuple(tuplePattern, true, out tuple);
	}

	public object[] Rd(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, false);
	}

	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		return TryGetTuple(tuplePattern, false, out tuple);
	}

	public void EvalRegister(string key, string ironpythonCode) {
		var url = $"eval/{key}";

		var request = new HttpRequestMessage(HttpMethod.Put, url) {
			Content = new StringContent(ironpythonCode, MediaTypeHeaderValue.Parse("text/ironpython"))
		};

		using var response = SendRequest(request);
		response.EnsureSuccessStatusCode();
	}

	public void EvalRegisterFile(string key, string ironpythonFilePath) {
		var content = File.ReadAllText(ironpythonFilePath);
		EvalRegister(key, content);
	}

	public void EvalInvoke(string key, object? parameter = null) {
		var url = $"eval/{key}";

		var request = new HttpRequestMessage(HttpMethod.Post, url) {
			Content = JsonContent.Create(parameter, options: serializationOptions)
		};

		using var response = SendRequest(request);
		response.EnsureSuccessStatusCode();
	}


	public void Eval(string ironpythonCode) {
		var request = new HttpRequestMessage(HttpMethod.Post, "eval") {
			Content = new StringContent(ironpythonCode, MediaTypeHeaderValue.Parse("text/ironpython"))
		};

		using var response = SendRequest(request);
		response.EnsureSuccessStatusCode();
	}

	public void EvalFile(string ironpythonFilePath) {
		var content = File.ReadAllText(ironpythonFilePath);
		Eval(content);
	}

	public void Dispose() {
		GC.SuppressFinalize(this);

		cancellationTokenSource.Cancel();
		cancellationTokenSource.Dispose();
		actionsHttpClient.Dispose();
	}

	public bool IsHealthy() {
		try {
			var requestTask = healthHttpClient.GetAsync("ping");
			requestTask.Wait();
			using var response = requestTask.Result;

			return response.IsSuccessStatusCode;
		} catch (Exception) {
			return false;
		}
	}
}
