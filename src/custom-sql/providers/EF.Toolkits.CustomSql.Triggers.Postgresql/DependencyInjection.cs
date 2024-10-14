using System.Diagnostics.CodeAnalysis;
using EF.Toolkits.CustomSql.Models;
using EF.Toolkits.CustomSql.Triggers.Postgresql;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Toolkits.EntityFrameworkCore
{
    public static class DependencyInjection
    {
        public static UseCustomSqlOptions UseTriggers([NotNull] this UseCustomSqlOptions options)
        {
            var extension = new PostgresqlTriggerExtension();

            ((IDbContextOptionsBuilderInfrastructure)options.OptionsBuilder).AddOrUpdateExtension(extension);

            return options;
        }
    }
}