using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods
    /// of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public double? Age { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) { return false; }

            if(obj.GetType() != typeof(PersonResponse)) { return false; }

            PersonResponse person = (PersonResponse)obj;


            return this.PersonID == person.CountryID 
                && this.PersonName == person.PersonName 
                && this.Email == person.Email &&this.DateOfBirth == person.DateOfBirth
                && this.Gender == person.Gender &&this.CountryID== person.CountryID
                && this.Address == person.Address && this.ReceiveNewsLetters && person.ReceiveNewsLetters;  
        

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            //person => personResopse
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                CountryID = person.CountryID,
                Age = (person.DateOfBirth!= null)? Math.Round((DateTime.Now 
                - person.DateOfBirth.Value).TotalDays/365.25) : null
            };
        }
    }
}
