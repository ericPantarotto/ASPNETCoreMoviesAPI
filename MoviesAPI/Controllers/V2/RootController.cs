using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;
using System.Collections.Generic;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers.V2
{
    [ApiController]
    [Route("api")]
    [HttpHeaderIsPresent("x-version","V2")]
    
    public class RootController: ControllerBase
    {
        [HttpGet(Name = "getRootV2")]
        public ActionResult<IEnumerable<Link>> Get()
        {
            List<Link> links = new List<Link>();

            links.Add(new Link(href: Url.Link("getRoot", new { }), rel: "self", method: "GET"));

            return links;
        }
    }
}
