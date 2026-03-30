using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a=>a.Id);

            builder.Property(a=>a.FullName).HasMaxLength(200).IsRequired();

            builder.Property(a=>a.Biography).HasMaxLength(4000);

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}