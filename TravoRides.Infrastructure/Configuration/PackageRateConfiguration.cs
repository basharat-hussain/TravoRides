using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class PackageRateConfiguration : IEntityTypeConfiguration<PackageRate>
    {
        public void Configure(EntityTypeBuilder<PackageRate> builder)
        {
            builder.ToTable("PackageRates");
            // Composite Primary Key
            builder.HasKey(x => new
            {
                x.CabId,
                x.PackageId
            });

            // Rate
            builder.Property(x => x.Rate)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Discount
            builder.Property(x => x.Discount)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            // Cab -> PackageRate
            builder.HasOne(x => x.Cab)
                .WithMany(c => c.PackageRates)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Cascade);

            // Package -> PackageRate
            builder.HasOne(x => x.Package)
                .WithMany(p => p.PackageRates)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
