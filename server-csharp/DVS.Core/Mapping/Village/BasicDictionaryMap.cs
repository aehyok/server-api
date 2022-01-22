
using DVS.Common.Mapping;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sunlight.Sea.Core.Mapping.OrganizationStats
{
    public class BasicDictionaryMap : DvsMapBase<BasicDictionary>
    {
        public override void Configure(EntityTypeBuilder<BasicDictionary> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a=>a.UpdatedBy);
            base.Configure(builder);
        }
    }
}
