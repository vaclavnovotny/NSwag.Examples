![Build and Publish](https://github.com/vaclavnovotny/NSwag.Examples/workflows/Build%20and%20Publish/badge.svg) ![Nuget](https://img.shields.io/nuget/v/NSwag.Examples?color=blue)
# Response and Request Examples for NSwag<!-- omit from toc -->
This library allows you to programmatically define swagger examples in your NSWag application. Example discovery occurs at start of application and uses reflection. 

### Overview:<!-- omit from toc -->
- [Setup](#setup)
  - [Install package](#install-package)
  - [Setup Startup.cs](#setup-startupcs)
- [Define examples for your types](#define-examples-for-your-types)
  - [Request Parameters](#request-parameters)
  - [Request Body Parameters](#request-body-parameters)
  - [Response Body](#response-body)
- [Use dependency injection](#use-dependency-injection)
- [Multiple examples](#multiple-examples)
- [Naming the examples](#naming-the-examples)
- [Set example for specific endpoint](#set-example-for-specific-endpoint)
- [Polymorphism](#polymorphism)
- [Support](#support)


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
## Request Parameters
For request other than body parameters like query, route or header parameters, simply define the example using `ExampleAnnotation` atribute and set `ExampleType` to `Request` or `Both`. 
```csharp
[ExampleAnnotation(Name = "Search text 'inspektor'", ExampleType = ExampleType.Request)]
public class PersonTextExample1 : IExampleProvider<string>
{
    public string GetExample() => "inspektor";
}
```
After that, you also need decorate endpoint which examples you wish to be showed

```csharp
[HttpGet]
[EndpointSpecificExample(typeof(PersonTextExample1), ParameterName = "searchText", ExampleType = ExampleType.Request)]
public IActionResult GetPeople([FromQuery] int? minAge = null, [FromQuery] string searchText = null)
{
    ...
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

# Restricting examples to request or response content
Examples can be annotated to restrict their usage to either a request or a response body.  This is useful when the same type may be used on both requests and responses, and you want to restrict examples without specifying specific examples.
```csharp
[ExampleAnnotation(ExampleType = ExampleType.Request)]
public class RequestExample : IExampleProvider<City> {...}

[ExampleAnnotation(ExampleType = ExampleType.Response)]
public class ResponseExample : IExampleProvider<City> {...}
```

# Set example for specific endpoint
It is possible to set specific implementation of `IExampleProvider<T>` to desired API endpoint. This may be handy when multiple endpoints return same type or if primitive type is returned but you need to specify specific value.
To set specific example, simply annotate controllers method with `EndpointSpecificExample` attribute and provide type of the example class.

```csharp
[HttpGet("{id}/age")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
[EndpointSpecificExample(typeof(PersonSpecificAgeExample))] // <-----
public IActionResult GetPersonAge([FromRoute] int id) {...}

public class PersonSpecificAgeExample : IExampleProvider<int>
{
    public int GetExample() => 69;
}
```

# Specify that an example is specifically for a request or response
When setting a specific implementation of `IExampleProvider<T>` for a desired API endpoint, it may be desired to specify specifically whether this example applies to the request or response.

```csharp
[HttpPost("age")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
[EndpointSpecificExample(typeof(PersonSpecificAge69Example), ExampleType = ExampleType.Response)] // <----- Prevents example from being used in the request
[EndpointSpecificExample(typeof(PersonSpecificAge50Example), ExampleType = ExampleType.Request)] // <----- Prevents example from being used in the response
public IActionResult GetPersonAge([FromBody] int id) {...}

// Included on response
public class PersonSpecificAge69Example : IExampleProvider<int>
{
    public int GetExample() => 69;
}

// Included on request
public class PersonSpecificAge50Example : IExampleProvider<int>
{
    public int GetExample() => 50;
}
```

# Specify that an example is specifically for a specific response code
When setting a specific implementation of `IExampleProvider<T>` for a desired API endpoint, it may be desired to specify specifically whether this example applies to the request or response.

```csharp
[HttpGet("{id}/age")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
[EndpointSpecificExample(typeof(PersonSpecificAgeExample), ExampleType = ExampleType.Response, ResponseStatusCode = StatusCodes.Status200OK)] // <----- Only shown for 200 responses
[EndpointSpecificExample(typeof(BadRequestExample), ExampleType = ExampleType.Response, ResponseStatusCode = StatusCodes.Status400BadRequest)] // <----- Only shown for 400 responses
public IActionResult GetPersonAge([FromRoute] int id) {...}

public class PersonSpecificAgeExample : IExampleProvider<int>
{
    public int GetExample() => 69;
}

public class BadRequestExample : IExampleProvider<string>
{
    public int GetExample() => "Oops! Invalid id format error.";
}
```

# Set multiple examples for specific endpoint
It is possible to set multiple specific implementations of `IExampleProvider<T>` to desired API endpoint. This may be handy when multiple endpoints return same type or if primitive type is returned but you need to specify specific value.
To set specific example, simply annotate controllers method with `EndpointSpecificExample` attribute and provide type of the example class.

```csharp
[HttpGet("{id}/age")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
[EndpointSpecificExample(typeof(PersonSpecificAge69Example), typeof(PersonSpecificAge50Example))] // <-----
public IActionResult GetPersonAge([FromRoute] int id) {...}

// Included
public class PersonSpecificAge69Example : IExampleProvider<int>
{
    public int GetExample() => 69;
}

// Included
public class PersonSpecificAge50Example : IExampleProvider<int>
{
    public int GetExample() => 50;
}

// Excluded
public class PersonSpecificAge35Example : IExampleProvider<int>
{
    public int GetExample() => 35;
}
```

# Polymorphism
To define specific examples of abstract classes that are returned or received in controller, simply implement `IExampleProvider<T>` where `T` is the type of abstract class. 
In example below, we have abstract class `Animal` which is returned as collection from `GetAnimals` endpoint. To specify examples for each implementation of `Animal` type, we create classes that implement `IExampleProvider<Animal>`.
```csharp
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Animal>))]
public IActionResult GetAnimals() {...}

public abstract class Animal {...}

public class Monkey : Animal
{
    public int PoopsThrown { get; set; }
}

public class Sloth : Animal
{
    public uint YawnsCount { get; set; }
}

[ExampleAnnotation(Name = "Monkey")]
public class MonkeyExample : IExampleProvider<Animal>
{
    public Animal GetExample() => new Monkey { Age = 5, Name = "Harambe", PawnCount = 4, PoopsThrown = 18 };
}

[ExampleAnnotation(Name = "Sloth")]
public class SlothExample : IExampleProvider<Animal>
{
    public Animal GetExample() => new Sloth { Age = 18, Name = "Vence", PawnCount = 4, YawnsCount = 158 };
}
```
Swagger then shows all examples in response body with dropdown:
![Polymorphism single item response body](https://github.com/vaclavnovotny/images/blob/main/polymorphism3.jpg)

Or in case of enumerable in response body:
![Polymorphism in response body](https://github.com/vaclavnovotny/images/blob/main/polymorphism.jpg)

Or in case of request body, Swagger shows dropdown of all defined examples:
![Polymorphism in request body](https://github.com/vaclavnovotny/images/blob/main/polymorphism2.jpg)

_Note:_ Set your JsonSerializer settings [TypeNameHandling](https://www.newtonsoft.com/json/help/html/serializetypenamehandling.htm) to see discriminator. For example:

```csharp
services.AddOpenApiDocument(
    (settings, provider) =>
    {
        settings.SerializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
        settings.AddExamples(provider);
    });
```


# Support
I personally use this NuGet in my projects, so I will keep this repository up-to-date. Any ideas for extending functionalities are welcome, so create an issue with proposal. 

### Did I save you some hours?<!-- omit from toc -->
[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/U7U72G1A2)
