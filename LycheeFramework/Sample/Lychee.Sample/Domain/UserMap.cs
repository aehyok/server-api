using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Sample.Domain
{
    public class UserMap : SampleMapBase<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
        }
    }
}