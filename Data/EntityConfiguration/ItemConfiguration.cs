using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");

            builder.HasKey(i => i.Id);

            builder.Property(i=>i.AcquisitionDate)
                    .HasColumnType("Date")
                    .IsRequired();
            
            builder.Property(i => i.Barcode)
                .HasMaxLength(50)
                .IsRequired();
                
            builder.HasIndex(i => i.Barcode)
                .IsUnique()
                .HasDatabaseName("IX_Items_Barcode");
                
            builder.Property(i => i.Location)
                .HasMaxLength(100);
                
            builder.Property(i => i.Notes)
                .HasMaxLength(500);
                
            builder.Property(i => i.Price)
                .HasPrecision(10, 2);
                
            builder.Property(i => i.ItemStatus)
                .HasConversion<string>();
                
            builder.HasOne(i => i.Edition)
                .WithMany(e => e.Items)
                .HasForeignKey(i => i.EditionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}