using TravoRides.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TravoRides.Infrastructure.Configuration
{
    public class LatestThinkingConfiguration : IEntityTypeConfiguration<LatestThinking>
    {
        public void Configure(EntityTypeBuilder<LatestThinking> builder)
        {
            builder.ToTable("LatestThinkings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Subtitle)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(x => x.ImageUrl)
                .IsRequired();

            builder.Property(x => x.ImageAltText)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(x => x.Author)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
               .IsRequired();

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
