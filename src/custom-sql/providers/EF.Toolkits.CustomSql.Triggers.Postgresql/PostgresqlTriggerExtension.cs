using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Toolkits.Triggers.Abstractions;

namespace Toolkits.Triggers.Postgresql
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