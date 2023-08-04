using LindaSharp.Server;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LindaSharp.Client;

public class RemoteLinda : ILinda {
	private readonly HttpClient httpClient;
	private readonly CancellationTokenSource cancellationTokenSource = new();

	public RemoteLinda(string host, ushort port) {
		httpClient = new() {
			BaseAddress = new Uri($"http://{host}:{port}"),
			Timeout = Timeout.InfiniteTimeSpan
		};
	}

	private HttpResponseMessage SendRequest(HttpRequestMessage request) {
		HttpResponseMessage response;
		try {
			using var requestTask = httpClient.SendAsync(request, cancellationTokenSource.Token);
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
		var deserializationOptions = new JsonSerializerOptions {
			Converters = {
				new TupleJsonDeserializer()
			},
			WriteIndented = true
		};

		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>(deserializationOptions));
		decodingTask.Wait();
		return decodingTask.Result!;
	}

	private object[] WaitTuple(object?[] tuplePattern, string method) {
		var request = new HttpRequestMessage(HttpMethod.Get, method) {
			Content = JsonContent.Create(tuplePattern)
		};

		using var response = SendRequest(request);

		return ReadTuple(response);
	}

	private bool TryGetTuple(object?[] tuplePattern, string method, [MaybeNullWhen(false)] out object[] tuple) {
		var request = new HttpRequestMessage(HttpMethod.Get, method) {
			Content = JsonContent.Create(tuplePattern)
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
			Content = JsonContent.Create(tuple)
		};

		using var response = SendRequest(request);
	}

	public object[] In(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, "in");
	}

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		return TryGetTuple(tuplePattern, "inp", out tuple);
	}

	public object[] Rd(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, "rd");
	}

	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		return TryGetTuple(tuplePattern, "rdp", out tuple);
	}

	public void Eval(Action<ILinda> function) {
		throw new NotImplementedException();
	}

	public void Eval(string pythonCode) {
		var request = new HttpRequestMessage(HttpMethod.Post, "eval") {
			Content = new StringContent(pythonCode, MediaTypeHeaderValue.Parse("text/ironpython"))
		};

		using var response = SendRequest(request);
	}

	public void EvalFile(string pythonCodePath) {
		var fileContent = File.ReadAllText(pythonCodePath);

		Eval(fileContent);
	}

	public void Dispose() {
		GC.SuppressFinalize(this);

		cancellationTokenSource.Cancel();
		cancellationTokenSource.Dispose();
		httpClient.Dispose();
	}
}
