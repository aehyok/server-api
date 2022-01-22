using DVS.Application.Services.IPS;
using DVS.Common.Models;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.IPS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.IPS.Api.Controllers.App
{
    /// <summary>
    /// 日程安排
    /// </summary>
    [Route("/api/ips/app/schedule")]
    public class IPSScheduleController : DvsControllerBase
    {

        private readonly IIPSScheduleService _service;

        public IPSScheduleController(IIPSScheduleService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取日程列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<IPSScheduleDto>> GetScheduleAsync(PagedListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.ListSchedule(model.Keyword, model.Page, model.Limit, loginuser.AreaId);
        }

        /// <summary>
        /// 日程详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<IPSScheduleDto> GetDetailScheduleAsync(DetailQueryModel model)
        {
            return await this._service.DetailSchedule(model.Id);
        }

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<int> DeleteScheduleAsync(DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.DeleteSchedule(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 保存日程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<int> SaveScheduleAsync(IpsSchedule model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id > 0)
            {
                model.UpdatedBy = loginuser.UserId;
            }
            else {
                model.CreatedBy = loginuser.UserId;
                model.UpdatedBy = loginuser.UserId;
                model.AreaId = loginuser.AreaId;
            }
            return await this._service.SaveSchedule(model);
        }

        /// <summary>
        /// 统计日程
        /// </summary>
        /// <returns></returns>
        [HttpPost("statistics")]
        public async Task<IPSStatisticDto> StatisticScheduleAsync()
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.StatisticSchedule(loginuser.AreaId);
        }
    }
}
