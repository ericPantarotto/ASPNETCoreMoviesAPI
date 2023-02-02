using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.DTOs
{
    public class PersonDTO : IGenerateHATEOASLinks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Picture { get; set; }

        public List<Link> Links { get; set; } = new List<Link>();

        public void GenerateLinks(IUrlHelper urlHelper)
        {
            Links.Add(new Link(urlHelper.Link("getPerson", new { Id = Id }), rel: "get-person link", method: "GET"));
            Links.Add(new Link(urlHelper.Link("putPerson", new { Id = Id }), rel: "put-person link", method: "PUT"));
            Links.Add(new Link(urlHelper.Link("patchPerson", new { Id = Id }), rel: "patch-person link", method: "PATCH"));
            Links.Add(new Link(urlHelper.Link("deletePerson", new { Id = Id }), rel: "delete-person link", method: "DELETE"));
        }

        public ResourceCollection<PersonDTO> GenerateLinksCollection<PersonDTO>(List<PersonDTO> dtos, IUrlHelper urlHelper)
        {
            var resourceCollection = new ResourceCollection<PersonDTO>(dtos);
            resourceCollection.Links.Add(new Link(urlHelper.Link("getPeople", new { }), rel: "self", method: "GET"));
            resourceCollection.Links.Add(new Link(urlHelper.Link("createPerson", new { }), rel: "create-person", method: "POST"));
            return resourceCollection;
        }
    }
}
