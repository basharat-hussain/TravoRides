using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Booking Number
            builder.Property(x => x.BookingNo)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.BookingNo)
                .IsUnique();

            // Cab
            builder.Property(x => x.CabId)
                .IsRequired();

            builder.HasOne(x => x.Cab)
                .WithMany()
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking Type
            builder.Property(x => x.BookingType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // Optional Transit
            builder.HasOne(x => x.Transit)
                .WithMany()
                .HasForeignKey(x => x.TransitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional Package
            builder.HasOne(x => x.Package)
                .WithMany()
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer Details
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.WhatsApp)
                .IsRequired()
                .HasMaxLength(20);

            // Travel Information
            builder.Property(x => x.PickupLocation)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.DropLocation)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Passengers)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Luggage)
                .HasMaxLength(250);

            builder.Property(x => x.SpecialRequirements)
                .HasMaxLength(1000);

            // Dates
            builder.Property(x => x.TravelDate)
                .IsRequired();

            builder.Property(x => x.PickupTime)
                .IsRequired();

            // Confirmation
            builder.Property(x => x.IsConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            // Final Booking Rate
            builder.Property(x => x.Rate)
                .IsRequired()
                .HasPrecision(18, 2);

            // Booking -> Payments
            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Booking)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

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