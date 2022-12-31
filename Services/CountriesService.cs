using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest cant be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(CountryAddRequest));
            }

            //Validation: countryName cant be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: countryName cant be twice
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null) ;
            {
                throw new ArgumentException("Given country name already exists");
            }


            Country country = countryAddRequest.ToCountry();

            country.CountryId = Guid.NewGuid();


            await _countriesRepository.AddCountry(country);

            return country.ToCountryRespone();
        }

        public async Task<List<CountryResponse>> GetAllCountry()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();

            return (await _countriesRepository.GetAllCountries()).Select(p => p.ToCountryRespone()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list =
                await _countriesRepository.GetCountryByCountryId(countryID.Value);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryRespone();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Country"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if ( await _countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                    }
                }
            }

            return countriesInserted;
        }
    }
}