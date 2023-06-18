using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSwag.Examples;
using NSwagWithExamples.Models;
using NSwagWithExamples.Models.Examples;

namespace NSwagWithExamples.Controllers;

[ApiController]
[Route("api/v1/people")]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalError))]
public class PeopleController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Person>))]
    public IActionResult GetPeople() => Ok(new List<Person>());

    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public IActionResult GetNumberOfPeople() => Ok(0);

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
    public IActionResult GetPerson([FromRoute] int id) => Ok(new Person());

    [HttpGet("{id}/age")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
    [EndpointSpecificExample(typeof(PersonSpecificAgeExample))]
    public IActionResult GetPersonAge([FromRoute] int id) => Ok(50);

    [HttpGet("{id}/birth")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DateTime))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
    public IActionResult GetPersonBirth([FromRoute] int id) => Ok(DateTime.UtcNow);

    [HttpPost]
    public IActionResult CreatePerson([FromBody][BindRequired] Person person) =>
        // create person logic
        Ok();

    [HttpPost("from-file")]
    public IActionResult CreatePersonFromFile([FromForm] IFormFile file) => Ok();
}