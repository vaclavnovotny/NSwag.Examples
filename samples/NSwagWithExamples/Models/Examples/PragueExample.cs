using System.Collections.Generic;
using System.Linq;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples;

[ExampleAnnotation(Name = "Praha")]
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