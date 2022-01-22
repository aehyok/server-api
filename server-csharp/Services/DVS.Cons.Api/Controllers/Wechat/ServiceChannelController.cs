using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DVS.Application.Services.Cons;
using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DVS.Cons.Api.Controllers.Wechat
{
    [Route("/api/cons/wechat/ServiceChannel")]
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
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<ListServiceChannelModel>> GetList([FromBody] PagedListQueryModel model)
        {
            return await this.dataService.GetDataList(model);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<DetailServiceChannelModel> GetDetail([FromBody]DetailQueryModel model)
        {

            return await this.dataService.GetDetail(model.Id,PlatFormCode.WECHAT,LoginUser);

        }
    }


}