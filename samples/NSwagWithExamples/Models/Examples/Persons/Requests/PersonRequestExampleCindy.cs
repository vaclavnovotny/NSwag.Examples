using System;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Cindy", ExampleType = ExampleType.Request)]
public class PersonRequestExampleCindy : IExampleProvider<Person>
{
    public Person GetExample() => new Person("Cindy", "Crowford", new DateTime(1966, 2, 20));
}