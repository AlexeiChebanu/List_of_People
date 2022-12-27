using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonDbContext _db;

        public CountriesService(PersonDbContext personDbContext)
        {
            _db = personDbContext;
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
            if(_db.Countries.Count( p => p.CountryName == countryAddRequest.CountryName)> 0 )
            {
                throw new ArgumentException("Given country name already exists");
            }


            Country country = countryAddRequest.ToCountry();

            country.CountryId = Guid.NewGuid();


            _db.Countries.Add(country);
            _db.SaveChanges();

            return country.ToCountryRespone();
        }

        public List<CountryResponse> GetAllCountry()
        {
            return _db.Countries.Select(p => p.ToCountryRespone()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list =
                _db.Countries.FirstOrDefault(n => n.CountryId == countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryRespone();
        }
    }
}