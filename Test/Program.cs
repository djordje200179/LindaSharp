using LindaSharp;

var linda = new Linda();

linda.Out(new object[] { "mutex" });

linda.Out(new object[] { "a", 0UL });
linda.Out(new object[] { "b", 1UL });
linda.Out(new object[] { "iteration", 1 });

for (var i = 0; i < 8; i++) {
	var thread = new Thread(() => {
		while (true) {
			linda.In(new object?[] { "mutex" });

			var tupleA = linda.In(new object?[] { "a", null });
			var a = (ulong)tupleA[1];

			var tupleB = linda.In(new object?[] { "b", null });
			var b = (ulong)tupleB[1];

			var tupleIteration = linda.In(new object?[] { "iteration", null });
			var iteration = (int)tupleIteration[1];
			iteration++;

			var c = a + b;

			if (c < a)
				break;

			Console.WriteLine($"{iteration}:\t{c}");

			linda.Out(new object[] { "a", b });
			linda.Out(new object[] { "b", c });
			linda.Out(new object[] { "iteration", iteration });

			linda.Out(new object[] { "mutex" });
		}
	});

	thread.Start();
}