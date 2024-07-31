using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace LindaSharp;

public class LocalLinda : IActionEvalLinda, ISpaceViewLinda {
	private volatile bool disposed = false;

	private readonly IList<object[]> tupleSpace = [];

	private record WaitingTuple(object?[] TuplePattern, ChannelWriter<object[]> Sender);

	private readonly IList<WaitingTuple> inWaitingTuples = [];
	private readonly IList<WaitingTuple> rdWaitingTuples = [];

	private static bool IsTupleCompatible(object?[] tuplePattern, object[] tuple) {
		if (tuple.Length != tuplePattern.Length)
			return false;

		for (var i = 0; i < tuple.Length; i++) {
			if (tuplePattern[i] is not null && !tuplePattern[i].Equals(tuple[i]))
				return false;
		}

		return true;
	}

	private object[] WaitTuple(object?[] tuplePattern, bool removeFromSpace) {
		ChannelReader<object[]> receiver;
		lock (this) {
			if (TryGetTuple(tuplePattern, removeFromSpace, out var foundedTuple))
				return foundedTuple!;

			var channel = Channel.CreateBounded<object[]>(1);
			receiver = channel.Reader;

			(removeFromSpace ? inWaitingTuples : rdWaitingTuples).Add(new WaitingTuple(tuplePattern, channel.Writer));
		}

		var success = receiver.TryRead(out var tuple);
		ObjectDisposedException.ThrowIf(!success, this);

		return tuple!;
	}

	private bool TryGetTuple(object?[] tuplePattern, bool removeFromSpace, [MaybeNullWhen(false)] out object[] tuple) {
		lock (this) {
			tuple = tupleSpace.FirstOrDefault(tuple => IsTupleCompatible(tuplePattern, tuple));

			if (tuple is null)
				return false;

			if (removeFromSpace)
				tupleSpace.Remove(tuple);
		}

		return true;
	}

	public void Out(object[] tuple) {
		lock (this) {
			foreach (var waitingTuple in rdWaitingTuples.Reverse()) { // TODO: Check reverse
				if (!IsTupleCompatible(waitingTuple.TuplePattern, tuple))
					continue;

				waitingTuple.Sender.TryWrite((object[])tuple.Clone());
				rdWaitingTuples.Remove(waitingTuple);
			}

			var tupleInputted = false;
			foreach (var waitingTuple in inWaitingTuples.Reverse()) {
				if (!IsTupleCompatible(waitingTuple.TuplePattern, tuple))
					continue;

				waitingTuple.Sender.TryWrite((object[])tuple.Clone());
				inWaitingTuples.Remove(waitingTuple);
				tupleInputted = true;

				break;
			}

			if (!tupleInputted)
				tupleSpace.Add((object[])tuple.Clone());
		}
	}

	public object[] In(object?[] tuplePattern) => WaitTuple(tuplePattern, true);
	public object[] Rd(object?[] tuplePattern) => WaitTuple(tuplePattern, false);

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) => TryGetTuple(tuplePattern, true, out tuple);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) => TryGetTuple(tuplePattern, false, out tuple);

	public void Eval(Action<IActionEvalLinda> func) => new Thread(() => func(this)).Start();

	public void Dispose() {
		if (disposed)
			return;

		disposed = true;
		GC.SuppressFinalize(this);

		//foreach (var waitingTuple in inWaitingTuples)
		//	waitingTuple.ConditionWaiter.Set();
		//foreach (var waitingTuple in rdWaitingTuples)
		//	waitingTuple.ConditionWaiter.Set();
	}

	public IEnumerable<object[]> ReadAll() {
		return tupleSpace.Select(tuple => (object[])tuple.Clone()).ToList();
	}
}