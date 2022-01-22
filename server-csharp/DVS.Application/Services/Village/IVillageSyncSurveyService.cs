using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village.Query;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public interface IVillageSyncSurveyService : IServiceBase<VillageHouseholdCode>
	{
        /// <summary>
        /// 同步村情概况到数据大屏
        /// </summary>
        /// <returns></returns>
        Task<int> SyncVillageSurveyToDVM();
    }
}
