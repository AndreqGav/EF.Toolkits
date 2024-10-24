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
        public string[] XmlPaths { get; }

        public bool AutoEnumValuesComment { get; set; }

        private Dictionary<string, List<string>> NotFinedXmlPaths { get; } = new();

        public AutoCommentsOptionsExtension(AutoCommentOptions autoCommentOptions)
        {
            XmlPaths = GetXmlPaths(autoCommentOptions).ToArray();
            AutoEnumValuesComment = autoCommentOptions.AutoEnumValuesComment;

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
            foreach (var xmlPath in XmlPaths)
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

        private IEnumerable<string> GetXmlPaths(AutoCommentOptions options)
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            foreach (var xmlPath in options.XmlPaths)
            {
                var pathCandidates = new List<string> {xmlPath, Path.Combine(assemblyLocation, xmlPath)};

                foreach (var path in pathCandidates)
                {
                    if (!File.Exists(path))
                    {
                        NotFinedXmlPaths.TryAdd(xmlPath, new List<string>());
                        NotFinedXmlPaths[xmlPath].Add(path);
                    }
                    else
                    {
                        NotFinedXmlPaths.Remove(path);

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
            foreach (var item in Extension.XmlPaths)
            {
                hash.Add(item);
            }

            hash.Add(Extension.AutoEnumValuesComment);

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

                    foreach (var xmlPath in Extension.XmlPaths)
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