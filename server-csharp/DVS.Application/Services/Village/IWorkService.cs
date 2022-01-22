using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public interface IWorkService : IServiceBase<VillageWork>
    {
        Task<IPagedList<VillageWorkDto>> GetWorkList(PagePostBody body, string ids = "");
        Task<List<VillageWorkInfoDto>> GetWorkInfoList(int householdId, int year);
        Task<List<VillageWorkInfoDto>> GetWorkInfoList(int populationId);
        Task<MessageResult<bool>> SaveWorkInfo(SaveWorkBody body);

        Task<VillageWorkInfoDto> GetWorkInfoDetail(int id);
        Task<bool> DeleteWrokInfo(int id);
        

    }
}
