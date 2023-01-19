using System.Collections.Generic;
using System.Threading.Tasks;
using MoviesAPI.Entities;

namespace MoviesAPI.Services
{
    public interface IRepository
    {
        public Task<List<Genre>> GetAllGenres();
        public Genre GetGenreById(int Id);
    }
}