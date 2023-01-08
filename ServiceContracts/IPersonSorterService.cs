using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonSorterService
    {
        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property(Key), based on which the persons
        /// should be sorted</param>
        /// <param name="sortOrder">Asc or Decs</param>
        /// <returns>Returns sorted persons as PersonResponse List</returns>
       Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

    }
}
