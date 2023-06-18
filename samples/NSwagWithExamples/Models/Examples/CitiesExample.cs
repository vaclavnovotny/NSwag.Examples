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
                    new Person {Id = 1, FirstName = "Henry", LastName = "Cavill"},
                    new Person {Id = 2, FirstName = "John", LastName = "Doe"}
                }
            }
        };
    }
}