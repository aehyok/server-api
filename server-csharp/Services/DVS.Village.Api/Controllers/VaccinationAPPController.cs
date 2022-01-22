using DVS.Application.Services.Village;
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
    [Route("/api/village/app/vaccination")]
    public class VaccinationAPPController : DvsControllerBase
    {
        /// <summary>
        /// 疫苗接种台账查询
        /// 1 需求接种户籍，2 已接种，3 接种第一针，4 接种第一针本地 ，5 接种第一针异地，6 接种第二针，7 接种第二针本地，8 接种第二针异地，9 未登记，10 未接种
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<VaccinationDto>> GetVaccinationInfoList([FromServices] IVaccinationService service, VaccinationPagePostBody body)
        {
            if (body.AreaId == 0)
            {
                body.AreaId = LoginUser.AreaId;
            }
            var data = await service.GetVaccinationInfoList(body);
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
        /// 接种统计台账
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("statistics")]
        public async Task<IPagedList<StatisticsVaccinationDto>> GetVaccinationStatisticsList([FromServices] IVaccinationService service, VaccinationPagePostBody body)
        {
            body.AreaId = LoginUser.AreaId;
            return await service.GetVaccinationStatisticsList(body);
        }
    }
}
