using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NSwag.Examples;

internal class ExamplesConverter
{
    private readonly JsonSerializerSettings? _jsonSerializerSettings;
    private readonly JsonSerializerOptions? _systemTextJsonSettings;

    internal ExamplesConverter(JsonSerializerSettings? jsonSerializerSettings, JsonSerializerOptions? systemTextJsonSettings) {
        _jsonSerializerSettings = jsonSerializerSettings;
        _systemTextJsonSettings = systemTextJsonSettings;
    }

    private object SerializeExampleJson(object value) {
        var serializeObject = _jsonSerializerSettings is not null ? JsonConvert.SerializeObject(value, _jsonSerializerSettings) : JsonSerializer.Serialize(value, _systemTextJsonSettings);
        return JToken.Parse(serializeObject);
    }

    internal IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionary(IEnumerable<KeyValuePair<string, Tuple<object, string?>>> examples) {
        return examples.ToDictionary(
            example => example.Key,
            example => new OpenApiExample { Value = SerializeExampleJson(example.Value.Item1), Description = example.Value.Item2 });
    }
}