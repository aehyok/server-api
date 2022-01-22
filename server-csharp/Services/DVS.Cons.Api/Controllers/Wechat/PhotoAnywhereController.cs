using AutoMapper;
using DVS.Application.Services.Cons;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Dtos.Common;
using DVS.Common.Models;

namespace DVS.Cons.Api.Controllers.Wechat
{
    /// <summary>
    /// 随手拍
    /// </summary>
    [Route("/api/cons/wechat/PhotoAnywhere")]
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
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<ListPhotoAnywhereModel>> GetList([FromBody]PhotoAnywhereListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.AreaId = loginuser.AreaId;
            model.UserId = loginuser.UserId;
            return await this.dataService.GetDataList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
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
        /// <returns></returns>
        [HttpPost("detail")]
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
        public async Task<int> Delete([FromBody]DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            return await this.dataService.DeletePhotoAnywhereAsync(model.Id, userid);
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
            var info = await this .dataService.StatisticPhotoAnywhereByType(model.AreaId);
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
        /// 随手拍处理情况
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("statisticPhotoAnywhereByStatus")]
        public List<PhotoAnywhereTypeDto> StatisticPhotoAnywhereByStatus([FromBody] PhotoAnywhereQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            var info = this.dataService.StatisticPhotoAnywhereStatusByUser(loginuser.UserId);
            return info;
        }

        /// <summary>
        /// 我发的随手拍数量
        /// </summary>
        /// <returns></returns>
        [HttpPost("myPhotoAnywhereCount")]
        public async Task<int> MyPhotoAnywhereCount()
        {
            LoginUser loginuser = this.LoginUser;
            return await this.dataService.MyPhotoAnywhereCount(loginuser.UserId);
        }

        /// <summary>
        /// 我发的随手拍
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("myPhotoAnywhere")]
        public async Task<IPagedList<ListPhotoAnywhereModel>> MyPhotoAnywhere([FromBody] PhotoAnywhereListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            return await this.dataService.GetDataList(model);
        }
    }
}
