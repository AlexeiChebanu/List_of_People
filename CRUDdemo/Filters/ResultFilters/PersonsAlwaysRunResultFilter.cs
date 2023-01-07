using Microsoft.AspNetCore.Mvc.Filters;


/*Result Filter

Preventing IActionResult from execution.

Adding last-moment changes to response (such as adding response headers).*/

namespace CRUD.Filters.ResultFilters
{
    public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        void IResultFilter.OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                return;
            }
        }
    }
}