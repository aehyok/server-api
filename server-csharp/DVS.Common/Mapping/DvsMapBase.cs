using DVS.Common.Models;
using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Common.Mapping
{
    public abstract class DvsMapBase<TEntity> : IMappingConfiguration<TEntity> where TEntity : DvsEntityBase, new()
    {
        public void ApplyConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(this);
        }

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(typeof(TEntity).Name);

            builder.HasKey(a => a.Id);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(a => a.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}