using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    // [Route("api/genres")]
    [Route("api/[controller]")]
    public class GenresController
    {
        private readonly IRepository repository;

        public GenresController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] // api/genres
        [HttpGet("list")] // api/genres/list
        [HttpGet("/allgenres")] //overriding the route as we start with /
        public List<Genre> Get()
        {
            return repository.GetAllGenres();
        }

        //[HttpGet("example")] // api/genres/example  - https://localhost:5001/api/genres/example?id=1        
        //[HttpGet("{Id}")] // api/genres/1 
        //[HttpGet("{Id}/{param2=felipe}")] // providing a default value for a second optional parameter
        [HttpGet("{Id:int}/{param2=felipe}")] // specifying a route constraint => you will get a 404 if the parameter does not have the correct type
        public Genre Get(int Id, string param2)
        {
            return repository.GetGenreById(Id);
        }

        
        [HttpPost]
        public void Post()
        {

        }

        [HttpPut]
        public void Put()
        {
        }

        [HttpDelete]
        public void Delete()
        {
        }

    }
}