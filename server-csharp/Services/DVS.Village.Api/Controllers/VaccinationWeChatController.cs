using DVS.Application.Services.Village;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Vaccination;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 疫苗接种管理
    /// </summary>
    [Route("/api/village/wechat/vaccination")]
    public class VaccinationWeChatController : DvsControllerBase
    {

        /// <summary>
        /// 疫苗接种查询（按户码统计）
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<VaccinationHouseholdDto>> GetVaccinationList([FromServices] IVaccinationService service, PagePostBody body)
        {
            var data = await service.GetVaccinationList(body);
            return data;
        }

        /// <summary>
        /// 疫苗接种记录
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body">年度</param>
        /// <returns></returns>
        [HttpPost("infolist")]
        public async Task<IPagedList<VaccinationDto>> GetVaccinationInfoList([FromServices] IVaccinationService service, VaccinationPagePostBody body)
        {
            var data = await service.GetVaccinationInfoList(body.HouseholdId, body.Year, 0, body.Page, body.Limit);
            return data;
        }

        /// <summary>
        /// 疫苗接种记录(按户查询)
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("listbyhousehold")]
        public async Task<IPagedList<VaccinationDto>> GetVaccinationHouseholdList([FromServices] IVaccinationService service, VaccinationPagePostBody body)
        {
            var data = await service.GetVaccinationHouseholdList(body);
            return data;
        }

        /// <summary>
        /// 登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("add")]
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
        [HttpPost("listinfo")]
        public async Task<List<VaccinationDto>> ListVaccinationInfo([FromServices] IVaccinationService service, PagePostBody body)
        {
            return await service.VaccinationInfoList(body.PopulationId, body.HouseholdId);
        }

        /// <summary>
        /// 按户码统计接种记录
        /// </summary>
        /// <param name="service"></param>
        /// <param name="householdId">户码id</param>
        /// <param name="page">分页</param>
        /// <param name="limit">分页大小</param>
        /// <returns></returns>
        [HttpGet("statisticbyhousehold")]
        public async Task<IPagedList<VaccinationInfoDto>> getVaccinationInfo([FromServices] IVaccinationService service, int householdId, int page = 1, int limit = 10)
        {
            return await service.GetVaccinationStatisticsList(householdId, page, limit);
        }
    }
}
