using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;

namespace LindaSharp.Client;

public class RemoteLinda : ILinda {
	private readonly Uri url;

	private HttpClient CreateClient() {
		return new() {
			BaseAddress = url
		};
	}

	public RemoteLinda(string host, ushort port) {
		url = new Uri($"http://{host}:{port}");
	}

	public void Out(object[] tuple) {
		var client = CreateClient();

		var request = new HttpRequestMessage(HttpMethod.Post, "out") {
			Content = JsonContent.Create(tuple)
		};

		using var response = client.Send(request);

		if (response.StatusCode == HttpStatusCode.InternalServerError)
			throw new ObjectDisposedException(nameof(ILinda));
	}

	public object[] In(object?[] tuplePattern) {
		var client = CreateClient();

		var request = new HttpRequestMessage(HttpMethod.Get, "in") {
			Content = JsonContent.Create(tuplePattern)
		};

		using var response = client.Send(request);

		if (response.StatusCode == HttpStatusCode.InternalServerError)
			throw new ObjectDisposedException(nameof(ILinda));

		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>());
		decodingTask.Wait();
		return decodingTask.Result!;
	}

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		var client = CreateClient();

		var request = new HttpRequestMessage(HttpMethod.Get, "inp") {
			Content = JsonContent.Create(tuplePattern)
		};

		using var response = client.Send(request);

		if (response.StatusCode == HttpStatusCode.InternalServerError)
			throw new ObjectDisposedException(nameof(ILinda));
		else if (response.StatusCode == HttpStatusCode.NotFound) {
			tuple = null;
			return false;
		}

		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>());
		decodingTask.Wait();
		tuple = decodingTask.Result!;
		return true;
	}

	public object[] Rd(object?[] tuplePattern) {
		var client = CreateClient();

		var request = new HttpRequestMessage(HttpMethod.Get, "rd") {
			Content = JsonContent.Create(tuplePattern)
		};

		using var response = client.Send(request);

		if (response.StatusCode == HttpStatusCode.InternalServerError)
			throw new ObjectDisposedException(nameof(ILinda));

		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>());
		decodingTask.Wait();
		return decodingTask.Result!;
	}

	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		var client = CreateClient();

		var request = new HttpRequestMessage(HttpMethod.Get, "rdp") {
			Content = JsonContent.Create(tuplePattern)
		};

		using var response = client.Send(request);

		if (response.StatusCode == HttpStatusCode.InternalServerError)
			throw new ObjectDisposedException(nameof(ILinda));
		else if (response.StatusCode == HttpStatusCode.NotFound) {
			tuple = null;
			return false;
		}

		var decodingTask = Task.Run(() => response.Content.ReadFromJsonAsync<object[]>());
		decodingTask.Wait();
		tuple = decodingTask.Result!;
		return true;
	}

	public void Eval(Action<ILinda> function) {
		throw new NotImplementedException();
	}

	public void Eval(Action<ILinda, object> function, object parameter) {
		throw new NotImplementedException();
	}

	public void Dispose() {
		throw new NotImplementedException();
	}
}
