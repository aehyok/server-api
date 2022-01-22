using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
	public interface IGISPlotItemService : IServiceBase<GISPlotItem>
	{
        /// <summary>
        /// 删除打点信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> DeletePlotItemAsync(int id, int userid);

        /// <summary>
        /// 获取打点信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<GISPlotItemDto>> ListPlotItemAsync(GISListQueryModel model);

        /// <summary>
        /// 获取打点信息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GISPlotItemDto> DetailPlotItemAsync(int id);

        /// <summary>
        /// 保存打点信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SavePlotItemAsync(GISPlotItem model);

        //Task<int> SynctoDisplay(GISPlotItem model);
    }
}
