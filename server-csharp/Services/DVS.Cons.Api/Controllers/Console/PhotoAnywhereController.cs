using DVS.Application.Services.Cons;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Cons.Api.Controllers.Console
{
    /// <summary>
    /// 随手拍
    /// </summary>
    [Route("/api/cons/console/PhotoAnywhere")]
    public class PhotoAnywhereController : DvsControllerBase
    {
        private readonly IPhotoAnywhereService dataService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public PhotoAnywhereController(IPhotoAnywhereService service)
        {
            this.dataService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter(ModuleCodes.随手拍待处理, 128)]
        public async Task<IPagedList<ListPhotoAnywhereModel>> GetList([FromBody]PhotoAnywhereListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.AreaId == 0)
            {
                model.AreaId = loginuser.AreaId;
            }
            return await this.dataService.GetDataList(model);
        }

        /// <summary>
        /// 获取已处理列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("reply/list")]
        [PermissionFilter(ModuleCodes.随手拍已处理, 128)]
        public async Task<IPagedList<ListPhotoAnywhereModel>> GetReplyList([FromBody] PhotoAnywhereListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.AreaId == 0)
            {
                model.AreaId = loginuser.AreaId;
            }
            model.IsReply = 1;
            return await this.dataService.GetDataList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<int> Save([FromBody]CreatePhotoAnywhereModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.UserId = userid;
                model.CreatedBy = userid;
                model.UpdatedBy = userid;
                model.HouseholdId = loginuser.HouseholdId;
                model.AreaId = loginuser.AreaId;
            }
            else
            {
                model.UpdatedBy = userid;
            }
            if (string.IsNullOrEmpty(model.Address))
            {
                model.Address = loginuser != null ? loginuser.AreaName : "";
            }
            var ret = await this.dataService.Save(model);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter(ModuleCodes.随手拍已处理, 8)]
        public async Task<DetailPhotoAnywhereModel> GetDetail([FromBody]DetailQueryModel model)
        {
            return await this.dataService.GetDetail(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter(ModuleCodes.随手拍待处理, 2)]
        public async Task<StatusCodeResult> DeleteAsync([FromBody]DeleteModel model)
        {
            string[] ids = model.Ids.Split(',');
            foreach (var id in ids)
            {
                LoginUser loginuser = this.LoginUser;
                int userid = loginuser != null ? loginuser.UserId : 0;
                await this.dataService.DeletePhotoAnywhereAsync(Convert.ToInt32(id), userid);
            }
            return Ok();
        }

        /// <summary>
        /// 随手拍统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("statisticByType")]
        public async Task<List<PhotoAnywhereTypeDto>> StatisticPhotoAnywhereByType([FromBody] PhotoAnywhereQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.AreaId == 0)
            {
                model.AreaId = loginuser.AreaId;
            }
            var info = await this.dataService.StatisticPhotoAnywhereByType(model.AreaId);
            return info;
        }

        /// <summary>
        /// 随手拍办理情况
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("statisticByeTypeMonth")]
        public async Task<List<PhotoAnywhereTypeMonthDto>> StatisticPhotoAnywhereByTypeMonth([FromBody] PhotoAnywhereQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.AreaId == 0)
            {
                model.AreaId = loginuser.AreaId;
            }
            var info = await this.dataService.StatisticPhotoAnywhereByTypeMonth(model.AreaId);
            return info;
         }

        /// <summary>
        /// 获取随手拍事件类型
        /// </summary>
        /// <returns></returns>
        [HttpPost("getPhotoAnywhereType")]
        public async Task<List<BasicDictionaryDto>> GetPhotoAnywhereType()
        {
            var info = await this.dataService.GetPhotoAnywhereType();
            return info;
        }

        /// <summary>
        /// 随手拍回复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("reply")]
        [PermissionFilter(ModuleCodes.随手拍待处理, 4)]
        public async Task<bool> Reply([FromBody]ReplyPhotoAnywhereModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.dataService.Reply(model, loginuser) > 0;
        }
    }
}
