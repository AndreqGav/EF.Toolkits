using System.Diagnostics.CodeAnalysis;
using EF.Toolkits.AutoComments.Conventions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Toolkits.AutoComments.Extensions
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