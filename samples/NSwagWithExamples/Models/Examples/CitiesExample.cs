using System.Collections.Generic;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples;

public class CitiesExample : IExampleProvider<List<City>>
{
    public List<City> GetExample()
    {
        return new List<City> {
            new City {
                Id = 5,
                Name = "Brno",
                People = new List<Person> {
                    new Person (1, "Henry", "Cavill"),
                    new Person (2, "John", "Doe")
                }
            }
        };
    }
}