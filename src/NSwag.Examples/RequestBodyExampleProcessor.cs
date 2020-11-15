using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            var logger = _serviceProvider.GetRequiredService<ILogger<RequestBodyExampleProcessor>>();
            var exampleProvider = _serviceProvider.GetRequiredService<SwaggerExampleProvider>();
            foreach (var apiParameter in context.OperationDescription.Operation.Parameters.Where(x => x.Kind == OpenApiParameterKind.Body))
            {
                var parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
                apiParameter.ActualSchema.Example = exampleProvider.GetProviderValue(parameter.Key.ParameterType);
            }

            foreach (var response in context.OperationDescription.Operation.Responses)
            {
                var attributesWithSameKey = context.MethodInfo
                    .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                    .Where(x => x.StatusCode.ToString() == response.Key)
                    .ToList();
                if (attributesWithSameKey.Count > 1)
                    logger.LogWarning($"Multiple {nameof(ProducesResponseTypeAttribute)} defined for method {context.MethodInfo.Name}, selecting first.");
                response.Value.Examples = exampleProvider.GetProviderValue(attributesWithSameKey.FirstOrDefault()?.Type);
            }

            return true;
        }
    }
}