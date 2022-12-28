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
    }
}
