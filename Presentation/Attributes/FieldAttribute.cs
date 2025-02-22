using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Models;

namespace Presentation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FieldAttribute(FieldType type) : Attribute
{
    public string? Label { get; private set; }
    public FieldType Type { get; set; } = type;
    public string Value { get; set; }
    public List<SelectListItem>? Options { get; set; }

    public void SetLabelFromProperty(PropertyInfo property, string value = "")
    {
        var displayName = property.GetCustomAttribute<DisplayAttribute>()?.Name;
        Value = value;
        if (!string.IsNullOrEmpty(displayName))
        {
            Label = displayName;
        }
    }
}