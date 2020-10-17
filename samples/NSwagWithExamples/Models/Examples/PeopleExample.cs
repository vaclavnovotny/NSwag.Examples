using System.Collections.Generic;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples
{
    public class PeopleExample : IExampleProvider<List<Person>>
    {
        private readonly PersonExample _personExample;

        public PeopleExample(PersonExample personExample)
        {
            _personExample = personExample;
        }

        public List<Person> GetExample()
        {
            return new List<Person>()
            {
                _personExample.GetExample(),
                _personExample.GetExample(),
                _personExample.GetExample()
            };
        }
    }
}