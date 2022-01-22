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
    public interface IPopulationTagService : IServiceBase<VillagePopulationTag>
    {
        Task<bool> SaveTags(int populationId, string tags);

        Task<IEnumerable<VillageTagDto>> GetTags(int populationId);

        Task<IEnumerable<VillageTagDto>> GetTags(List<int> populationIds);

    }





  
}
