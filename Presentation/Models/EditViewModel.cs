using DTO;

namespace Presentation.Models;

public class EditViewModel
{
    public string Title { get; set; }
    public bool Error { get; set; }
    public List<Field> Fields { get; set; } = [];
}