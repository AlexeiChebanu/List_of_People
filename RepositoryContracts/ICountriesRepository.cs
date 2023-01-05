using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country obj to the data store
        /// </summary>
        /// <param name="country"></param>
        /// <returns>Return the country obj after</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All coutries from the table</returns>
        Task<List<Country>> GetAllCountry();
        /// <summary>
        /// Returns a country obj based on the given country id, otherwise, it returns null
        /// </summary>
        /// <param name="countryId">CountryId to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryId(Guid countryId);
        /// <summary>
        /// Returns a country obj based on the given country name
        /// </summary>
        /// <param name="countryName">Country name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}