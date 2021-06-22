using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples
{
    public class PersonAgeExample : IExampleProvider<int>
    {
        public int GetExample() => 40;
    }
}