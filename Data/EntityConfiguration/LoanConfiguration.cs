using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");
            
            builder.HasKey(l => l.Id);
            
            builder.Property(l => l.Notes)
                .HasMaxLength(500);
                
            builder.Property(l => l.Status)
                .HasConversion<string>();
                
            builder.Property(l => l.BorrowDate)
                .IsRequired();
                
            builder.Property(l => l.DueDate)
                .IsRequired();
                
            builder.HasOne(l => l.Item)
                .WithMany(i => i.Loans)
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(l => l.Fine)
                .WithOne(f => f.Loan)
                .HasForeignKey<Fine>(f => f.LoanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}