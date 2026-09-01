using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRiders.Infrastructure.Configurations
{
    public class CabFeaturesConfiguration : IEntityTypeConfiguration<CabFeatures>
    {
        public void Configure(EntityTypeBuilder<CabFeatures> builder)
        {
            builder.ToTable("CabFeatures");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            builder.HasOne(x => x.Cab)
                .WithMany(x => x.CabFeatures)
                .HasForeignKey(x => x.CabId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Feature)
                .WithMany(x => x.CabFeatures)
                .HasForeignKey(x => x.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
