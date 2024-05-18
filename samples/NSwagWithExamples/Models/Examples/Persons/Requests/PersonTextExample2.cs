using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Search text 'podez'", ExampleType = ExampleType.Request)]
public class PersonTextExample2 : IExampleProvider<string>
{
    public string GetExample() => "podez";
}