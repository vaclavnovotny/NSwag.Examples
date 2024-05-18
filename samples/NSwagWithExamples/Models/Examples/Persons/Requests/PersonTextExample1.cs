using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Search text 'inspektor'", ExampleType = ExampleType.Request)]
public class PersonTextExample1 : IExampleProvider<string>
{
    public string GetExample() => "inspektor";
}