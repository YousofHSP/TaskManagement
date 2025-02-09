using Entity.Common;

namespace Entity;

public class Audit: BaseEntity
{
    public string Model { get; set; }
    public int UserId { get; set; }
    public string Method { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}