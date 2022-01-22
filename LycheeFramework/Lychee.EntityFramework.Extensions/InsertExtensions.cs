using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lychee.EntityFramework.Extensions
{
    public static class InsertExtensions
    {
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<TEntity> InsertAsync<TEntity>(this IGenericRepository<TEntity> repository, TEntity entity) where TEntity : EntityBase
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                await repository.Table.AddAsync(entity);
                await repository.Context.SaveChangesAsync();

                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public static async Task<int> InsertRangeAsync<TEntity>(this IGenericRepository<TEntity> repository, IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                await repository.Table.AddRangeAsync(entities);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public static async Task<int> InsertRangeAsync<TEntity>(this IGenericRepository<TEntity> repository, params TEntity[] entities) where TEntity : EntityBase
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                await repository.Table.AddRangeAsync(entities);
                return await repository.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(repository.GetFullErrorTextAndRollback(ex), ex);
            }
        }
    }
}