using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lychee.EntityFramework.Extensions
{
    public static class UpdateExtensions
    {
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<TEntity>(this IGenericRepository<TEntity> repository, TEntity entity) where TEntity : EntityBase
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                repository.Table.Update(entity);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public static async Task<int> UpdateRangeAsync<TEntity>(this IGenericRepository<TEntity> repository, params TEntity[] entities) where TEntity : EntityBase
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                repository.Table.UpdateRange(entities);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public static async Task<int> UpdateRangeAsync<TEntity>(this IGenericRepository<TEntity> repository, IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                repository.Table.UpdateRange(entities);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }
    }
}