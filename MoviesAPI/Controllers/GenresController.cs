using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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

        [HttpGet] // api/genres
        public async Task<List<GenreDTO>> Get()
        {
            List<Genre> genres = await context.Genres.AsNoTracking().ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);
        }

        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> Get([FromRoute] int Id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if (genre is null) { return NotFound(); }
            
            return mapper.Map<GenreDTO>(genre);
        }

        [HttpPost]
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

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreUpdateDTO genreUpdate)
        {
            Genre genre = mapper.Map<Genre>(genreUpdate);
            genre.Id = id;
            context.Entry(genre).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
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