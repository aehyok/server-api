using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;

namespace DVS.Application.Services.Village
{
    public interface IHouseholdCodeTagService : IServiceBase<VillageHouseholdCodeTag>
    {

        Task<bool> SaveTags(int householdId, string tags,int loginUserId);

        Task<IEnumerable<VillageTagDto>> GetTags(int householdId);
        Task<IEnumerable<VillageTagDto>> GetTags(List<int> householdIds);

    }





  
}
