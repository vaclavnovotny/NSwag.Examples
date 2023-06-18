using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples;

public class IntExample : IExampleProvider<int>
{
    public int GetExample() => 40;
}