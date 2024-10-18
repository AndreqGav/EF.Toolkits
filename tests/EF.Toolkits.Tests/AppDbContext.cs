using System.Collections.Generic;
using EF.Toolkits.Tests.Models;
using EF.Toolkits.Tests.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Toolkits.CustomSql;
using Toolkits.Triggers;

namespace EF.Toolkits.Tests
{
    public class AppDbContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; }

        public DbSet<Figure> Figures { get; set; }

        public DbSet<Emploee> Emploees { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddCustomSql("animals_view", "SELECT * FROM \"Animals\" a WHERE a.\"AnimalType\" = 1",
                    "DROP VIEW IF EXISTS animals_view");

            modelBuilder.Entity<Figure>(entity =>
            {
                entity.BeforeInsert("set_square", "new.square = 0;");

                entity.BeforeUpdate("prevent_update_with_negative_square",
                    "IF new.square < 0 THEN raise exception 'square negative'; END IF;");
            });

            modelBuilder.Entity<Animal>(entity =>
            {
                var triggersGenerator = new TriggersGenerator(this, modelBuilder);

                entity.BeforeInsertOrUpdate("before_insert_or_update", triggersGenerator.GenerateTriggersScript());
            });
            
            modelBuilder
                .AddCustomSql("get_name", AppDbFunctions.GetNameSqlUp(), AppDbFunctions.GetNameSqlDown())
                .HasDbFunction(typeof(AppDbFunctions).GetMethod(nameof(AppDbFunctions.GetName))!)
                .HasName("get_name");

            modelBuilder.Entity<Animal>()
                .HasData(new List<Animal>
                {
                    new Animal
                    {
                        Id = 1,
                        Name = "Batman",
                        Species = "SuperCat",
                        AnimalType = AnimalType.Cat
                    },
                    new Animal
                    {
                        Id = 2,
                        Name = "Mammal",
                        Species = "Doggs",
                        AnimalType = AnimalType.Dog
                    },
                    new Animal
                    {
                        Id = 3,
                        Name = "Freddy",
                        Species = "MegaFish",
                        AnimalType = AnimalType.Fish
                    },
                });
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            DbContextConfigurator.Configure(optionsBuilder);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}