using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Lychee.EntityFramework
{
    public interface ITransactionRepository<TEntity> where TEntity : EntityBase
    {
        IDbContextTransaction Transaction { get; }

        Task<IDbContextTransaction> BeginTransaction();

        Task CommitTransaction();

        Task RollbackTransaction();

        ValueTask DisposeTransaction();
    }
}