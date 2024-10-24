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
            params string[] xmlFiles)
        {
            var options = new AutoCommentOptions
            {
                XmlFiles = xmlFiles
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
        internal string[] XmlFiles { get; set; }

        internal bool AutoCommentEnumDescriptions { get; set; }
    }

    public static class AutoCommentOptionsExtensions
    {
        public static AutoCommentOptions FromXmlFiles(this AutoCommentOptions options, params string[] xmlFiles)
        {
            options.XmlFiles = xmlFiles;

            return options;
        }

        public static AutoCommentOptions AddEnumDescriptions(this AutoCommentOptions options)
        {
            options.AutoCommentEnumDescriptions = true;

            return options;
        }
    }
}