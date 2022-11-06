using CityInfo.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.DbContexts;

public class CityInfoContext : DbContext
{

    public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
    {
        //by exposing this constructor we can provide these options when we register our dbcontext
    }


    public DbSet<City> Cities { get; set; } = null!;

    public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>()
            .HasData(
            new City("New York City")
            {
                Id = 1,
                Description = "The one with big park"
            },
            new City("Antwerp")
            {
                Id = 2,
                Description = "The one with the cathedral that was never really finished"
            },
            new City("Paris")
            {
                Id = 3,
                Description = "The one with big tower."
            }

            );

        modelBuilder.Entity<PointOfInterest>()
            .HasData(
            new PointOfInterest("The Louvre")
            {
                Id = 1,
                CityId = 3,
                Description = "The world's largest musuem"

            },
           new PointOfInterest("Eiffel Tower")
           {
               Id = 2,
               CityId = 3,
               Description = "An iron tower"


           }, 
           new PointOfInterest("Central Park")
           {
               Id = 3,
               CityId =1,
               Description = "The most visited urban park in the United States"
           },
           new PointOfInterest("Empire State Building")
           {
               Id = 4,
               CityId = 1,
               Description = "A skyscrapper located in mid-town Manhatten"
           },
             new PointOfInterest("Cathedral")
             {
                 Id = 5,
                 CityId = 2,
                 Description = "A christian place of worship"
             }




            );



        base.OnModelCreating(modelBuilder);
    }


    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite("connectionString"); //tells the dbcontext it's being used to connect to a sqllite db
    //    base.OnConfiguring(optionsBuilder);
    //}
}

