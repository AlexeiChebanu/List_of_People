using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //add after logic here
            _logger.LogInformation("PersonsOnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //add before logic
            _logger.LogInformation("PersonsOnActionExecuting");
        }
    }
}
