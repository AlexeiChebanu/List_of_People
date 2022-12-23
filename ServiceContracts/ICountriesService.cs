using Entities;
using ServiceContracts.DTO;

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
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries fro, the list
        /// </summary>
        /// <returns></returns>
        List<CountryResponse> GetAllCountry();

        /// <summary>
        /// Returns a country obj based on the given country id
        /// </summary>
        /// <param name="CountryID">CountryID(guid) to search</param>
        /// <returns>Matching cpuntry as CountryResponse obj</returns>
        CountryResponse? GetCountryByCountryId(Guid? CountryID);
    }
}

