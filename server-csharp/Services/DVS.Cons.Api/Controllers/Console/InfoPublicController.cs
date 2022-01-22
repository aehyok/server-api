using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Cons;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DVS.Cons.Api.Controllers.Console
{
    /// <summary>
    /// 公开消息
    /// </summary>
    [Route("/api/cons/console/infopublic")]
    public class InfoPublicController : DvsControllerBase
    {

        private readonly IInfoPublicService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public InfoPublicController(IInfoPublicService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 查询公开信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("threethings/list")]
        [PermissionFilter("三务公开", 128)]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetThreethingsListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.Type = 1;
            return  await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, model.AreaId);
        }

        /// <summary>
        /// 查询公开信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("partybuilding/list")]
        [PermissionFilter("党建宣传", 128)]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetPartybuildingListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.Type = 2;
            return await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, model.AreaId);
        }

        /// <summary>
        /// 查询公开信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("spiritualcivilization/list")]
        [PermissionFilter("精神文明", 128)]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetSpiritualcivilizationListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.Type = 3;
            return await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, model.AreaId);
        }

        /// <summary>
        /// 查询公开信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("conveniencemessage/list")]
        [PermissionFilter("便民信息", 128)]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetConveniencemessageListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.Type = 4;
            return await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, model.AreaId);
        }

        /// <summary>
        /// 删除公开消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("threethings/delete")]
        [PermissionFilter("三务公开", 2)]
        public Task<int> DeleteThreethingsAsync(DetailQueryModel model) {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            return this._service.DeleteConsInfoPublic(model.Id, userid);
        }

        /// <summary>
        /// 删除公开消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("partybuilding/delete")]
        [PermissionFilter("党建宣传", 2)]
        public Task<int> DeletePartybuildingAsync(DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            return this._service.DeleteConsInfoPublic(model.Id, userid);
        }

        /// <summary>
        /// 删除公开消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("spiritualcivilization/delete")]
        [PermissionFilter("精神文明", 2)]
        public Task<int> DeleteSpiritualcivilizationAsync(DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            return this._service.DeleteConsInfoPublic(model.Id, userid);
        }

        /// <summary>
        /// 删除公开消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("conveniencemessage/delete")]
        [PermissionFilter("便民信息", 2)]
        public Task<int> DeleteConveniencemessageAsync(DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            return this._service.DeleteConsInfoPublic(model.Id, userid);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("threethings/save")]
        [PermissionFilter("三务公开", 1)]
        public async Task<int> SaveThreethingsAsync( ConsInfoPublic model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Type != 1) {
                throw new ValidException("参数无效");
            }
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                //model.AreaId = loginuser.AreaId;
            }
            else {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            return await this._service.SaveConsInfoPublic(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("partybuilding/save")]
        [PermissionFilter("党建宣传", 1)]
        public async Task<int> SavePartybuildingAsync(ConsInfoPublic model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Type != 2)
            {
                throw new ValidException("参数无效");
            }
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                //model.AreaId = loginuser.AreaId;
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            return await this._service.SaveConsInfoPublic(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("spiritualcivilization/save")]
        [PermissionFilter("精神文明", 1)]
        public async Task<int> SaveSpiritualcivilizationAsync(ConsInfoPublic model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Type != 3)
            {
                throw new ValidException("参数无效");
            }
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                //model.AreaId = loginuser.AreaId;
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            return await this._service.SaveConsInfoPublic(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("conveniencemessage/save")]
        [PermissionFilter("便民信息", 1)]
        public async Task<int> SaveConveniencemessageAsync(ConsInfoPublic model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Type != 4)
            {
                throw new ValidException("参数无效");
            }
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                //model.AreaId = loginuser.AreaId;
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            return await this._service.SaveConsInfoPublic(model);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("threethings/detail")]
        [PermissionFilter("三务公开", 8)]
        public Task<ConsInfoPublicDto> DetailThreethingsAsync(DetailQueryModel model)
        {
            return this._service.DetailConsInfoPublic(model.Id);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("partybuilding/detail")]
        [PermissionFilter("党建宣传", 8)]
        public Task<ConsInfoPublicDto> DetailPartybuildingAsync(DetailQueryModel model)
        {
            return this._service.DetailConsInfoPublic(model.Id);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("spiritualcivilization/detail")]
        [PermissionFilter("精神文明", 8)]
        public Task<ConsInfoPublicDto> DetailSpiritualcivilizationAsync(DetailQueryModel model)
        {
            return this._service.DetailConsInfoPublic(model.Id);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("conveniencemessage/detail")]
        [PermissionFilter("便民信息", 8)]
        public Task<ConsInfoPublicDto> DetailConveniencemessageAsync(DetailQueryModel model)
        {
            return this._service.DetailConsInfoPublic(model.Id);
        }
    }
}
