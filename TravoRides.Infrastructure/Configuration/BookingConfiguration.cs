using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BookingNo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Rate)
                .HasPrecision(18, 2);

            // Booking -> Cab
            builder.HasOne(x => x.Cab)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Transit
            builder.HasOne(x => x.Transit)
                .WithMany()
                .HasForeignKey(x => x.TransitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Package
            builder.HasOne(x => x.Package)
                .WithMany()
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
