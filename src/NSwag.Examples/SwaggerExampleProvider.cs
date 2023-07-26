using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NSwag.Examples;

internal class SwaggerExampleProvider
{
    private readonly SwaggerExampleTypeMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    internal SwaggerExampleProvider(SwaggerExampleTypeMapper mapper, IServiceProvider serviceProvider) {
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    internal IEnumerable<KeyValuePair<string?, Tuple<object, string?>>> GetProviderValues(Type? valueType, IEnumerable<Type> exampleTypes, ExampleType exampleType) {
        if (valueType == null)
            yield break;

        var providerTypes = exampleTypes.Any() ? exampleTypes : _mapper.GetProviderTypes(valueType);
        foreach (var providerType in providerTypes) {
            var providerServices = _serviceProvider.GetServices(providerType);
            var exampleAnnotationAttribute = providerType.GetCustomAttribute<ExampleAnnotationAttribute>();
            foreach (var example in providerServices.Select(x => ((dynamic)x).GetExample())) {
                if (exampleAnnotationAttribute == null || exampleAnnotationAttribute.ExampleType == exampleType || exampleAnnotationAttribute.ExampleType == ExampleType.Both) {
                    yield return new KeyValuePair<string?, Tuple<object, string?>>(exampleAnnotationAttribute?.Name, new (example, exampleAnnotationAttribute?.Description));
                }
            }
        }
    }
}