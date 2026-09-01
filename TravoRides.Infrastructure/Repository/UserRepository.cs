using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using AlArwaSolutions.Application.Repositories;

namespace TravoRides.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted, cancellationToken);
        }

        public Task<User?> GetByEmailWithRefreshTokensAsync(string email, CancellationToken cancellationToken = default)
        {
            return context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted, cancellationToken);
        }

      

        
    }
}
