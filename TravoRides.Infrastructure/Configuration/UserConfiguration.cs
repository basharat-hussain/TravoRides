using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.IsEmailVerified)
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
