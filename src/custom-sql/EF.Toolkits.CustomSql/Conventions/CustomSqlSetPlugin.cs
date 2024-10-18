using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Toolkits.CustomSql.Conventions
{
    public class CustomSqlSetPlugin : IConventionSetPlugin
    {
        public ConventionSet ModifyConventions(ConventionSet conventionSet)
        {
            conventionSet.ModelFinalizingConventions.Add(new DbFunctionSqlConvention());

            return conventionSet;
        }
    }
}