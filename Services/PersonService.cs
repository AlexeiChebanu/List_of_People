using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonService : IPersonService
    {

        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;
        public PersonService()
        {
            _persons= new List<Person>();
            _countriesService= new CountriesService();
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

            //Validate PersonName
            if(string.IsNullOrEmpty(personAddRequest.PersonName))
            {
                throw new ArgumentException("PersonName cant be empty");
            }

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            _persons.Add(person);

            //convert the Person obj into PersonResponse type
            return ConvertPersonToPersonResponse(person);


        }

        public List<PersonResponse> GetAllPersons()
        {
            throw new NotImplementedException();
        }
    }
}
