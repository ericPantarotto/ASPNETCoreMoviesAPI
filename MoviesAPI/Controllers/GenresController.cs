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
        [HttpGet] // api/genres
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // [EnableCors(PolicyName = "AllowResttesttest")]
        public async Task<List<GenreDTO>> Get()
        {
            List<Genre> genres = await context.Genres.AsNoTracking().ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);
        }

        /// <summary>
        /// Get a single movie genre
        /// </summary>
        /// <param name="Id">passing the id of the genre</param>
        /// <returns>returns a single genre if success</returns>
        [ProducesResponseType(typeof(GenreDTO), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{Id:int}", Name = "getGenre")]
        // [DisableCors]
        public async Task<ActionResult<GenreDTO>> Get([FromRoute] int Id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if (genre is null) { return NotFound(); }
            
            return mapper.Map<GenreDTO>(genre);
        }

        /// <summary>
        /// Create a new genre in the dB
        /// </summary>
        /// <param name="genreCreation">Genre name first letter upper case</param>
        /// <returns>redirect to the created movie genre</returns>
        [ProducesResponseType(201)]
        [HttpPost]
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
        [HttpPut("{id:int}")]
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
        [HttpDelete("{id:int}")]
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