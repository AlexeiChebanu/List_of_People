using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<List<CountryResponse>> GetAllCountry()
        {
            List<Country> countries = await _countriesRepository.GetAllCountry();

            return countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list =
                await _countriesRepository.GetCountryByCountryId(countryID.Value);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }

    }
}