using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore;

namespace NSwag.Examples
{
    public static class ServiceExtensions
    {
        public static void AddExamples(this AspNetCoreOpenApiDocumentGeneratorSettings settings, IServiceProvider serviceProvider)
        {
            settings.OperationProcessors.Add(new RequestBodyExampleProcessor(serviceProvider));
        }

        public static void AddExampleProviders(this IServiceCollection collection, params Assembly[] assemblies)
        {
            var typeMapper = new SwaggerExampleTypeMapper();
            foreach (var assembly in assemblies)
            {
                foreach (var providerType in assembly.GetTypes().Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IExampleProvider<>))))
                {
                    var valueType = providerType.GetTypeInfo()
                        .ImplementedInterfaces
                        .Single(x => x.GetGenericTypeDefinition() == typeof(IExampleProvider<>))
                        .GetTypeInfo()
                        .GenericTypeArguments
                        .Single();
                    typeMapper.Add(valueType, providerType);
                    collection.AddSingleton(providerType);
                }
            }

            collection.AddSingleton(typeMapper);
            collection.AddSingleton<SwaggerExampleProvider>(provider => new SwaggerExampleProvider(provider.GetRequiredService<SwaggerExampleTypeMapper>(), provider));
        }
    }
}