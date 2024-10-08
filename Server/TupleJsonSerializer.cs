﻿using LindaSharp.Server.Types;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LindaSharp.Server;

internal class TupleJsonSerializer : JsonConverter<object?> {
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
				throw new JsonException($"Cannot parse {doc.RootElement} as number");
		}
		case JsonTokenType.StartArray: {
			var list = new ComparableList();
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
				if (reader.TokenType == JsonTokenType.EndObject) {
					return dictionary;
				} else if (reader.TokenType == JsonTokenType.PropertyName) {
					var key = reader.GetString()!;
					reader.Read();

					var value = Read(ref reader, typeof(object), options);

					dictionary.Add(key, value);
				} else {
					throw new JsonException();
				}
			}

			throw new JsonException();
		default:
			throw new JsonException($"Unknown token: {reader.TokenType}");
		}
	}

	public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options) {
		if (value is null) {
			JsonSerializer.Serialize(writer, value);
			return;
		}

		if (value is BigInteger bigInteger) {
			writer.WriteRawValue(bigInteger.ToString());
		} else if (value.GetType() == typeof(object)) {
			writer.WriteStartObject();
			writer.WriteEndObject();
		} else {
			JsonSerializer.Serialize(writer, value, value.GetType(), options);
		}
	}
}
