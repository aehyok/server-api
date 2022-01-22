using DVS.Application.Services.FFP;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 摸排管理
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
    public class MoPaiController : DvsControllerBase
    {
        private readonly IFFPMoPaiLogService mopaiService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public MoPaiController(IFFPMoPaiLogService service)
        {
            this.mopaiService = service;
        }

        /// <summary>
        /// 历史摸排列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/mopai/list")]
        public async Task<IPagedList<FFPMatrixHouseholdDto>> GetMoPaiListAsync(PagedListQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await this.mopaiService.ListMoPai(model.Keyword, loginUser.UserId, model.Page, model.Limit);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/mopai/detail")]
        public async Task<FFPMoPaiLogDto> GetMoPaiDetailAsync(DetailQueryModel model)
        {
            return await this.mopaiService.DetailMoPai(model.Id);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/mopai/add")]
        public async Task<int> AddMoPaiAsync(FFPMoPaiLog model)
        {
            LoginUser loginUser = this.LoginUser;
            model.CreatedBy = loginUser.UserId;
            return await this.mopaiService.SaveMoPai(model);
        }

        ///// <summary>
        ///// 修改
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("app/mopai/edit")]
        //public async Task<int> EditMoPaiAsync(FFPMoPaiLog model)
        //{
        //    LoginUser loginUser = this.LoginUser;
        //    model.UpdatedBy = loginUser.UserId;
        //    return await this.mopaiService.SaveMoPai(model);
        //}

        /// <summary>
        /// 统计历史摸排户数
        /// </summary>
        /// <returns></returns>
        [HttpPost("app/mopai/statistic")]
        public async Task<FFPHouseholdStatisticDto> StatisticHouseholdAsync()
        {
            LoginUser loginUser = this.LoginUser;
            return await mopaiService.StatisticHousehold(loginUser.UserId);
        }

        /// <summary>
        /// 按月统计摸排户数
        /// </summary>
        /// <returns></returns>
        [HttpPost("app/mopai/monthstatistic")]
        public async Task<List<FFPHouseholdStatisticDto>> StatisticHouseholdByMonthAsync()
        {
            LoginUser loginUser = this.LoginUser;
            return await mopaiService.StatisticHouseholdByMonth(loginUser.UserId);
        }

        /// <summary>
        /// 查询本月摸排通过待评议，评议通过待公示列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/mopai/listbystatus")]
        public async Task<IPagedList<FFPMatrixHouseholdDto>> GetMoPaiListByStatusAsync(MoPaiLogListQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await this.mopaiService.ListMoPaiByStatus(model.Keyword, loginUser.UserId, model.FlowStatus, model.HouseholdType, model.Page, model.Limit);
        }

        /// <summary>
        /// 本月未摸排花名册列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/mopai/nomopailist")]
        public async Task<IPagedList<FFPMatrixHouseholdDto>> GetNoMoPaiListAsync(MoPaiLogListQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await this.mopaiService.ListMoPaiMonth(model.Keyword, loginUser.UserId, model.HouseholdType, model.Page, model.Limit, 0);
        }

        /// <summary>
        /// 本月已摸排列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/mopai/mopaiedlist")]
        public async Task<IPagedList<FFPMatrixHouseholdDto>> GetMoPaiedListAsync(MoPaiLogListQueryModel model)
        {
            LoginUser loginUser = this.LoginUser;
            return await this.mopaiService.ListMoPaiMonth(model.Keyword, loginUser.UserId, model.HouseholdType, model.Page, model.Limit, 1);
        }
    }
}
