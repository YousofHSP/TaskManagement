using System.ComponentModel.DataAnnotations;
using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

[Display(Name = "وظیفه")]
public class Job:BaseEntity
{
    public string Title { get; set; }
    public int UserId { get; set; }
    public int? CustomerId { get; set; }
    public int? ProjectId { get; set; }
    public int? ParentId { get; set; }
    public string? Description { get; set; } = "";
    public int EventId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public JobStatus Status { get; set; }

    public Job? Parent { get; set; }
    public List<Job>? Children { get; set; } = [];
    public User User { get; set; } = null!;
    public Customer? Customer { get; set; }
    public Project? Project { get; set; }
    public Event Event { get; set; } = null!;
}

public enum JobStatus
{
    [Display(Name = "در انتظار")]
    Todo,
    [Display(Name = "در حال پردازش")]
    InProgress,
    [Display(Name = "انجام شده")]
    Done,
}

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasOne(j => j.User)
            .WithMany(u => u.OwnedJobs)
            .HasForeignKey(j => j.UserId);
        builder.HasOne(j => j.Customer)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CustomerId);
        builder.HasOne(i => i.Project)
            .WithMany(i => i.Jobs)
            .HasForeignKey(i => i.ProjectId);
    }
}