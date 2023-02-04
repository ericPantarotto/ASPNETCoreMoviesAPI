using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoviesAPI.Filters;
using MoviesAPI.Helpers;

namespace MoviesAPI.Configuration
{
    public class ControllersInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
           services.AddHttpContextAccessor();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
                options.Conventions.Add(new GroupingByNamespaceConvention());
            })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();
            
        }
    }
}