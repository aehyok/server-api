using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace Lychee.EntityFramework
{
    public interface IGenericRepository<TEntity> where TEntity : EntityBase
    {
        DbSet<TEntity> Table { get; }

        DbContext Context { get; }

        DbConnection Connection { get; }

        DbContext GetDbContext();

        IQueryable<TEntity> GetQueryable();

        string GetFullErrorTextAndRollback(DbUpdateException exception);

        Task<TEntity> InsertAsync(TEntity entity);

        Task<int> InsertRangeAsync(IEnumerable<TEntity> entities);

        Task<int> InsertRangeAsync(params TEntity[] entities);

        Task<int> DeleteAsync(object id);

        Task<int> DeleteAsync(TEntity entity);

        Task<int> DeleteRangeAsync(TEntity[] entities);

        Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(TEntity entity);

        Task<int> UpdateRangeAsync(params TEntity[] entities);

        Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task<int> CountAsync();

        Task<TEntity> GetAsync(object id);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate, int page, int limit);

        Task<IPagedList<TEntity>> GetPagedListAsync<TOrder>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TOrder>> orderBy, int page, int limit = 10, bool asc = true);
    }
}