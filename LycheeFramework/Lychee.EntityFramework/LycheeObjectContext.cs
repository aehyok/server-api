using Lychee.TypeFinder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.EntityFramework
{
    public class LycheeObjectContext : DbContext
    {
        public LycheeObjectContext(DbContextOptions<LycheeObjectContext> options)
            : base(options)
        {
        }

        protected virtual async Task BeforeSaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        break;
                }
            }

            await Task.CompletedTask;
        }

        protected virtual Task AfterSaveChanges() => Task.CompletedTask;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var mappingConfigurations = FindTypes.InAllAssemblies
                .ThatInherit(typeof(IMappingConfiguration))
                .Excluding(typeof(IMappingConfiguration<>))
                .Where(a => !a.IsAbstract)
                .ToList();

            foreach(var type in mappingConfigurations)
            {
                var mapping = (IMappingConfiguration)Activator.CreateInstance(type);
                mapping.ApplyConfiguration(modelBuilder);
            }
        }

        public override int SaveChanges()
        {
            BeforeSaveChanges().GetAwaiter().GetResult();
            var result = base.SaveChanges();
            AfterSaveChanges().GetAwaiter().GetResult();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await BeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await AfterSaveChanges();
            return result;
        }
    }
}