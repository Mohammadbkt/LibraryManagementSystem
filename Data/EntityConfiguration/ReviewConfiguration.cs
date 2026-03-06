using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Comment)
                .HasMaxLength(2000);
                
            builder.Property(r => r.Rating)
                .IsRequired();
                
            builder.HasIndex(r => new { r.BookId, r.UserId })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_Book_User");
                
            builder.HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}