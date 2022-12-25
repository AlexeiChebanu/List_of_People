using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries= new List<Country>();
            if (initialize)
            {
               _countries.AddRange(new List<Country>() {
               new Country() 
               { 
                   CountryId = Guid.Parse("878B7EF4-CA66-4AC1-BE8E-7858283F69D5"),
                    CountryName = "Ukraine" },
                new Country()
                {
                    CountryId = Guid.Parse("04F7D9A9-D4D8-487F-BC1C-2E13D214697B"),
                    CountryName = "Canada"
                },
                new Country()
                {
                    CountryId = Guid.Parse("1DA41B97-6E89-4025-B6C5-2C59D8C2FBC0"),
                    CountryName = "USA"
                },
                new Country()
                {
                    CountryId = Guid.Parse("2BFEFFEC-C470-4287-B4E2-1E28DEC73024"),
                    CountryName = "Poland"
                },
                new Country()
                {
                    CountryId = Guid.Parse("E4DE052C-4994-4415-8427-2CAD7AFB3524"),
                    CountryName = "UK"
                }
            });
                
                
            }
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest cant be null
            if(countryAddRequest==null)
            {
                throw new ArgumentNullException(nameof(CountryAddRequest));
            }

            //Validation: countryName cant be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: countryName cant be twice
            if(_countries.Where( p => p.CountryName == countryAddRequest.CountryName).Count() > 0 )
            {
                throw new ArgumentException("Given country name already exists");
            }


            Country country = countryAddRequest.ToCountry();

            country.CountryId = Guid.NewGuid();


            _countries.Add(country);

            return country.ToCountryRespone();
        }

        public List<CountryResponse> GetAllCountry() => _countries
            .Select(p => p.ToCountryRespone()).ToList();

        public CountryResponse? GetCountryByCountryId(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list =
                _countries.FirstOrDefault(n => n.CountryId == countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryRespone();
        }
    }
}