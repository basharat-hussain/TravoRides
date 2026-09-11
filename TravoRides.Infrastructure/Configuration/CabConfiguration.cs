using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Persistence.Configurations
{
    public class CabConfiguration : IEntityTypeConfiguration<Cab>
    {
        public void Configure(EntityTypeBuilder<Cab> builder)
        {
            builder.ToTable("Cabs");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Category
            builder.Property(x => x.CategoryId)
                .IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Cabs)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            // Description
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // Capacity
            builder.Property(x => x.LuggageCapacity)
                .IsRequired();

            builder.Property(x => x.SeatingCapacity)
                .IsRequired();

            // Price
            builder.Property(x => x.PricePerDay)
                .IsRequired()
                .HasPrecision(18, 2);

            // Discount
            builder.Property(x => x.Discount)
                .HasPrecision(18, 2);

            // Fuel Enum
            builder.Property(x => x.Fuel)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            // Transmission
            builder.Property(x => x.Transmission)
                .IsRequired()
                .HasMaxLength(50);

            // Image
            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Cab → Bookings (One-to-Many)
            builder.HasMany(x => x.Bookings)
                .WithOne(x => x.Cab)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cab → CabFeatures (One-to-Many)
            builder.HasMany(x => x.CabFeatures)
                .WithOne(x => x.Cab)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cab → PackageRates (One-to-Many)
            builder.HasMany(x => x.PackageRates)
                .WithOne(x => x.Cab)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cab → TransitRates (One-to-Many)
            builder.HasMany(x => x.TransitRates)
                .WithOne(x => x.Cab)
                .HasForeignKey(x => x.CabId)
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