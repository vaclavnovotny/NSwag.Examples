using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NSwag.Examples
{
    public static class ServiceExtensions
    {
        public static void AddExampleProviders(this IServiceCollection collection, params Assembly[] assemblies)
        {
            var typeMapper = new SwaggerExampleTypeMapper();
            foreach (var assembly in assemblies)
            {
                foreach (var providerType in assembly.GetTypes().Where(x => x.GetInterfaces().Any(i => i == typeof(IExampleProvider<>))))
                {
                    var valueType = providerType.GetTypeInfo()
                        .ImplementedInterfaces
                        .Single(x => x == typeof(IExampleProvider<>))
                        .GetTypeInfo()
                        .GenericTypeArguments
                        .Single();
                    typeMapper.Add(valueType, providerType);
                    collection.AddSingleton(providerType);
                }
            }

            collection.AddSingleton(typeMapper);
        }
    }
}