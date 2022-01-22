
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    /// <summary>
    /// 
    /// </summary>
    public class VillageHouseCodeTagManageMap : DvsMapBase<VillageHouseCodeTagManage>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseCodeTagManage> builder)
        {
            base.Configure(builder);
        }
    }
}
