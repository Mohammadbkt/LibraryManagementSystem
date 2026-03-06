using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c=>c.Id);

            builder.Property(c=>c.Name)
                    .HasMaxLength(150)
                    .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);
                
            builder.Property(c => c.SortOrder)
                .HasDefaultValue(0);

            builder.HasOne(c=>c.ParentCategory)
                    .WithMany(pc=>pc.SubCategories)
                    .HasForeignKey(c=>c.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.Name, c.ParentId })
                .IsUnique()
                .HasDatabaseName("IX_Categories_Name_Parent");
        }
    }
}