using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

public class Plan: BaseEntity
{

    public string Title { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public List<Customer>? Customers { get; set; } = [];
    public List<Job>? Jobs { get; set; } = [];
}

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasMany(p => p.Customers)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);
        builder.HasMany(p => p.Jobs)
            .WithOne(j => j.Plan)
            .HasForeignKey(j => j.PlanId);
    }
}