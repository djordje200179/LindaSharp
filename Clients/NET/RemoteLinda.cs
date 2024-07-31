using LindaSharp.Server;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LindaSharp.Client;

public class RemoteLinda(string host, ushort port) : ILinda {
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

	private HttpResponseMessage SendRequest(HttpRequestMessage request) {
		HttpResponseMessage response;
		try {
			using var requestTask = actionsHttpClient.SendAsync(request, cancellationTokenSource.Token);
			requestTask.Wait();
			response = requestTask.Result;
		} catch (TaskCanceledException) {
			throw new ObjectDisposedException(nameof(RemoteLinda));
		}

		ObjectDisposedException.ThrowIf(response.StatusCode == HttpStatusCode.InternalServerError, this);

		return response;
	}

	private static object[] ReadTuple(HttpResponseMessage response) {
		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>(serializationOptions));
		decodingTask.Wait();
		return decodingTask.Result!;
	}

	private object[] WaitTuple(object?[] tuplePattern, bool delete) {
		var request = new HttpRequestMessage(
			delete ? HttpMethod.Delete : HttpMethod.Get,
			delete ? "in" : "rd"
		) {
			Content = JsonContent.Create(tuplePattern, options: serializationOptions)
		};

		using var response = SendRequest(request);

		return ReadTuple(response);
	}

	private bool TryGetTuple(object?[] tuplePattern, bool delete, [MaybeNullWhen(false)] out object[] tuple) {
		var request = new HttpRequestMessage(
			delete ? HttpMethod.Delete : HttpMethod.Get, 
			delete ? "inp" : "rdp"
		) {
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

	public object[] In(object?[] tuplePattern) => WaitTuple(tuplePattern, true);
	public object[] Rd(object?[] tuplePattern) => WaitTuple(tuplePattern, false);


	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) => TryGetTuple(tuplePattern, true, out tuple);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) => TryGetTuple(tuplePattern, false, out tuple);

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
