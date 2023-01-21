using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Filters
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this.logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Method: {0}, Header: {1}", context.HttpContext.Request.Method, context.HttpContext.Request.Headers);
            logger.LogWarning("on action executing;");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogWarning("on action executed;");
        }

    }
}
