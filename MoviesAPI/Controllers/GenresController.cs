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
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(PolicyName = "AllowResttesttest")]
    public class GenresController: CustomBaseController
    {
        private readonly ILogger<GenresController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ILogger<GenresController> logger,
                                ApplicationDbContext context,
                                IMapper mapper)
        : base(context, mapper)
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
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        // [EnableCors(PolicyName = "AllowResttesttest")]
        // public async Task<List<GenreDTO>> Get()
        public async Task<IActionResult> Get()
        {
            return  Ok(await Get<Genre, GenreDTO>());
        }

        /// <summary>
        /// Get a single movie genre 
        /// </summary>
        /// <param name="id">passing the id of the genre</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GenreDTO), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{Id:int}", Name = "getGenre")]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        // [DisableCors]
        public async Task<ActionResult<GenreDTO>> Get([FromRoute] int id)
        {
            return await Get<Genre, GenreDTO>(id);
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
            return await Post<GenreCreationDTO, Genre, GenreDTO>(genreCreation, "getGenre");
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
            return await Put<GenreUpdateDTO, Genre>(id, genreUpdate);
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
            return await Delete<Genre>(id);
        }

    }
}