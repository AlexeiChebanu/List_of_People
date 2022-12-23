using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;



        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }


        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            { //Act
                _countriesService.AddCountry(request);
            }
            );
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            { //Act
                _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is duplicate, it should throw ArgumentException

        [Fact]
        public void AddCountry_DuplacateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };


            //Assert
            Assert.Throws<ArgumentException>(() =>
            { //Act
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            }
            );
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries

        [Fact]
        public void AddCountry_ProperCountryDatails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            

            //Act
            CountryResponse response = _countriesService.AddCountry(request);
            List<CountryResponse> countriesFromGetAllCountries = _countriesService.GetAllCountry();

            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countriesFromGetAllCountries);
        }
        #endregion

        #region GetAllCountries  

        [Fact]
        //The list of countries should be empty by default
        public void GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountry();

            //Assert
            Assert.Empty(actual_country_response_list);

        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName= "USA" },
                new CountryAddRequest() { CountryName= "Canada" },
                new CountryAddRequest() { CountryName= "Ukraine" }

            };


            //Act
            List<CountryResponse> conutnry_list_from_addC = new List<CountryResponse>();

            foreach (CountryAddRequest item in country_request_list)
            {
                conutnry_list_from_addC.Add(_countriesService.AddCountry(item));
            }

           List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountry();

            //read each ele,emt from countries
            foreach (var item in conutnry_list_from_addC)
            {
                //Assert
                Assert.Contains(item, actualCountryResponseList);
            }
        }

        #endregion


    }
}
