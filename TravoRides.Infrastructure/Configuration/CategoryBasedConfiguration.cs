using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRiders.Infrastructure.Configurations
{
    public class CategoryBasedConfiguration : IEntityTypeConfiguration<CategoryBased>
    {
        public void Configure(EntityTypeBuilder<CategoryBased> builder)
        {
            builder.ToTable("CategoryBased");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(x => x.ImageUrl)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);
        }
    }
}
