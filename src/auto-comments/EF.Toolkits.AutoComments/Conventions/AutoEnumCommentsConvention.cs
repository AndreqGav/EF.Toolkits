﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Toolkits.AutoComments.Attributes;
using Toolkits.AutoComments.Extensions;

namespace Toolkits.AutoComments.Conventions
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