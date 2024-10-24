using System;

namespace Toolkits.AutoComments.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoCommentEnumDescriptionAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAutoCommentEnumDescriptionAttribute : Attribute
    {
    }
}