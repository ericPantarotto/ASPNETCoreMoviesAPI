using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();
            CreateMap<GenreUpdateDTO, Genre>();
            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<PersonCreationDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore())
                .ForMember(x => x.Picture, options => options.MapFrom(x => x.PictureCustom));
                
            CreateMap<PersonUpdateDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<Person, PersonPatchDTO>().ReverseMap();
            
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.Poster, options => options.MapFrom(x => x.PictureCustom))
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MoviePatchDTO>().ReverseMap();

            CreateMap<Movie, MovieDetailsDTO>()
               .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
               .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));

              CreateMap<IdentityUser, UserDTO>()
                .ForMember(x => x.EmailAddress, options => options.MapFrom(x => x.Email))
                .ForMember(x => x.UserId, options => options.MapFrom(x => x.Id));
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();
            foreach (var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { PersonId = actor.PersonId, Character = actor.Character });
            }
            return result;
        }

        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenreDTO>();
            foreach (var movieGenre in movie.MoviesGenres)
            {
                result.Add(new GenreDTO() { Id = movieGenre.GenreId, Name = movieGenre.Genre.Name });
            }
            return result;
        }

        private List<ActorDTO> MapMoviesActors(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<ActorDTO>();
            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDTO() { PersonId = actor.PersonId, Character = actor.Character, PersonName = actor.Person.Name });
            }
            return result;
        }
    }
}
