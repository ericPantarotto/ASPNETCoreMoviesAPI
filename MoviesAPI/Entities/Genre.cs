 using MoviesAPI.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Genre : IId
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(40)]
        [FirstLetterUppercase]
        public string Name { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
    }    
}