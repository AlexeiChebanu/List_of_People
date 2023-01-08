using ServiceContracts;
using Services;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonGetterService _personGetterService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        private readonly IPersonsRepository _personsRepository;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonDeleterService _personsDeleterService;
        private readonly IPersonUpdaterService _personsUpdaterService;
        private readonly IPersonAdderService _personsAdderService;
        private readonly IPersonSorterService _personsSorterService;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonGetterService>>();

            _personGetterService = new PersonGetterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);

            _personsAdderService = new PersonAdderService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);

            _personsDeleterService = new PersonDeleterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);

            _personsSorterService = new PersonSorterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);

            _personsUpdaterService = new PersonUpdaterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //When supply null value as PersonAddRequest,
        // it should throw ArgumentNullException 
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullExcp()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Assert

            Func<Task> action = async () =>
            {
               await  _personsAdderService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgExcp()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(t => t.PersonName, null as string).Create();

            Person person = personAddRequest.ToPerson();
            //When PersonsRepo.AddPerson is called, it has to return the same "person" obj
            _personRepositoryMock.Setup(t => t.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Assert

           Func<Task> action = async() =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When supply proper details, it should insert the person into
        //persons list;
        //and it should return an obj of PersonResponse, which includes with
        // the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSucsesfull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(t=>t.Email, "some@gmail.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponse_expected = person.ToPersonResponse();

            //if supply any argument value to the AddPerson method,
            //it should return the same return value
            _personRepositoryMock.Setup(t => t.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add =
               await _personsAdderService.AddPerson(personAddRequest);

            personResponse_expected.PersonID = person_response_from_add.PersonID;
            //Assert

            /*Assert.True(person_response_from_add.PersonID != Guid.Empty);*/

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            
            person_response_from_add.Should().Be(personResponse_expected);

        }
        #endregion

        #region GetPersonByPersonID

        //Uf we supply null as PersonId, it should return null aas PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange

            Guid? personID = null;

            //Act
            PersonResponse? personResponseFromGet =
                await _personGetterService.GetPersonByPersonID(personID);

            //Asssert
            personResponseFromGet.Should().BeNull();
        }

        //If we supply a valid perosn id, it should return the valid person
        //details as PersonResponse obj
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSucsesfull()
        {
            //Arange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "some@gmail.com")
             .With(temp => temp.Country, null as Country)
             .Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonsByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get = await _personGetterService.GetPersonByPersonID(person.PersonID);

            //Assert
            person_response_from_get.Should().Be(person_response_expected);
        }

        #endregion

        #region GetAllPersons

        //The GetAlPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_IsEmpty()
        {
            //Arrange
            var persons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_from_get = await _personGetterService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }

        //Add few persons, then call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons_ToBeSucsesfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>().With(t => t.Email, "sample1@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample2@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample3@gmail.com").With(t =>t.Country, null as Country).Create()
            };



            List<PersonResponse> person_response_list_expected = persons.Select(t => t.ToPersonResponse()).ToList();
            _testOutputHelper.WriteLine("Expected:");
            
            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
            _personRepositoryMock.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_list_from_get = await _personGetterService.GetAllPersons();

            //print person_response_list_from_add
            foreach (PersonResponse item in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //   Assert.Contains(person_response_from_add, persons_list_from_get);
            //}
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSucsesfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>().With(t => t.Email, "sample1@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample2@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample3@gmail.com").With(t =>t.Country, null as Country).Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(t => t.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            _personRepositoryMock.Setup(t => t.GetFillteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act

            List<PersonResponse> persons_list_from_search = await _personGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print person_response_list_from_add
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
            //}
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        //Add few persons, then search based on person name with some search string. Should return the mathcing persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSucsesfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>().With(t => t.Email, "sample1@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample2@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample3@gmail.com").With(t =>t.Country, null as Country).Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(t => t.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            _personRepositoryMock.Setup(t => t.GetFillteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act

            List<PersonResponse> persons_list_from_search = await _personGetterService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            //print person_response_list_from_add
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
            //}
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetSortedPersons

        //Sort based on PersonName in Desc, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPesrons_ToBeSucsesfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>().With(t => t.Email, "sample1@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample2@gmail.com").With(t =>t.Country, null as Country).Create(),

           _fixture.Build<Person>().With(t => t.Email, "sample3@gmail.com").With(t =>t.Country, null as Country).Create()
            };



            List<PersonResponse> person_response_list_expected = persons.Select(t => t.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(p => p.GetAllPersons()).ReturnsAsync(persons);
            _testOutputHelper.WriteLine("Expected:");
           
            //print person_response_list_from_add
            foreach (PersonResponse item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            List<PersonResponse> allPersons = await _personGetterService.GetAllPersons();
            //Act
            List<PersonResponse> persons_list_from_sort = await _personsSorterService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.Desc);

            //print person_response_list_from_get
            foreach (PersonResponse item in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            //}

            //persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);

            persons_list_from_sort.Should().BeInDescendingOrder(t => t.PersonName);

        }
        #endregion

        #region UpdatePerson
        //Supply null as PersonUpdateRequest, it should throw ArgumentNullException

        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

           await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //Supply invalid person id, it should throw ArgumentException

        [Fact]
        public async Task UpdatePerson_isIdInvalid_ToBeArgumentExc()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();
            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //PersonName is null, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "some@gmail.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();


            //Act
            var action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //Add a new person and try to update the person and email
        [Fact]
        public async Task UpdatePerson_FullDatails_ToBeSucsesfull()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "some@gmail.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock
             .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            _personRepositoryMock
             .Setup(temp => temp.GetPersonsByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personsUpdaterService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);
        }


        #endregion

        #region DeletePerson

        //Supply a valid PersonId, it should return true
        [Fact]
        public async Task DeletePerson_isValid_ToBeSucsesfull()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "some@gmail.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Female")
             .Create();


            _personRepositoryMock
             .Setup(temp => temp.DeletePersonById(It.IsAny<Guid>()))
             .ReturnsAsync(true);

            _personRepositoryMock
             .Setup(temp => temp.GetPersonsByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonID);

            //Assert
            isDeleted.Should().BeTrue();

        }

        
        [Fact]
        public async Task DeletePerson_isEmpty()
        {
            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert

            //Assert.False(isDeleted);

            isDeleted.Should().BeFalse();

        }

        #endregion
    }
}
