using CRUD.Filters;
using CRUD.Filters.ActionFilters;
using CRUD.Filters.Authorization;
using CRUD.Filters.ExceptionFilter;
using CRUD.Filters.ResourceFilters;
using CRUD.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Reflection;

namespace CRUD.Controllers
{
    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-_Key-From-Controller", "My-_Value-From-Controller", 3 }, Order = 3)]
    [ResponseHeaderFilterFactory("My-Key-From-Controller", "My-Value-From-Controller", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]

    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonGetterService _personsService;
        private readonly IPersonDeleterService _personsDeleterService;
        private readonly IPersonUpdaterService _personsUpdaterService;
        private readonly IPersonAdderService _personsAdderService;
        private readonly IPersonSorterService _personsSorterService;

        private readonly ICountriesGetterService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonGetterService personsGetterService, IPersonAdderService personAdderService, IPersonDeleterService personDeleterService, IPersonSorterService personSorterService, IPersonUpdaterService personUpdaterService, ICountriesGetterService countriesService, ILogger<PersonsController> logger)
        {
            _personsService = personsGetterService;
            _countriesService = countriesService;
            _logger = logger;
            _personsAdderService = personAdderService;
            _personsUpdaterService = personUpdaterService;
            _personsSorterService = personSorterService;
            _personsDeleterService = personDeleterService;

        }

        [Route("[action]")]
        [Route("/")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]

        [ResponseHeaderFilterFactory("MyKey-FromAction", "MyValue-From-Action", 1)]

        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName),
            SortOrderOptions sortOrder = SortOrderOptions.Asc)
        {

            _logger.LogInformation("Index action method of PersonsController");

            _logger.LogDebug($"SearchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");
            //Search


            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);


            return View(sortedPersons);
        }

        //Executes when the user clicks on "Create Person" hyperlink
        //while opening the create view
        [Route("[action]")]
        [HttpGet]
        [ResponseHeaderFilterFactory("my-Key", "my-Value", 4)]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountry();
            ViewBag.Countries = countries.Select(t => new SelectListItem()
            {
                Text = t.CountryName,
                Value = t.CountryId.ToString()
            });
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisableResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            
            //call the service method
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest);

            //navigate to Index() action method(it makes another ger request to "persons/indexf")
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountry();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]       
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
            
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid? personId)
        {
           PersonResponse? response = await _personsService.GetPersonByPersonID(personId);
            if(response == null)
            {
                return RedirectToAction("Index");
            }
            return View(response);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
           PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

           await _personsDeleterService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get list of persons
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            //return View as pdf
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20, Right= 20, Bottom = 20, Left = 20
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
           MemoryStream memoryStream = await _personsService.GetPersonsCSV();

            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}