using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Examples;

public class RequestExampleProcessor : IOperationProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ExamplesConverter _examplesConverter;

    public RequestExampleProcessor(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
        _examplesConverter = new ExamplesConverter(AspNetCoreOpenApiDocumentGenerator.GetJsonSerializerSettings(_serviceProvider), AspNetCoreOpenApiDocumentGenerator.GetSystemTextJsonSettings(_serviceProvider));
    }

    public bool Process(OperationProcessorContext context) {
        var exampleProvider = _serviceProvider.GetRequiredService<SwaggerExampleProvider>();
        SetRequestExamples(context, exampleProvider);

        return true;
    }

    private void SetRequestExamples(OperationProcessorContext context, SwaggerExampleProvider exampleProvider) {
        var endpointSpecificExampleAttributes = context.MethodInfo.GetCustomAttributes<EndpointSpecificExampleAttribute>().ToList();
        foreach (var apiParameter in context.OperationDescription.Operation.Parameters.Where(x => x.Kind != OpenApiParameterKind.Body)) {
            var exampleTypes = endpointSpecificExampleAttributes
                .Where(
                    x =>
                        (x.ExampleType == ExampleType.Request || x.ExampleType == ExampleType.Both)
                        &&
                        (x.ParameterName == null || x.ParameterName == apiParameter.Name))
                .SelectMany(x => x.ExampleTypes)
                .ToList();

            if (!exampleTypes.Any())
                continue;

            KeyValuePair<ParameterInfo, OpenApiParameter>? parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
            if (parameter == null)
                continue;

            SetExamples(
                parameter.Value, 
                GetExamples(exampleProvider, parameter.Value.Key.ParameterType, exampleTypes, ExampleType.Request)
            );
        }
    }

    private static void SetExamples( KeyValuePair<ParameterInfo, OpenApiParameter> par, IDictionary<string, OpenApiExample> openApiExamples)
    {
        switch (openApiExamples)
        {
            case { Count: > 1 }:
                {
                    par.Value.Examples = new Dictionary<string, OpenApiExample>();
                    par.Value.Examples.Add(new KeyValuePair<string, OpenApiExample>("-", new OpenApiExample { Value = null }));
                    foreach (var openApiExample in openApiExamples)
                        par.Value.Examples.Add(openApiExample.Key, openApiExample.Value);

                    break;
                }
            case { Count: 1 }:
                par.Value.Example = openApiExamples.First().Value.Value;
                break;
        }
    }

    private IDictionary<string, OpenApiExample> GetExamples(SwaggerExampleProvider exampleProvider, Type? valueType, IEnumerable<Type> exampleTypes, ExampleType exampleType) {
        var providerValues = exampleProvider.GetProviderValues(valueType, exampleTypes, exampleType);
        var openApiExamples = _examplesConverter.ToOpenApiExamplesDictionary(providerValues.Select((x, i) => new KeyValuePair<string, Tuple<object, string?>>(x.Key ?? $"Example {i + 1}", x.Value)));
        return openApiExamples;
    }
}