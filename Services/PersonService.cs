using Entities;
using Microsoft.VisualBasic;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly PersonDbContext _db;
        private readonly ICountriesService _countriesService;
        public PersonService(PersonDbContext personDbContext, ICountriesService countriesService)
        {
            _db = personDbContext;
            _countriesService = countriesService;
        }
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryId(person.CountryID)?.CountryName;
            return personResponse;

        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //check if PersonAddRequest is not null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //ModelValidation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            _db.Persons.Add(person);
            _db.SaveChanges();

            /*_db.sp_InsertPerson(person);*/

            //convert the Person obj into PersonResponse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        { 
            return _db.Persons.ToList().Select(n => ConvertPersonToPersonResponse(n)).ToList(); 

            //return _db.sp_GetALlPersons().Select(n => ConvertPersonToPersonResponse(n)).ToList();


        }


        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
                return null;

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilterdPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.PersonName) ? temp
                    .PersonName
                    .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp =>
                            (!string.IsNullOrEmpty(temp.Email) ? temp
                            .Email
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp =>
                            (temp.DateOfBirth != null) ? temp
                            .DateOfBirth.Value.ToString("dd MMMM yyyy")
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp =>
                            (!string.IsNullOrEmpty(temp.Gender) ? temp
                            .Gender
                            .StartsWith(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp =>
                            (!string.IsNullOrEmpty(temp.Country) ? temp
                            .Country
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp =>
                            (!string.IsNullOrEmpty(temp.Address) ? temp
                            .Address
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.PersonName,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.PersonName,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.Email,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.Email,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.DateOfBirth)
                                                                                .ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.DateOfBirth)
                                                                                .ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.Age)
                                                                                .ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.Age)
                                                                                .ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.Gender,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.Gender,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.Country,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.Country,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.Address,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.Address,
                                                                                StringComparer.OrdinalIgnoreCase)
                                                                                .ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.Asc) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters)
                                                                                .ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.Desc) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters)
                                                                                .ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person obj to update
            Person? matchingPerson =
                _db.Persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);

            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesnt exist");
            }

            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            _db.SaveChanges(); //upd

            return ConvertPersonToPersonResponse(matchingPerson);

        }

        public bool DeletePerson(Guid? personId)
        {
            if (personId == null) throw new ArgumentNullException(nameof(personId));

            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personId);

            if (person == null) return false;

            _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personId));

            _db.SaveChanges();

            return true;
        }
    }
}
