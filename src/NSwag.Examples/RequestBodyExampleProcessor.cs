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

public class RequestBodyExampleProcessor : IOperationProcessor
{
    private const string MediaTypeName = "application/json";
    private readonly ILogger<RequestBodyExampleProcessor>? _logger;
    private readonly IServiceProvider _serviceProvider;
    private ExamplesConverter _examplesConverter;

    public RequestBodyExampleProcessor(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetRequiredService<ILogger<RequestBodyExampleProcessor>>();
        _examplesConverter = new ExamplesConverter(AspNetCoreOpenApiDocumentGenerator.GetJsonSerializerSettings(_serviceProvider), AspNetCoreOpenApiDocumentGenerator.GetSystemTextJsonSettings(_serviceProvider));
    }

    public bool Process(OperationProcessorContext context) {
        var exampleProvider = _serviceProvider.GetRequiredService<SwaggerExampleProvider>();
        SetRequestExamples(context, exampleProvider);
        SetResponseExamples(context, exampleProvider);

        return true;
    }

    private void SetRequestExamples(OperationProcessorContext context, SwaggerExampleProvider exampleProvider) {
        foreach (var apiParameter in context.OperationDescription.Operation.Parameters.Where(x => x.Kind == OpenApiParameterKind.Body && !x.IsBinaryBodyParameter)) {
            var parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
            if (!context.OperationDescription.Operation.RequestBody.Content.TryGetValue(MediaTypeName, out var mediaType))
                continue;

            var endpointSpecificExampleAttributes = context.MethodInfo.GetCustomAttributes<EndpointSpecificExampleAttribute>();
            SetExamples(GetExamples(exampleProvider, parameter.Key.ParameterType, endpointSpecificExampleAttributes
                .Where(x => x.ExampleType == ExampleType.Request || x.ExampleType == ExampleType.Both)
                .SelectMany(x => x.ExampleTypes), ExampleType.Request), mediaType);
        }
    }

    private void SetResponseExamples(OperationProcessorContext context, SwaggerExampleProvider exampleProvider) {
        foreach (var response in context.OperationDescription.Operation.Responses) {
            if (!int.TryParse(response.Key, out var responseStatusCode))
                continue;

            if (!response.Value.Content.TryGetValue(MediaTypeName, out var mediaType))
                continue;

            var attributesWithSameKey = GetAttributesWithSameStatusCode(context.MethodInfo, responseStatusCode);

            //get attributes from controller, in case when no attribute on action was found
            if (!attributesWithSameKey.Any())
                attributesWithSameKey = GetAttributesWithSameStatusCode(context.MethodInfo.DeclaringType, responseStatusCode);

            if (attributesWithSameKey.Count > 1)
                _logger.LogWarning($"Multiple {nameof(ProducesResponseTypeAttribute)} defined for method {context.MethodInfo.Name}, selecting first.");
            else if (attributesWithSameKey.Count == 0)
                continue;

            var endpointSpecificExampleAttributes = context.MethodInfo.GetCustomAttributes<EndpointSpecificExampleAttribute>();
            var valueType = attributesWithSameKey.FirstOrDefault()?.Type;
            SetExamples(GetExamples(exampleProvider, valueType, endpointSpecificExampleAttributes
                .Where(x => x.ExampleType == ExampleType.Response || x.ExampleType == ExampleType.Both)
                .Where(x => x.ResponseStatusCode != 0 && x.ResponseStatusCode == responseStatusCode)
                .SelectMany(x => x.ExampleTypes), ExampleType.Response), mediaType);
        }
    }

    private static void SetExamples(IDictionary<string, OpenApiExample> openApiExamples, OpenApiMediaType mediaType) {
        switch (openApiExamples) {
            case { Count: > 1 }:
            {
                foreach (var openApiExample in openApiExamples)
                    mediaType.Examples.Add(openApiExample.Key, openApiExample.Value);

                break;
            }
            case { Count: 1 }:
                mediaType.Example = openApiExamples.Single().Value.Value;
                break;
        }
    }

    private IDictionary<string, OpenApiExample> GetExamples(SwaggerExampleProvider exampleProvider, Type? valueType, IEnumerable<Type> exampleTypes, ExampleType exampleType) {
        var providerValues = exampleProvider.GetProviderValues(valueType, exampleTypes, exampleType);
        var openApiExamples = _examplesConverter.ToOpenApiExamplesDictionary(providerValues.Select((x, i) => new KeyValuePair<string, Tuple<object, string?>>(x.Key ?? $"Example {i + 1}", x.Value)));
        return openApiExamples;
    }

    private static List<ProducesResponseTypeAttribute> GetAttributesWithSameStatusCode(MemberInfo memberInfo, int responseStatusCode) {
        return memberInfo
            .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
            .Where(x => x.StatusCode == responseStatusCode)
            .ToList();
    }
}