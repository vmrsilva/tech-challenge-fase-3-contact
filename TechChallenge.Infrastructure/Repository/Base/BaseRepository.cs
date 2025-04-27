using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechChallenge.Domain.Base.Entity;
using TechChallenge.Domain.Base.Repository;
using TechChallenge.Infrastructure.Context;

namespace TechChallenge.Infrastructure.Repository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly TechChallangeContext _context;
        protected DbSet<T> _dbSet { get; set; }
        public BaseRepository(TechChallangeContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {

            _dbSet.Add(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> search)
        {
            return await _dbSet
                     .AsNoTracking()
                     .Where(search).ToListAsync().ConfigureAwait(false);
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> search)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(search)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted).ConfigureAwait(false);
            return entity;
        }
        public async Task<T> GetOneWithIncludeAsync(Expression<Func<T, bool>> search, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking()
                .Where(search);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task RemoveByIdAsync(Guid id)
        {
            var entity = await GetByIdAsync(id).ConfigureAwait(false);

            if (entity == null)
                return;

            _dbSet.Remove(entity);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<IList<T>> GetPagedAsync(Expression<Func<T, bool>> search, int pageSize, int page, Expression<Func<T, dynamic>> orderDesc)
        {
            var x = await _dbSet
                .AsNoTracking()
                .Where(search)
                .OrderByDescending(orderDesc)
                .Skip(page == 0 ? 0 : (page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return x;
        }
        public async Task<int> GetCountAsync(Expression<Func<T, bool>> search)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(search)
                .CountAsync();
        }
    }
}
