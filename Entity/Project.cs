using System.ComponentModel.DataAnnotations;
using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

[Display(Name = "پروژه")]
public class Project: BaseEntity
{

    public string Title { get; set; }
    public int CustomerId { get; set; }

    public Customer Customer { get; set; }
    public List<Job>? Jobs { get; set; }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasOne(i => i.Customer)
            .WithMany(i => i.Projects)
            .HasForeignKey(i => i.CustomerId);
        builder.HasMany(i => i.Jobs)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId);
    }
}