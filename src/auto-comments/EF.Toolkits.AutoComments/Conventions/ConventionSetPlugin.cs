using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Toolkits.AutoComments.Conventions
{
    public class ConventionSetPlugin : IConventionSetPlugin
    {
        private readonly AutoCommentsOptionsExtension _extension;

        public ConventionSetPlugin([NotNull] IDbContextOptions options)
        {
            _extension = options.FindExtension<AutoCommentsOptionsExtension>()!;
        }

        public ConventionSet ModifyConventions(ConventionSet conventionSet)
        {
            var enumAnnotationConvention = new AutoEnumCommentsConvention(_extension.AutoEnumValuesComment);
            conventionSet.ModelFinalizingConventions.Add(enumAnnotationConvention);

            var autoCommentsConvention = new AutoCommentsConvention(_extension.XmlPaths);
            conventionSet.ModelFinalizingConventions.Add(autoCommentsConvention);

            return conventionSet;
        }
    }
}