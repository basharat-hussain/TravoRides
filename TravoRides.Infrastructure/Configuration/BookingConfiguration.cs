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
            builder.Property(x => x.Name)
                 .IsRequired();
            builder.Property(x => x.Phone)
                .IsRequired();
            builder.Property(x => x.WhatsApp)
                .IsRequired();
            builder.Property(x => x.Email)
                .IsRequired();
            builder.Property(x => x.TravelDate)
                .IsRequired();
            builder.Property(x => x.PickupLocation)
                .IsRequired();
            builder.Property(x => x.DropLocation)
                .IsRequired();
            builder.Property(x => x.PickupTime)
                .IsRequired();
            builder.Property(x => x.Passengers)
                .IsRequired();
            builder.Property(x => x.Luggage)
                .IsRequired(false);
            builder.Property(x => x.SpecialRequirements)
                .IsRequired(false);
   
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
