using System.ComponentModel.DataAnnotations;
using Entity.Common;

namespace Entity;

[Display(Name = "فعالیت")]
public class Event: BaseEntity
{

    public string Title { get; set; }
    public string? Description { get; set; }
}