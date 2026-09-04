using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration:IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.RevokedAt)
                .IsRequired(false);

            builder.Property(x => x.ReplacedByToken)
                .HasMaxLength(500)
                .IsRequired(false);

            // User → RefreshTokens
            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
