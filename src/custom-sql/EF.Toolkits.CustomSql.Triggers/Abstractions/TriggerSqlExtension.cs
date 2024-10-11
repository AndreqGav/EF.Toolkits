using System.Collections.Generic;
using EF.Toolkits.CustomSql.Triggers.Conventions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Toolkits.CustomSql.Triggers.Abstractions
{
    public abstract class TriggerSqlExtension : IDbContextOptionsExtension
    {
        protected TriggerSqlExtension()
        {
            Info = new PostgresqlTriggerExtensionInfo(this);
        }

        public virtual void ApplyServices(IServiceCollection services)
        {
            new EntityFrameworkServicesBuilder(services)
                .TryAdd<IConventionSetPlugin, TriggerSqlSetPlugin>();
        }

        public virtual void Validate(IDbContextOptions options)
        {
        }

        public DbContextOptionsExtensionInfo Info { get; }
    }

    public class PostgresqlTriggerExtensionInfo : DbContextOptionsExtensionInfo
    {
        public PostgresqlTriggerExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        {
        }

#if NET6_0_OR_GREATER
        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is PostgresqlTriggerExtensionInfo;

        public override int GetServiceProviderHashCode() => 0;
#else
        public override long GetServiceProviderHashCode() => 0;
#endif

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["PostgresqlTriggerExtension"] = "1";
        }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "PostgresqlTriggerExtension";
    }
}