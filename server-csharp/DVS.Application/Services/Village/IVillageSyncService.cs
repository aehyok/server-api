using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village.Query;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public interface IVillageSyncService : IServiceBase<VillageHouseholdCode>
	{
        /// <summary>
        /// 获取户码列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<VillageHouseholdCode>> ListHouseHoldAsync(PostBody model);

        /// <summary>
        /// 获取户籍列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<VillagePopulation>> ListPopulationAsync(PostBody model);

        /// <summary>
        /// 获取家庭防疫情况列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<VillageEpidemic>> ListEpidemicAsync(PostBody model);

        /// <summary>
        /// 获取家庭收入情况列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<VillageIncome>> ListIncomeAsync(PostBody model);

        /// <summary>
        /// 获取家庭外出务工情况列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<VillageWork>> ListWorkAsync(PostBody model);

        /// <summary>
        /// 同步户码到数据大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SyncHouseHoldToDVM(VillageHouseholdCode model);

        /// <summary>
        /// 同步户籍人口到数据大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SyncPersonToDVM(VillagePopulation model);

        /// <summary>
        /// 同步防疫情况到数据大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SyncEpidemicToDVM(VillageEpidemic model);

        /// <summary>
        /// 同步收入情况到数据大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SyncIncomeToDVM(VillageIncome model);

        /// <summary>
        /// 同步务工情况到数据大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SyncWorkToDVM(VillageWork model);

    }
}
