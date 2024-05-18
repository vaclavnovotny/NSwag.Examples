using System;
using NSwag.Examples;
using RandomNameGeneratorLibrary;

namespace NSwagWithExamples.Models.Examples.Persons;

public class PersonExample : IExampleProvider<Person>
{
    private readonly IPersonNameGenerator _nameGenerator;

    // Use dependency injection to resolve any registered service
    public PersonExample(IPersonNameGenerator nameGenerator)
    {
        _nameGenerator = nameGenerator;
    }

    public Person GetExample() => new Person(
            _nameGenerator.GenerateRandomFirstName(),
            _nameGenerator.GenerateRandomLastName()
        );
}

public class PersonBirthExample : IExampleProvider<DateTime>
{
    public DateTime GetExample() => DateTime.UtcNow.Date;
}