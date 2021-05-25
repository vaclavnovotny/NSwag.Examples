using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Examples
{
    public class RequestBodyExampleProcessor : IOperationProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestBodyExampleProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool Process(OperationProcessorContext context)
        {
            var serializer = JsonSerializer.Create(context.Settings.SerializerSettings);
            var logger = _serviceProvider.GetRequiredService<ILogger<RequestBodyExampleProcessor>>();
            var exampleProvider = _serviceProvider.GetRequiredService<SwaggerExampleProvider>();
            foreach (var apiParameter in context.OperationDescription.Operation.Parameters.Where(x => x.Kind == OpenApiParameterKind.Body))
            {
                var parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
                apiParameter.ActualSchema.Example = new OpenApiExample {Value = GetJObject(exampleProvider.GetProviderValue(parameter.Key.ParameterType), serializer)};
            }

            foreach (var response in context.OperationDescription.Operation.Responses)
            {
                if (!int.TryParse(response.Key, out int responseStatusCode))
                {
                    continue;
                }

                var attributesWithSameKey = GetAttributesWithSameStatusCode(context.MethodInfo, responseStatusCode);

                if (!attributesWithSameKey.Any())
                {
                    //get attributes from controller, in case when no attribute on action was found
                    attributesWithSameKey = GetAttributesWithSameStatusCode(context.MethodInfo.DeclaringType, responseStatusCode);
                }

                if (attributesWithSameKey.Count > 1)
                    logger.LogWarning($"Multiple {nameof(ProducesResponseTypeAttribute)} defined for method {context.MethodInfo.Name}, selecting first.");
                
                response.Value.Examples = new List<OpenApiExample> {new OpenApiExample {Value = GetJObject(exampleProvider.GetProviderValue(attributesWithSameKey.FirstOrDefault()?.Type), serializer)}};
            }

            return true;
        }

        private static object GetJObject(object value, JsonSerializer serializer) {
            if (value == null)
                return null;

            return value is IEnumerable ? (object) JArray.FromObject(value, serializer) : JObject.FromObject(value, serializer);
        }

        private List<ProducesResponseTypeAttribute> GetAttributesWithSameStatusCode(MemberInfo memberInfo, int responseStatusCode)
        {
            return memberInfo
                    .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                    .Where(x => x.StatusCode == responseStatusCode)
                    .ToList();
        }
    }
}