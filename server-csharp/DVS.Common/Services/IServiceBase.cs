using Lychee.EntityFramework;
using System.Collections.Generic;

namespace DVS.Common.Services
{
    public interface IServiceBase<TEntity> : IGenericRepository<TEntity> where TEntity : EntityBase
    {
        
    }
}