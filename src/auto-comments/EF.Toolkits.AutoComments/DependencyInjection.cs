using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Toolkits.AutoComments;

// ReSharper disable once CheckNamespace
namespace Toolkits.EntityFrameworkCore
{
    public static class DependencyInjection
    {
        public static DbContextOptionsBuilder UseAutoComments([NotNull] this DbContextOptionsBuilder optionsBuilder,
            params string[] xmlPaths)
        {
            var options = new AutoCommentOptions
            {
                XmlPaths = xmlPaths
            };

            var extension = new AutoCommentsOptionsExtension(options);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseAutoComments([NotNull] this DbContextOptionsBuilder optionsBuilder,
            Action<AutoCommentOptions> configure)
        {
            var options = new AutoCommentOptions();
            configure.Invoke(options);

            var extension = new AutoCommentsOptionsExtension(options);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }

    public class AutoCommentOptions
    {
        internal string[] XmlPaths { get; set; }

        internal bool AutoEnumValuesComment { get; set; }
    }

    public static class AutoCommentOptionsExtensions
    {
        public static AutoCommentOptions WithXmlPaths(this AutoCommentOptions options, params string[] xmlPaths)
        {
            options.XmlPaths = xmlPaths;

            return options;
        }

        public static AutoCommentOptions AddEnumValuesComments(this AutoCommentOptions options)
        {
            options.AutoEnumValuesComment = true;

            return options;
        }
    }
}