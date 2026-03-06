using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.EntityConfiguration
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Status)
                .HasConversion<string>();
                
            builder.Property(r => r.QueuePosition)
                .IsRequired();
                
            builder.HasOne(r => r.Item)
                .WithMany(i => i.Reservations)
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}