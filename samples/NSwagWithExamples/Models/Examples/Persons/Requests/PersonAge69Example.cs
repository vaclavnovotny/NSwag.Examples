using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Age 69", ExampleType = ExampleType.Request)]
public class PersonAge69Example : IExampleProvider<int>
{
    public int GetExample() => 69;
}