using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DVS.Application.Services.Cons;
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
    [Route("/api/cons/console/ServiceGuide")]
    public class ServiceGuideController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IServiceGuideService dataService;

        public ServiceGuideController(IMapper mapper, IServiceGuideService service)
        {
            this.mapper = mapper;
            this.dataService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<ListServiceGuideModel>> GetList([FromBody] PagedListQueryModel model)
        {
            return await this.dataService.GetDataList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<int> Save([FromBody]CreateServiceGuideModel model)
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
        /// <returns></returns>
        [HttpPost("add")]
        [PermissionFilter("办事指南管理", 1)]
        public async Task<int> AddAsync([FromBody] CreateServiceGuideModel model)
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
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter("办事指南管理", 4)]
        public async Task<int> EditAsync([FromBody] CreateServiceGuideModel model)
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
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<DetailServiceGuideModel> GetDetail([FromBody]DetailQueryModel model)
        {

            return await this.dataService.GetDetail(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter("办事指南管理", 2)]
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