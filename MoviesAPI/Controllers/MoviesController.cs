using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using MoviesAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly ILogger<MoviesController> logger;
        private readonly string containerName = "movies";

        public MoviesController(ApplicationDbContext context,
            IMapper mapper,
            IFileStorageService fileStorageService,
            ILogger<MoviesController> logger)
        : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.logger = logger;
        }

        
        /// <summary>
        /// Get the top 6 upcoming movies and top 6 movies currently in theatre
        /// </summary>
        /// <returns>list of upcoming and currently in theatre movies</returns>
        [ProducesResponseType(typeof(IndexMoviePageDTO),200)]
        [HttpGet]
        public async Task<ActionResult<IndexMoviePageDTO>> Get()
        {
            int top = 6;
            DateTime today = DateTime.Today;
            List<Movie> upcomingReleases = await context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            List<Movie> inTheaters = await context.Movies
                .Where(x => x.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new IndexMoviePageDTO();
            result.InTheaters = mapper.Map<List<MovieDTO>>(inTheaters);
            result.UpcomingReleases = mapper.Map<List<MovieDTO>>(upcomingReleases);

            return result;
        }

        /// <summary>
        /// Filtering movies list
        /// </summary>
        /// <param name="filterMoviesDTO">Model allowing to filter on key attributes of a movie</param>
        /// <returns>filtered list of movies</returns>
        [ProducesResponseType(typeof(List<FilterMoviesDTO>), 200)]
        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.OrderingField))
            {
                try
                {
                    moviesQueryable = moviesQueryable
                        .OrderBy($"{filterMoviesDTO.OrderingField} {(filterMoviesDTO.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    // log this
                    logger.LogWarning("Could not order by field: " + filterMoviesDTO.OrderingField);
                }
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable,
                filterMoviesDTO.RecordsPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();

            return mapper.Map<List<MovieDTO>>(movies);
        }

        /// <summary>
        /// get a single movie based on the id provided
        /// </summary>
        /// <param name="id">id of the mvoie to return</param>
        /// <returns>a single movie</returns>
        [ProducesResponseType(typeof(MovieDetailsDTO), 200)]
        [HttpGet("{id}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            var queryable = context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .AsQueryable();

            return await Get<Movie, MovieDetailsDTO>(id, queryable);
        }

        /// <summary>
        /// Creating a new movie
        /// </summary>
        /// <param name="movieCreationDTO">movie characteristics including list of genres and actors</param>
        /// <returns>created movie</returns>
        [ProducesResponseType(201)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            return await Post<MovieCreationDTO, Movie, MovieDTO>(movieCreationDTO, "getMovie", fileStorageService, containerName);
        }

        public void AnnotateOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        /// <summary>
        /// update the selected movie
        /// </summary>
        /// <param name="id">id of the movie to update</param>
        /// <param name="movieCreationDTO">movie characteristics including list of genres and actors</param>
        /// <returns>No content if successful</returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movieDB == null)
            {
                return NotFound();
            }

            movieDB = mapper.Map(movieCreationDTO, movieDB);

            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
                    movieDB.Poster =
                        await fileStorageService.EditFile(content, extension, containerName,
                                                            movieDB.Poster,
                                                            movieCreationDTO.Poster.ContentType);
                }
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieDB.Id}; delete from MoviesGenres where MovieId = {movieDB.Id}");

            AnnotateOrder(movieDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Patching a single movie
        /// </summary>
        /// <param name="id">id of the movie to patch</param>
        /// <param name="patchDocument">json formatted document for updating a movie</param>
        /// <returns>no content if successful</returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
              return await Patch<Movie, MoviePatchDTO>(id, patchDocument);
        }

        /// <summary>
        /// deleting the selected movie 
        /// </summary>
        /// <param name="id">id of the movie to delete</param>
        /// <returns>no content if successfull</returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Movie>(id, fileStorageService, containerName);
        }
    }
}
