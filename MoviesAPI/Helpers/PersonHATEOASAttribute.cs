using Microsoft.AspNetCore.Mvc.Filters;
using MoviesAPI.DTOs;
using MoviesAPI.Services;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class PersonHATEOASAttribute: HATEOASAttribute
    {
        private readonly LinksGenerator linksGenerator;

        public PersonHATEOASAttribute(LinksGenerator linksGenerator)
        {
            this.linksGenerator = linksGenerator;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool includeHATEOAS = ShouldIncludeHATEOAS(context);

            if (!includeHATEOAS)
            {
                await next();
                return;
            }

            await linksGenerator.Generate<PersonDTO>(context, next);
        }
    }
}
