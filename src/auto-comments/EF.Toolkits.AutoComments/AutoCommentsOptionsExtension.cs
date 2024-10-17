using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Toolkits.AutoComments.Conventions;
using Toolkits.EntityFrameworkCore;

namespace Toolkits.AutoComments
{
    /// <summary>
    /// Расширение, которое позволяет передать и сохранить пути до XML файлов с комментариями.
    /// Также регистрирует плагин установки конвенций.
    /// </summary>
    internal class AutoCommentsOptionsExtension : IDbContextOptionsExtension
    {
        public AutoCommentOptions AutoCommentOptions { get; }

        public AutoCommentsOptionsExtension(AutoCommentOptions autoCommentOptions)
        {
            AutoCommentOptions = autoCommentOptions;
            Info = new AutoCommentsExtensionInfo(this);
        }

        public DbContextOptionsExtensionInfo Info { get; }

        public void ApplyServices(IServiceCollection services)
        {
            new EntityFrameworkServicesBuilder(services)
                .TryAdd<IConventionSetPlugin, ConventionSetPlugin>();
        }

        public void Validate(IDbContextOptions options)
        {
            foreach (var xmlPath in AutoCommentOptions.XmlPaths)
            {
                if (!File.Exists(xmlPath))
                {
                    throw new FileNotFoundException($"XML file {xmlPath} not exists", xmlPath);
                }
            }
        }
    }

    public class AutoCommentsExtensionInfo : DbContextOptionsExtensionInfo
    {
        private string _logFragment;

        public AutoCommentsExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        {
        }

        public override bool IsDatabaseProvider => false;

#if NET6_0_OR_GREATER
        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is AutoCommentsExtensionInfo;

        public override int GetServiceProviderHashCode() => CalculateHashCode();
#else
        public override long GetServiceProviderHashCode() => CalculateHashCode();
#endif

        private int CalculateHashCode()
        {
            var hash = new HashCode();
            foreach (var item in Extension.AutoCommentOptions.XmlPaths)
            {
                hash.Add(item);
            }

            hash.Add(Extension.AutoCommentOptions.AutoEnumValuesComment);

            return hash.ToHashCode();
        }

        private new AutoCommentsOptionsExtension Extension => (AutoCommentsOptionsExtension)base.Extension;

        public override string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    var builder = new StringBuilder();

                    foreach (var xmlPath in Extension.AutoCommentOptions.XmlPaths)
                    {
                        builder.AppendJoin(' ', $"XML comments file: {xmlPath}");
                    }

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["AutoCommentsOptionsExtension"] = "1";
        }
    }
}