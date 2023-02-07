using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class GenresControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenres()
        {
            // Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>();

            context.Genres.Add(new Genre() { Name = "Genre 1" });
            context.Genres.Add(new Genre() { Name = "Genre 2" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            // Testing
            var controller = new GenresController(loggerMock.Object, context2, mapper);
            var response = await controller.Get();

            // Verification
            // var genres = response.Value; //DEBUG: working with ActionResult, not IActionResult
            var okResult = response as OkObjectResult;
            var genres = okResult.Value as List<GenreDTO>;
            Assert.AreEqual(2, genres.Count);
        }

        [TestMethod]
        public async Task GetGenreByIdDoesNotExist()
        {
            // Preparation
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>();
            // Testing
            var controller = new GenresController(loggerMock.Object, context, mapper);
            var response = await controller.Get(1);
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetGenreByIdExist()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>().Object;

            context.Genres.Add(new Genre() { Name = "Genre 1" });
            context.Genres.Add(new Genre() { Name = "Genre 2" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            var controller = new GenresController(loggerMock, context2, mapper);

            int id = 1;
            var response = await controller.Get(id);
            var result = response.Value;
            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        public async Task CreateGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>().Object;

            var newGenre = new GenreCreationDTO() { Name = "New Genre" };

            var controller = new GenresController(loggerMock, context, mapper);

            var response = await controller.Post(newGenre);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(databaseName);
            var count = await context2.Genres.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UpdateGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>();

            context.Genres.Add(new Genre() { Name = "Genre 1" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);
            var controller = new GenresController(loggerMock.Object, context2, mapper);

            var genreUpdateDTO = new GenreUpdateDTO() { Name = "New name" };

            int id = 1;
            var response = await controller.Put(id, genreUpdateDTO);

            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            bool exists = await context3.Genres.AnyAsync(x => x.Name == "New name");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task DeleteGenreNotFound()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>();

            var controller = new GenresController(loggerMock.Object, context, mapper);
            // controller.ControllerContext = BuildControllerContextWithDefaultUser();

            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteGenre()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var mapper = BuildMap();
            var loggerMock = new Mock<ILogger<GenresController>>().Object;

            context.Genres.Add(new Genre() { Name = "Genre 1" });
            context.SaveChanges();

            var context2 = BuildContext(databaseName);

            var controller = new GenresController(loggerMock, context2, mapper);
            // controller.ControllerContext = BuildControllerContextWithDefaultUser();

            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(databaseName);
            bool exists = await context3.Genres.AnyAsync();
            Assert.IsFalse(exists);
        }
    }
}
