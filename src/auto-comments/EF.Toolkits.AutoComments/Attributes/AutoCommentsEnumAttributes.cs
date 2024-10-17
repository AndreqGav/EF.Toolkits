using System;

namespace Toolkits.AutoComments.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoCommentsEnumValuesAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAutoCommentsEnumValuesAttribute : Attribute
    {
    }
}