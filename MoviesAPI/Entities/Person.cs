using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesAPI.Validations;

namespace MoviesAPI.Entities
{
    public class Person: IId, IPicture
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        [FirstLetterUppercase()]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Picture { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
    }
}
