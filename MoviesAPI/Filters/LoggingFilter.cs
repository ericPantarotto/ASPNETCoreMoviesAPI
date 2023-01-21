using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MoviesAPI.Filters
{
    public class LoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingFilter> logger;

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            this.logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            logger.LogInformation("Before endpoint stuff");

            await next();

            logger.LogInformation("After endpoint stuff");
        }
    }
}
