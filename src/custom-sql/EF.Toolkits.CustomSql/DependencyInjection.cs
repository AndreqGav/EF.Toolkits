using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Toolkits.CustomSql;
using Toolkits.CustomSql.Models;

// ReSharper disable once CheckNamespace
namespace Toolkits.EntityFrameworkCore
{
    public static class DependencyInjection
    {
        public static DbContextOptionsBuilder UseCustomSql([NotNull] this DbContextOptionsBuilder optionsBuilder,
            Action<UseCustomSqlOptions> optionsAction = null)
        {
            var extension = new CustomSqlOptionsExtension(optionsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            optionsAction?.Invoke(new UseCustomSqlOptions(optionsBuilder));

            return optionsBuilder;
        }
    }
}