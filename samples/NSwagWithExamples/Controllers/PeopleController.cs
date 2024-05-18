using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSwag.Examples;
using NSwagWithExamples.Models;
using NSwagWithExamples.Models.Examples;
using NSwagWithExamples.Models.Examples.Persons.Requests;

namespace NSwagWithExamples.Controllers;

[ApiController]
[Route("api/v1/people")]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalError))]
public class PeopleController : ControllerBase
{
    private static readonly ConcurrentDictionary<int, Person> People = new ConcurrentDictionary<int, Person>();
    private static readonly object Lock = new object();

    private static void NewPerson(Person person)
    {
        lock (Lock)
        {
            person.Id = People.Keys.Any() ? People.Keys.Max() + 1 : 1;
            People.TryAdd(person.Id, person);
        }
    }

    static PeopleController()
    {
        NewPerson(new Person("Franta", "Jetel"));
        NewPerson(new Person("Jára", "Cimrman"));
        NewPerson(new Person("Jindra", "Hlaváček"));
        NewPerson(new Person("Vilma", "Böhmová"));
        NewPerson(new Person("Emanuel", "Pecháček"));
        NewPerson(new Person("Inspektor", "Trachta"));
        NewPerson(new Person("Inspektor", "Klečka"));
        NewPerson(new Person("první", "podezřelý"));
        NewPerson(new Person("druhý", "podezřelý"));
        NewPerson(new Person("třetí", "podezřelý"));
        NewPerson(new Person("čtvrtý", "podezřelý"));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Person>))]
    [EndpointSpecificExample(typeof(PersonAge18Example), typeof(PersonAge69Example), ParameterName = "minAge", ExampleType = ExampleType.Request)]
    [EndpointSpecificExample(typeof(PersonTextExample1), typeof(PersonTextExample2), typeof(PersonTextExample3), ParameterName = "searchText", ExampleType = ExampleType.Request)]
    public IActionResult GetPeople([FromQuery] int? minAge = null, [FromQuery] string searchText = null)
    {
        if (minAge == null && string.IsNullOrEmpty(searchText))
            return Ok(People);

        if (minAge == null)
            return Ok(People.Values.Where(p => $"{p.FirstName}~{p.LastName}".Contains(searchText, StringComparison.CurrentCultureIgnoreCase)));

        if (string.IsNullOrEmpty(searchText))
            return Ok(People.Values.Where(p => p.Age >= minAge));

        return Ok(People.Values.Where(p => p.Age >= minAge && $"{p.FirstName}~{p.LastName}".Contains(searchText, StringComparison.CurrentCultureIgnoreCase)));
    }

    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public IActionResult GetNumberOfPeople() => Ok(People.Count);

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
    public IActionResult GetPerson([FromRoute] int id)
    {
        if (People.TryGetValue(id, out Person person))
            return Ok(person);

        return NotFound();
    }

    [HttpGet("{id}/age")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
    public IActionResult GetPersonAge([FromRoute] int id)
    {
        if (People.TryGetValue(id, out Person person))
            return Ok(person.Age);

        return NotFound();
    }

    [HttpGet("{id}/birth")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DateTime))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
    public IActionResult GetPersonBirth([FromRoute] int id)
    {
        if (People.TryGetValue(id, out Person person))
            return Ok(person.BirthDay);

        return NotFound();
    }

    [HttpPost]
    public IActionResult CreatePerson([FromBody] Person person)
    {
        if (person == null)
            return BadRequest("{person} is null");

        if (person.BirthDay.Year < 1800 || person.BirthDay > DateTime.Now)
            return BadRequest($"BirthDay is out of range (1800/01/01..today)");

        NewPerson(person);
        return Ok(person.Id);
    }

    [HttpPost("from-file")]
    public IActionResult CreatePersonFromFile([FromForm] IFormFile file) => Ok();
}