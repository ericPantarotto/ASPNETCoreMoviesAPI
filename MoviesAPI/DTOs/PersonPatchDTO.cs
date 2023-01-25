using System.ComponentModel.DataAnnotations;
using System;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class PersonPatchDTO
    {
        [Required]
        [StringLength(120)]
        [FirstLetterUppercase]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
