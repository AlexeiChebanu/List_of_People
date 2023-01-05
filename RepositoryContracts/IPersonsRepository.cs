using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Persons entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person obj to the data store 
        /// </summary>
        /// <param name="person">Person obj to add</param>
        /// <returns>Returns the person obj after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>List of person obj from table</returns>
        Task<List<Person>> GetAllPersons();
        /// <summary>
        /// Returns a person obj based on the given person id
        /// </summary>
        /// <param name="personId">PersonId(guid) to search</param>
        /// <returns>A person obj or null</returns>
        Task<Person?> GetPersonsByPersonId(Guid personId);
        /// <summary>
        /// Returns all person objs based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFillteredPersons(Expression<Func<Person, bool>> predicate);
        /// <summary>
        /// Deletes a person obj based on the person id
        /// </summary>
        /// <param name="personId">Person Id(guid) to search</param>
        /// <returns>Returns true, if the deletion is successful , otherwise false</returns>
        Task<bool> DeletePersonById(Guid personId);
        /// <summary>
        /// Updates a person obj(person name and other details) based on the given person id
        /// </summary>
        /// <param name="person">Person obj to update</param>
        /// <returns>Returns the upd person obj</returns>
        Task<Person> UpdatePerson(Person person);

    }
}
