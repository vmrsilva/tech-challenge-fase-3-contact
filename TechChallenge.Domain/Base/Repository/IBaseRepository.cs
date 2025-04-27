using System.Linq.Expressions;
using TechChallenge.Domain.Base.Entity;

namespace TechChallenge.Domain.Base.Repository
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        Task RemoveByIdAsync(Guid id);

        Task UpdateAsync(T entity);

        Task<T> GetByIdAsync(Guid id);

        public Task<T> GetAsync(Expression<Func<T, bool>> search);

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> search);
        Task<T> GetOneWithIncludeAsync(Expression<Func<T, bool>> search, params Expression<Func<T, object>>[] includes);
        Task<IList<T>> GetPagedAsync(Expression<Func<T, bool>> search, int pageSize, int page, Expression<Func<T, dynamic>> orderDesc);
        Task<int> GetCountAsync(Expression<Func<T, bool>> search);
    }
}
