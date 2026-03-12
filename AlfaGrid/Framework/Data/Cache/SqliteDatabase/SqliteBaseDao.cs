using System.Linq.Expressions;

namespace AlfaGrid.Framework.Data.Cache.SqliteDatabase
{
    public class SqliteBaseDao<T> : ISqliteBaseDao<T> where T : new()
    {
        protected readonly DbContext _context;

        public SqliteBaseDao()
        {
            _context = new DbContext();
        }

        public async Task InitializeAsync()
        {
            await _context.CreateTableAsync<T>();
        }
        public async Task<int> AddAsync(T entity)
        {
            return await _context.Database.InsertAsync(entity);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Database.InsertAllAsync(entities);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task<List<T>> GetAsync(int skip, int take)
        {
            return await _context.Set<T>().Skip(skip).Take(take).ToListAsync();
        }

        public virtual async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate, int skip, int take)
        {
            return await _context.Set<T>().Where(predicate).Skip(skip).Take(take).ToListAsync();
        }

        public virtual async Task RemoveAsync(T entity)
        {
            await _context.Database.DeleteAsync(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            await _context.Database.UpdateAsync(entity);
        }

        public async Task<T> GetFirstOrDefault()
        {
            return await _context.Database.Table<T>().FirstOrDefaultAsync();
        }

        public async Task RemoveAllAsync()
        {
            await _context.Database.DeleteAllAsync<T>();
        }

        public virtual async Task<List<T>> GetAsync()
        {
            return await _context.Database.Table<T>().ToListAsync();
        }
    }
}