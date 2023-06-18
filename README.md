![Build and Publish](https://github.com/vaclavnovotny/NSwag.Examples/workflows/Build%20and%20Publish/badge.svg) ![Nuget](https://img.shields.io/nuget/v/NSwag.Examples?color=blue)
# Response and Request Body Examples for NSwag
This library allows you to programmatically define swagger examples in your NSWag application. Example discovery occurs at start of application and uses reflection. 

# Setup

## Install package

```csharp
Install-Package NSwag.Examples
```

## Setup Startup.cs

Add services needed for example definition using `AddExampleProviders()` and provide assemblies where are your examples located.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddExampleProviders(typeof(CityExample).Assembly);
    services.AddOpenApiDocument((settings, provider) =>
    {
        settings.AddExamples(provider);
    });
}
```

# Define examples for your types

Create class and implement interface IExampleProvider<T> where T is the type you want to define example for.
```csharp
public class CityExample : IExampleProvider<City>
{
    public City GetExample()
    {
        return new City()
        {
            Id = 5,
            Name = "Brno",
            People = new List<Person>()
            {
                new Person() {Id = 1, FirstName = "Henry", LastName = "Cavill"},
                new Person() {Id = 2, FirstName = "John", LastName = "Doe"}
            }
        };
    }
}
```

## Request Body Parameters

For request body parameters there is nothing you need to worry about, just mark your parameter `[FromBody]`.
```csharp
[HttpPost]
public async Task<IActionResult> CreatePerson([FromBody, BindRequired] Person person)
{
    // create person logic
    return Ok();
}
```
Result in swagger:
![Image of request body](https://github.com/vaclavnovotny/images/blob/main/requestExample.JPG)

## Response Body

For response body types you need to decorate your method with `[ProducesResponseType]`
```csharp
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
public async Task<IActionResult> GetPerson([FromRoute]int id)
{
    return Ok(new Person());
}
```

Result in swagger:
![Image of request body](https://github.com/vaclavnovotny/images/blob/main/responseExampleSingle.JPG)

# Use dependency injection

You can also use dependency injection in constructor of your example provider class.

Constructor and `GetExample` method gets called when operation processors are executed - when swagger specification is being generated which is during first request on swagger.
```csharp
public class PersonExample : IExampleProvider<Person>
{
    private readonly IPersonNameGenerator _nameGenerator;
    private readonly Random _random;

    public PersonExample(IPersonNameGenerator nameGenerator)
    {
        _nameGenerator = nameGenerator;
        _random = new Random(); 
    }

    public Person GetExample()
    {
        return new Person()
        {
            Id = _random.Next(1, 100),
            FirstName = _nameGenerator.GenerateRandomFirstName(),
            LastName = _nameGenerator.GenerateRandomLastName()
        };
    }
}
```

You can also combine examples by reusing `IExampleProvider<T>`. You can directly inject desired example providers into constructor and use them within your example provider.
```csharp
public class PragueExample : IExampleProvider<City>
{
    private readonly IEnumerable<IExampleProvider<Person>> _peopleExamples;

    public PragueExample(IEnumerable<IExampleProvider<Person>> peopleExamples) {
        _peopleExamples = peopleExamples;
    }
    
    public City GetExample() {
        return new City {
            Id = 420,
            Name = "Prague",
            People = _peopleExamples.Select(x => x.GetExample()).ToList()
        };
    }
}
```

# Multiple examples
To define multiple examples for request or response body, simply create more classes implementing `IExampleProvider<T>` multiple times.
Here, to define multiple examples of type `City`, we create two classes that implement interface `IExampleProvider<City>`.
```csharp
public class BrnoExample : IExampleProvider<City>
{
    public City GetExample()
    {
        return new City {
            Id = 5,
            Name = "Brno",
            People = new List<Person> {
                new Person {Id = 1, FirstName = "Henry", LastName = "Cavill"},
                new Person {Id = 2, FirstName = "John", LastName = "Doe"}
            }
        };
    }
}

public class PragueExample : IExampleProvider<City>
{
    public City GetExample() {
        return new City {
            Id = 420,
            Name = "Prague",
            People = new List<Person> {
                new Person {Id = 1, FirstName = "Henry", LastName = "Cavill"},
                new Person {Id = 2, FirstName = "John", LastName = "Doe"}
            }
        };
    }
}
```
Swagger then contains dropdown with these options:
![Multiple examples](https://github.com/vaclavnovotny/images/blob/main/multipleexamples.jpg)

# Naming the examples
You can name examples so they do not show up in Swagger as Example 1, Example 2, etc.
Simply annotate your example with `ExampleAnnotation` attribute and set `Name` property.

```csharp
[ExampleAnnotation(Name = "Brno")]
public class BrnoExample : IExampleProvider<City>
```
Swagger then shows these names in dropdown:
![Named examples](https://github.com/vaclavnovotny/images/blob/main/namingexamples.jpg)

# Set example for specific endpoint
It is possible to set specific implementation of `IExampleProvider<T>` to desired API endpoint. This may be handy when multiple endpoints return same type or if primitive type is returned but you need to specify specific value.
To set specific example, simply annotate controllers method with `EndpointSpecificExample` attribute and provide type of the example class.

```csharp
[HttpGet("{id}/age")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
[EndpointSpecificExample(typeof(PersonSpecificAgeExample))]
public IActionResult GetPersonAge([FromRoute] int id) {
    ...
}
```

# Support
I personally use this NuGet in my projects, so I will keep this repository up-to-date. Any ideas for extending functionalities are welcome, so create an issue with proposal. 

### Did I save you some hours?
[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/U7U72G1A2)
