using AutoFixture;
using Moq;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using CRUD.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonGetterService _personGetterService;
        private readonly ICountriesGetterService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly IPersonDeleterService _personsDeleterService;
        private readonly IPersonUpdaterService _personsUpdaterService;
        private readonly IPersonAdderService _personsAdderService;
        private readonly IPersonSorterService _personsSorterService;



        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Mock<IPersonGetterService> _mockPersonGetterService;
        private readonly Mock<ICountriesGetterService> _mockCountriesService;

        private readonly Mock<IPersonUpdaterService> _mockPersonUpdaterService;
        private readonly Mock<IPersonDeleterService> _mockPersonDeleterService;
        private readonly Mock<IPersonAdderService> _mockPersonAdderService;
        private readonly Mock<IPersonSorterService> _mockPersonSorterService;



        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _mockCountriesService = new Mock<ICountriesGetterService>();
            _mockPersonGetterService = new Mock<IPersonGetterService>();
            _mockPersonUpdaterService = new Mock<IPersonUpdaterService>();
            _mockPersonDeleterService = new Mock<IPersonDeleterService>();
            _mockPersonSorterService = new Mock<IPersonSorterService>();
            _mockPersonAdderService = new Mock<IPersonAdderService>();

            _loggerMock = new Mock<ILogger<PersonsController>>();



            _countriesService = _mockCountriesService.Object;
            _personGetterService = _mockPersonGetterService.Object;
            _personsUpdaterService = _mockPersonUpdaterService.Object;
            _personsSorterService = _mockPersonSorterService.Object;
            _personsAdderService = _mockPersonAdderService.Object;
            _personsDeleterService = _mockPersonDeleterService.Object;

            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ReturnIndexViewWithPersonsList()
        {
            //Arrange

            List<PersonResponse> personResponses_list = _fixture.Create<List<PersonResponse>>();


            PersonsController personsController = new PersonsController(_personGetterService, _personsAdderService, _personsDeleterService, _personsSorterService, _personsUpdaterService, _countriesService, _logger);

            _mockPersonGetterService.Setup(t => t.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponses_list);

            _mockPersonSorterService.Setup(t => t.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponses_list);
            //Act
           IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personResponses_list);
        }


        #endregion

        #region Create
        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _mockCountriesService.Setup(temp => temp.GetAllCountry()).ReturnsAsync(countries);

            _mockPersonAdderService.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personGetterService, _personsAdderService, _personsDeleterService, _personsSorterService, _personsUpdaterService, _countriesService, _logger);



            //Act
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
