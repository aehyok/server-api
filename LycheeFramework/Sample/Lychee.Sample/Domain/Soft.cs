using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Sample.Domain
{
    public class Soft : OperatorEntityBase, ISoftDelete
    {
        public new int Id { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class SoftMap : SampleMapBase<Soft>
    {
        public override void Configure(EntityTypeBuilder<Soft> builder)
        {
            builder.ToTable("Soft");

            base.Configure(builder);
        }
    }
}