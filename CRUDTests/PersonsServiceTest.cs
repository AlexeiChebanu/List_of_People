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
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            ApplicationDbContext dbContext = dbContextMock.Object;

            dbContextMock.CreateDbSetMock(t => t.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(t => t.Persons, personsInitialData);

            _countriesService = new CountriesService(dbContext);

             _countriesService = new CountriesService(dbContext);

             _personService = new PersonService(dbContext, _countriesService);

             _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //When supply null value as PersonAddRequest,
        // it should throw ArgumentNullException 
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Assert

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
               await  _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(t => t.PersonName, null as string).Create();  

            //Assert

            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }

        //When supply proper details, it should insert the person into
        //persons list;
        //and it should return an obj of PersonResponse, which includes with
        // the newly generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(t=>t.Email, "some@gmail.com").Create();

            //Act
            PersonResponse person_response_from_add =
               await _personService.AddPerson(personAddRequest);
            List<PersonResponse> person_list = await _personService.GetAllPersons();
            //Assert

            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            Assert.Contains(person_response_from_add, person_list);
        }
        #endregion

        #region GetPersonByPersonID

        //Uf we supply null as PersonId, it should return null aas PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange

            Guid? personID = null;

            //Act
            PersonResponse? personResponseFromGet =
                await _personService.GetPersonByPersonID(personID);

            //Asssert
            Assert.Null(personResponseFromGet);
        }

        //If we supply a valid perosn id, it should return the valid person
        //details as PersonResponse obj
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample@gmail.com").Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion

        #region GetAllPersons

        //The GetAlPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_IsEmpty()
        {
            //Act

            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();
            //Assert

            Assert.Empty(persons_from_get);
        }

        //Add few persons, then call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample1@gmail.com").Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample2@gmail.com").Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample3@gmail.com").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //print person_response_list_from_add
            foreach (PersonResponse item in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }
        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample1@gmail.com").Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample2@gmail.com").Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(t => t.Email, "sample3@gmail.com").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilterdPersons(nameof(Person.PersonName), "");

            //print person_response_list_from_add
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        //Add few persons, then search based on person name with some search string. Should return the mathcing persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t=> t.PersonName, "Maria").With(t => t.Email, "sample1@gmail.com").Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t => t.PersonName, "Alman").With(t => t.Email, "sample2@gmail.com").Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_2.CountryId).With(t => t.PersonName, "Alex").With(t => t.Email, "sample3@gmail.com").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilterdPersons(nameof(Person.PersonName), "ma");

            //print person_response_list_from_add
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_search);
                    }
                }
            }
        }

        #endregion

        #region GetSortedPersons

        //Sort based on PersonName in Desc, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPesrons()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t => t.PersonName, "Maria").With(t => t.Email, "sample1@gmail.com").Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t => t.PersonName, "Alman").With(t => t.Email, "sample2@gmail.com").Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_2.CountryId).With(t => t.PersonName, "Alex").With(t => t.Email, "sample3@gmail.com").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            //Act
            List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.Desc);

            //print person_response_list_from_get
            foreach (PersonResponse item in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson
        //Supply null as PersonUpdateRequest, it should throw ArgumentNullException

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }

        //Supply invalid person id, it should throw ArgumentException

        [Fact]
        public async Task UpdatePerson_isIdInvalid()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();
            //Assert
            await Assert.ThrowsAsync<ArgumentException>( async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);

            });
        }

        //PersonName is null, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t => t.PersonName, "Maria").With(t => t.Email, "sample1@gmail.com").Create();

            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest? personUpdateRequest= personResponse.ToPersonUpdateRequest();

            personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }

        //Add a new person and try to update the person and email
        [Fact]
        public async Task UpdatePerson_FullDatails()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t => t.PersonName, "Maria").With(t => t.Email, "sample1@gmail.com").Create();


            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest? personUpdateRequest = personResponse.ToPersonUpdateRequest();

            personUpdateRequest.PersonName = "William";
            personUpdateRequest.Email = "will@gmail.com";

            //Act
            PersonResponse personResponseFromUpdate = await _personService.UpdatePerson(personUpdateRequest);

            PersonResponse? personResponseFrom_get =  await _personService.GetPersonByPersonID(personUpdateRequest.PersonID);

            //Assert
            Assert.Equal(personResponseFrom_get, personResponseFromUpdate);
        }


        #endregion

        #region DeletePerson

        //Supply a valid PersonId, it should return true
        [Fact]
        public async Task DeletePerson_isValid()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(t => t.CountryID, country_response_1.CountryId).With(t => t.PersonName, "Maria").With(t => t.Email, "sample1@gmail.com").Create();


            var addPerson = await _personService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = await _personService.DeletePerson(addPerson.PersonID);

            //Assert

            Assert.True(isDeleted);

        }

        
        [Fact]
        public async Task DeletePerson_isEmpty()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert

            Assert.False(isDeleted);

        }

        #endregion
    }
}
