using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.GatewayName)
                .HasMaxLength(50);

            builder.Property(x => x.GatewayOrderId)
                .HasMaxLength(150);

            builder.Property(x => x.GatewayTransactionId)
                .HasMaxLength(150);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(500);

            builder.Property(x => x.RefundedAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            builder.HasOne(x => x.Booking)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.BookingId)
                .IsUnique()
                .HasFilter("[Status] = 1");

            builder.HasIndex(x => x.GatewayOrderId);

            builder.HasIndex(x => x.GatewayTransactionId)
                .IsUnique()
                .HasFilter("[GatewayTransactionId] IS NOT NULL");

            builder.Property(x => x.PaymentNumber)
                .HasMaxLength(50)
                .IsRequired()
                .ValueGeneratedNever();

            builder.HasIndex(x => x.PaymentNumber)
                .IsUnique();

            builder.Property(x => x.AttemptNumber)
                .IsRequired();
        }
    }
}
