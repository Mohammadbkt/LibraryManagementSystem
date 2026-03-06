using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.ToTable("Publishers");

            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();
                
            builder.HasIndex(p => p.Name)
                .IsUnique()
                .HasDatabaseName("IX_Publishers_Name");
                
            builder.Property(p => p.Website)
                .HasMaxLength(200);
        }
    }
}