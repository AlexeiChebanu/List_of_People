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

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(new PersonDbContext(new DbContextOptionsBuilder<PersonDbContext>().Options));

            _personService = new PersonService(new PersonDbContext(new DbContextOptionsBuilder<PersonDbContext>().Options), _countriesService);
            
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
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };

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
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest() {
                PersonName = "person name...",
                Email = "email@sample.com",
                Address = "address",
                CountryID = country_response.CountryId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "Poland" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryId,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest() {
                PersonName = "Mary",
                Email = "mary@gmail.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest() {
                PersonName = "Roman",
                Email = "roman@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "Poland" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryId,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@gmail.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Roman",
                Email = "roman@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "Poland" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryId,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@gmail.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Roman",
                Email = "roman@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "Poland" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryId,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@gmail.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Roman",
                Email = "roman@gmail.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryId,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

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
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };

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
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = countryResponse.CountryId,
                Email = "john@gmail.com",
                Gender= GenderOptions.Male
            };

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
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = countryResponse.CountryId,
                Address = "Address",
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Email = "John@gmail.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters= true
            };

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
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "Germany"
            };
            var country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Janiten",
                Address = "address",
                CountryID = country_response_from_add.CountryId,
                DateOfBirth = Convert.ToDateTime("2000-01-01"),
                Email = "janiten@gmail.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

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
