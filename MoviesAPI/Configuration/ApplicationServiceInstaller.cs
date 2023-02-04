using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Configuration
{
    public class ApplicationServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<GenreHATEOASAttribute>();
            services.AddTransient<PersonHATEOASAttribute>();
            services.AddTransient<LinksGenerator>();

            //services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddTransient<IFileStorageService, AzureStorageService>();

            services.AddTransient<IHostedService, MovieInTheatersService>();

            services.AddTransient<HashService>();
        }
    }
}