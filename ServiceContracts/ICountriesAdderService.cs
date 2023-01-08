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
    public interface ICountriesAdderService
    {
        /// <summary>
        ///  Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it
        /// (including newly generated country id) </returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    }
}

