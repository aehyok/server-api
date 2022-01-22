using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lychee.EntityFramework.Extensions
{
    public static class DeleteExtensions
    {
        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<TEntity>(this IGenericRepository<TEntity> repository, object id) where TEntity : EntityBase
        {
            var entity = await repository.GetAsync(id);
            return await repository.DeleteAsync(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<int> DeleteAsync<TEntity>(this IGenericRepository<TEntity> repository, TEntity entity) where TEntity : EntityBase
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                repository.Table.Remove(entity);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public static async Task<int> DeleteRangeAsync<TEntity>(this IGenericRepository<TEntity> repository, TEntity[] entities) where TEntity : EntityBase
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                repository.Table.RemoveRange(entities);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public static async Task<int> DeleteRangeAsync<TEntity>(this IGenericRepository<TEntity> repository, IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                repository.Table.RemoveRange(entities);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }
    }
}