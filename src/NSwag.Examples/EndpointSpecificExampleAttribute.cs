using System;
using System.Linq;

namespace NSwag.Examples;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointSpecificExampleAttribute : Attribute
{
    public Type ExampleType { get; }

    public EndpointSpecificExampleAttribute(Type exampleType) {
        if (exampleType.GetInterfaces().All(i => i.IsGenericType && i.GetGenericTypeDefinition() != typeof(IExampleProvider<>)))
            throw new InvalidCastException($"Type {exampleType} does not implement {typeof(IExampleProvider<>)}.");

        ExampleType = exampleType;
    }
}