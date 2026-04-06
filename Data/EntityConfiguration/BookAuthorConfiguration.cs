using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            builder.ToTable("BookAuthors");

            builder.HasKey(ba => new { ba.AuthorId, ba.BookId });

            builder.HasOne(ba => ba.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(ba => ba.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ba => ba.Author)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(ba => ba.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ba => ba.AuthorOrder)
                    .IsRequired()
                    .HasDefaultValue(1);


                builder.HasQueryFilter(ba => !ba.Book.IsDeleted && !ba.Author.IsDeleted);



        }
    }
}