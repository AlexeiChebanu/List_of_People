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
    public interface ICountriesGetterService
    { 
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
    }
}

