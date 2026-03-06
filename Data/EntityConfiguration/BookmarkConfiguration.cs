using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.ToTable("Bookmarks");

            builder.HasKey(bm=>new {bm.BookId, bm.UserId});

            builder.Property(bm=>bm.Notes)
                    .HasMaxLength(500)
                    .IsRequired(false);
            
            builder.HasOne(bm=>bm.Book)
                    .WithMany(b=>b.Bookmarks)
                    .HasForeignKey(bm=>bm.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bm=>bm.User)
                    .WithMany(u=>u.Bookmarks)
                    .HasForeignKey(bm=>bm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
        }
    }
}