using CRUD.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

/*Notation
 * Action Filter

 * Manipulating & validating the action method parameters.

 * Manipulating the ViewData.

 * Overriding the IActionResult provided by action method.*/

namespace CRUD.Filters.ActionFilters
{
    public class PersonCreateEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesGetterService _countriesService;
        private readonly ILogger<PersonCreateEditPostActionFilter> _logger;

        public PersonCreateEditPostActionFilter(ICountriesGetterService countriesService, ILogger<PersonCreateEditPostActionFilter> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (context.Controller is PersonsController personsController)
            {
                //before logic
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountry();

                    personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];

                    context.Result = personsController.View(personRequest); //short-circuits or ski[s the subsequent action
                }
                else
                {
                    await next(); //invokes the subsequent filter or action method
                }                   
            }
            else
            {
                await next();
            }

            _logger.LogInformation("In after logic of PersonCreateEditPostActionFilter");
        }
    }
}
