using LindaSharp.Client;

var remoteClient = new RemoteLinda("localhost", 8080);

Thread.Sleep(1000);
remoteClient.Out(new object[] { "a", 0 });
remoteClient.Out(new object[] { "b", 1 });
remoteClient.Out(new object[] { "iteration", 1 });

var fibonachiCalculationCode = @"
from System import Array, Object

a_tuple = linda.In(Array[Object](['a', None]))
b_tuple = linda.In(Array[Object](['b', None]))
iteration_tuple = linda.In(Array[Object](['iteration', None]))

a = int(a_tuple[1])
b = int(b_tuple[1])
iteration = int(iteration_tuple[1])

iteration += 1

c = a + b

linda.Out(Array[Object](['a', b]))
linda.Out(Array[Object](['b', c]))

if iteration != 9:
	linda.Out(Array[Object](['iteration', iteration]))
else:
	linda.Out(Array[Object](['done']))
";

for (var i = 0; i < 8; i++)
	remoteClient.Eval(fibonachiCalculationCode);

remoteClient.In(new object?[] { "done" });

var resultTuple = remoteClient.In(new object?[] { "b", null });
var result = (long)resultTuple[1];

Console.WriteLine($"Fib[9]: {result}");

Console.ReadKey();