using LindaSharp.Client;

var remoteClient = new RemoteLinda("localhost", 8080);

Thread.Sleep(1000);

remoteClient.Out(new object[] { "a", 0 });
remoteClient.Out(new object[] { "b", 1 });
remoteClient.Out(new object[] { "iteration", 1 });

remoteClient.EvalRegisterFile("fib-calc", "FibonachiCalculation.pyi");

for (var i = 0; i < 8; i++)
	remoteClient.EvalInvoke("fib-calc");

remoteClient.In(new object?[] { "done" });

var resultTuple = remoteClient.In(new object?[] { "b", null });
var result = (long)resultTuple[1];

Console.WriteLine($"Fib[9]: {result}");

Console.ReadKey();