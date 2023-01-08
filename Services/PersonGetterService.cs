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
    public class PersonGetterService : IPersonGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonGetterService(IPersonsRepository personsRepository, ILogger<PersonGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");

            var persons = await _personsRepository.GetAllPersons();


            return persons.Select(n => n.ToPersonResponse()).ToList(); 

            //return _db.sp_GetALlPersons().Select(n => ConvertPersonToPersonResponse(n)).ToList();
        }


        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = await _personsRepository.GetPersonsByPersonId(personID.Value);
            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");
            List<Person> persons;

            using (Operation.Time("Time for Filtered Persons from Database"))
            {
                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _personsRepository.GetFillteredPersons(temp =>
                        temp.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                        await _personsRepository.GetFillteredPersons(temp =>
                        temp.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _personsRepository.GetFillteredPersons(temp =>
                        temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                        await _personsRepository.GetFillteredPersons(temp =>
                        temp.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryID) =>
                         await _personsRepository.GetFillteredPersons(temp =>
                         temp.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                        await _personsRepository.GetFillteredPersons(temp =>
                        temp.Address.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons()
                };
            }//end of "using block" of serilog timings


            _diagnosticContext.Set("Persons", persons);

            return persons.Select(t => t.ToPersonResponse()).ToList();
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

            csvWriter.NextRecord();

            List<PersonResponse> persons = await GetAllPersons();

            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            /*await csvWriter.WriteRecordsAsync(persons);*/
            
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using(ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Address";
                workSheet.Cells["G1"].Value = "Country";
                workSheet.Cells["H1"].Value = "Receive News Letter";

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Aqua);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;

                List<PersonResponse> persons = await GetAllPersons();

                foreach (var person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if(person.DateOfBirth.HasValue)
                        workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Address;
                    workSheet.Cells[row, 7].Value = person.Country;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;

                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();

            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
