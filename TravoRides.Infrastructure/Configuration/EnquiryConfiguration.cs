using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRiders.Infrastructure.Configuration
{
    public class EnquiryConfiguration : IEntityTypeConfiguration<Enquiry>
    {
        public void Configure(EntityTypeBuilder<Enquiry> builder)
        {
            builder.ToTable("Enquiries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(255);

            builder.Property(x => x.Phone)
                .IsRequired();

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(2000);

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
