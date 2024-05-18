using System;
using System.Collections.Generic;
using System.Linq;

namespace NSwag.Examples;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class EndpointSpecificExampleAttribute : Attribute
{
    public IEnumerable<Type> ExampleTypes { get; }

    public int ResponseStatusCode { get; set; }

    public ExampleType ExampleType { get; set; } = ExampleType.Both;

    /// <summary>
    /// A parameter name is required for a parameter kind other than body.<br />
    /// </summary>
    public string? ParameterName { get; set; }

    public EndpointSpecificExampleAttribute(Type exampleType, params Type[] additionalExampleTypes)
    {
        if (exampleType.GetInterfaces().All(i => i.IsGenericType && i.GetGenericTypeDefinition() != typeof(IExampleProvider<>)))
            throw new InvalidCastException($"Type {exampleType} does not implement {typeof(IExampleProvider<>)}.");

        foreach (var additionalExampleType in additionalExampleTypes)
        {
            if (additionalExampleType.GetInterfaces().All(i => i.IsGenericType && i.GetGenericTypeDefinition() != typeof(IExampleProvider<>)))
                throw new InvalidCastException($"Type {additionalExampleType} does not implement {typeof(IExampleProvider<>)}.");
        }

        ExampleTypes = additionalExampleTypes.Prepend(exampleType);
    }
}