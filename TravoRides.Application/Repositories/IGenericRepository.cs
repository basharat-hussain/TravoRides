using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Application.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        IQueryable<TEntity> AsQueryable();
    }

}
