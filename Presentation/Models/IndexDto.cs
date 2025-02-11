namespace Presentation.Models;

public class IndexDto
{
    public int Page { get; set; } = 0;
    public Dictionary<string, string> Filters { get; set; } = new();
}