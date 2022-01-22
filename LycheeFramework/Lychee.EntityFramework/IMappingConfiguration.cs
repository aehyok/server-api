using Microsoft.EntityFrameworkCore;

namespace Lychee.EntityFramework
{
    public interface IMappingConfiguration
    {
        void ApplyConfiguration(ModelBuilder builder);
    }

    public interface IMappingConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>, IMappingConfiguration where TEntity : EntityBase
    {
    }
}