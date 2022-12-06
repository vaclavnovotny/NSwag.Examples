namespace NSwag.Examples
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NSwag;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ExamplesConverter
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public ExamplesConverter(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings;
        }

        public object SerializeExampleJson(object value)
        {
            var serialized = JsonConvert.SerializeObject(value, this.jsonSerializerSettings);
            return JToken.Parse(serialized);
        }

        public IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionary(
            IEnumerable<IOpenApiExample<object>> examples)
        {
            return ToOpenApiExamplesDictionary(examples, value =>
            {
                return this.SerializeExampleJson(value);
            });
        }

        private static IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionary(
            IEnumerable<IOpenApiExample<object>> examples,
            Func<object, object> exampleConverter)
        {
            return examples.ToDictionary(
                ex => ex.Name,
                ex => new OpenApiExample
                {
                    Value = exampleConverter(ex.Value),
                });
        }
    }
}
