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

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonService> _mockPersonService;
        private readonly Mock<ICountriesService> _mockCountriesService;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _mockCountriesService = new Mock<ICountriesService>();
            _mockPersonService = new Mock<IPersonService>();

            _countriesService = _mockCountriesService.Object;
            _personService = _mockPersonService.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ReturnIndexViewWithPersonsList()
        {
            //Arrange

            List<PersonResponse> personResponses_list = _fixture.Create<List<PersonResponse>>();


            PersonsController personsController = new PersonsController(_personService, _countriesService);

            _mockPersonService.Setup(t => t.GetFilterdPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponses_list);

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
        public async void Create_IfModelErrors_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _mockCountriesService.Setup(temp => temp.GetAllCountry()).ReturnsAsync(countries);

            _mockPersonService.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService);


            //Act
            personsController.ModelState.AddModelError("PersonName", "Person Name cant be blank");

            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

            viewResult.ViewData.Model.Should().Be(person_add_request);
        }


        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _mockCountriesService.Setup(temp => temp.GetAllCountry()).ReturnsAsync(countries);

            _mockPersonService.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService);


            //Act
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
