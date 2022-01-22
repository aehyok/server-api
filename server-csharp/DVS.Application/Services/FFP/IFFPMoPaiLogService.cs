using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public interface IFFPMoPaiLogService : IServiceBase<FFPMoPaiLog>
    {
        /// <summary>
        /// 摸排列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="inspector"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IPagedList<FFPMatrixHouseholdDto>> ListMoPai(string keyword, int inspector, int page, int limit);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FFPMoPaiLogDto> DetailMoPai(int id);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="mopailog"></param>
        /// <returns></returns>
        Task<int> SaveMoPai(FFPMoPaiLog mopailog);

        /// <summary>
        /// 统计历史摸排户数
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        Task<FFPHouseholdStatisticDto> StatisticHousehold(int inspector);

        /// <summary>
        /// 统计历史摸排户数
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        Task<List<FFPHouseholdStatisticDto>> StatisticHouseholdByMonth(int inspector);


        /// <summary>
        /// 获取摸排列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="inspector"></param>
        /// <param name="workflowstatus"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IPagedList<FFPMatrixHouseholdDto>> ListMoPaiByStatus(string keyword, int inspector, int workflowstatus, string householdType, int page, int limit);

        Task<IPagedList<FFPMatrixHouseholdDto>> ListMoPaiMonth(string keyword, int inspector, string householdType, int page, int limit, int isMoPai = -1);
    }
}
