using System;

namespace NSwag.Examples;

[AttributeUsage(AttributeTargets.Method)]
public class EndpointSpecificExampleAttribute : Attribute
{
    public Type ExampleType { get; }

    public EndpointSpecificExampleAttribute(Type exampleType) {
        ExampleType = exampleType;
    }
}