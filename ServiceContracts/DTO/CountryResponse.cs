using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class is used as return type for most of
    /// CountriesServece method
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryrespone(this Country country)
        {
            return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}


