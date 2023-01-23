using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using static MoviesAPI.Validations.ContentTypeValidator;

namespace MoviesAPI.DTOs
{
    public class PersonCreationDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        [FileSizeValidator(1)]
        [ContentTypeValidator(contentTypeGroup: ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}
