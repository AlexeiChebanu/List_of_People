using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Services;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonService _personService;

        public PersonsServiceTest()
        {
            _personService = new PersonService();
        }

        #region AddPerson
        //When supply null value as PersonAddRequest,
        // it should throw ArgumentNullException 
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Assert

            Assert.Throws<ArgumentNullException>(() =>
            {
                _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };

            //Assert

            Assert.Throws<ArgumentException>(() =>
            {
                _personService.AddPerson(personAddRequest);
            });
        }

        //When supply proper details, it should insert the person into
        //persons list;
        //and it should return an obj of PersonResponse, which includes with
        // the newly generated person id
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person Name",
                Email = "per@gmail.com",
                Address = "sample add",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true

            };
            //Act
            PersonResponse person_response_from_add =
                _personService.AddPerson(personAddRequest);
            List<PersonResponse> person_list = _personService.GetAllPersons();
            //Assert

            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            Assert.Contains(person_response_from_add, person_list);
        }
        #endregion

    }
}
