using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DVS.Application.Services.Cons;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DVS.Core.Domains.Cons;
using X.PagedList;
using System.Linq.Expressions;
using LinqKit;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.Cons;
using DVS.Common.Models;
using DVS.Models.Enum;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Infrastructures;

namespace DVS.Cons.Api.Controllers.Console
{
    /// <summary>
    /// 农产品销售
    /// </summary>
    [Route("/api/cons/console/ProduceSale")]
    public class ProduceSaleController : DvsControllerBase
    {
        private readonly IProduceSaleService saleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public ProduceSaleController(IProduceSaleService service)
        {
            this.saleService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter("农产品销售", 128)]
        public async Task<IPagedList<ListProduceSaleModel>> GetProduceSaleListAsync([FromBody] ProduceSaleListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            return await this.saleService.GetProduceSaleList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("save")]
        [PermissionFilter("农产品销售", 1)]
        public async Task<int> Save([FromBody]CreateProduceSaleModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                model.UpdatedBy = model.CreatedBy;
                model.CreatedUserType = 2; // 村委代发布
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            model.Address = loginuser != null ? loginuser.AreaName : "";
            var ret = await this.saleService.Save(model, PlatFormCode.CONSOLE);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter("农产品销售", 8)]
        public async Task<DetailProduceSaleModel> GetDetail([FromBody]DetailQueryModel model)
        {

            return await this.saleService.GetDetail(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter("农产品销售", 2)]
        public async Task<int> Delete([FromBody]DetailQueryModel model)
        {
            var info = await this.saleService.GetAsync(a => a.Id == model.Id);
            if (info == null)
            {
                throw new ValidException("数据不存在");
            }
            else
            {
                LoginUser loginuser = this.LoginUser;
                int userid = loginuser != null ? loginuser.UserId : 0;
                //if (info.CreatedBy != userid) {
                //    throw new ValidException("不能删除");
                //}
                info.UpdatedBy = userid;
                info.IsDeleted = 1;
                return await this.saleService.UpdateAsync(info);
            }
        }

        /// <summary>
        /// 获取发布人
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("getpublisher")]
        public async Task<List<PublisherModel>> GetPublisher([FromBody] PostBody model)
        {
            LoginUser loginuser = this.LoginUser;
            model.AreaId = loginuser.AreaId;
            return await this.saleService.GetPublisher(model);
        }
    }
}
