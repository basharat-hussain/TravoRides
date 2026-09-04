using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configurations
{
    public class SelfDriveConfiguration : IEntityTypeConfiguration<SelfDrive>
    {
        public void Configure(EntityTypeBuilder<SelfDrive> builder)
        {
            builder.ToTable("SelfDrives");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PricePerDay)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Discount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsActive)
                .IsRequired();


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            // One-to-one with Cab
            builder.HasOne(x => x.Cab)
                .WithOne(x => x.SelfDrive)
                .HasForeignKey<SelfDrive>(x => x.CabId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
