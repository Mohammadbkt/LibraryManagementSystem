using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b=>b.Id);

            builder.Property(b=>b.Title)
                    .HasMaxLength(150)
                    .IsRequired();

            builder.Property(b=>b.Description)
                .HasMaxLength(500);

            builder.Property(b=>b.PublisherId)
                .IsRequired();

            builder.HasOne(b=>b.Publisher)
                    .WithMany(p=>p.Books)
                    .HasForeignKey(b=>b.PublisherId)
                    .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}