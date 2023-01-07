using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        private string? Key { get; set; }
        private string? Value { get; set; }
        private int Order { get; set; }

        public bool IsReusable => false;

        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }

        //Controller -> FilterFactory -> Filter
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;
            //return filter obj
            return filter;
        }
    }
    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        public string Key { get; set; }
        public string Value { get; set; }

        public int Order { get; set; }
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            _logger.LogInformation("Before logic - ResponseHeaderActionFilter");


            await next(); // calls the subsequent filter or action

            _logger.LogInformation("After logic - ResponseHeaderActionFilter");

            context.HttpContext.Response.Headers[Key] = Value;


        }
    }
}
