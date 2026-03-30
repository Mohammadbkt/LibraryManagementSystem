using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class EditionConfiguration : IEntityTypeConfiguration<Edition>
    {
        public void Configure(EntityTypeBuilder<Edition> builder)
        {
            builder.ToTable("Editions");

            builder.HasKey(e=>e.Id);

            builder.Property(e=>e.ISBN)
                    .HasMaxLength(20)
                    .IsRequired();

            builder.HasIndex(e=>e.ISBN)
                    .IsUnique()
                    .HasDatabaseName("IX_Editions_ISBN");

            builder.Property(e => e.CoverImageUrl)
                .HasMaxLength(500);
                
            builder.Property(e => e.Language)
                .HasMaxLength(50)
                .HasDefaultValue("English");
                
            builder.Property(e => e.Format)
                .HasConversion<string>()
                .HasMaxLength(50);
                
            builder.Property(e => e.PublicationYear)
                .IsRequired(false);
                
            builder.Property(e => e.PageCount)
                .IsRequired(false);
                
            builder.HasOne(e => e.Book)
                .WithMany(b => b.Editions)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}