using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EF.Toolkits.AutoComments
{
    public static class DependencyInjection
    {
        public static DbContextOptionsBuilder UseAutoComments([NotNull] this DbContextOptionsBuilder optionsBuilder,
            params string[] xmlPaths)
        {
            var extension = new AutoCommentsOptionsExtension(xmlPaths);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}