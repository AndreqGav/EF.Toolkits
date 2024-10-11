using EF.Toolkits.AutoComments;
using EF.Toolkits.CustomSql;
using EF.Toolkits.CustomSql.Triggers.Postgresql;
using Microsoft.EntityFrameworkCore;

namespace EF.Toolkits.Tests
{
    public class DbContextConfigurator
    {
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("Host=localhost;Port=5432;Database=EF.Toolkits.Tests;Username=postgres;Password=aSTraNTudeGI")
                .UseCustomSql(options => options.UseTriggers())
                .UseAutoComments("Comments.xml");
        }
    }
}