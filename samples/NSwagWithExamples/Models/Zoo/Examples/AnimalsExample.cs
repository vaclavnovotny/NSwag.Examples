using System.Collections.Generic;
using System.Linq;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Zoo.Examples;

public class AnimalsExample : IExampleProvider<IEnumerable<Animal>>
{
    private readonly IEnumerable<IExampleProvider<Animal>> _exampleProviders;

    public AnimalsExample(IEnumerable<IExampleProvider<Animal>> exampleProviders) {
        _exampleProviders = exampleProviders;
    }

    public IEnumerable<Animal> GetExample() => _exampleProviders.Select(x => x.GetExample());
}