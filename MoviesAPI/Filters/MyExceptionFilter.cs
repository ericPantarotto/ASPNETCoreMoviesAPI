using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Filters
{
    public class MyExceptionFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<MyExceptionFilter> logger;

        public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogInformation("Method: {0}, Header: {1}", context.HttpContext.Request.Method, context.HttpContext.Request.Headers);
            logger.LogError(exception: context.Exception, message: context.Exception.Message);
            base.OnException(context);
        }
    }
}
