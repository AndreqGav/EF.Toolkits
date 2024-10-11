﻿using System;
using System.Diagnostics.CodeAnalysis;
using EF.Toolkits.CustomSql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EF.Toolkits.CustomSql
{
    public static class DependencyInjection
    {
        public static DbContextOptionsBuilder UseCustomSql([NotNull] this DbContextOptionsBuilder optionsBuilder,
            Action<UseCustomSqlOptions> optionsAction = null)
        {
            var extension = new CustomSqlOptionsExtension();
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            optionsAction?.Invoke(new UseCustomSqlOptions(optionsBuilder));

            return optionsBuilder;
        }
    }
}