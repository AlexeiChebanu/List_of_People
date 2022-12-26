using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName),
            SortOrderOptions sortOrder = SortOrderOptions.Asc)
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" },

            };

            List<PersonResponse> persons = _personsService.GetFilterdPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        //Executes when the user clicks on "Create Person" hyperlink
        //while opening the create view
        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountry();
            ViewBag.Countries = countries.Select(t => new SelectListItem()
            {
                Text = t.CountryName,
                Value = t.CountryId.ToString()
            });



            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountry();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            //call the service method
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            //navigate to Index() action method(it makes another ger request to "persons/indexf")
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = _countriesService.GetAllCountry();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = _countriesService.GetAllCountry();
                ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public IActionResult Delete(Guid? personId)
        {
           PersonResponse? response = _personsService.GetPersonByPersonID(personId);
            if(response == null)
            {
                return RedirectToAction("Index");
            }
            return View(response);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public IActionResult Delete(PersonUpdateRequest personUpdateRequest)
        {
           PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            _personsService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index");
        }
    }
}