using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Persistence.Configurations
{
    public class PackageRateConfiguration
        : IEntityTypeConfiguration<PackageRate>
    {
        public void Configure(EntityTypeBuilder<PackageRate> builder)
        {
            builder.ToTable("PackageRates");

            builder.HasKey(x => x.Id);

            // Cab → PackageRates
            builder.HasOne(x => x.Cab)
                .WithMany(x => x.PackageRates)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Restrict);

            // Package → PackageRates
            builder.HasOne(x => x.Package)
                .WithMany(x => x.PackageRates)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rate
            builder.Property(x => x.Rate)
                .IsRequired()
                .HasPrecision(18, 2);

            // Discount
            builder.Property(x => x.Discount)
                .HasPrecision(18, 2);

            // Prevent duplicate Cab + Package combination
            builder.HasIndex(x => new
            {
                x.CabId,
                x.PackageId
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