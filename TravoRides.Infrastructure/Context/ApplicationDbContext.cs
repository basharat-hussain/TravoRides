using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;


namespace TravoRides.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        #region Identity Tables

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        #endregion

        public DbSet<Cab> Cabs => Set<Cab>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<FeaturesMaster> FeatureMasters => Set<FeaturesMaster>();
        public DbSet<CabFeatures> CabFeatures => Set<CabFeatures>();
        public DbSet<Package> Packages => Set<Package>();

        public DbSet<SelfDrive> SelfDrives => Set<SelfDrive>();

        public DbSet<CategoryBased> CategoryBased => Set<CategoryBased>();
      
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Enquiry> Enquiries => Set<Enquiry>();

        #region OTP Verification

        public DbSet<VerificationOtp> VerificationOtps => Set<VerificationOtp>();

        #endregion

        #region Booking and Payment
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
