using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class PaymentRefundConfiguration : IEntityTypeConfiguration<PaymentRefund>
    {
        public void Configure(EntityTypeBuilder<PaymentRefund> builder)
        {
            builder.ToTable("PaymentRefunds", table =>
            {
                table.HasCheckConstraint(
                    "CK_PaymentRefunds_Amount_Positive",
                    "[Amount] > 0");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(x => x.GatewayRefundId)
                .HasMaxLength(100);

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.PaymentId);

            builder.HasIndex(x => x.GatewayRefundId)
                .IsUnique()
                .HasFilter("[GatewayRefundId] IS NOT NULL");

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.PaymentRefunds)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.RefundNumber)
                .IsUnique();

            builder.HasIndex(x => x.IdempotencyKey)
                .IsUnique();

            builder.Property(x => x.RefundNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.IdempotencyKey)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}

