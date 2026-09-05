using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Table
            builder.ToTable("Payments");

            // Primary Key
            builder.HasKey(p => p.Id);

            // PaymentNo
            builder.Property(p => p.PaymentNo)
                .IsRequired();

            // BookingId
            builder.Property(p => p.BookingId)
                .IsRequired();

            // Amount
            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            // Currency
            builder.Property(p => p.Currency)
                .HasMaxLength(10)
                .IsRequired();

            // PaymentStatus
            builder.Property(p => p.PaymentStatus)
                .IsRequired();

            // GatewayName
            builder.Property(p => p.GatewayName)
                .HasMaxLength(100);

            // GateTransId
            builder.Property(p => p.GateTransId)
                .HasMaxLength(200);

            // GatewayOrderId
            builder.Property(p => p.GatewayOrderId)
                .HasMaxLength(200);

            // FailureReason
            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            // AttemptNumber
            builder.Property(p => p.AttemptNumber)
                .IsRequired();

            builder.Property(x => x.IsActive)
    .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);
            // Relationship: Booking 1 -> Many Payments
            builder.HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}