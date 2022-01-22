using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
   public  interface IFFPMatrixHouseholdService : IServiceBase<FFPMatrixHousehold>
    {
        /// <summary>
        /// 从网格移除户码
        /// </summary>
        /// <param name="matrixId"></param>
        /// <param name="householdIds"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> RemoveMatrixHousehold(int matrixId, string householdIds, int userid);

        /// <summary>
        /// 获取网格所属户码列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="areaId"></param>
        /// <param name="matrixId"></param>
        /// <param name="orders"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IPagedList<FFPMatrixHouseholdDto>> ListMatrixHousehold(string keyword, int areaId, int matrixId, List<OrderBy> orders, int page, int limit);

        /// <summary>
        /// 添加户码到网格
        /// </summary>
        /// <param name="matrixId"></param>
        /// <param name="householdIds"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> SaveMatrixHousehold(int matrixId, string householdIds, int userid);

        /// <summary>
        /// 获取户码列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="areaId"></param>
        /// <param name="isUsed"></param>
        /// <param name="orders"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IPagedList<FFPMatrixHouseholdDto>> ListHousehold(string keyword, int areaId, int isUsed, List<OrderBy> orders, int page, int limit);


        /// <summary>
        /// 花名册
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="areaId"></param>
        /// <param name="matrixId"></param>
        /// <param name="householdTypes"></param>
        /// <param name="inspector"></param>
        /// <param name="orders"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="householdId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IPagedList<FFPMatrixHouseholdDto>> ListHouseholdByMatrix(string keyword, int areaId, int matrixId, string householdTypes, int inspector, List<OrderBy> orders, int page, int limit, int householdId = 0, string ids = "");

        /// <summary>
        /// 统计花名册
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        Task<FFPHouseholdStatisticDto> StatisticHousehold(int inspector);

        /// <summary>
        /// 监测对象统计
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        Task<FFPHouseholdStatisticDto> StatisticMonitorHousehold(int inspector);

        /// <summary>
        /// 监测对象分类统计
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        Task<List<FFPHouseholdStatisticDto>> StatisticMonitorHouseholdByType(int inspector);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="matrixId"></param>
        /// <param name="inspector"></param>
        /// <param name="keyword"></param>
        /// <param name="areaId"></param>
        /// <param name="householdTypes"></param>
        /// <param name="houseName"></param>
        /// <param name="houseNumber"></param>
        /// <returns></returns>
        Task<byte[]> GetExcelData(int matrixId, int inspector = 0, string keyword = "", int areaId = 0, string householdTypes = "", string houseName = "", string houseNumber = "", string ids = "");
    }
}
