using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Lychee.EntityFramework
{
    public sealed class TransactionRepository<TEntity> : GenericRepository<TEntity>, ITransactionRepository<TEntity> where TEntity : EntityBase, new()
    {
        public TransactionRepository(DbContext dbContext)
            : base(dbContext)
        { }

        public IDbContextTransaction Transaction => this.Context.Database.CurrentTransaction;

        public Task<IDbContextTransaction> BeginTransaction()
        {
            return this.Context.Database.BeginTransactionAsync();
        }

        public Task CommitTransaction()
        {
            return this.Transaction.CommitAsync();
        }

        public ValueTask DisposeTransaction()
        {
            return this.Transaction.DisposeAsync();
        }

        public Task RollbackTransaction()
        {
            return this.Transaction.RollbackAsync();
        }
    }
}