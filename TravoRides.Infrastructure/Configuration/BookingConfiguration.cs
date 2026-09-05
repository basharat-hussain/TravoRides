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
            // Table
            builder.ToTable("Bookings");

            // Primary Key
            builder.HasKey(b => b.Id);

            // Booking Number
            builder.Property(b => b.BookingNo)
                .IsRequired()
                .HasMaxLength(50);

            // Name
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Email
            builder.Property(b => b.Email)
                .IsRequired()
                .HasMaxLength(100);

            // Phone
            builder.Property(b => b.Phone)
                .IsRequired()
                .HasMaxLength(12);

            // WhatsApp
            builder.Property(b => b.WhatsApp)
                .HasMaxLength(12);

            // IsConfirmed
            builder.Property(b => b.IsConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            // Travel Date
            builder.Property(b => b.TravelDate)
                .IsRequired();

            // Pickup Location
            builder.Property(b => b.PickupLocation)
                .IsRequired()
                .HasMaxLength(250);

            // Drop Location
            builder.Property(b => b.DropLocation)
                .IsRequired()
                .HasMaxLength(250);

            // Pickup Time
            builder.Property(b => b.PickupTime)
                .IsRequired();

            // Passengers
            builder.Property(b => b.Passengers)
                .IsRequired()
                .HasMaxLength(50);

            // Luggage
            builder.Property(b => b.Luggage)
                .HasMaxLength(500);

            // Special Requirements
            builder.Property(b => b.SpecialRequirements)
                .HasMaxLength(1000);
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
