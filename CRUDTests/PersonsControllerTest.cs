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
        private readonly IPersonGetterService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Mock<IPersonGetterService> _mockPersonService;
        private readonly Mock<ICountriesService> _mockCountriesService;


        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _mockCountriesService = new Mock<ICountriesService>();
            _mockPersonService = new Mock<IPersonGetterService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();

            _countriesService = _mockCountriesService.Object;
            _personService = _mockPersonService.Object;
            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ReturnIndexViewWithPersonsList()
        {
            //Arrange

            List<PersonResponse> personResponses_list = _fixture.Create<List<PersonResponse>>();


            PersonsController personsController = new PersonsController(_personService, _countriesService, _logger);

            _mockPersonService.Setup(t => t.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponses_list);

            _mockPersonService.Setup(t => t.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponses_list);
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

            _mockPersonService.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService, _logger);


            //Act
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
