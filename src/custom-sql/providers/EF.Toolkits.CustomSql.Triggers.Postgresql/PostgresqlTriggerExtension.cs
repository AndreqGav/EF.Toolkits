using EF.Toolkits.CustomSql.Triggers.Abstractions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Toolkits.CustomSql.Triggers.Postgresql
{
    public class PostgresqlTriggerExtension : TriggerSqlExtension
    {
        public override void ApplyServices(IServiceCollection services)
        {
            base.ApplyServices(services);

            new EntityFrameworkServicesBuilder(services)
                .TryAddProviderSpecificServices(serviceMap =>
                    serviceMap.TryAddTransient<ITriggerSqlGenerator, PostgreSqlTriggerSqlGenerator>());
        }
    }
}