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
        _random = new Random(DateTime.Now.Microsecond);
    }

    public Person GetExample() => new Person(
            _nameGenerator.GenerateRandomFirstName(),
            _nameGenerator.GenerateRandomLastName()
        );
}

[ExampleAnnotation(Name = "Cindy", ExampleType = ExampleType.Request)]
public class PersonRequestExample_Cindy : IExampleProvider<Person>
{
    public Person GetExample() => new Person("Cindy", "Crowford", new DateTime(1966, 2, 20));
}

[ExampleAnnotation(Name = "Tom", ExampleType = ExampleType.Request)]
public class PersonRequestExample_Tom : IExampleProvider<Person>
{
    public Person GetExample() => new Person("Tom", "Hanks", new DateTime(1956, 7, 9));
}

[ExampleAnnotation(Name = "-")]
public class PersonAgeNoneExample : IExampleProvider<int?>
{
    public int? GetExample() => null;
}
[ExampleAnnotation(Name = "Age 18", ExampleType = ExampleType.Request)]
public class PersonAge18Example : IExampleProvider<int>
{
    public int GetExample() => 18;
}

[ExampleAnnotation(Name = "Age 69", ExampleType = ExampleType.Request)]
public class PersonAge69Example : IExampleProvider<int>
{
    public int GetExample() => 69;
}

[ExampleAnnotation(Name = "-", ExampleType = ExampleType.Request)]
public class PersonTextExample0 : IExampleProvider<string>
{
    public string GetExample() => null;
}
[ExampleAnnotation(Name = "Search text 'inspektor'", ExampleType = ExampleType.Request)]
public class PersonTextExample1 : IExampleProvider<string>
{
    public string GetExample() => "inspektor";
}

[ExampleAnnotation(Name = "Search text 'podez'", ExampleType = ExampleType.Request)]
public class PersonTextExample2 : IExampleProvider<string>
{
    public string GetExample() => "podez";
}

[ExampleAnnotation(Name = "Search text 'ra'", ExampleType = ExampleType.Request)]
public class PersonTextExample3 : IExampleProvider<string>
{
    public string GetExample() => "ra";
}

public class PersonBirthExample : IExampleProvider<DateTime>
{
    public DateTime GetExample() => DateTime.UtcNow.Date;
}