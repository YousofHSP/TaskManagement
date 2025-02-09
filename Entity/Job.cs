using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

public class Job:BaseEntity
{
    public string Title { get; set; }
    public int UserId { get; set; }
    public int CustomerId { get; set; }
    public int? ParentId { get; set; }
    public int PlanId { get; set; }
    public string Description { get; set; } = "";
    public int EventId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }

    public Job? Parent { get; set; }
    public List<Job>? Children { get; set; } = [];
    public User User { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Event Event { get; set; } = null!;
    public Plan Plan { get; set; }
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
        builder.HasOne(j => j.Plan)
            .WithMany(p => p.Jobs)
            .HasForeignKey(j => j.PlanId);
    }
}