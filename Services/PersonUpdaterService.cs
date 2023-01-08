using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonUpdaterService : IPersonUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonUpdaterService(IPersonsRepository personsRepository, ILogger<PersonGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(PersonUpdateRequest));

            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person obj to update
            Person? matchingPerson =
               await _personsRepository.GetPersonsByPersonId(personUpdateRequest.PersonID);

            if (matchingPerson == null)
            {
                throw new InvalidPersonIdException("Given person id doesnt exist");
            }

            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson); //upd

            return matchingPerson.ToPersonResponse();

        }

    }
}
