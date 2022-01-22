using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DVS.Application.Services.Cons;
using DVS.Common;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DVS.Cons.Api.Controllers.Console
{
    [Route("/api/cons/console/ServiceChannel")]
    public class ServiceChannelController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IServiceChannelService dataService;

        public ServiceChannelController(IMapper mapper, IServiceChannelService service)
        {
            this.mapper = mapper;
            this.dataService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<ListServiceChannelModel>> GetList([FromBody] PagedListQueryModel model)
        {
            return await this.dataService.GetDataList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<int> Save([FromBody]CreateServiceChannelModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                model.UpdatedBy = model.CreatedBy;
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            var ret = await this.dataService.Save(model);

            return ret;
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [PermissionFilter("查询渠道管理", 1)]
        public async Task<int> AddAsync([FromBody] CreateServiceChannelModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                model.UpdatedBy = model.CreatedBy;
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            var ret = await this.dataService.Save(model);

            return ret;
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter("查询渠道管理", 4)]
        public async Task<int> EditAsync([FromBody] CreateServiceChannelModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                model.UpdatedBy = model.CreatedBy;
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
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
        public async Task<DetailServiceChannelModel> GetDetail([FromBody]DetailQueryModel model)
        {

            return await this.dataService.GetDetail(model.Id);

        }

        /// <summary>
        /// 取二维码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost("GetQrCode")]
        public string GetQrCode(string url)
        {

            var t = Utils.GetQRCodeImage(url);

            return "data:image/jpg;base64," + t;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter("查询渠道管理", 2)]
        public async Task<int> Delete([FromBody]DetailQueryModel model)
        {
            var info = await this.dataService.GetAsync(a => a.Id == model.Id);
            if (info == null)
            {
                throw new ValidException("数据不存在");
            }
            else
            {
                LoginUser loginuser = this.LoginUser;
                int userid = loginuser != null ? loginuser.UserId : 0;
                info.UpdatedBy = userid;
                info.IsDeleted = 1;
                return await this.dataService.UpdateAsync(info);
            }
        }

    }


}