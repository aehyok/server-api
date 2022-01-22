using AutoMapper;
using DVS.Application.Services.FFP;
using DVS.Common.Models;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 公示管理操作
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
    public class PublicityManageController : DvsControllerBase
    {
        IFFPPublicityManageService publicityService;
        private readonly IFFPWorkflowService workflowService;
        private readonly IMapper mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        /// <param name="publicityService"></param>
        public PublicityManageController(IMapper mapper,
            IFFPWorkflowService service,
            IFFPPublicityManageService publicityService)
        {
            this.publicityService = publicityService;
            this.mapper = mapper;
            this.workflowService = service;
        }

        /// <summary>
        /// 保存公示信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/PublicityManage/SavePublicityManage")]
        public async Task<int> SavePublicityManageAsync(PublicityManageAddDto model)
        {
            LoginUser loginUser = this.LoginUser;

            var ret = 0;
            if (model.Ids.Count() > 0)
            {
                var info = new ConsInfoPublic();
                if (model.Type == 1)
                {                   
                    info.Type = model.PublishType;
                    info.AreaId = loginUser.AreaId;
                    info.CreatedBy = loginUser.UserId;
                    info.MessageName = loginUser.AreaName + "防返贫监测民主评议结果公示" + DateTime.Now.ToString("yyyy-MM-dd");
                    info.MessageText = string.Format(@"{0}防返贫监测民主评议结果公示

根据农户申请、授权等程序，{1}，我村组织召开了村“两委”（扩大）会议，履行防返贫监测民主评议程序。现将民主评议结果公示如下（名单附后）。

如有异议，请从即日起5日内向村委会提出意见。

          村民委员会（公章）

          {1}" ,loginUser.AreaName, DateTime.Now.ToString("yyyy年MM月dd日"));
                    

                }

                var title = (model.Type == 1) ? "防返贫监测民主评议结果公示" : "防返贫监测民主评议结果报告";
                FFPPublicityManage data = new FFPPublicityManage();
                data.AreaId = loginUser.AreaId;
                data.AreaName = loginUser.AreaName;
                data.Title = title;
                data.Type = model.Type;
                data.CreatedUser = loginUser.NickName;
                ret = await publicityService.SavePublicityManage(data, model,info);
            }
            else
            {
                throw new ValidException("户码ID不能为空");
            }
            return ret;
        }

        /// <summary>
        /// 取评议公示详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("app/PublicityManage/GetPublicityManageDetail")]
        // [AllowAnonymous]
        public async Task<PublicityManageDetailDto> GetPublicityManageDetailAsync(int id)
        {
            var data = await workflowService.GetPublicityHouseholdList(id);
            var list = this.mapper.Map<List<PublicityList>>(data);

            var ret = new PublicityManageDetailDto();
            ret.PublishDate = DateTime.Now;
            ret.AreaName = "乔营子村";
            ret.PublicityList = list;

            return ret;
        }

        /// <summary>
        /// 取公示报告详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("app/PublicityManage/GetReviewReportManageDetail")]
        // [AllowAnonymous]
        public async Task<ReviewReportManageDetailDto> GetReviewReportManageDetailAsync(int id)
        {
            var data = await workflowService.GetPublicityHouseholdList(id);
            var list = this.mapper.Map<List<PublicityList>>(data);

            var ret = new ReviewReportManageDetailDto();
            ret.ReportDate = DateTime.Now;
            ret.AreaName = "乔营子村";
            ret.ReportList = list;

            return ret;
        }



    }
}
