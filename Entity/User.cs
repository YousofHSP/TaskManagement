using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Entity.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

public class User : IdentityUser<int>, IEntity<int>
{
    [Required] [StringLength(100)] public string FullName { get; set; } = null!;
    public GenderType Gender { get; set; }
    public UserStatus Status { get; set; }
    public DateTimeOffset BirthDate { get; set; }
    public Grade Grade { get; set; }
    public FieldOfStudy FieldOfStudy { get; set; }
    public DateTimeOffset LastLoginDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public IEnumerable<Ticket>? Tickets { get; set; }

}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.UserName).IsRequired().HasMaxLength(100);
        builder.HasMany(user => user.Tickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);

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
public enum Grade
{
    [Display(Name = "نهم")] Ninth = 9,
    [Display(Name = "دهم")] Tenth,
    [Display(Name = "یازدهم")] Eleventh,
    [Display(Name = "دوازدهم")] Twelfth
}

public enum FieldOfStudy
{
    [Display(Name = "ریاضی")] Mathematics = 1,
    [Display(Name = "انسانی")] Humanities,
    [Display(Name = "تجربی")] Experimental,
    [Display(Name = "هنر")] Art,
    [Display(Name = "زبان")] Language,
    [Display(Name = "فنی")] Technical,
}