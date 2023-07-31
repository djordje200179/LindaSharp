namespace LindaSharp.Remote.Server;

public class ComparableDictionary : Dictionary<string, object?> {
	public override bool Equals(object? other) {
		if (other is not ComparableDictionary)
			return false;

		var secondObject = (ComparableDictionary)other;

		if (Count != secondObject.Count)
			return false;

		foreach (var (key, firstValue) in this) {
			if (!secondObject.TryGetValue(key, out var secondValue))
				return false;

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

	public override int GetHashCode() {
		return base.GetHashCode();
	}
}