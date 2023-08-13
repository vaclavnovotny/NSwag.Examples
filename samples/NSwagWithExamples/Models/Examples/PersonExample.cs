using System;
using NSwag.Examples;
using RandomNameGeneratorLibrary;

namespace NSwagWithExamples.Models.Examples;

public class PersonExample : IExampleProvider<Person>
{
    private readonly IPersonNameGenerator _nameGenerator;
    private readonly Random _random;

    // Use dependency injection to resolve any registered service
    public PersonExample(IPersonNameGenerator nameGenerator)
    {
        _nameGenerator = nameGenerator;
        _random = new Random(); 
    }

    public Person GetExample()
    {
        return new Person {
            Id = _random.Next(1, 100),
            FirstName = _nameGenerator.GenerateRandomFirstName(),
            LastName = _nameGenerator.GenerateRandomLastName()
        };
    }
}

[ExampleAnnotation(ExampleType = ExampleType.Request)]
public class PersonRequestExample : IExampleProvider<Person>
{
    public Person GetExample()
    {
        return new Person
        {
            Id = 1,
            FirstName = "Fixed First Name",
            LastName = "Fixed Last Name",
        };
    }
}