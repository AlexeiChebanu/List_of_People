using Entities;
using ServiceContracts.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace ServiceContracts
{

    /// <summary>
    /// Represents business logic for manippulating
    /// Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        ///  Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it
        /// (including newly generated country id) </returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries fro, the list
        /// </summary>
        /// <returns></returns>
        Task<List<CountryResponse>> GetAllCountry();

        /// <summary>
        /// Returns a country obj based on the given country id
        /// </summary>
        /// <param name="CountryID">CountryID(guid) to search</param>
        /// <returns>Matching cpuntry as CountryResponse obj</returns>
        Task<CountryResponse?> GetCountryByCountryId(Guid? CountryID);
        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}

