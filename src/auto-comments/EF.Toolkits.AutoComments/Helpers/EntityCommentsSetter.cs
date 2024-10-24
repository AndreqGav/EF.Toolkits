using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Toolkits.AutoComments.Helpers
{
    public class EntityCommentsSetter
    {
        private const string NewLinePlaceholder = "\n";

        private readonly XmlDocument _xmlDocument;

        public EntityCommentsSetter(IEnumerable<string> xmlFiles)
        {
            _xmlDocument = null;

            foreach (var xmlFile in xmlFiles)
            {
                var xmlDocumentPart = new XmlDocument();
                using var commentStream = File.Open(xmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                xmlDocumentPart.Load(commentStream);

                if (_xmlDocument == null)
                {
                    _xmlDocument = xmlDocumentPart;
                }
                else
                {
                    if (xmlDocumentPart.DocumentElement != null)
                    {
                        var importNode = _xmlDocument.ImportNode(xmlDocumentPart.DocumentElement, true);
                        _xmlDocument.DocumentElement?.AppendChild(importNode);
                    }
                }
            }

            _xmlDocument ??= new XmlDocument();
        }

        public void SetTableComment(IConventionEntityType entityType)
        {
            foreach (var type in TypeHelper.GetParentTypes(entityType?.ClrType))
            {
                var xmlNode =
                    _xmlDocument.SelectSingleNode($"//member[@name='T:{GetFullName(type)}']/summary");

                if (xmlNode is not null)
                {
                    entityType!.SetComment(GetCommentTextWithNewlineReplacement(xmlNode));

                    break;
                }
            }
        }

        public void SetColumnComment(IConventionProperty property)
        {
            foreach (var type in TypeHelper.GetParentTypes(property.PropertyInfo?.DeclaringType))
            {
                var xmlNode = _xmlDocument.SelectSingleNode(
                    $"//member[@name='P:{GetFullName(type)}.{property.Name}']/summary");

                if (xmlNode is not null)
                {
                    property!.SetComment(GetCommentTextWithNewlineReplacement(xmlNode));

                    break;
                }
            }
        }

        public void AddEnumDescriptionComment(IConventionProperty property)
        {
            var propType = property.PropertyInfo?.PropertyType;
            var enumType = propType is not null ? Nullable.GetUnderlyingType(propType) ?? propType : null;

            if (enumType is null || !enumType.IsEnum)
            {
                return;
            }

            var enumIsString = false;

#pragma warning disable EF1001
            var providerClrTypeAnnotation = property.FindAnnotation(CoreAnnotationNames.ProviderClrType);
#pragma warning restore EF1001
            if (providerClrTypeAnnotation?.Value as Type == typeof(string))
            {
                enumIsString = true;
            }

            var stringBuilder = new StringBuilder(property.GetComment());
            stringBuilder.AppendLine();

            foreach (var value in Enum.GetValues(enumType))
            {
                var name = value.ToString();
                var xmlNode = _xmlDocument.SelectSingleNode(
                    $"//member[@name='F:{GetFullName(enumType)}.{name}']/summary");

                if (xmlNode is not null)
                {
                    var description = GetCommentTextWithNewlineReplacement(xmlNode);
                    stringBuilder.AppendLine();
                    if (enumIsString)
                    {
                        stringBuilder.Append(name);
                    }
                    else
                    {
                        stringBuilder.Append((int)value);
                    }

                    stringBuilder.Append(" - ");
                    stringBuilder.Append(description);
                }
            }

            var comment = GetCommentTextWithNewlineReplacement(stringBuilder.ToString());
            property!.SetComment(comment);
        }

        private static string GetFullName(Type type)
        {
            // Заменяем + на . для вложенных типов.
            return type?.FullName?.Replace("+", ".") ?? string.Empty;
        }

        private static string GetCommentTextWithNewlineReplacement(XmlNode a) =>
            GetCommentTextWithNewlineReplacement(a.InnerText);

        private static string GetCommentTextWithNewlineReplacement(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return string.Join(NewLinePlaceholder, str.Trim().Split(new[]
                {
                    "\r\n", "\r", "\n"
                }, StringSplitOptions.None)
                .Select(line => line.Trim()));
        }
    }
}