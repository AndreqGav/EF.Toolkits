using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public string[] XmlFiles { get; }

        public bool AutoCommentEnumDescriptions { get; set; }

        private Dictionary<string, List<string>> NotFinedXmlPaths { get; } = new();

        public AutoCommentsOptionsExtension(AutoCommentOptions autoCommentOptions)
        {
            XmlFiles = GetXmlFiles(autoCommentOptions).ToArray();
            AutoCommentEnumDescriptions = autoCommentOptions.AutoCommentEnumDescriptions;

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
            foreach (var xmlPath in XmlFiles)
            {
                if (!File.Exists(xmlPath))
                {
                    throw new FileNotFoundException($"XML file {xmlPath} not exists", xmlPath);
                }
            }

            foreach (var (key, value) in NotFinedXmlPaths)
            {
                var paths = string.Join(", ", value);
                        
                Console.ForegroundColor = ConsoleColor.Yellow; 
                Console.WriteLine($"XML comments file {key} not found. The following locations were searched: {paths}");
                Console.ResetColor();
            }
        }

        private IEnumerable<string> GetXmlFiles(AutoCommentOptions options)
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            foreach (var xmlFile in options.XmlFiles)
            {
                var paths = new List<string> {xmlFile, Path.Combine(assemblyLocation, xmlFile)};

                foreach (var path in paths)
                {
                    if (!File.Exists(path))
                    {
                        NotFinedXmlPaths.TryAdd(xmlFile, new List<string>());
                        NotFinedXmlPaths[xmlFile].Add(path);
                    }
                    else
                    {
                        NotFinedXmlPaths.Remove(xmlFile);

                        yield return path;
                        break;
                    }
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
            foreach (var item in Extension.XmlFiles)
            {
                hash.Add(item);
            }

            hash.Add(Extension.AutoCommentEnumDescriptions);

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

                    foreach (var xmlPath in Extension.XmlFiles)
                    {
                        builder.AppendJoin(' ', $"Used XML comments file: {xmlPath}");
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