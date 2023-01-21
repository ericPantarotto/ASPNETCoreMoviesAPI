using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MoviesAPI.Entities;
using MoviesAPI.Filters;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    // [Route("api/genres")]
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController: ControllerBase
    {
        private readonly IRepository repository;
        private readonly ILogger<GenresController> logger;

        public GenresController(IRepository repository, ILogger<GenresController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet] // api/genres
        [HttpGet("list")] // api/genres/list
        [HttpGet("/allgenres")] //overriding the route as we start with /
        [ResponseCache(Duration = 60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<List<Genre>> Get()
        {
            logger.LogInformation("getting all the genres");
            return await repository.GetAllGenres();
        }

        //[HttpGet("example")] // api/genres/example  - https://localhost:5001/api/genres/example?id=1        
        //[HttpGet("{Id}")] // api/genres/1 
        //[HttpGet("{Id}/{param2=felipe}")] // providing a default value for a second optional parameter
        //[HttpGet("{Id:int}/{param2=felipe}")] // specifying a route constraint => you will get a 404 if the parameter does not have the correct type
        [HttpGet("{Id:int}", Name = "getGenre")] // specifying a route constraint => you will get a 404 if the parameter does not have the correct type
        public ActionResult<Genre> Get([FromRoute] int Id, [BindRequired, FromHeader] string param2)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            logger.LogDebug("GetById executing ...");

            var genre = repository.GetGenreById(Id);
            if (genre is null)
            {
                logger.LogWarning($"Genre with Id {Id} not found");
                throw new ApplicationException();
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
        public ActionResult Post([FromBody] Genre genre)
        {
            repository.AddGenre(genre);

            // return at 201 Created At
            return new CreatedAtRouteResult(routeName: "getGenre",
                                            routeValues: new { Id = genre.Id }, // using anonymous type to pass the Id parameter expected for that route
                                            value: genre);
            
            //return NoContent(); returning 204
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre genre)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }

    }
}