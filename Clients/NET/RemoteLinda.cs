using System.Diagnostics.CodeAnalysis;

namespace LindaSharp.Client;

public class RemoteLinda : ILinda {
	private readonly string url;

	public RemoteLinda(string host, ushort port) {
		url = $"http://{host}:{port}";
	}

	public void Out(object[] tuple) {
		var client = new HttpClient();

		var responseTask = Task.Run(() => client.GetAsync(url));
		responseTask.Wait();
		using var response = responseTask.Result;
	}

	public object[] In(object?[] tuplePattern) {
		throw new NotImplementedException();
	}

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		throw new NotImplementedException();
	}

	public object[] Rd(object?[] tuplePattern) {
		throw new NotImplementedException();
	}

	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		throw new NotImplementedException();
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
