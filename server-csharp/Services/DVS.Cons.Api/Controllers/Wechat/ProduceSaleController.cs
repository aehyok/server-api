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

namespace DVS.Cons.Api.Controllers.Wechat
{
    /// <summary>
    /// 农产品销售
    /// </summary>
    [Route("/api/cons/wechat/ProduceSale")]
    public class ProduceSaleController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProduceSaleService saleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public ProduceSaleController(IMapper mapper, IProduceSaleService service)
        {
            this.mapper = mapper;
            this.saleService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<ListProduceSaleModel>> GetList([FromBody] ProduceSaleListQueryModel model)
        {
            //var t = await this.saleService.GetCategoryIds(model.CategoryId);

            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            return await this.saleService.GetDataList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<int> Save([FromBody]CreateProduceSaleModel model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.CreatedBy = userid;
                model.UpdatedBy = model.CreatedBy;
                model.PublishId = loginuser.HouseholdId > 0 ? loginuser.HouseholdId : model.PublishId;
                model.CreatedUserType = 1; // 公众号发布
            }
            else
            {
                model.UpdatedBy = userid;
            }
            model.Address = loginuser != null ? loginuser.AreaName : "";

            var ret = await this.saleService.Save(model, PlatFormCode.WECHAT);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<DetailProduceSaleModel> GetDetail([FromBody]DetailQueryModel model)
        {
            
            return await this.saleService.GetDetail(model.Id, PlatFormCode.WECHAT, LoginUser);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
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
                if (info.CreatedBy != loginuser.UserId)
                {
                    throw new ValidException("不能删除");
                }
                info.IsDeleted = 1;
                info.UpdatedBy = loginuser.UserId;
                return await this.saleService.UpdateAsync(info);
            }

        }

        /// <summary>
        /// 我发布的销售
        /// </summary>
        /// <param name="model"></param>        
        /// <returns></returns>
        [HttpPost("mylist")]
        public async Task<IPagedList<ListProduceSaleModel>> MyList([FromBody]ProduceSaleListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            return await this.saleService.MyDataList(model, PlatFormCode.WECHAT);
        }

        /// <summary>
        /// 所有销售信息
        /// </summary>
        /// <param name="model"></param>         
        /// <returns></returns>
        [HttpPost("alllist")]
        public async Task<IPagedList<ListProduceSaleModel>> GetDataByAreaIdList([FromBody]ProduceSaleListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.AreaId == 0)
            {
                model.AreaId = loginuser.AreaId;
            }
            return await this.saleService.GetDataByAreaIdList(model);
        }

        /// <summary>
        /// 我发布的农产品数量
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpPost("mypublishcount")]
        public async Task<int> MyPublishCount()
        {
            LoginUser loginuser = this.LoginUser;
            return await this.saleService.MyPublishCount(loginuser.UserId, PlatFormCode.WECHAT);
        }
    }
}
