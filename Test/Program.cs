using LindaSharp;
using LindaSharp.Client;
using System.Numerics;

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
	var resultTuple = await remoteClient.Get("fib", new BigInteger(i), null);
	var result = (BigInteger)resultTuple[2];

	Console.WriteLine($"Fib[{i}] = {result}");
}

await remoteClient.Put("done");