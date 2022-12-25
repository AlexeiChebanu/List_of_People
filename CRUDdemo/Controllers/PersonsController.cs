using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUD.Controllers
{
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonService _personsService;

        public PersonsController(IPersonService personsService)
        {
            _personsService = personsService;
        }

        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index()
        {
           List<PersonResponse> persons = _personsService.GetAllPersons();

            return View(persons);
        }
    }
}
