using System.ComponentModel;
using System.Reflection;
using Presentation.Models;

namespace Presentation.Attributes;

public class FieldAttribute : Attribute
{
    public string Label { get; private set; }
    public FieldType Type { get; set; }
    public string[]? Options { get; set; }

    public FieldAttribute(FieldType type, string[]? options = null)
    {
        Type = type;
        Options = options;
    }
    
    public void SetLabelFromProperty(PropertyInfo property)
    {
        var displayName = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        if (!string.IsNullOrEmpty(displayName))
        {
            Label = displayName; // مقدار Label را از DisplayName تنظیم می‌کنیم
        }
    }
}