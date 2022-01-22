using DVS.Application.Services.Village;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 疫情防控管理
    /// </summary>
    [Route("/api/village/console")]
    public class EpidemicController : DvsControllerBase
    {


        /// <summary>
        /// 疫情防控人员详情
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <param name="service"></param>
        /// <param name="year">年度</param>
        /// <param name="populationId">人口Id 可选</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetEpidemicInfoList")]
        [PermissionFilter("返乡人员登记", PermissionCodes.View)]
        public async Task<IPagedList<EpidemicPopulationDto>> GetEpidemicInfoList([FromServices] IEpidemicService service, int householdId, int year, int populationId = 0, int page = 1, int limit = 10)
        {
            var data = await service.GetEpidemicInfoList(householdId, year, populationId, page, limit);
            return data;
        }

        /// <summary>
        /// 某个人的返乡人员登记
        /// </summary>
        /// <param name="service"></param>
        /// <param name="populationId">人口Id 可选</param>
        /// <returns></returns>
        [HttpGet("GetUserEpidemicInfoList")]
        public async Task<List<EpidemicPopulationDto>> GetUserEpidemicInfoList([FromServices] IEpidemicService service, int populationId = 0)
        {
            var data = await service.GetEpidemicInfoList(0, 0, populationId);
            return data;
        }


        /// <summary>
        /// 疫情防控管理
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetEpidemicList")]
        [PermissionFilter("返乡人员登记", PermissionCodes.List)]
        public async Task<IPagedList<VillageEpidemicDto>> GetEpidemicList([FromServices] IEpidemicService service, PagePostBody body)
        {
            var data = await service.GetEpidemicList(body);
            return data;
        }

        /// <summary>
        /// 增加登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddEpidemicInfo")]
        [PermissionFilter("返乡人员登记", PermissionCodes.Add)]
        public async Task<bool> AddEpidemicInfo([FromServices] IEpidemicService service, VillageEpidemic body)
        {
            if (body.Id > 0)
            {
                throw new ValidException("id不能大于0");
            }
            if (body.AreaId == 0)
            {
                body.AreaId = LoginUser.AreaId;
            }
            body.CreatedBy = LoginUser.UserId;
            body.UpdatedBy = LoginUser.UserId;
            var result = await service.SaveEpidemicInfo(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 修改登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditEpidemicInfo")]
        [PermissionFilter("返乡人员登记", PermissionCodes.Edit)]
        public async Task<bool> UpdateEpidemicInfo([FromServices] IEpidemicService service, VillageEpidemic body)
        {
            if (body.Id == 0)
            {
                throw new ValidException("id必须大于0");
            }
            body.UpdatedBy = LoginUser.UserId;
            var result = await service.SaveEpidemicInfo(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("DeleteEpidemicInfo")]
        [PermissionFilter("返乡人员登记", PermissionCodes.Remove)]
        public async Task<bool> DeleteEpidemicInfo([FromServices] IEpidemicService service, VillageDetailQueryModel body)
        {
            var result = await service.DeleteEpidemicAsync(body.Id, LoginUser.UserId);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 返乡统计
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("StatisticEpidemic")]
        [PermissionFilter("疫情防控台账", PermissionCodes.List)]
        public async Task<IPagedList<StatisticsEpidemicDto>> GetEpidemicStatisticsList([FromServices] IEpidemicService service, PagePostBody body)
        {
            return await service.GetEpidemicStatisticsList(body);
        }

        /// <summary>
        /// 返乡统计
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <param name="year"></param>
        /// <param name="keyword"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("epidemic/statistics/export")]
        [PermissionFilter("疫情防控台账", PermissionCodes.Export)]
        public async Task<FileContentResult> ExportEpidemicStatistics([FromServices] IEpidemicService service, [FromQuery] int areaId, int year, string keyword = "", string orders = "")
        {
            PagePostBody body = new PagePostBody()
            {
                AreaId = areaId,
                Year = year,
                Keyword= keyword,
                Page = 1,
                Limit = 10000,
            };
            if (!string.IsNullOrWhiteSpace(orders))
            {
                try
                {
                    List<OrderBy> orderList = JsonSerializer.Deserialize<List<OrderBy>>(orders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (orderList != null)
                    {
                        body.Orders = orderList;
                    }
                }
                catch { }
            }
            byte[] epidemicBytes = await service.GetEpidemicStatisticsExcelData(body);
            return File(epidemicBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// 返乡人员列表导出
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <param name="year"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("epidemic/export")]
        [PermissionFilter("返乡人员登记", PermissionCodes.Export)]
        public async Task<FileContentResult> ExportEpidemic([FromServices] IEpidemicService service, [FromQuery] int areaId, int year, string ids)
        {
            byte[] epidemicBytes = await service.GetEpidemicExcelData(areaId, year, ids);
            return File(epidemicBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
