using System.ComponentModel.DataAnnotations;
using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

[Display(Name = "مشتریان")]
public class Customer: BaseEntity
{
    public string Title { get; set; }
    public List<Job>? Jobs { get; set; }
    public List<Project>? Projects{ get; set; }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasMany(c => c.Jobs)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId);
        builder.HasMany(i => i.Projects)
            .WithOne(i => i.Customer)
            .HasForeignKey(i => i.CustomerId);
    }
}