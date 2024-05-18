using System;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples.Persons.Requests;

[ExampleAnnotation(Name = "Tom", ExampleType = ExampleType.Request)]
public class PersonRequestExampleTom : IExampleProvider<Person>
{
    public Person GetExample() => new Person("Tom", "Hanks", new DateTime(1956, 7, 9));
}