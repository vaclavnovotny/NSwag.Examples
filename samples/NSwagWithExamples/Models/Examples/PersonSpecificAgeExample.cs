using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples;

[ExampleAnnotation(Name = "Specific number for age response")]
public class PersonSpecificAgeExample : IExampleProvider<int>
{
    public int GetExample() => 69;
}