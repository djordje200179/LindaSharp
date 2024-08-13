using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace LindaSharp;

public class LocalLinda : IActionEvalLinda, ISpaceViewLinda {
	private readonly List<object[]> tupleSpace = [];

	private record WaitingTuple(object?[] Pattern, ChannelWriter<object[]> Sender);

	private readonly List<WaitingTuple> inWaitingTuples = [], rdWaitingTuples = [];

	private static bool IsTupleCompatible(object?[] pattern, object[] tuple) {
		if (tuple.Length != pattern.Length)
			return false;

		for (var i = 0; i < tuple.Length; i++) {
			if (pattern[i] is object field && !field.Equals(tuple[i]))
				return false;
		}

		return true;
	}

	private async Task<object[]> WaitTuple(object?[] pattern, bool removeFromSpace) {
		if (TryGetTuple(pattern, removeFromSpace, out var existingTuple))
			return existingTuple;

		ChannelReader<object[]> receiver;
		lock (this) {
			var channel = Channel.CreateBounded<object[]>(new BoundedChannelOptions(1) {
				SingleReader = true,
				SingleWriter = true,
			});
			receiver = channel.Reader;

			(removeFromSpace ? inWaitingTuples : rdWaitingTuples).Add(new WaitingTuple(pattern, channel.Writer));
		}

		return await receiver.ReadAsync();
	}

	private bool TryGetTuple(object?[] pattern, bool removeFromSpace, [NotNullWhen(true)] out object[]? tuple) {
		lock (this) {
			tuple = tupleSpace.FirstOrDefault(tuple => IsTupleCompatible(pattern, tuple));
			if (tuple is null)
				return false;

			if (removeFromSpace)
				tupleSpace.Remove(tuple);

			return true;
		}
	}

	public async Task Put(params object[] tuple) {
		var senders = new List<ChannelWriter<object[]>>();
		lock (this) {
			for (var i = rdWaitingTuples.Count - 1; i >= 0; i--) {
				if (!IsTupleCompatible(rdWaitingTuples[i].Pattern, tuple))
					continue;

				senders.Add(rdWaitingTuples[i].Sender);
				rdWaitingTuples.RemoveAt(i);
			}

			var tupleInputted = false;
			for (var i = inWaitingTuples.Count - 1; i >= 0; i--) {
				if (IsTupleCompatible(inWaitingTuples[i].Pattern, tuple)) {
					senders.Add(inWaitingTuples[i].Sender);
					inWaitingTuples.RemoveAt(i);
					tupleInputted = true;

					break;
				}
			}

			if (!tupleInputted)
				tupleSpace.Add((object[])tuple.Clone());
		}

		foreach (var sender in senders)
			await sender.WriteAsync((object[])tuple.Clone());
	}

	public Task<object[]> Get(params object?[] pattern) => WaitTuple(pattern, true);
	public Task<object[]> Query(params object?[] pattern) => WaitTuple(pattern, false);

	public Task<object[]?> TryGet(params object?[] pattern) {
		TryGetTuple(pattern, true, out var tuple);
		return Task.FromResult(tuple);
	}

	public Task<object[]?> TryQuery(params object?[] pattern) {
		TryGetTuple(pattern, false, out var tuple);
		return Task.FromResult(tuple);
	}

	public void Eval(Action<IActionEvalLinda> func) => new Thread(() => func(this)).Start();

	public Task<IEnumerable<object[]>> QueryAll() {
		List<object[]> tuples;
		lock (this) {
			tuples = tupleSpace.Select(tuple => (object[])tuple.Clone()).ToList();
		}

		return Task.FromResult(tuples.AsEnumerable());
	}
}