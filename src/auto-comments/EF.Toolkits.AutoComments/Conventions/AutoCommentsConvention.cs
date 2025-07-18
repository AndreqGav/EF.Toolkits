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
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                if (!entityType.IsOwned() && entityType.GetViewName() == null && entityType.GetTableName() != null)
                {
                    _commentsSetter.SetTableComment(entityType);
                }

                foreach (var property in entityType.GetProperties())
                {
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