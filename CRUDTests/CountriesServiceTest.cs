using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
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
            _countriesService = new CountriesService(new PersonDbContext(new DbContextOptionsBuilder<PersonDbContext>().Options));
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            { //Act
               await _countriesService.AddCountry(request);
            }
            );
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            { //Act
                await _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is duplicate, it should throw ArgumentException

        [Fact]
        public async Task AddCountry_DuplacateCountryName()
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
            await Assert.ThrowsAsync<ArgumentException>( async() =>
            { //Act
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            }
            );
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries

        [Fact]
        public async Task AddCountry_ProperCountryDatails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countriesFromGetAllCountries = await _countriesService.GetAllCountry();

            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countriesFromGetAllCountries);
        }
        #endregion

        #region GetAllCountries  

        [Fact]
        //The list of countries should be empty by default
        public async Task GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountry();

            //Assert
            Assert.Empty(actual_country_response_list);

        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
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
                conutnry_list_from_addC.Add(await _countriesService.AddCountry(item));
            }

           List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountry();

            //read each ele,emt from countries
            foreach (var item in conutnry_list_from_addC)
            {
                //Assert
                Assert.Contains(item, actualCountryResponseList);
            }
        }

        #endregion

        #region GetCountryByCountryId

        [Fact]
        //If we supply null as CountryID, it should return null as
        //CountryResponse
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countrID= null;

            //Act
            CountryResponse? country_response_from_get_method = 
               await _countriesService.GetCountryByCountryId(countrID);

            //Assert

            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        //If we supply a valid country id, it should return the mathcing 
        //country details as CountryResponse obj
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = new
                CountryAddRequest()
            {
                CountryName = "Australia"
            };
            CountryResponse country_response_from_add_request =
              await  _countriesService.AddCountry(country_add_request);


            //Act
            
           CountryResponse? country_response_from_get = await
                _countriesService.GetCountryByCountryId(country_response_from_add_request.CountryId);

            //Assert

            Assert.Equal(country_response_from_add_request, country_response_from_get);

        }
        #endregion
    }
}
