using Entity.Common;

namespace Entity;

public class Event: BaseEntity
{

    public string Title { get; set; }
    public string? Description { get; set; }
}