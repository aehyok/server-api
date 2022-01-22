using DVS.Application.Services.Village;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Vaccination;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 疫苗接种管理
    /// </summary>
    [Route("/api/village/console/vaccination")]
    public class VaccinationController : DvsControllerBase
    {

        /// <summary>
        /// 疫苗接种查询（按户码统计）
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter("新冠疫苗接种登记", PermissionCodes.List)]
        public async Task<IPagedList<VaccinationHouseholdDto>> GetVaccinationList([FromServices] IVaccinationService service, PagePostBody body)
        {
            var data = await service.GetVaccinationList(body);
            return data;
        }

        /// <summary>
        /// 疫苗接种记录(按户码查询)
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("infolist")]
        [PermissionFilter("新冠疫苗接种登记", PermissionCodes.View)]
        public async Task<IPagedList<VaccinationDto>> GetVaccinationInfoList([FromServices] IVaccinationService service, VaccinationPagePostBody body)
        {
            var data = await service.GetVaccinationInfoList(body.HouseholdId, body.Year, 0, body.Page, body.Limit);
            return data;
        }


        /// <summary>
        /// 登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [PermissionFilter("新冠疫苗接种登记", PermissionCodes.Add)]
        public async Task<bool> AddVaccinationInfo([FromServices] IVaccinationService service, VillageVaccination body)
        {
            if (body.Id > 0)
            {
                throw new ValidException("id不能大于0");
            }
            body.CreatedBy = LoginUser.UserId;
            body.UpdatedBy = LoginUser.UserId;
            var result = await service.SaveVaccinationInfo(body);
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
        /// 登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter("新冠疫苗接种登记", PermissionCodes.Edit)]
        public async Task<bool> UpdateVaccinationInfo([FromServices] IVaccinationService service, VillageVaccination body)
        {
            if (body.Id == 0)
            {
                throw new ValidException("id必须大于0");
            }
            body.UpdatedBy = LoginUser.UserId;
            var result = await service.SaveVaccinationInfo(body);
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
        [HttpPost("delete")]
        [PermissionFilter("新冠疫苗接种登记", PermissionCodes.Remove)]
        public async Task<bool> DeleteVaccinationInfo([FromServices] IVaccinationService service, VillageDetailQueryModel body)
        {
            var result = await service.DeleteVaccinationInfo(body.Id, LoginUser.UserId);
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
        /// 接种详情
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter("新冠疫苗接种登记", PermissionCodes.View)]
        public async Task<VaccinationDto> DetailVaccinationInfo([FromServices] IVaccinationService service, VillageDetailQueryModel body)
        {
            return await service.DetailVaccinationInfo(body.Id);
        }

        /// <summary>
        /// 接种详情
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("statistics")]
        [PermissionFilter("疫情防控台账", PermissionCodes.List)]
        public async Task<IPagedList<StatisticsVaccinationDto>> GetVaccinationStatisticsList([FromServices] IVaccinationService service, VaccinationPagePostBody body)
        {
            return await service.GetVaccinationStatisticsList(body);
        }


        /// <summary>
        /// 疫情防控台账导出
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="keyword"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("statistics/export")]
        [PermissionFilter(ModuleCodes.疫情防控台账, PermissionCodes.Export)]
        public async Task<FileContentResult> StatisticExport([FromServices] IVaccinationService service, [FromQuery] int areaId, DateTime? beginDate, DateTime? endDate, string keyword = "", string orders = "")
        {
            VaccinationPagePostBody body = new VaccinationPagePostBody()
            {
                AreaId = areaId,
                BeginDate = beginDate,
                EndDate = endDate,
                Keyword= keyword,
                Page=1,
                Limit=10000,
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
            byte[] vaccinationBytes = await service.GetVaccinationStatisticsExcelData(body);
            return File(vaccinationBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// 疫苗接种列表导出
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <param name="year"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("export")]
        [PermissionFilter(ModuleCodes.新冠疫苗接种登记, PermissionCodes.Export)]
        public async Task<FileContentResult> Export([FromServices] IVaccinationService service, [FromQuery] int areaId, int year, string ids)
        {
            byte[] vaccinationBytes = await service.GetVaccinationExcelData(areaId, year, ids);
            return File(vaccinationBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
