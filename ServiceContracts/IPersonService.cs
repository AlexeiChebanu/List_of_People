using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonService
    {
        /// <summary>
        /// Adds a new person into the list of persons
        /// </summary>
        /// <param name="personAddRequest"></param>
        /// <returns></returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);
        /// <summary>
        /// Returns all presons
        /// </summary>
        /// <returns></returns>
        List<PersonResponse> GetAllPersons();
    }
}
