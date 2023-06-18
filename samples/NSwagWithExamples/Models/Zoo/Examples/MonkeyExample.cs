using System;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Zoo.Examples;

[ExampleAnnotation(Name = "Monkey")]
public class MonkeyExample : IExampleProvider<Animal>
{
    public Animal GetExample() => new Monkey { Age = 5, Name = "Harambe", PawnCount = 4, PoopsThrown = 18 };
}