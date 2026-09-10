using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class TransitRateConfiguration : IEntityTypeConfiguration<TransitRate>
    {
        public void Configure(EntityTypeBuilder<TransitRate> builder)
        {
            builder.ToTable("TransitRates");
            // Composite Primary Key
            builder.HasKey(x => new
            {
                x.CabId,
                x.TransitId
            });

            // Rate
            builder.Property(x => x.Rate)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Discount
            builder.Property(x => x.Discount)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            // Cab -> TransitRate
            builder.HasOne(x => x.Cab)
                .WithMany(c => c.TransitRates)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Cascade);

            // transit -> TransitRate
            builder.HasOne(x => x.Transit)
                .WithMany(p => p.TransitRates)
                .HasForeignKey(x => x.TransitId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
