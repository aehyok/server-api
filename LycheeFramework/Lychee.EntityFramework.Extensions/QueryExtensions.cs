using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace Lychee.EntityFramework.Extensions
{
    public static class QueryExtensions
    {
        /// <summary>
        /// 统计结果总数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task<int> CountAsync<TEntity>(this IGenericRepository<TEntity> repository) where TEntity : EntityBase
        {
            return await repository.Table.CountAsync();
        }

        /// <summary>
        /// 根据主键获取一条数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<TEntity> GetAsync<TEntity>(this IGenericRepository<TEntity> repository, object id) where TEntity : EntityBase
        {
            return await repository.GetQueryable().FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static async Task<TEntity> GetAsync<TEntity>(this IGenericRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
        {
            return await repository.GetQueryable().Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static async Task<IList<TEntity>> GetListAsync<TEntity>(this IGenericRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
        {
            return await repository.GetQueryable().Where(predicate).ToListAsync();
        }
    }
}