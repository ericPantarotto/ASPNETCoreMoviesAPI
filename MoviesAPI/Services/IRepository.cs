using System.Collections.Generic;
using MoviesAPI.Entities;

namespace MoviesAPI.Services
{
    public interface IRepository
    {
        public List<Genre> GetAllGenres();
        public Genre GetGenreById(int Id);
    }
}