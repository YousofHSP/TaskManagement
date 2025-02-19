using System.ComponentModel.DataAnnotations;
using Entity.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

[Display(Name = "کاربران")]
public class User : IdentityUser<int>, IEntity<int>
{
    [Required] [StringLength(100)] public string FullName { get; set; } = null!;
    public GenderType Gender { get; set; }
    public UserStatus Status { get; set; }
    public DateTimeOffset BirthDate { get; set; }
    public DateTimeOffset LastLoginDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public IEnumerable<Ticket>? Tickets { get; set; }
    public List<Job>? OwnedJobs { get; set; }
    public List<Role> Roles { get; set; }

}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.UserName).IsRequired().HasMaxLength(100);
        builder.HasMany(user => user.Tickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
        builder.HasMany(user => user.OwnedJobs)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<int>>();

    }
}

public enum GenderType
{
    [Display(Name = "مرد")] Male = 1,
    [Display(Name = "زن")] Female = 2,
}

public enum UserStatus
{
    [Display(Name = "فعال")] Active = 1,
    [Display(Name = "غیرفعال")] Disable = 0,
}