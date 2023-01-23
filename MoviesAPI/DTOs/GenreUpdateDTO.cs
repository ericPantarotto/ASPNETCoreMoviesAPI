using MoviesAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class GenreUpdateDTO
    {
        [Required]
        [StringLength(40)]
        [FirstLetterUppercase]
        public string Name { get; set; }
    }
}
