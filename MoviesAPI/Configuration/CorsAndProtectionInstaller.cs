using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MoviesAPI.Configuration
{
    public class CorsAndProtectionInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();
            
            //DEBUG: if we want to customize it at controller or endpoint level
            // services.AddCors(options => 
            // {
            //     options.AddPolicy("AllowResttesttest",
            //     builder => builder.WithOrigins("https://resttesttest.com")
            //         .WithMethods("GET", "POST")
            //         .AllowAnyHeader());
            // });

            services.AddDataProtection();
        }
    }
}