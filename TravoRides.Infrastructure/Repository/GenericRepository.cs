using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TravoRides.Domain.Common;
using TravoRides.Infrastructure.Context;
using TravoRides.Application.Repositories;

namespace TravoRides.Infrastructure.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {

        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
            => await _dbSet.ToListAsync(cancellationToken);

        public async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        public async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public IQueryable<TEntity> AsQueryable()
            => _dbSet.AsQueryable().AsNoTracking();

    }

}
