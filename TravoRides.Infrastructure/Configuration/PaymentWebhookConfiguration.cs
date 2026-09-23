using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class PaymentWebhookConfiguration : IEntityTypeConfiguration<PaymentWebhook>
    {
        public void Configure(EntityTypeBuilder<PaymentWebhook> builder)
        {
            builder.ToTable("PaymentWebhooks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.RazorpayCreatedAt)
                .IsRequired();

            builder.Property(x => x.PaymentGatewayId)
                .HasMaxLength(100);

            builder.Property(x => x.GatewayOrderId)
                .HasMaxLength(100);

            builder.Property(x => x.Payload)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.Property(x => x.IsProcessed)
                .IsRequired();

            builder.Property(x => x.ReceivedAt)
                .IsRequired();

            builder.HasIndex(x => x.EventId)
                .IsUnique();

            builder.Property(x => x.HangfireJobId)
                .HasMaxLength(100);
        }
    }
}

