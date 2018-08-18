using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class CityInfoContext: DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {
            //the line belows ensures that is the database has not been created yet , that it is created but if it has then nothing is done
            //Database.EnsureCreated();

            //BY using this the migrationfiles are used to create the db and it also allows us to go back and fourth using the up and down method in them..not existing at all to initial and upcoming versions
            Database.Migrate();
        }
        public DbSet<City> Cities { get; set; }

        public DbSet<PointOfInterest> PointOfInterest { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionString");
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
