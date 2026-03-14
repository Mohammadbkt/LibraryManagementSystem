using library.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library.Data.Configurations
{
    public class OtpConfig : IEntityTypeConfiguration<Otp>
    {
        public void Configure(EntityTypeBuilder<Otp> builder)
        {
            builder.ToTable("Otps");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.OtpHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.FailedAttempts)
                .IsRequired()
                .HasDefaultValue(0);


            builder.Property(x => x.VerifiedAt)
                .IsRequired(false);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45); 

            builder.Property(x => x.RequestId)
                .HasMaxLength(100);

            builder.HasOne(x => x.User)
                .WithMany(u => u.otps)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.RequestId);

            builder.HasIndex(x => x.ExpiresAt);

            builder.HasIndex(x => new { x.UserId, x.IsUsed });
        }
    }
}