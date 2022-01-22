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
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : EntityBase, new()
    {
        public GenericRepository(DbContext dbContext)
        {
            this.Context = dbContext;
        }

        public DbSet<TEntity> Table => Context.Set<TEntity>();

        public DbContext Context { get; }

        public DbConnection Connection => this.Context.Database.GetDbConnection();

        public DbContext GetDbContext()
        {
            return Context;
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return this.Table.AsNoTracking();
        }

        /// <summary>
        /// 获取详细错误信息并回滚
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public virtual string GetFullErrorTextAndRollback(DbUpdateException exception)
        {
            var entries = this.Context.ChangeTracker.Entries().Where(a => a.State == EntityState.Added || a.State == EntityState.Modified).ToList();

            entries.ForEach(a => a.State = EntityState.Unchanged);

            this.Context.SaveChanges();

            return exception.ToString();
        }

        /// <summary>
        /// 获取数据表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetTabelName(Type type)
        {
            var entityType = this.Context.Model.FindEntityType(type);
            var tableName = entityType?.FindAnnotation("Relational:TableName")?.Value.ToString();
            return tableName;
        }

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetTableSchema(Type type)
        {
            var entityType = this.Context.Model.FindEntityType(type);

            // Relational:Schema

            return string.Empty;
        }

        public string GetColumnName(Type type, string property)
        {
            return string.Empty;
        }

        #region Insert

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                await this.Table.AddAsync(entity);
                await this.Context.SaveChangesAsync();

                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public virtual async Task<int> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                await this.Table.AddRangeAsync(entities);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public virtual async Task<int> InsertRangeAsync(params TEntity[] entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                await this.Table.AddRangeAsync(entities);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        #endregion Insert

        #region Delete

        public virtual async Task<int> DeleteAsync(object id)
        {
            var entity = await this.GetAsync(id);
            return await this.DeleteAsync(entity);
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                this.Table.Remove(entity);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public virtual async Task<int> DeleteRangeAsync(TEntity[] entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                this.Table.RemoveRange(entities);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public virtual async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                this.Table.RemoveRange(entities);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        #endregion Delete

        #region Update

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                this.Table.Update(entity);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public virtual async Task<int> UpdateRangeAsync(params TEntity[] entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                this.Table.UpdateRange(entities);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                this.Table.UpdateRange(entities);
                return await this.Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(this.GetFullErrorTextAndRollback(ex), ex);
            }
        }

        #endregion Update

        #region Select

        /// <summary>
        /// 统计结果总数
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> CountAsync()
        {
            return await this.Table.CountAsync();
        }

        /// <summary>
        /// 根据主键获取一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetAsync(object id)
        {
            return await this.Table.FindAsync(id);
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.GetQueryable().Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取对象集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.GetQueryable().Where(predicate).ToListAsync();
        }

        #endregion Select

        #region PagedQuery

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="page">分页索引</param>
        /// <param name="limit">分页大小</param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate, int page, int limit)
        {
            return await this.GetQueryable().Where(predicate).ToPagedListAsync(page, limit);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> GetPagedListAsync<TOrder>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TOrder>> orderBy, int page, int limit = 10, bool asc = true)
        {
            var query = this.GetQueryable().Where(predicate);
            return asc ? await query.OrderBy(orderBy).ToPagedListAsync(page, limit) : await query.OrderByDescending(orderBy).ToPagedListAsync(page, limit);
        }

        #endregion PagedQuery
    }
}