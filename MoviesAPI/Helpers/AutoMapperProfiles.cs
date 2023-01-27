﻿using AutoMapper;
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
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<PersonUpdateDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<Person, PersonPatchDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore());
            CreateMap<Movie, MoviePatchDTO>().ReverseMap();
        }
    }
}
