using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.FFP;
using DVS.Common.SO;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 会议评议
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
    public class ReviewController : DvsControllerBase
    {

        private readonly IFFPWorkflowService workflowService;
        private readonly IMapper mapper;
        private readonly IModuleDictionaryService moduleDictionaryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        /// <param name="moduleDictionaryService"></param>
        public ReviewController(IMapper mapper,
            IFFPWorkflowService service,
             IModuleDictionaryService moduleDictionaryService)
        {
            this.mapper = mapper;
            this.workflowService = service;
            this.moduleDictionaryService = moduleDictionaryService;
        }

        /// <summary>
        /// 取会议评议列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetReviewList")]
        [AllowAnonymous]
        public async Task<IPagedList<ReviewListDto>> GetReviewListAsync(ReviewListReq model)
        {
            var ret = await workflowService.GetReviewList(model);

            return ret;
        }
        /// <summary>
        /// 取评议详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetReviewDetail")]
        [AllowAnonymous]
        public async Task<ReviewDetailDto> GetReviewDetailAsync(int id)
        {
            var ret = await workflowService.DetailWorkflow(id);
            return ret;
        }


        /// <summary>
        /// 取评议公示管理列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetPublicityManageList")]
        [AllowAnonymous]
        public async Task<IPagedList<PublicityManageDto>> GetPublicityManageListAsync(PublicityManageListReq model)
        {
            // 类型type  1评议公示2评议报告
            var data = await workflowService.GetPublicityManageList(model, 1);
            var list = new List<PublicityManageDto>();
            foreach (var t in data)
            {
                var r = this.mapper.Map<PublicityManageDto>(t);
                list.Add(r);
            }
            var ret = new StaticPagedList<PublicityManageDto>(list, data.PageNumber, data.PageSize, data.TotalItemCount);

            return ret;
        }

        /// <summary>
        /// 取评议公示详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetPublicityManageDetail")]
        [AllowAnonymous]
        public async Task<PublicityManageDetailDto> GetPublicityManageDetailAsync(int id)
        {
            var data = await workflowService.GetPublicityHouseholdList(id);
            var list = this.mapper.Map<List<PublicityList>>(data);

            var ret = new PublicityManageDetailDto();
            ret.PublishDate = DateTime.Now;
            ret.AreaName = "乔营子村";
            ret.PublicityList = list;

            return ret;

            //var ret = new PublicityManageDetailDto();
            //ret.PublishDate = DateTime.Now;
            //ret.AreaName = "田东村";

            //var tlist = new List<PublicityList>();
            //tlist.Add(new PublicityList()
            //{
            //    HouseholdMan = "赵东鹏",
            //    HouseholdType = "突发严重困难户",
            //    IdCard = "430122195601101206",
            //    Mobile = "13800138000",
            //    HouseName = "上东二组",
            //    MonitorObj = "是",
            //    Relationship = "户主",
            //    Sex = "男",
            //    Remark = ""
            //});
            //ret.PublicityList = tlist;
            //var elist = new List<EliminateRiskList>();
            //elist.Add(new EliminateRiskList()
            //{
            //    HouseholdMan = "赵东鹏",
            //    IdCard = "430122195601101206",
            //    HouseName = "上东二组",
            //    PeopleCount = 5,
            //    Type = "自然消除",
            //    Remark = ""
            //});
            //ret.EliminateRiskList = elist;

            //return ret;
        }



        /// <summary>
        /// 取公示名单管理列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetPublicityListManageList")]
        [AllowAnonymous]
        public async Task<IPagedList<PublicityListManageDto>> GetPublicityListManageListAsync(PublicityManageListReq model)
        {
            var data = await workflowService.GetPublicityListManageList(model);
            var dictionarys100A03 = await this.moduleDictionaryService.GetModuleDictionaryAsync("100A03");
            var list = new List<PublicityListManageDto>();
            foreach (var item in data)
            {
                var r = this.mapper.Map<PublicityListManageDto>(item);
                if (item.HouseholdType == "100A03001" || item.HouseholdType == "100A03002" || item.HouseholdType == "100A03003" || item.HouseholdType == "100A03A02")
                {
                    r.ReviewType = "致贫风险";
                }
                else
                {
                    r.ReviewType = "";
                }
                r.Mobile = BasicSO.Decrypt(r.Mobile);

                var dic100A03 = dictionarys100A03.FirstOrDefault(a => a.Code == item.HouseholdType);
                if (dic100A03 != null)
                {
                    r.HouseholdType = dic100A03.Name;
                }
                list.Add(r);
            }
            var ret = new StaticPagedList<PublicityListManageDto>(list, data.PageNumber, data.PageSize, data.TotalItemCount);
            return ret;
        }

        /// <summary>
        /// 取公示名单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetPublicityListDetail")]
        [AllowAnonymous]
        public async Task<PublicityListDetailDto> GetPublicityListDetailAsync(int id)
        {
            var ret = await workflowService.GetPublicityListDetail(id);
            return ret;
        }



        /// <summary>
        /// 取评议报告管理列表(####)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetReviewReportManageList")]
        [AllowAnonymous]
        public async Task<IPagedList<ReviewReportManageDto>> GetReviewReportManageListAsync(PublicityManageListReq model)
        {
            // 类型type  1评议公示2评议报告
            var data = await workflowService.GetPublicityManageList(model, 2);
            var list = new List<ReviewReportManageDto>();
            foreach (var t in data)
            {
                var r = this.mapper.Map<ReviewReportManageDto>(t);
                list.Add(r);
            }
            var ret = new StaticPagedList<ReviewReportManageDto>(list, data.PageNumber, data.PageSize, data.TotalItemCount);


            return ret;
        }

        /// <summary>
        /// 取评议报告管理详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("console/review/GetReviewReportManageDetail")]
        [AllowAnonymous]
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
