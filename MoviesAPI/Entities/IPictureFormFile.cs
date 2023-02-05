using Microsoft.AspNetCore.Http;

namespace MoviesAPI.Entities
{
    public interface IPictureFormFile
    {
        public IFormFile Picture { get; set; }
    }
}
