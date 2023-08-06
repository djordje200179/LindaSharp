using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LindaSharp.Server;

public class TupleJsonDeserializer : JsonConverter<object?> {
	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		switch (reader.TokenType) {
		case JsonTokenType.Null:
			return null;
		case JsonTokenType.False:
			return false;
		case JsonTokenType.True:
			return true;
		case JsonTokenType.String:
			return reader.GetString();
		case JsonTokenType.Number: {
			using var doc = JsonDocument.ParseValue(ref reader);

			if (BigInteger.TryParse(doc.RootElement.GetRawText(), out var integerNumber))
				return integerNumber;
			else if (double.TryParse(doc.RootElement.GetRawText(), out var realNumber))
				return realNumber;
			else
				throw new JsonException(string.Format("Cannot parse number {0}", doc.RootElement.ToString()));
		}
		case JsonTokenType.StartArray: {
			var list = new List<object?>();
			while (reader.Read()) {
				if (reader.TokenType == JsonTokenType.EndArray)
					return list.ToArray();

				list.Add(Read(ref reader, typeof(object), options));
			}

			throw new JsonException();
		}
		case JsonTokenType.StartObject:
			var dictionary = new ComparableDictionary();

			while (reader.Read()) {
				if (reader.TokenType == JsonTokenType.EndObject)
					return dictionary;
				else if (reader.TokenType == JsonTokenType.PropertyName) {
					var key = reader.GetString()!;
					reader.Read();

					var value = Read(ref reader, typeof(object), options);

					dictionary.Add(key, value);
				} else
					throw new JsonException();
			}

			throw new JsonException();
		default:
			throw new JsonException(string.Format("Unknown token {0}", reader.TokenType));
		}
	}

	public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options) {
		if (value is null) {
			JsonSerializer.Serialize(writer, value);
			return;
		}

		if (value is BigInteger bigInteger)
			writer.WriteRawValue(bigInteger.ToString());
		else if (value.GetType() == typeof(object)) {
			writer.WriteStartObject();
			writer.WriteEndObject();
		} else 
			JsonSerializer.Serialize(writer, value, value.GetType(), options);
	}
}
