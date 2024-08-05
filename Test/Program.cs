using LindaSharp;
using LindaSharp.Client;
using System.Numerics;

using var remoteClient = new RemoteLinda("localhost", 8080);

while (true) {
	if (await remoteClient.IsHealthy())
		break;

	Console.WriteLine("Waiting for server...");
	Thread.Sleep(1000);
}

await remoteClient.Out(["a", 0]);
await remoteClient.Out(["b", 1]);
await remoteClient.Out(["ind", 1]);

await ((IScriptEvalLinda)remoteClient).EvalRegisterFile("fib-calc", "FibonachiCalculation.py");

for (var i = 0; i < 8; i++)
	await remoteClient.EvalInvoke("fib-calc");

for (var i = 2; i <= 100; i++) {
	var resultTuple = await remoteClient.In(["fib", new BigInteger(i), null]);
	var result = (BigInteger)resultTuple[2];

	Console.WriteLine($"Fib[{i}] = {result}");
}

await remoteClient.Out(["done"]);

Console.ReadKey();