using NSwag.Examples;

namespace NSwagWithExamples.Models.Zoo.Examples;

[ExampleAnnotation(Name = "Sloth")]
public class SlothExample : IExampleProvider<Animal>
{
    public Animal GetExample() => new Sloth { Age = 18, Name = "Vence", PawnCount = 4, YawnsCount = 158 };
}