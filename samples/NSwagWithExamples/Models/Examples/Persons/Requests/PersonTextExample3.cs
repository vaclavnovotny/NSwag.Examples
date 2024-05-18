using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Search text 'ra'", ExampleType = ExampleType.Request)]
public class PersonTextExample3 : IExampleProvider<string>
{
    public string GetExample() => "ra";
}