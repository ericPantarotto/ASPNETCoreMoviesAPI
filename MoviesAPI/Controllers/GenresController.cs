using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    // [Route("api/genres")]
    [Route("api/[controller]")]
    public class GenresController: ControllerBase
    {
        private readonly IRepository repository;

        public GenresController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] // api/genres
        [HttpGet("list")] // api/genres/list
        [HttpGet("/allgenres")] //overriding the route as we start with /
        public async Task<List<Genre>> Get()
        {
            return await repository.GetAllGenres();
        }

        //[HttpGet("example")] // api/genres/example  - https://localhost:5001/api/genres/example?id=1        
        //[HttpGet("{Id}")] // api/genres/1 
        //[HttpGet("{Id}/{param2=felipe}")] // providing a default value for a second optional parameter
        [HttpGet("{Id:int}/{param2=felipe}")] // specifying a route constraint => you will get a 404 if the parameter does not have the correct type
        public ActionResult<Genre> Get(int Id, string param2)
        {
            var genre = repository.GetGenreById(Id);
            if (genre is null)
            {
                return NotFound();
            }
            return genre;
        }

        [HttpGet("IActionResult/{Id:int}")]  //prefer ActionResult as you can define the type you want to return, with IActionResult you can define whatever type we want
        public IActionResult Get(int Id)
        {
            var genre = repository.GetGenreById(Id);
            if (genre is null)
            {
                return NotFound();
            }
            return Ok(genre);
            //return Ok(2);
            //return Ok("anystring");
        }


        [HttpPost]
        public ActionResult Post()
        {
            return NoContent();
        }

        [HttpPut]
        public ActionResult Put()
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }

    }
}