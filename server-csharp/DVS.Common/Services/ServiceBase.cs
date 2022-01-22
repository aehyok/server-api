using AutoMapper;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace DVS.Common.Services
{
    public partial class ServiceBase<TEntity> : GenericRepository<TEntity>, IServiceBase<TEntity> where TEntity : DvsEntityBase, new()
    {
        protected readonly IMapper mapper;

        public ServiceBase(DbContext dbContext, IMapper mapper)
            : base(dbContext)
        {
         
            this.mapper = mapper;
        }

        public override Task<int> UpdateAsync(TEntity entity)
        {
            if (entity.CreatedAt == null)
            {
                entity.CreatedAt = DateTime.Now;
            }

            entity.UpdatedAt = DateTime.Now;

            return base.UpdateAsync(entity);
        }

        public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.GetQueryable().Where(predicate).AsNoTracking().FirstOrDefaultAsync();
        }

        protected virtual async Task<int> ExecuteSqlAsync(string sql)
        {
            return await this.Context.Database.ExecuteSqlRawAsync(sql);
        }

        protected virtual async Task<int> ExecuteSqlAsync(string sql, params object[] parameters)
        {
            return await this.Context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.Table.Where(predicate).DeleteAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.Table.Where(predicate).CountAsync();
        } 
    }
}