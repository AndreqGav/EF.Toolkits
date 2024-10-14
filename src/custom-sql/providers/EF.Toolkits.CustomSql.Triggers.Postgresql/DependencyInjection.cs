using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Toolkits.CustomSql.Models;
using Toolkits.Triggers.Postgresql;

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