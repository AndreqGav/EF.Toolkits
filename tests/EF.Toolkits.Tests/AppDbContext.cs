using EF.Toolkits.Tests.Models;
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
                .AddCustomSql("animals_view", "SELECT * FROM animals a WHERE a.type = 1", "DROP VIEW  IF EXISTS animals_view");
            
            modelBuilder.Entity<Figure>(entity =>
            {
                entity.BeforeInsert("set_square", "new.square = 0");

                entity.BeforeUpdate("prevent_update_with_negative_square", "IF new.square < 0 THEN raise exception 'square negative' END IF;");
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