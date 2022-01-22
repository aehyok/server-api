using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Lychee.EntityFramework
{
    [Obsolete("数据库操作审计功能暂未完成")]
    public class AuditObjectContext : LycheeObjectContext
    {
        public AuditObjectContext(DbContextOptions<LycheeObjectContext> options)
            : base(options)
        {
        }

        protected override Task BeforeSaveChanges()
        {
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                if (entityEntry.State == EntityState.Detached || entityEntry.State == EntityState.Unchanged)
                {
                    continue;
                }
            }

            return base.BeforeSaveChanges();
        }

        protected override Task AfterSaveChanges()
        {
            return base.AfterSaveChanges();
        }
    }
}