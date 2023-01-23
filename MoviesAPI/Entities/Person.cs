using MoviesAPI.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Person
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Picture { get; set; }
    }
}
