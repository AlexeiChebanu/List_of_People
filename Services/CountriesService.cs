using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {

        //private field

        private readonly List<Country> _countries;

        public CountriesService()
        {
            _countries= new List<Country>();
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