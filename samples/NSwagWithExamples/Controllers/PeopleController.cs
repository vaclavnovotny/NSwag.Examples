using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSwagWithExamples.Models;

namespace NSwagWithExamples.Controllers
{
    [ApiController, Route("api/v1/people")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalError))]
    public class PeopleController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Person>))]
        public async Task<IActionResult> GetPeople()
        {
            return Ok(new List<Person>());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
        public async Task<IActionResult> GetPerson([FromRoute]int id)
        {
            return Ok(new Person());
        }

        [HttpGet("{id}/age")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
        public async Task<IActionResult> GetPersonAge([FromRoute] int id)
        {
            return Ok(50);
        }


        [HttpGet("{id}/birth")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DateTime))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CustomInternalErrorOnMethodLevel))]
        public async Task<IActionResult> GetPersonBirth([FromRoute] int id)
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody, BindRequired] Person person)
        {
            // create person logic
            return Ok();
        }

        [HttpPost("from-file")]
        public async Task<IActionResult> CreatePersonFromFile([FromForm] IFormFile file) {
            return Ok();
        }
    }
}