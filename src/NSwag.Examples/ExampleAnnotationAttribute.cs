using System;

namespace NSwag.Examples;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ExampleAnnotationAttribute : Attribute
{
    public string? Name { get; set; }
}