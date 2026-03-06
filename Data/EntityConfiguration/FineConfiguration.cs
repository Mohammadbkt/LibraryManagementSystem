using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Models.Entities
{
    public class FineConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.ToTable("Fines");

            builder.HasKey(f => f.Id);
            
            builder.Property(f => f.Amount)
                .HasPrecision(10, 2)
                .IsRequired();
                
            builder.Property(f => f.Reason)
                .HasMaxLength(500);
                
            builder.Property(f => f.Status)
                .HasConversion<string>();
                
            builder.HasOne(f => f.Loan)
                .WithOne(l => l.Fine)
                .HasForeignKey<Fine>(f => f.LoanId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(f => f.User)
                .WithMany(u => u.Fines)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}