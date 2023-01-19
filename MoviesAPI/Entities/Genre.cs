using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field with FieldName '{0}' is required")]
        [StringLength(10)]
        public string Name { get; set; }
    }    
}