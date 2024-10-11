using System.Diagnostics.CodeAnalysis;
using EF.Toolkits.CustomSql.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EF.Toolkits.CustomSql.Triggers.Postgresql
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