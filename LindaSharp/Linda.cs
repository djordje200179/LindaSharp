namespace LindaSharp;

public interface ILinda {
	public void Out(object[] tuple);

	public object[] In(object?[] tuplePattern);
	public bool Inp(object?[] tuplePattern, out object[]? tuple);

	public object[] Rd(object?[] tuplePattern);
	public bool Rdp(object?[] tuplePattern, out object[]? tuple);
}

public class Linda : ILinda {
    private readonly IList<object[]> tupleSpace = new List<object[]>();
	
	private class WaitingTuple {
		public object?[] TuplePattern { get; }
		public object[]? Tuple { get; set; } = null;

		public WaitingTuple(object?[] tuplePattern) {
			TuplePattern = tuplePattern;
		}
	}

	private readonly IList<WaitingTuple> waitingTuples = new List<WaitingTuple>();

	private object[]? Find(object?[] tuplePattern) {
		return tupleSpace.FirstOrDefault(tuple => IsTupleCompatible(tuplePattern, tuple));
	}

	private static bool IsTupleCompatible(object?[] tuplePattern, object[] tuple) {
		if (tuple.Length != tuplePattern.Length)
			return false;

		for (var i = 0; i < tuple.Length; i++)
			if (tuplePattern[i] is not null && !tuplePattern[i]!.Equals(tuple[i]))
				return false;

		return true;
	}

	private object[] WaitTuple(object?[] tuplePattern, bool removeFromSpace) {
		object[]? foundedTuple;
		lock (this) {
			foundedTuple = Find(tuplePattern);

			if (foundedTuple is null) {
				var waitingTuple = new WaitingTuple(tuplePattern);
				waitingTuples.Add(waitingTuple);

				while (waitingTuple.Tuple is null)
					Monitor.Wait(this);

				foundedTuple = waitingTuple.Tuple;
			}

			if (removeFromSpace)
				tupleSpace.Remove(foundedTuple);
		}

		return foundedTuple;
	}

	private bool TryGetTuple(object?[] tuplePattern, bool removeFromSpace, out object[]? tuple) {
		lock (this) {
			tuple = Find(tuplePattern);

			if (tuple is null)
				return false;

			if (removeFromSpace)
				tupleSpace.Remove(tuple);
		}

		return true;
	}

	public void Out(object[] tuple) {
		lock (this) {
			var clonedTuple = (object[])tuple.Clone();

			tupleSpace.Add(clonedTuple);

			foreach (var waitingTuple in waitingTuples) {
				if (IsTupleCompatible(waitingTuple.TuplePattern, clonedTuple)) {
					waitingTuple.Tuple = clonedTuple;
					waitingTuples.Remove(waitingTuple);

					Monitor.PulseAll(this);

					break;
				}
			}
		}
	}

	public object[] In(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, true);
	}

	public bool Inp(object?[] tuplePattern, out object[]? tuple) {
		return TryGetTuple(tuplePattern, true, out tuple);
	}

	public object[] Rd(object?[] tuplePattern) {
		return WaitTuple(tuplePattern, false);
	}

	public bool Rdp(object?[] tuplePattern, out object[]? tuple) {
		return TryGetTuple(tuplePattern, false, out tuple);
	}
}