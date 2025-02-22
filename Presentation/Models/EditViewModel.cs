using System.Reflection;
using DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Models;

public class EditViewModel
{
    public string Title { get; set; }
    public bool Error { get; set; }
    public PropertyInfo[] Properties { get; set; }
    public Dictionary<string, List<SelectListItem>> Options { get; set; } = new();
    public List<Field> Fields { get; set; } = [];
}