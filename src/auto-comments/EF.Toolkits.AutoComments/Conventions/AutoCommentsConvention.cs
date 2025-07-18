using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Toolkits.AutoComments.Extensions;
using Toolkits.AutoComments.Helpers;

namespace Toolkits.AutoComments.Conventions
{
    internal class AutoCommentsConvention : IModelFinalizingConvention
    {
        private readonly EntityCommentsSetter _commentsSetter;

        public AutoCommentsConvention(IEnumerable<string> xmlFiles)
        {
            _commentsSetter = new EntityCommentsSetter(xmlFiles);
        }

        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            // Установка комментария на таблицу.
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                // Для owned типов (предполагается что у owned нет таблицы)
                if (entityType.IsOwned())
                {
                    continue;
                }
                
                // Для вьюх.
                if (entityType.GetViewName() != null)
                {
                    continue;
                }
                
                // Для абстракных классов в наследовании TPC.
                if (entityType.GetTableName() == null)
                {
                    continue;
                }
                
                // Для наследников в TPH.
                if (entityType.BaseType != null && entityType.BaseType.GetTableName() != null)
                {
                    continue;
                }
                
                if (entityType.GetComment() != null)
                {
                    continue;
                }
                
                _commentsSetter.SetTableComment(entityType);
            }

            // Установка комментариев на столбцы.
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.GetComment() != null)
                    {
                        continue;
                    }
                    
                    _commentsSetter.SetColumnComment(property);

                    if (property.HasEnumDescriptionComment())
                    {
                        _commentsSetter.AddEnumDescriptionComment(property);
                    }
                }
            }
        }
    }
}