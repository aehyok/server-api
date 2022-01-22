using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace Lychee.EntityFramework.Extensions
{
    public static class PgaeQueryExtensions
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate">查询条件</param>
        /// <param name="page">分页索引</param>
        /// <param name="limit">分页大小</param>
        /// <returns></returns>
        public static async Task<IPagedList<TEntity>> GetPagedListAsync<TEntity>(this IGenericRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, int page, int limit) where TEntity : EntityBase
        {
            return await repository.GetQueryable().Where(predicate).ToPagedListAsync(page, limit);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static async Task<IPagedList<TEntity>> GetPagedListAsync<TEntity, TOrder>(this IGenericRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TOrder>> orderBy, int page, int limit = 10, bool asc = true) where TEntity : EntityBase
        {
            var query = repository.GetQueryable().Where(predicate);
            return asc ? await query.OrderBy(orderBy).ToPagedListAsync(page, limit) : await query.OrderByDescending(orderBy).ToPagedListAsync(page, limit);
        }
    }
}