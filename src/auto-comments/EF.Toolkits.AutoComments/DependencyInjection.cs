using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Toolkits.AutoComments;

// ReSharper disable once CheckNamespace
namespace Toolkits.EntityFrameworkCore
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