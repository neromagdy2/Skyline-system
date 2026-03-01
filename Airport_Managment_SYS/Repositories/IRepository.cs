using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Airport_Managment_SYS.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null,
            bool trackd = true,
            CancellationToken cancellationToken = default
            );
        Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expressions = null,
            Expression<Func<T, object>>?[] includes = null,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null,
            bool trackd = true,
            CancellationToken cancellationToken = default
            );

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
