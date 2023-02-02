using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.DTOs
{
    public class GenreDTO: IGenerateHATEOASLinks
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Link> Links { get; set; } = new List<Link>();

        public void GenerateLinks(IUrlHelper urlHelper)
        {
            Links.Add(new Link(urlHelper.Link("getGenre", new { Id = Id }), rel: "get-genre link", method: "GET"));
            Links.Add(new Link(urlHelper.Link("putGenre", new { Id = Id }), rel: "put-genre link", method: "PUT"));
            Links.Add(new Link(urlHelper.Link("deleteGenre", new { Id = Id }), rel: "delete-genre link", method: "DELETE"));
        }

        public ResourceCollection<GenreDTO> GenerateLinksCollection<GenreDTO>(List<GenreDTO>  dtos, IUrlHelper urlHelper)
        {
            var resourceCollection = new ResourceCollection<GenreDTO>(dtos);
            resourceCollection.Links.Add(new Link(urlHelper.Link("getGenres", new { }), rel: "self", method: "GET"));
            resourceCollection.Links.Add(new Link(urlHelper.Link("createGenre", new { }), rel: "create-genre", method: "POST"));
            return resourceCollection;
        }
    }
}
