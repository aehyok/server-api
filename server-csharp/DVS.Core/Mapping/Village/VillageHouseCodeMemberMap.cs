
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    /// <summary>
    /// 
    /// </summary>
    public class VillageHouseCodeMemberMap : DvsMapBase<VillageHouseCodeMember>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseCodeMember> builder)
        {
            base.Configure(builder);
        }
    }
}
