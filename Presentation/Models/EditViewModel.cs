using DTO;

namespace Presentation.Models;

public class EditViewModel
{
    public bool Error { get; set; }
    public List<Field> Fields { get; set; } = [];
}