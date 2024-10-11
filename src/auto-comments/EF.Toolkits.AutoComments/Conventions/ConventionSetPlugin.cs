using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EF.Toolkits.AutoComments.Conventions
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
            var xmlPaths = _extension.XmlPaths;

            var enumAnnotationConvention = new AutoEnumCommentsConvention();
            var autoCommentsConvention = new AutoCommentsConvention(xmlPaths);

            conventionSet.ModelFinalizingConventions.Add(enumAnnotationConvention);
            conventionSet.ModelFinalizingConventions.Add(autoCommentsConvention);

            return conventionSet;
        }
    }
}