using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRiders.Infrastructure.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Packages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Itinerary)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(x => x.Route)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.PlacesCovered)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.Inclusions)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.Duration)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Distance)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Discount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.IsActive)
                .IsRequired();


            builder.Property(x => x.IsDeleted)
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
