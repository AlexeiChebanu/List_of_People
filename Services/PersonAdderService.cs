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
    public class PersonAdderService : IPersonAdderService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonAdderService(IPersonsRepository personsRepository, ILogger<PersonGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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

            await _personsRepository.AddPerson(person);

            /*_db.sp_InsertPerson(person);*/

            //convert the Person obj into PersonResponse type
            return person.ToPersonResponse();
        }
    }
}
