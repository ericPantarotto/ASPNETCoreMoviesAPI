using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using static MoviesAPI.Validations.ContentTypeValidator;

namespace MoviesAPI.DTOs
{
    public class PersonDTOBase
    {
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        [FileSizeValidator(1)]
        [ContentTypeValidator(contentTypeGroup: ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}