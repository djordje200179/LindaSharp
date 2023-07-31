using LindaSharp.Client;

var remoteClient = new RemoteLinda("localhost", 8080);

remoteClient.Out(new object[] { "mutex" });
remoteClient.Out(new object[] { "iteration", 1 });
remoteClient.Out(new object[] { "a", 0 });
remoteClient.Out(new object[] { "b", 1 });

for (var i = 0; i < 8; i++) {
	var thread = new Thread(() => {
		remoteClient.In(new object?[] { "mutex" });

		var iteration = (long)remoteClient.In(new object?[] { "iteration", null })[1]!;
		iteration++;

		var a = (long)remoteClient.In(new object?[] { "a", null })[1]!;
		var b = (long)remoteClient.In(new object?[] { "b", null })[1]!;

		var c = a + b;

		Console.WriteLine($"{iteration}: {c}");

		remoteClient.Out(new object[] { "a", b });
		remoteClient.Out(new object[] { "b", c });
		remoteClient.Out(new object[] { "iteration", iteration });
		remoteClient.Out(new object[] { "mutex" });
	});

	thread.Start();
}