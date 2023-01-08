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
    public interface ICountriesUploaderService
    {
        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}

