using EF.Toolkits.CustomSql.Extensions;
using EF.Toolkits.CustomSql.Triggers;
using EF.Toolkits.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
            modelBuilder.AddCustomSql("Test", "Body_Up", "Body_Down");

            modelBuilder.Entity<Animal>(entity =>
            {
                entity.BeforeInsert("Test_BeforeInsert", "SELECT 1");
                
                entity.AfterDelete("Test_AfterDelete", "SELECT 1");
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