using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwagWithExamples.Models;

namespace NSwagWithExamples.Controllers
{
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalError))]
    [ApiController, Route("api/v1/cities")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<City>))]
        public async Task<IActionResult> GetCities()
        {
            return Ok(new List<City>());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(City))]
        public async Task<IActionResult> GetCity([FromRoute]int id)
        {
            return Ok(new City());
        }
    }
}
