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

            builder.HasKey(x => x.Id);

            // Cab → TransitRates
            builder.HasOne(x => x.Cab)
                .WithMany(x => x.TransitRates)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transit → TransitRates
            builder.HasOne(x => x.Transit)
                .WithMany(x => x.TransitRates)
                .HasForeignKey(x => x.TransitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rate
            builder.Property(x => x.Rate)
                .IsRequired()
                .HasPrecision(18, 2);

            // Discount
            builder.Property(x => x.Discount)
                .HasPrecision(18, 2);

            // Prevent duplicate Cab + Transit combination
            builder.HasIndex(x => new
            {
                x.CabId,
                x.TransitId
            })
            .IsUnique();

            builder.Property(x => x.IsActive)
               .IsRequired();


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);
        }

    }
}
