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
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;
        public PersonService(bool initialize = true)
        {
            _persons= new List<Person>();
            _countriesService= new CountriesService();

            if(initialize)
            {
                _persons.Add(new Person()
                {
                    PersonID= Guid.Parse("051B64A1-0E7C-4A6E-9FAF-E94A8F983534"),
                    PersonName = "Hillary",
                    Email = "hlovat0@gnu.org",
                    DateOfBirth = DateTime.Parse("1990-03-29"),
                    Gender = "Male", 
                    Address = "81 Rusk Lane",
                    ReceiveNewsLetters= false,
                    CountryID = Guid.Parse("878B7EF4-CA66-4AC1-BE8E-7858283F69D5")
                 });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("63318F84-92ED-4375-9E7D-C347DC889022"),
                    PersonName = "Llywellyn",
                    Email = "lchstney1@npr.org",
                    DateOfBirth = DateTime.Parse("1997-05-12"),
                    Gender = "Male",
                    Address = "28623 Mallory Court",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("04F7D9A9-D4D8-487F-BC1C-2E13D214697B")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("37A39B66-FA39-43D8-8370-165E7423292A"),
                    PersonName = "Celestyn",
                    Email = "cchin2@myspace.com",
                    DateOfBirth = DateTime.Parse("1993-11-08"),
                    Gender = "Female",
                    Address = "8 Lindbergh Parkway",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("1DA41B97-6E89-4025-B6C5-2C59D8C2FBC0")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("FD995D0C-DDAC-49CC-A331-D07AE1BAB7AE"),
                    PersonName = "Dimitri",
                    Email = "dduignan3@diigo.com",
                    DateOfBirth = DateTime.Parse("1996-02-20"),
                    Gender = "Male",
                    Address = "365 Kim Junction",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("2BFEFFEC-C470-4287-B4E2-1E28DEC73024")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("DA4BA557-5EDD-497E-8B82-39C314971FDC"),
                    PersonName = "Emanuele",
                    Email = "eheikkinen4@cnbc.com",
                    DateOfBirth = DateTime.Parse("1993-09-30"),
                    Gender = "Male",
                    Address = "8140 Washington Avenue",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("E4DE052C-4994-4415-8427-2CAD7AFB3524")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("9BE192B3-5E30-427C-A571-4EEBB1F3249D"),
                    PersonName = "Opalina",
                    Email = "ogerrit5@ovh.net",
                    DateOfBirth = DateTime.Parse("1998-02-28"),
                    Gender = "Female",
                    Address = "36488 Thierer Terrace",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("878B7EF4-CA66-4AC1-BE8E-7858283F69D5")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("C42E4565-57A5-4345-9AAB-7955B21ECCE0"),
                    PersonName = "Bowie",
                    Email = "barlidge6@surveymonkey.com",
                    DateOfBirth = DateTime.Parse("1990-03-30"),
                    Gender = "Male",
                    Address = "3449 Pankratz Circle",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("04F7D9A9-D4D8-487F-BC1C-2E13D214697B")
                });
                
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("EBD680CF-A80E-41F3-B976-FE294A36CD63"),
                    PersonName = "Sherie",
                    Email = "slamps7@ihg.com",
                    DateOfBirth = DateTime.Parse("1990-10-16"),
                    Gender = "Female",
                    Address = "36 Sunfield Park",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("1DA41B97-6E89-4025-B6C5-2C59D8C2FBC0")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("DDDDC499-DB9E-4A7A-A746-1E8FF38D652C"),
                    PersonName = "Rebecca",
                    Email = "rpecey8@princeton.edu",
                    DateOfBirth = DateTime.Parse("1997-03-25"),
                    Gender = "Female",
                    Address = "97089 Shopko Pass",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("2BFEFFEC-C470-4287-B4E2-1E28DEC73024")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("F1718F4A-88CD-4743-A1D6-98A9048740E0"),
                    PersonName = "Sella",
                    Email = "ssnoday9@amazonaws.com",
                    DateOfBirth = DateTime.Parse("1999-09-26"),
                    Gender = "Female",
                    Address = "8 Fremont Pass",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("E4DE052C-4994-4415-8427-2CAD7AFB3524")
                });            
            }
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
            if(personAddRequest == null) 
                throw new ArgumentNullException(nameof(personAddRequest));

            //ModelValidation
            ValidationHelper.ModelValidation(personAddRequest);
           
            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            _persons.Add(person);

            //convert the Person obj into PersonResponse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons() 
            => _persons.Select(n => n.ToPersonResponse()).ToList();


        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilterdPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch(searchBy)
            {
                case nameof(PersonResponse.PersonName):
                            matchingPersons = allPersons.Where(temp => 
                            (!string.IsNullOrEmpty(temp.PersonName)?temp
                            .PersonName
                            .Contains(searchString, StringComparison.OrdinalIgnoreCase): true)).ToList();
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
                _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);

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

            return matchingPerson.ToPersonResponse();

        }

        public bool DeletePerson(Guid? personId)
        {
            if (personId == null) throw new ArgumentNullException(nameof(personId));

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personId);

            if (person == null) return false;

            _persons.RemoveAll(temp => temp.PersonID == personId);

            return true;
        }
    }
}
