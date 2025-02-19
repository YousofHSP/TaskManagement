using System.ComponentModel.DataAnnotations;
using Entity.Common;
using Microsoft.AspNetCore.Identity;

namespace Entity;

[Display(Name = "نقش")]
public class Role: IdentityRole<int>, IEntity<int>
{
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public List<User> Users { get; set; }
}