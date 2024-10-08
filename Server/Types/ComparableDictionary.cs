﻿using System.Text;

namespace LindaSharp.Server.Types;

internal class ComparableDictionary : Dictionary<string, object?>, IEquatable<ComparableDictionary> {
	public override bool Equals(object? other) {
		return other is ComparableDictionary otherDict && Equals(otherDict);
	}

	public override int GetHashCode() => base.GetHashCode();

	public bool Equals(ComparableDictionary? other) {
		if (other is null)
			return false;

		if (Count != other.Count)
			return false;

		foreach (var (key, firstValue) in this) {
			if (!other.TryGetValue(key, out var secondValue))
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

	public override string ToString() {
		var sb = new StringBuilder();
		sb.Append('{');

		var first = true;
		foreach (var (key, value) in this) {
			if (!first)
				sb.Append(", ");

			sb.Append(key);
			sb.Append(':');
			sb.Append(value);

			first = false;
		}

		sb.Append('}');

		return sb.ToString();
	}
}