using System.ComponentModel.DataAnnotations;
using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity;

[Display(Name = "مشتریان")]
public class Customer: BaseEntity
{
    public string Title { get; set; }
    public int? ParentId { get; set; }

    public Customer? Parent { get; set; }
    public List<Customer>? Children { get; set; }
    public List<Job>? Jobs { get; set; }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId);

        builder.HasMany(c => c.Children)
            .WithOne(c => c.Parent)
            .HasForeignKey(c => c.ParentId);
        
        builder.HasMany(c => c.Jobs)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId);
    }
}