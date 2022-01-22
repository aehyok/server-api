using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DVS.Application.Services.Cons;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DVS.Cons.Api.Controllers.Wechat
{
    [Route("/api/cons/wechat/ServiceGuide")]
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
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<DetailServiceGuideModel> GetDetail([FromBody]DetailQueryModel model)
        {

            return await this.dataService.GetDetail(model.Id,Models.Enum.PlatFormCode.WECHAT,LoginUser);

        }
    }
}