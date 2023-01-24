﻿using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using System.Diagnostics.CodeAnalysis;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }

    }
}