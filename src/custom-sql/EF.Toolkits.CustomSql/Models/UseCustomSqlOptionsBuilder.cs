using Microsoft.EntityFrameworkCore;

namespace Toolkits.CustomSql.Models
{
    public class UseCustomSqlOptions
    {
        public DbContextOptionsBuilder OptionsBuilder { get; }

        public UseCustomSqlOptions(DbContextOptionsBuilder optionsBuilder)
        {
            OptionsBuilder = optionsBuilder;
        }
    }
}