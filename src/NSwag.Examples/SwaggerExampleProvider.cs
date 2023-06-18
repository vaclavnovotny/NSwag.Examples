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

    internal IEnumerable<KeyValuePair<string?, object>> GetProviderValues(Type? valueType, Type? exampleType) {
        if (valueType == null)
            yield break;

        var providerTypes = exampleType is not null ? new[] { exampleType } : _mapper.GetProviderTypes(valueType);
        foreach (var providerType in providerTypes) {
            var providerServices = _serviceProvider.GetServices(providerType);
            var exampleAnnotationAttribute = providerType.GetCustomAttribute<ExampleAnnotationAttribute>();
            foreach (var example in providerServices.Select(x => ((dynamic)x).GetExample()))
                yield return new KeyValuePair<string?, object>(exampleAnnotationAttribute?.Name, example);
        }
    }
}