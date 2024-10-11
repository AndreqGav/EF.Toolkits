﻿using System.Collections.Generic;
using EF.Toolkits.AutoComments.Extensions;
using EF.Toolkits.AutoComments.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EF.Toolkits.AutoComments.Conventions
{
    internal class AutoCommentsConvention : IModelFinalizingConvention
    {
        private readonly EntityCommentsSetter _commentsSetter;

        public AutoCommentsConvention(IEnumerable<string> xmlPaths)
        {
            _commentsSetter = new EntityCommentsSetter(xmlPaths);
        }
        
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                if (!entityType.IsOwned() && entityType.BaseType == null)
                {
                    _commentsSetter.SetTableComment(entityType);
                }

                foreach (var property in entityType.GetProperties())
                {
                    _commentsSetter.SetColumnComment(property);

                    if (property.HasEnumValueComments())
                    {
                        _commentsSetter.AddEnumValuesComment(property);
                    }
                }
            }
        }
    }
}