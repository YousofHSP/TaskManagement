using Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Display(Name = "تیکت")]
    public class Ticket: BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public int UserId { get; set; }
        public int? ParentId { get; set; }
        public bool IsSeen { get; set; } = false;
        public TicketPriority Priority { get; set; }

        public User User { get; set; } = new();
        public Ticket? Parent { get; set; }
        public IEnumerable<Ticket>? Children { get; set; }
    }

    public enum TicketPriority
    {
        [Display(Name = "پایین")]
        Low,
        [Display(Name = "متوسط")]
        Medium,
        [Display(Name = "بالا")]
        High,
    }

    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.Property(t => t.Title).IsRequired();
            builder.Property(t => t.Text).IsRequired();
            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.ParentId);
            builder.HasOne(ticket => ticket.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ticket => ticket.Parent)
                .WithMany(u => u.Children)
                .HasForeignKey(t => t.ParentId);
            builder.HasMany(ticket => ticket.Children)
                .WithOne(ticket => ticket.Parent)
                .HasForeignKey(ticket => ticket.ParentId);
        }
    }
}
