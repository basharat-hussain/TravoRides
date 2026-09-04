using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configurations
{
    public class FeaturesMasterConfiguration : IEntityTypeConfiguration<FeaturesMaster>
    {
        public void Configure(EntityTypeBuilder<FeaturesMaster> builder)
        {
            builder.ToTable("FeaturesMaster");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Icon)
                .IsRequired()
                .HasMaxLength(255);

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
