using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Sample.Domain
{
    public abstract class SampleMapBase<TEntity> : IMappingConfiguration<TEntity> where TEntity : EntityBase, new()
    {
        public void ApplyConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(this);
        }

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var type = typeof(TEntity);

            if (typeof(ISoftDelete).IsAssignableFrom(type))
            {
                builder.HasQueryFilter(a => (a as ISoftDelete).IsDeleted == false);
            }

            if (typeof(OperatorEntityBase).IsAssignableFrom(type))
            {
                builder.HasOne(a => (a as OperatorEntityBase).ChangedUser).WithMany().HasForeignKey(a => (a as OperatorEntityBase).ChangedBy);
            }
        }
    }
}