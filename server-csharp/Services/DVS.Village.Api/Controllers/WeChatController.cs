using DVS.Application.Services;
using DVS.Application.Services.Village;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DVS.Core.Domains.Village;
using X.PagedList;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using DVS.Application.Services.Common;
using DVS.Core.Domains.Common;
using DVS.Model.Dtos.Village;
using DVS.Model.Dtos.Village.Query;
using DVS.Application.SMS;
using DVS.Models.Dtos.Common;
using Microsoft.Extensions.Logging;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Application.Services.Warning;
using DVS.Models.Dtos.Warning;
using DVS.Core.Domains.Warning;
using DVS.Models.Dtos.Village.Household;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 公众号相关接口
    /// </summary>
    [Route("/api/village/wechat")]
    public class WeChatController : DvsControllerBase
    {
        private readonly ILogger logger;
        private readonly IWarningMessageService warningMessageService;
        private readonly IHouseholdCodeService householdCodeService;
        private readonly IPopulationService populationService;
        public WeChatController(ILogger<WeChatController> logger,
            IWarningMessageService warningMessageService,
            IHouseholdCodeService householdCodeService,
            IPopulationService populationService
            )
        {
            this.logger = logger;
            this.warningMessageService = warningMessageService;
            this.householdCodeService = householdCodeService;
            this.populationService = populationService;
        }

        /// <summary>
        /// 获取户码详情！
        /// </summary>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpGet("GetHouseholdCodeDetail")]
        public async Task<VillageHouseholdCode> GetHouseholdCodeDetail([FromServices] IHouseholdCodeService service, int id)
        {
            var data = await service.GetHouseholdCodeDetail(id);
            return data;
        }



        /// <summary>
        /// 户籍人口列表！
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetPopulationList")]
        public async Task<IPagedList<PopulationDto>> GetPopulationList([FromBody] PopulationListBody body)
        {
            body.AreaId = LoginUser.AreaId;
            if (body.HouseholdId <= 0)
            {
                throw new ValidException("缺少必要参数");
            }
            var data = await this.populationService.GetPopulationList(body);
            return data;
        }


        /// <summary>
        /// 户籍人口详情
        /// </summary>
        /// <param name="service"></param>
        /// <param name="id"></param>
        /// <param name="idCard"></param>
        /// <param name="isConvert">是否转字典Id成文字，1转，0不转</param>
        ///  <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetPopulationDetail")]
        public async Task<PopulationDetailDto> GetPopulationDetail(int id, string idCard = "", int isConvert = 0, int householdId = 0)
        {
            var data = await this.populationService.GetPopulationDetail(id, idCard, isConvert, 0, householdId);
            return data;
        }


        /// <summary>
        /// 添加编辑户籍人口
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        [HttpPost("AddPopulation")]
        public async Task<bool> AddPopulation(PopulationDetailDto population)
        {
            population.AreaId = LoginUser.AreaId;
            population.Id = 0;
            var result = await this.populationService.SavePopulation(population);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 添加编辑户籍人口
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        [HttpPost("EditPopulation")]
        public async Task<bool> EditPopulation(PopulationDetailDto population)
        {
            population.AreaId = LoginUser.AreaId;
            if (population.Id <= 0)
            {
                throw new ValidException("请输入Id");
            }
            var result = await this.populationService.SavePopulation(population);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 获取用户所在的户列表
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpPost("population/UserHouseholdList")]
        public async Task<List<UserHouseholdListRes>> GetUserHouseholdList()
        {

            List<UserHouseholdListRes> households = await populationService.GetHouseholdByUserIdAsync(LoginUser.UserId);
            return households;
        }

        /// <summary>
        /// 切换户
        /// </summary>
        /// <param name="service"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("population/SwitchHousehold")]
        public async Task<bool> SwitchHousehold( [FromBody] SwitchHouseholdReq req)
        {
            bool result = await populationService.SwitchHouseholdCodeAsync(req.HoudeholdId, LoginUser.UserId);
            return result;
        }


        /// <summary>
        /// 收入来源详情！
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <param name="year">年度</param>
        /// <returns></returns>
        [HttpGet("GetIncomeDetail")]
        public async Task<VillageIncome> GetIncomeDetail([FromServices] IIncomeService service, int householdId, int year)
        {
            var data = await service.GetIncomeDetail(householdId, year);
            return data;
        }

        /// <summary>
        /// 疫情防控信息！
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetEpidemicPopulationList")]
        public async Task<List<EpidemicPopulationDto>> GetEpidemicPopulationList([FromServices] IEpidemicService service, int householdId)
        {
            var data = await service.GetEpidemicPopulationList(householdId);
            return data;
        }

        /// <summary>
        /// 疫情防控历史！
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <param name="year">年度</param>
        /// <param name="populationId">人口Id 可选</param>
        /// <returns></returns>
        [HttpGet("GetEpidemicInfoList")]
        public async Task<List<EpidemicPopulationDto>> GetEpidemicInfoList([FromServices] IEpidemicService service, int householdId, int year, int populationId = 0)
        {

            var data = await service.GetEpidemicInfoList(householdId, year, populationId);
            return data;
        }



        /// <summary>
        /// 务工人员详情信息！
        /// </summary>
        /// <param name="service"></param>
        /// <param name="householdId"></param>
        /// <param name="year"></param>
        /// <returns></returns>

        [HttpGet("GetWorkInfoList")]
        public async Task<List<VillageWorkInfoDto>> GetWorkInfoList([FromServices] IWorkService service, int householdId, int year)
        {
            var data = await service.GetWorkInfoList(householdId, year);
            return data;
        }

        /// <summary>
        /// 新增外出务工信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SaveWorkInfo")]
        public async Task<bool> SaveWorkInfo([FromServices] IWorkService service, SaveWorkBody body)
        {
            body.AreaId = LoginUser.AreaId;
            var result = await service.SaveWorkInfo(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet("GetBasicDictionaryList")]
        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList([FromServices] IBasicDictionaryService service, int code)
        {
            var data = await service.GetBasicDictionaryList(code);
            return data;
        }


        /// <summary>
        /// 获取用户认证状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserAuthStatus")]
        public async Task<UserAuthStatusDto> GetUserAuthStatus([FromServices] IBasicUserService service)
        {
            if (this.LoginUser.UserId <= 0)
            {
                throw new ValidException("登录过期", -2);
            }
            // this.LoginUser.UserId
            var data = await service.GetUserAuthStatus(this.LoginUser.UserId);
            return data;
        }



        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SendMobileCode")]
        public async Task<object> SendMobileCode([FromServices] ISendSMSService service, SendMobileBody body)
        {

            if (body == null)
            {
                throw new ValidException("缺少必要参数");
            }
            if (string.IsNullOrWhiteSpace(body.Mobile))
            {
                throw new ValidException("请输入手机号码");
            }
            var res = await service.SendUserAuthCode(body.Mobile);
            if (!res.Flag)
            {
                throw new ValidException(res.Message);

            }
            return true;
        }
        /// <summary>
        /// 用户申请村民认证
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("ApplyUserAuth")]
        public async Task<UserAuthStatusDto> ApplyUserAuth([FromServices] IUserAuthRecordService service, ApplyUserAuthDto body)
        {
            var loginUser = this.LoginUser;
            if (loginUser.UserId <= 0)
            {
                throw new ValidException("登录过期", -2);
            }

            // body.AreaId = this.LoginUser.AreaId;
            var user = new VillageUserAuthRecord()
            {
                RealName = body.RealName,
                Mobile = body.Mobile,
                ImageId = body.ImageId,
                ImageUrls = body.ImageUrls,
                HouseholdId = body.HouseholdId,
                UserId = loginUser.UserId,
                IdCard = body.IdCard,
                AreaId = body.AreaId
            };

            var result = await service.ApplyUserAuth(user, body.Action, body.MobileCode);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 根据身份证获取门牌号列表
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="idCard"></param>
        /// <returns></returns>
        [HttpPost("GetHouseNumberListByIdCard")]
        public async Task<List<HouseNumberDto>> GetHouseNumberListByIdCard(int areaId, string idCard)
        {

            var data = await this.householdCodeService.GetHouseNumberListByIdCard(areaId, idCard);

            if (data == null)
            {
                return new List<HouseNumberDto>();
            }
            return data;
        }

        /// <summary>
        /// 户主门牌信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GethouseholderAndHouseNumber")]
        public async Task<HouseholderAndHouseNumberDto> GethouseholderAndHouseNumber(int householdId)
        {
            var data = await this.householdCodeService.GethouseholderAndHouseNumber(householdId);
            return data;
        }


        /// <summary>
        /// 获取区域树结构
        /// </summary>
        /// <param name="areaId">父节点，传0就是获取全部的树</param>
        /// <returns></returns>
        [HttpGet("GetAreaTree")]
        public async Task<BasicAreaTreeDto> GetAreaTree([FromServices] IBasicAreaService service, int areaId = 0)
        {
            // areaId = this.LoginUser.AreaId;
            var data = await service.GetBasicAreaTree(areaId);
            return data;
        }

        /// <summary>
        /// 获取土地信息汇总
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpGet("GetStatisticsFarmland")]
        public async Task<List<StatisticsFarmlandDto>> GetStatisticsFarmland([FromServices] IVillageFarmlandService service, int householdId)
        {
            //  var result = await service.SaveHouseholdCode(post);
            var data = await service.GetStatisticsFarmland(householdId);
            return data;
        }


        /// <summary>
        /// 告警信息添加
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddWarnigMessage")]
        public async Task<int> AddWarnigMessage([FromBody] WarnigMessageCreateBody body)
        {
            var data = new WarningMessage()
            {
                Id = body.Id,
                Mobile = body.Mobile,
                Address = body.Address,
                AreaId = body.AreaId,
                Category = body.Category,
                CreatedAt = DateTime.Now,
                Descrition = body.Descrition,
                ImageUrl = body.ImageUrl,
                VideoUrl = body.VideoUrl,
                UpdatedAt = DateTime.Now,
                Title = body.Title,
                IsFinish = 0,
                CreatedBy = LoginUser.UserId,
                IsDeleted = 0,
                Level = body.Level,
                UpdatedBy = 0
            };
            var res = await this.warningMessageService.AddWarningMessage(data, LoginUser.NickName);
            if (res != null)
            {
                return res.Id;
            }
            else
            {
                throw new ValidException("操作失败");
            }
        }
        /// <summary>
        /// 上传住宅（户码）图片
        /// </summary>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("SetHouseholdImage")]
        public async Task<bool> SetHouseholdImage(SetHouseholdImageBody body)
        {

            var result = await this.householdCodeService.SetHouseholdImage(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("DeleteEpidemicInfo")]
        public async Task<bool> DeleteEpidemicInfo([FromServices] IEpidemicService service, VillageDetailQueryModel body)
        {
            var result = await service.DeleteEpidemicAsync(body.Id, LoginUser.UserId);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SaveEpidemicInfo")]
        public async Task<bool> SaveEpidemicInfo([FromServices] IEpidemicService service, VillageEpidemic body)
        {
            if (body.Id == 0)
            {
                body.AreaId = LoginUser.AreaId;
                body.CreatedBy = LoginUser.UserId;
                body.UpdatedBy = LoginUser.UserId;
            }
            else
            {
                body.UpdatedBy = LoginUser.UserId;
            }
            var result = await service.SaveEpidemicInfo(body);
            if (!result.Flag)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// 设置设置户主
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("SetHouseholdRelationship")]
        public async Task<bool> SetHouseholdRelationship(SetRelationshipBody body)
        {
            var result = await this.populationService.SetHouseholdRelationship(body);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取图片列表 
        /// </summary>

        [HttpGet("GetSunFileInfoList")]
        public async Task<List<SunFileInfoDto>> GetSunFileInfoList([FromServices] ISunFileInfoService service, string ids)
        {
            var data = await service.GetSunFileInfoList(ids);
            return data;
        }


        /// <summary>
        /// 移除户码
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("removeFromHousehold")]
        public async Task<bool> SetFromHousehold([FromServices] IPopulationService service, SetFromHouseholdBody body)
        {
            body.Action = 0;
            var result = await service.SetFromHousehold(body);
            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 户籍人口详情
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="idCard"></param>
        /// <param name="isConvert">是否转字典Id成文字，1转，0不转</param>
        ///  <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetPopulationDetailByIdCard")]
        public async Task<PopulationDetailDto> GetPopulationDetailByIdCard(string idCard, int areaId, int isConvert = 0, int householdId = 0)
        {
            if (areaId <= 0)
            {
                areaId = LoginUser.AreaId;
            }
            var data = await this.populationService.GetPopulationDetail(0, idCard, isConvert, 0, householdId, areaId);
            return data;
        }

        /// <summary>
        /// 根据户籍id查询户码列表
        /// </summary>
        /// <param name="service"></param>
        /// <param name="populationId"></param>
        /// <returns></returns>
        [HttpGet("GetHouseholdList")]
        public async Task<List<VillageHouseholdCode>> GetHouseholdList([FromServices] IPopulationService service, int populationId)
        {
            var data = await service.GetHouseholdList(populationId);
            return data;
        }

        /// <summary>
        /// 村门牌名列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetHouseNameList")]

        public async Task<List<HouseNameDto>> GetHouseNameList([FromServices] IHouseholdCodeService service, int areaId)
        {
            if (areaId <= 0)
            {
                areaId = LoginUser.AreaId;
            }
            var data = await service.GetHouseNameList(areaId);
            return data;
        }

        /// <summary>
        /// 增加编辑户码
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("household/saveHouseholdCode")]
        public async Task<bool> SaveHouseholdCode([FromServices] IHouseholdCodeService service, HouseholdCodeBody body)
        {
            var post = new VillageHouseholdCode()
            {
                Id = body.Id,
                AreaId = body.AreaId,
                HouseNameId = body.HouseNameId,
                HouseNumber = body.HouseNumber,
                Tags = body.Tags,
                Status = body.Status,
                Remark = body.Remark,
            };
            var result = await service.SaveHouseholdCode(post, LoginUser);

            if (result.Flag == false)
            {
                throw new ValidException(result.Message);
            }
            else
            {
                return true;
            }
        }
    }
}
