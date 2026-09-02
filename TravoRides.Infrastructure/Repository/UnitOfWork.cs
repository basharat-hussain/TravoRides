using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public ICabRepository Cabs { get; }
        public ISelfDriveRepository SelfDrives { get; }
        public ICabFeaturesRepository CabFeatures { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<FeaturesMaster> FeatureMasters { get; }
        public IGenericRepository<Package> Packages { get; }
        public IGenericRepository<CategoryBased> CategoryBased { get; }



        public UnitOfWork(ApplicationDbContext context, ICabRepository cabs, ISelfDriveRepository selfDrives, ICabFeaturesRepository cabFeatures,IUserRepository user ,IRefreshTokenRepository refreshTokens)
        {
            _context = context;
            Cabs = cabs;
            SelfDrives = selfDrives;
            CabFeatures = cabFeatures;
            Users = user;
            RefreshTokens = refreshTokens;
            Categories = new GenericRepository<Category>(_context);
            FeatureMasters = new GenericRepository<FeaturesMaster>(_context);
            Packages = new GenericRepository<Package>(_context);
            CategoryBased = new GenericRepository<CategoryBased>(_context);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
           => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();

    }
}
