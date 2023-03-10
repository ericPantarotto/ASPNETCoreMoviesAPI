using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Movie: IId, IPicture
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
        
        public List<MoviesActors> MoviesActors { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
        private string picture;
        public string Picture
        {
            get { return Poster; }
            set { picture = value; }
        }
        
    }
}
