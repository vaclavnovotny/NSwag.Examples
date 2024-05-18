using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Age 18", ExampleType = ExampleType.Request)]
public class PersonAge18Example : IExampleProvider<int>
{
    public int GetExample() => 18;
}