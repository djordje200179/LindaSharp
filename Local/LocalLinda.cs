using System.Diagnostics.CodeAnalysis;

namespace LindaSharp;

public class LocalLinda : IActionEvalLinda {
	private volatile bool disposed = false;

	private readonly IList<object[]> tupleSpace = new List<object[]>();

	private class WaitingTuple {
		public object?[] TuplePattern { get; }
		public object[]? Tuple { get; set; } = null;
		public ManualResetEvent ConditionWaiter { get; } = new ManualResetEvent(false);

		public WaitingTuple(object?[] tuplePattern) {
			TuplePattern = tuplePattern;
		}
	}

	private readonly IList<WaitingTuple> inWaitingTuples = new List<WaitingTuple>();
	private readonly IList<WaitingTuple> rdWaitingTuples = new List<WaitingTuple>();

	private static bool IsTupleCompatible(object?[] tuplePattern, object[] tuple) {
		if (tuple.Length != tuplePattern.Length)
			return false;

		for (var i = 0; i < tuple.Length; i++)
			if (tuplePattern[i] is not null && !tuplePattern[i]!.Equals(tuple[i]))
				return false;

		return true;
	}

	private object[] WaitTuple(object?[] tuplePattern, bool removeFromSpace) {
		WaitingTuple? waitingTuple;
		lock (this) {
			if (TryGetTuple(tuplePattern, removeFromSpace, out var foundedTuple))
				return foundedTuple!;

			waitingTuple = new WaitingTuple(tuplePattern);

			(removeFromSpace ? inWaitingTuples : rdWaitingTuples).Add(waitingTuple);
		}

		waitingTuple.ConditionWaiter.WaitOne();
		if (disposed)
			throw new ObjectDisposedException(nameof(LocalLinda));
		waitingTuple.ConditionWaiter.Dispose();

		return waitingTuple.Tuple!;
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
			foreach (var waitingTuple in rdWaitingTuples.Reverse()) {
				if (!IsTupleCompatible(waitingTuple.TuplePattern, tuple))
					continue;

				waitingTuple.Tuple = (object[])tuple.Clone();
				rdWaitingTuples.Remove(waitingTuple);
				waitingTuple.ConditionWaiter.Set();

			}

			var tupleInputted = false;

			foreach (var waitingTuple in inWaitingTuples.Reverse()) {
				if (!IsTupleCompatible(waitingTuple.TuplePattern, tuple))
					continue;

				waitingTuple.Tuple = (object[])tuple.Clone();
				inWaitingTuples.Remove(waitingTuple);
				waitingTuple.ConditionWaiter.Set();

				tupleInputted = true;

				break;
			}

			if (!tupleInputted)
				tupleSpace.Add((object[])tuple.Clone());
		}
	}

	public object[] In(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, true);
	}

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		return TryGetTuple(tuplePattern, true, out tuple);
	}

	public object[] Rd(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, false);
	}

	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		return TryGetTuple(tuplePattern, false, out tuple);
	}

	public void Eval(Action<IActionEvalLinda> function) {
		new Thread(() => function(this)).Start();
	}

	public void Dispose() {
		if (disposed)
			return;

		disposed = true;
		GC.SuppressFinalize(this);

		foreach (var waitingTuple in inWaitingTuples)
			waitingTuple.ConditionWaiter.Set();
		foreach (var waitingTuple in rdWaitingTuples)
			waitingTuple.ConditionWaiter.Set();
	}
}