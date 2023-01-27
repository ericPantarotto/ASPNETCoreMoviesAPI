using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Validations;
using System.Collections.Generic;
using static MoviesAPI.Validations.ContentTypeValidator;

namespace MoviesAPI.DTOs
{
    public class MovieCreationDTO: MoviePatchDTO
    {
        [FileSizeValidator(maxFileSizeInMbs: 4)]
        [ContentTypeValidator(contentTypeGroup: ContentTypeGroup.Image)]
        public IFormFile Poster { get; set; }

        // [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        // public List<int> GenresIds { get; set; }

        // [ModelBinder(BinderType = typeof(TypeBinder<List<ActorCreationDTO>>))]
        // public List<ActorCreationDTO> Actors { get; set; }
    }
}
