using LindaSharp.Client;
using System.Numerics;

var remoteClient = new RemoteLinda("localhost", 8080);

while (true) {
	if (remoteClient.IsHealthy())
		break;

	Console.WriteLine("Waiting for server...");
	Thread.Sleep(1000);
}

remoteClient.Out(["a", 0]);
remoteClient.Out(["b", 1]);
remoteClient.Out(["ind", 1]);

remoteClient.EvalRegisterFile("fib-calc", "FibonachiCalculation.py");

for (var i = 0; i < 8; i++)
	remoteClient.EvalInvoke("fib-calc");

for (var i = 2; i <= 100; i++) {
	var resultTuple = remoteClient.In(["fib", new BigInteger(i), null]);
	var result = (BigInteger)resultTuple[2];

	Console.WriteLine($"Fib[{i}] = {result}");
}

remoteClient.Out(["done"]);

Console.ReadKey();