using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class MoviesControllerTests : BaseTests
    {
        private string CreateTestData()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var genre = new Genre() { Name = "genre 1" };

            var movies = new List<Movie>()
            {
                new Movie(){Title = "Movie 1", ReleaseDate = new DateTime(2010, 1,1), InTheaters = false, Poster = "url_init"},
                new Movie(){Title = "Future Movie", ReleaseDate = DateTime.Today.AddDays(1), InTheaters = false},
                new Movie(){Title = "In Theaters Movie", ReleaseDate = DateTime.Today.AddDays(-1), InTheaters = true}
            };

            var movieWithGenre = new Movie()
            {
                Title = "Movie With Genre",
                ReleaseDate = new DateTime(2010, 1, 1),
                InTheaters = false
            };
            movies.Add(movieWithGenre);

            context.Add(genre);
            context.AddRange(movies);
            context.SaveChanges();

            var movieGenre = new MoviesGenres() { GenreId = genre.Id, MovieId = movieWithGenre.Id };
            context.Add(movieGenre);
            context.SaveChanges();

            return databaseName;
        }

        [TestMethod]
        public async Task UpdateMovieWithImage()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var people = new List<Person>
            {
                new Person { Name= "Person1", Biography = "Bio P1"},
                new Person { Name= "Person2", Biography = "Bio P2"},
                new Person { Name= "Person3", Biography = "Bio P2"},
            };

            await context.AddRangeAsync(new MoviesActors { MovieId = 1, PersonId = 3 });

            await context.AddRangeAsync(people);
            await context.SaveChangesAsync();

            var content = Encoding.UTF8.GetBytes("This is a  dummy image");
            var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "dummy.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";

            var fileStorageMock = new Mock<IFileStorageService>();
            fileStorageMock.Setup(x => x.EditFile(content, ".jpg", "movies", null, file.ContentType))
                .Returns(Task.FromResult("url_modified"));

            var controller = new MoviesController(context, mapper, fileStorageMock.Object, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            

            var movieCreationDTO = new MovieCreationDTO() 
            { 
                Title = "Movie 1 modified", 
                Summary = "Modified Summary",
                ReleaseDate = new DateTime(2020, 1,1), 
                InTheaters = false,
                Poster = file,
                GenresIds = new List<int>{1},
                Actors = new List<ActorCreationDTO> 
                {  
                    new ActorCreationDTO {PersonId  = 1, Character = "Peter Pan"},  
                    new ActorCreationDTO {PersonId  = 2, Character = "Spider Man"}  
                }
            };
          
            int id = 1;
            var response = await controller.Put(id, movieCreationDTO);

            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            bool exists = await context3.Movies.AnyAsync(x => x.Title == "Movie 1 modified");
            Assert.IsTrue(exists);
            bool existsMovieActorTest = await context3.MoviesActors.AnyAsync(x => x.MovieId == 1 && x.PersonId == 3);
            Assert.IsFalse(existsMovieActorTest);

            var movieWithPicture = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            Assert.AreEqual("url_modified", movieWithPicture.Poster);
            Assert.AreEqual(1, fileStorageMock.Invocations.Count);
        }

        [TestMethod]
        public async Task FilterByTitle()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                Title = "Movie 1",
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual("Movie 1", movies[0].Title);
        }

        [TestMethod]
        public async Task FilterByInTheaters()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                InTheaters = true,
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual("In Theaters Movie", movies[0].Title);
        }

        [TestMethod]
        public async Task FilterByUpcomingReleases()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                UpcomingReleases = true,
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual("Future Movie", movies[0].Title);
        }

        [TestMethod]
        public async Task FilterByGenre()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var genreId = context.Genres.Select(x => x.Id).First();

            var context2 = BuildContext(databaseName);

            var controller = new MoviesController(context2, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                GenreId = genreId,
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual("Movie With Genre", movies[0].Title);
        }

        [TestMethod]
        public async Task FilterOrderByTitleAscending()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OrderingField = "title",
                AscendingOrder = true,
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(databaseName);
            var moviesFromDb = context2.Movies.OrderBy(x => x.Title).ToList();

            Assert.AreEqual(moviesFromDb.Count, movies.Count);
            for (int i = 0; i < moviesFromDb.Count; i++)
            {
                var movieFromController = movies[i];
                var movieFromDb = moviesFromDb[i];

                Assert.AreEqual(movieFromDb.Id, movieFromController.Id);
            }
        }

        [TestMethod]
        public async Task FilterOrderByTitleDescending()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OrderingField = "title",
                AscendingOrder = false,
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(databaseName);
            var moviesFromDb = context2.Movies.OrderByDescending(x => x.Title).ToList();

            Assert.AreEqual(moviesFromDb.Count, movies.Count);
            for (int i = 0; i < moviesFromDb.Count; i++)
            {
                var movieFromController = movies[i];
                var movieFromDb = moviesFromDb[i];

                Assert.AreEqual(movieFromDb.Id, movieFromController.Id);
            }
        }

        [TestMethod]
        public async Task FilterOrderyByWrongFieldStillReturnsMovies()
        {
            var databaseName = CreateTestData();
            var mapper = BuildMap();
            var context = BuildContext(databaseName);

            var mock = new Mock<ILogger<MoviesController>>();
            var controller = new MoviesController(context, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OrderingField = "abcd",
                AscendingOrder = false,
                RecordsPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(databaseName);
            var moviesFromDb = context2.Movies.ToList();
            Assert.AreEqual(moviesFromDb.Count, movies.Count);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
    }
}
