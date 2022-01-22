using AutoMapper;
using DVS.Application.Services.Cons;
using DVS.Common.Models;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Cons.Api.Controllers.App
{
    /// <summary>
    /// 农产品销售
    /// </summary>
    [Route("/api/cons/app/ProduceSale")]
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<ListProduceSaleModel>> GetList([FromBody] ProduceSaleListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            return await this.saleService.GetDataList(model);
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="model"></param>
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
                model.CreatedUserType = 2; // 村委代发布
            }
            else
            {
                model.UpdatedBy = userid;
            }
            model.Address = loginuser != null ? loginuser.AreaName : "";
            var ret = await this.saleService.Save(model, PlatFormCode.APP);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<DetailProduceSaleModel> GetDetail([FromBody] DetailQueryModel model)
        {

            return await this.saleService.GetDetail(model.Id, PlatFormCode.APP);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<int> Delete([FromBody] DetailQueryModel model)
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
                if (info.CreatedBy != userid)
                {
                    throw new ValidException("不能删除");
                }
                info.UpdatedBy = userid;
                info.IsDeleted = 1;
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
            if (model.PublishId == 0)
            {
                model.UserId = loginuser.UserId;
            }
            return await this.saleService.MyDataList(model, PlatFormCode.APP);
        }

        /// <summary>
        /// 所有销售信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("alllist")]
        public async Task<IPagedList<ListProduceSaleModel>> GetDataByAreaIdList([FromBody] ProduceSaleListQueryModel model)
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
        /// <returns></returns>
        [HttpPost("mypublishcount")]
        public async Task<int> MyPublishCount()
        {
            LoginUser loginuser = this.LoginUser;
            return await this.saleService.MyPublishCount(loginuser.UserId, PlatFormCode.APP);
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
