using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwagWithExamples.Models;

namespace NSwagWithExamples.Controllers;

[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalError))]
[ApiController]
[Route("api/v1/cities")]
public class CitiesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<City>))]
    public IActionResult GetCities() => Ok(new List<City>());

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(City))]
    public IActionResult GetCity([FromRoute] int id) => Ok(new City());

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DeleteCity([FromRoute] int id) => Ok();
}