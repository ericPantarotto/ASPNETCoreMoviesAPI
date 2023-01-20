using System.Collections.Generic;
using System.Threading.Tasks;
using MoviesAPI.Entities;

namespace MoviesAPI.Services
{
    public interface IRepository
    {
        void AddGenre(Genre genre);
        public Task<List<Genre>> GetAllGenres();
        public Genre GetGenreById(int Id);
    }
}