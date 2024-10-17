using Microsoft.EntityFrameworkCore;
using Toolkits.EntityFrameworkCore;

namespace EF.Toolkits.Tests
{
    public class DbContextConfigurator
    {
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("Host=localhost;Port=5432;Database=EF.Toolkits.Tests;Username=postgres;Password=aSTraNTudeGI")
                .UseCustomSql(options => options.UseTriggers())
                .UseAutoComments(options => options.WithXmlPaths("Comments.xml").AddEnumValuesComments())
                ;
        }
    }
}