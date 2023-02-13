using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesAPI.Data;
using System.Threading.Tasks;

namespace MoviesAPI
{
    public class Program
    {
        public static async  Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            IHost webHost = CreateHostBuilder(args).Build();
            
            using var scope = webHost.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            await context.Database.MigrateAsync();

            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
