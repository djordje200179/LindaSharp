using LindaSharp;
using LindaSharp.Client;

using var remoteClient = new RemoteLinda("http://localhost:5001");

while (!await remoteClient.IsHealthy()) {
	Console.WriteLine("Waiting for server...");
	Thread.Sleep(1000);
}

await remoteClient.Put("a", 0);
await remoteClient.Put("b", 1);
await remoteClient.Put("ind", 1);

await ((IScriptEvalLinda)remoteClient).RegisterScriptFile("fib-calc", "FibonachiCalculation.py");

for (var i = 0; i < 8; i++)
	await remoteClient.InvokeScript("fib-calc");

for (var i = 2; i <= 100; i++) {
	var resultTuple = await remoteClient.Get("fib", i, null);
	Console.WriteLine($"Fib[{i}] = {resultTuple[2]}");
}

await remoteClient.Put("done");