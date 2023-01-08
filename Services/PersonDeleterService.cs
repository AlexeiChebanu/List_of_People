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
    public class PersonDeleterService : IPersonDeleterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonDeleterService(IPersonsRepository personsRepository, ILogger<PersonGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null) throw new ArgumentNullException(nameof(personId));

            Person? person = await _personsRepository.GetPersonsByPersonId(personId.Value);

            if (person == null) return false;

            await _personsRepository.DeletePersonById(personId.Value);


            return true;
        }

    }
}
