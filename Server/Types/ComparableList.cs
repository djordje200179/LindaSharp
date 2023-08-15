namespace LindaSharp.Server.Types;

internal class ComparableList: List<object?>, IEquatable<ComparableList> {
	public override bool Equals(object? other) {
		return other is ComparableList otherList && Equals(otherList);
	}

	public bool Equals(ComparableList? other) {
		if (other is null)
			return false;

		if (Count != other.Count)
			return false;

		for (var i = 0; i < Count; i++) {
			var firstValue = this[i];
			var secondValue = other[i];

			if (firstValue is null) {
				if (secondValue is null)
					continue;
				else
					return false;
			}

			if (!firstValue.Equals(secondValue))
				return false;
		}

		return true;
	}
}
