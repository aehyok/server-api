using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public interface IGISPlotItemSyncService : IServiceBase<GISPlotItem>
	{
        /// <summary>
        /// 获取打点信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<GISPlotItem>> ListPlotItemAsync(GISListQueryModel model);

        /// <summary>
        /// 打点数据同步到数据大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SyncToDVM(GISPlotItem model);
    }
}
