using Microsoft.AspNetCore.Mvc.Filters;
using System.Runtime.CompilerServices;

namespace CRUD.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IOrderedFilter, IAsyncActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _Key;
        private readonly string _Value;
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
        {
            _logger = logger;
            _Key = key;
            _Value = value;
            Order = order;
        }

        public int Order { get; set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method-before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));


            await next(); // calls the subsequent filter or action

            _logger.LogInformation("{FilterName}.{MethodName} method-after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers[_Key] = _Value;
        }
    }
}
