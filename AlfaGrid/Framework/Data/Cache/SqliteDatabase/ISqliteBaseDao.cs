using System.Linq.Expressions;

namespace AlfaGrid.Framework.Data.Cache.SqliteDatabase
{
    public interface ISqliteBaseDao<T>
    {
        Task InitializeAsync();

        Task<int> AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetFirstOrDefault();

        Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetAsync(int skip, int take);

        Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate, int skip, int take);

        Task RemoveAsync(T entity);

        Task RemoveAllAsync();

        Task UpdateAsync(T entity);

        Task<List<T>> GetAsync();
    }
}