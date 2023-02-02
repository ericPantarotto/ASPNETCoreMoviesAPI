using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(PolicyName = "AllowResttesttest")]
    public class GenresController: ControllerBase
    {
        private readonly ILogger<GenresController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ILogger<GenresController> logger,
                                ApplicationDbContext context,
                                IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get entire list of movies genres
        /// </summary>
        /// <returns>list of genres</returns>
        [ProducesResponseType(typeof(List<GenreDTO>), 200)]
        [ProducesResponseType(401)]
        [HttpGet(Name = "getGenres")] // api/genres
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // [EnableCors(PolicyName = "AllowResttesttest")]
        // public async Task<List<GenreDTO>> Get()
        public async Task<IActionResult> Get([FromQuery] bool includeHATEOAS = true)
        {
            List<Genre> genres = await context.Genres.AsNoTracking().ToListAsync();
            List<GenreDTO> genreDTOs = mapper.Map<List<GenreDTO>>(genres);
            
            if (includeHATEOAS)
            {
                genreDTOs.ForEach(genre => GenerateLinks(genre));

                var resourceCollection = new ResourceCollection<GenreDTO>(genreDTOs);
                resourceCollection.Links.Add(new Link(href: Url.Link("getGenres", new{}), rel: "self", method: "GET"));
                resourceCollection.Links.Add(new Link(href: Url.Link("createGenre", new{}), rel: "create-genre", method: "POST"));

                return Ok(resourceCollection);    
            }
            
            return Ok(genreDTOs);
        }

        private void GenerateLinks(GenreDTO genreDTO)
        {
            genreDTO.Links.Add(new Link(Url.Link("getGenre", new { Id = genreDTO.Id }), rel: "get-genre link", method: "GET"));
            genreDTO.Links.Add(new Link(Url.Link("putGenre", new { Id = genreDTO.Id }), rel: "put-genre link", method: "PUT"));
            genreDTO.Links.Add(new Link(Url.Link("deleteGenre", new { Id = genreDTO.Id }), rel: "delete-genre link", method: "DELETE"));
        }

        /// <summary>
        /// Get a single movie genre 
        /// </summary>
        /// <param name="Id">passing the id of the genre</param>
        /// <param name="includeHATEOAS">boolean for HATEOAS display</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GenreDTO), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{Id:int}", Name = "getGenre")]
        // [DisableCors]
        public async Task<ActionResult<GenreDTO>> Get([FromRoute] int Id, [FromQuery] bool includeHATEOAS = true)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if (genre is null) { return NotFound(); }

            var genreDTO = mapper.Map<GenreDTO>(genre);
            if (includeHATEOAS) { GenerateLinks(genreDTO); }
            return genreDTO;
        }

        /// <summary>
        /// Create a new genre in the dB
        /// </summary>
        /// <param name="genreCreation">Genre name first letter upper case</param>
        /// <returns>redirect to the created movie genre</returns>
        [ProducesResponseType(201)]
        [HttpPost(Name ="createGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreation)
        {
            Genre genre = mapper.Map<Genre>(genreCreation);
            await context.AddAsync(genre);
            await context.SaveChangesAsync();

            GenreDTO genreDTO = mapper.Map<GenreDTO>(genre);

            return new CreatedAtRouteResult(routeName: "getGenre",
                                            routeValues: new { genreDTO.Id },
                                            value: genreDTO);            
        }

        /// <summary>
        /// Update the attributes of movie genre
        /// </summary>
        /// <param name="id">id of the genre for update</param>
        /// <param name="genreUpdate">new name for the genre to update</param>
        /// <returns>no content if success</returns>
        [ProducesResponseType(204)]
        [HttpPut("{id:int}", Name ="putGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreUpdateDTO genreUpdate)
        {
            Genre genre = mapper.Map<Genre>(genreUpdate);
            genre.Id = id;
            context.Entry(genre).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return NoContent();
        }
        
        /// <summary>
        /// Delete a genre in the database
        /// </summary>
        /// <param name="id">Id of the genre to delete</param>
        /// <returns>no content if deletion is successful</returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id:int}", Name ="deleteGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            bool exists = await context.Genres.AnyAsync(x => x.Id == id);
            if (!exists) { return NotFound(); }
            
            context.Remove(new Genre() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}