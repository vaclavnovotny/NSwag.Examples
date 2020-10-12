using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
            var exampleProvider = _serviceProvider.GetRequiredService<SwaggerExampleProvider>();
            foreach (var apiParameter in context.OperationDescription.Operation.Parameters.Where(x => x.Kind == OpenApiParameterKind.Body))
            {
                var parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
                apiParameter.ActualSchema.Example = exampleProvider.GetProviderValue(parameter.Key.ParameterType);
            }

            foreach (var response in context.OperationDescription.Operation.Responses.Where(x => x.Key == "200"))
            {
                var contextMethodInfo = context.MethodInfo.GetCustomAttribute<ProducesResponseTypeAttribute>();
                response.Value.Examples = exampleProvider.GetProviderValue(contextMethodInfo?.Type);
            }

            return true;
        }
    }
}