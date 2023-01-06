using Microsoft.AspNetCore.Mvc.Filters;

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