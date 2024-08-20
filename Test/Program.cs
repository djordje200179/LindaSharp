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

await remoteClient.RegisterScriptFile("fib-calc", "FibonachiCalculation.py");

var ids = new int[8];
for (var i = 0; i < ids.Length; i++)
	ids[i] = await remoteClient.InvokeScript("fib-calc");

for (var i = 2; i <= 70; i++) {
	var resultTuple = await remoteClient.Get("fib", i, null);
	Console.WriteLine($"Fib[{i}] = {resultTuple[2]:F0}");
}

await remoteClient.Put("done");

Console.WriteLine("Waiting for a second...");
Thread.Sleep(1000);

foreach (var id in ids) {
	var status = await remoteClient.GetScriptExecutionStatus(id);
	Console.WriteLine($"{id}: {status}");
}

Console.ReadKey();