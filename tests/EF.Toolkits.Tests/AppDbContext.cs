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

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeView> EmployeesView { get; set; }

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

            modelBuilder.Entity<EmployeeView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("EmployeeView");
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

            modelBuilder.Entity<Employee>(builder =>
            {
#if NET7_0_OR_GREATER
                
                builder.ToTable("Employees", t => t.HasComment("Рабочий"));
#else
                builder.HasComment("Рабочий");
                builder.ToTable("Employees");
#endif

                builder.OwnsOne(e => e.Company);

                builder.Navigation(e => e.Company).IsRequired();

                builder.Property(e => e.FirstName).HasComment("Имечко");
            });

            Tpc(modelBuilder);

            Tph(modelBuilder);
        }

        private void Tpc(ModelBuilder modelBuilder)
        {
#if NET8_0_OR_GREATER

            modelBuilder.Entity<BlogBase>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.UseTpcMappingStrategy();
            });

            modelBuilder.Entity<BlogA>();
            modelBuilder.Entity<BlogB>();

#endif
        }

        private void Tph(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(builder =>
            {
                builder.HasKey(entity => entity.Id);

#if NET7_0_OR_GREATER
                builder.UseTphMappingStrategy();
#endif
            });

            modelBuilder.Entity<PostA>(builder => { builder.HasBaseType<Post>(); });

            modelBuilder.Entity<PostB>(builder => { builder.HasBaseType<Post>(); });
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