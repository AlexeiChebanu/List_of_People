using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions options) : base (options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJSON = File.ReadAllText("countries.json");

           List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJSON);


            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            string personsJSON = File.ReadAllText("person.json");

            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJSON);


            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

        }

        public List<Person> sp_GetALlPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),           
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
        }
    }
}
