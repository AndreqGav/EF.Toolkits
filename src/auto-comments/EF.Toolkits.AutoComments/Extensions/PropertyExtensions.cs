using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toolkits.AutoComments.Conventions;

namespace Toolkits.AutoComments.Extensions
{
    public static class PropertyExtensions
    {
        public static void AddEnumValueComment([NotNull] this IConventionPropertyBuilder propertyBuilder)
        {
            var property = propertyBuilder.Metadata;
            property.Builder.HasAnnotation(AutoEnumCommentsConvention.Name, string.Empty);
        }

        public static bool HasEnumValueComments([NotNull] this IConventionProperty property)
        {
            return property.FindAnnotation(AutoEnumCommentsConvention.Name) is not null;
        }
    }
}