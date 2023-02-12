using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;
using System.Collections.Generic;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [HttpHeaderIsPresent("x-version", "V1")]
    public class RootController: ControllerBase
    {
        [HttpGet(Name = "getRoot")]
        public ActionResult<IEnumerable<Link>> Get()
        {
            List<Link> links = new()
            {
                new Link(href: Url.Link("getRoot", new { }), rel: "self", method: "GET"),
                new Link(href: Url.Link("createUser", new { }), rel: "create-user", method: "POST"),
                new Link(href: Url.Link("Login", new { }), rel: "login", method: "POST"),
                new Link(href: Url.Link("getGenres", new { }), rel: "get-genres", method: "GET"),
                new Link(href: Url.Link("getPeople", new { }), rel: "get-people", method: "GET")
            };

            return links;
        }
    }
}
