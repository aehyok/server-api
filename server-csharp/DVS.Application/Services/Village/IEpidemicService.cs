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
using DVS.Models.Dtos.Village.Statistics;

namespace DVS.Application.Services.Village
{
    public interface IEpidemicService : IServiceBase<VillageEpidemic>
    {

        Task<IPagedList<VillageEpidemicDto>> GetEpidemicList(PagePostBody body);
        Task<List<EpidemicPopulationDto>> GetEpidemicInfoList(int householdId, int year, int populationId = 0);
        Task<List<EpidemicPopulationDto>> GetEpidemicPopulationList(int householdId);
        Task<MessageResult<bool>> SaveEpidemicInfo(VillageEpidemic body);

        Task<MessageResult<bool>> DeleteEpidemicAsync(int id, int userid);

        Task<IPagedList<StatisticsEpidemicDto>> GetEpidemicStatisticsList(PagePostBody body);

        Task<byte[]> GetEpidemicStatisticsExcelData(PagePostBody body);

        Task<byte[]> GetEpidemicExcelData(int areaId, int year, string ids);
        
        Task<IPagedList<EpidemicPopulationDto>> GetEpidemicPopulationInfoList(EpidemicInfoListBody body);

        Task<IPagedList<EpidemicPopulationDto>> GetEpidemicInfoList(int householdId, int year, int populationId = 0, int page = 1, int limit = 10);
    }
 
}
