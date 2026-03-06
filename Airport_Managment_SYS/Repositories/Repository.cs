using Airport_Managment_SYS.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Airport_Managment_SYS.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbcontext _context;// = new ApplicationDbContext();
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbcontext context)
        {

            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result;
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null,
            bool trackd = true,
            CancellationToken cancellationToken = default
            )
        {
            var entities = _dbSet.AsQueryable();
            if (expression is not null)
            {
                entities = entities.Where(expression);
            }
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (includeFunc is not null)
            {
                entities = includeFunc(entities);
            }
            if (!trackd)
            {
                entities = entities.AsNoTracking();
            }
            return await entities.ToListAsync(cancellationToken);
        }
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expressions = null,
            Expression<Func<T, object>>?[] includes = null,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null,
            bool trackd = true,
            CancellationToken cancellationToken = default
            )
        {
            var entity = (await GetAsync(expressions, includes, includeFunc, trackd, cancellationToken)).FirstOrDefault();
            return entity;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
