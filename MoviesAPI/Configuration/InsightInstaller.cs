using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MoviesAPI.Configuration
{
    public class InsightInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry();
        }
    }
}
