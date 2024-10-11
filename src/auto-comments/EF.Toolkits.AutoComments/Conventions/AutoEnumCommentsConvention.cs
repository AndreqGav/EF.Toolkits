using System;
using EF.Toolkits.AutoComments.Attributes;
using EF.Toolkits.AutoComments.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EF.Toolkits.AutoComments.Conventions
{
    /// <summary>
    /// Добавление аннотаций о том, что требуется дополнить комментарий к Enum с перечислением его значений.
    /// </summary>
    internal class AutoEnumCommentsConvention : IModelFinalizingConvention
    {
        public const string Name = "AutoEnumValuesComment";

        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    TrySetAutoEnumValuesAnnotation(property);
                }
            }
        }

        private static void TrySetAutoEnumValuesAnnotation(IConventionProperty property)
        {
            var memberInfo = property.PropertyInfo;
            if (memberInfo == null)
            {
                return;
            }

            var autoEnumComment = Attribute.GetCustomAttribute(memberInfo, typeof(AutoCommentsEnumValuesAttribute));

            if (autoEnumComment is not null)
            {
                property.Builder.AddEnumValueComment();
            }
        }
    }
}