using System.Threading.Channels;

namespace LindaSharp;

public class LocalLinda : IActionEvalLinda, ISpaceViewLinda {
	private readonly IList<object[]> tupleSpace = [];

	private record WaitingTuple(object?[] Pattern, ChannelWriter<object[]> Sender);

	private readonly IList<WaitingTuple> inWaitingTuples = [];
	private readonly IList<WaitingTuple> rdWaitingTuples = [];

	private static bool IsTupleCompatible(object?[] pattern, object[] tuple) {
		if (tuple.Length != pattern.Length)
			return false;

		for (var i = 0; i < tuple.Length; i++) {
			if (pattern[i] is object field && !field.Equals(tuple[i]))
				return false;
		}

		return true;
	}

	private async Task<object[]> WaitTuple(object?[] tuplePattern, bool removeFromSpace) {
		ChannelReader<object[]> receiver;
		lock (this) {
			if (TryGetTuple(tuplePattern, removeFromSpace) is object[] existingTuple)
				return existingTuple;

			var channel = Channel.CreateBounded<object[]>(new BoundedChannelOptions(1) {
				SingleReader = true,
				SingleWriter = true,
			});
			receiver = channel.Reader;

			(removeFromSpace ? inWaitingTuples : rdWaitingTuples).Add(new WaitingTuple(tuplePattern, channel.Writer));
		}

		return await receiver.ReadAsync();
	}

	private object[]? TryGetTuple(object?[] tuplePattern, bool removeFromSpace) {
		lock (this) {
			var tuple = tupleSpace.FirstOrDefault(tuple => IsTupleCompatible(tuplePattern, tuple));
			if (tuple is null)
				return null;

			if (removeFromSpace)
				tupleSpace.Remove(tuple);

			return tuple;
		}
	}

	public async Task Out(object[] tuple) {
		var senders = new List<ChannelWriter<object[]>>();
		lock (this) {
			foreach (var waitingTuple in rdWaitingTuples.Reverse()) { // TODO: Check reverse
				if (!IsTupleCompatible(waitingTuple.Pattern, tuple))
					continue;

				senders.Add(waitingTuple.Sender);
				rdWaitingTuples.Remove(waitingTuple);
			}

			var tupleInputted = false;
			foreach (var waitingTuple in inWaitingTuples.Reverse()) {
				if (!IsTupleCompatible(waitingTuple.Pattern, tuple))
					continue;

				senders.Add(waitingTuple.Sender);
				inWaitingTuples.Remove(waitingTuple);
				tupleInputted = true;

				break;
			}

			if (!tupleInputted)
				tupleSpace.Add((object[])tuple.Clone());
		}

		foreach (var sender in senders)
			await sender.WriteAsync((object[])tuple.Clone());
	}

	public Task<object[]> In(object?[] tuplePattern) => WaitTuple(tuplePattern, true);
	public Task<object[]> Rd(object?[] tuplePattern) => WaitTuple(tuplePattern, false);

	public async Task<object[]?> Inp(object?[] tuplePattern) => TryGetTuple(tuplePattern, true);
	public async Task<object[]?> Rdp(object?[] tuplePattern) => TryGetTuple(tuplePattern, false);

	public void Eval(Action<IActionEvalLinda> func) => new Thread(() => func(this)).Start();

	public async Task<IEnumerable<object[]>> ReadAll() {
		lock (this) {
			return tupleSpace.Select(tuple => (object[])tuple.Clone());
		}
	}
}