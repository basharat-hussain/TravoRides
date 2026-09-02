using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRiders.Infrastructure.Configurations
{
    public class CabConfiguration : IEntityTypeConfiguration<Cab>
    {
        public void Configure(EntityTypeBuilder<Cab> builder)
        {
            builder.ToTable("Cabs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.LuggageCapacity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SeatingCapacity)
                .IsRequired();

            builder.Property(x => x.PricePerDay)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Discount)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Fuel)
                .IsRequired();

            builder.Property(x => x.Transmission)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.IsActive)
                .IsRequired();


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            // Category -> Cabs
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Cabs)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cab -> CabFeatures
            builder.HasMany(x => x.CabFeatures)
                .WithOne(x => x.Cab)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
