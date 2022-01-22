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
using DVS.Models.Dtos.Common;
using DVS.Models.Enum;
using DVS.Common.SO;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Core.Domains.Warning;
using DVS.Models.Dtos.Warning;
using DVS.Application.Services.Warning;
using DVS.Common.Services;
using DVS.Application.SMS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
 

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// APP相关接口
    /// </summary>
  [DVS.Common.Infrastructures.PermissionFilterAttribute("",10)]
    [Route("/api/village/app")]
    public class AppController : DvsControllerBase
    {

        // readonly IBasicSOService basicSOService;
        private readonly IWarningMessageService warningMessageService;
        private readonly IBasicUserService basicUserService;
        private readonly IPushService pushService;
        private readonly IServiceBase<BasicUserLogin> userLoginSerice;
        private readonly ISendSMSService sendSMSService;
        private readonly ILogger<AppController> logger;
        private readonly IConfiguration configuration;
        private readonly IPopulationService populationService;
        private readonly IIncomeService incomeService;
        public AppController(IWarningMessageService warningMessageService, 
            IBasicUserService basicUserService,
            IPushService pushService,
            IServiceBase<BasicUserLogin> userLoginSerice,
            ISendSMSService sendSMSService,
            ILogger<AppController> logger,
            IConfiguration configuration,
            IPopulationService populationService,
            IIncomeService incomeService
            ) {
            this.warningMessageService = warningMessageService;
            this.basicUserService = basicUserService;
            this.pushService = pushService;
            this.userLoginSerice = userLoginSerice;
            this.sendSMSService = sendSMSService;
            this.logger = logger;
            this.configuration = configuration;
            this.populationService = populationService;
            this.incomeService = incomeService;
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
        /// 获取户码列表
        /// </summary>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("GetHouseholdCodeList")]
        public async Task<IPagedList<HouseholdCodeDto>> GetHouseholdCodeList([FromServices] IHouseholdCodeService service, [FromBody] PostBody body) //  [FromBody]PostBody body  dynamic body
        {
            body.AreaId = LoginUser.AreaId;
            var data = await service.GetHouseholdCodeList(body);
            return data;
        }


        /// <summary>
        /// 获取户码详情
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
        /// 上传住宅（户码）图片
        /// </summary>
        /// <returns></returns>
        // [AllowAnonymous]
        [HttpPost("SetHouseholdImage")]
        public async Task<bool> SetHouseholdImage([FromServices] IHouseholdCodeService service, SetHouseholdImageBody body)
        {
           
            var result = await service.SetHouseholdImage(body);
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
        /// 户籍人口列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetPopulationList")]
        public async Task<IPagedList<PopulationDto>> GetPopulationList([FromServices] IPopulationService service, [FromBody] PopulationListBody body)
        {
            body.AreaId = LoginUser.AreaId;
            if (body.HouseholdId <= 0)
            {
                throw new ValidException("缺少必要参数");
            }
            var data = await service.GetPopulationList(body);
            return data;
        }


        /// <summary>
        /// 户籍人口详情
        /// </summary>
        /// <param name="service"></param>
        /// <param name="id"></param>
        /// <param name="idCard"></param>
        /// <param name="isConvert">是否转字典Id成文字，1转，0不转</param>
        /// <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetPopulationDetail")]
        public async Task<PopulationDetailDto> GetPopulationDetail([FromServices] IPopulationService service, int id, string idCard = "", int isConvert = 0, int householdId = 0)
        {
            var data = await service.GetPopulationDetail(id, idCard, isConvert,0, householdId);
            return data;
        }


        /// <summary>
        /// 添加编辑户籍人口
        /// </summary>
        /// <param name="villagePopulation"></param>
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
        /// <param name="villagePopulation"></param>
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
        /// 更换户主
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("household/setHouseholdRelationship")]
        public async Task<bool> SetHouseholdRelationship(SetRelationshipBody body) {
            MessageResult<bool> result= await this.populationService.SetHouseholdRelationship(body);
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
        /// 添加收入来源
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddIncome")]
        public async Task<int> AddIncome(SaveIncomeBody body)
        {
            body.AreaId = LoginUser.AreaId;
            var result = await this.incomeService.SaveIncome(body);
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
        /// 编辑收入来源
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditIncome")]
        public async Task<int> EditIncome(SaveIncomeBody body)
        {
            body.AreaId = LoginUser.AreaId;
            var result = await this.incomeService.SaveIncome(body);
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
        /// 收入来源详情
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
        /// 疫情防控信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GetEpidemicPopulationList")]
        public async Task<List<EpidemicPopulationDto>> GetEpidemicPopulationList([FromServices] IEpidemicService service, int householdId)
        {
            var data = await service.GetEpidemicPopulationList(householdId);
            return data;
        }

        /// <summary>
        /// 疫情防控历史
        /// </summary>
        /// <param name="service"></param>
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
        /// 登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AddEpidemicInfo")]
        public async Task<bool> AddEpidemicInfo([FromServices] IEpidemicService service, VillageEpidemic body)
        {
            body.AreaId = LoginUser.AreaId;
            body.Id = 0;
            if (body.Id == 0)
            {
                body.AreaId = LoginUser.AreaId;
                body.CreatedBy = LoginUser.UserId;
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
        /// 登记疫情信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditEpidemicInfo")]
        public async Task<bool> EditEpidemicInfo([FromServices] IEpidemicService service, VillageEpidemic body)
        {
            body.AreaId = LoginUser.AreaId;
            if (body.Id <= 0) {
                throw new ValidException("缺少Id");
            }
            body.UpdatedBy = LoginUser.UserId;
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
        /// 务工人员详情信息
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
        [HttpPost("AddWorkInfo")]
        public async Task<bool> AddWorkInfo([FromServices] IWorkService service, SaveWorkBody body)
        {
            body.AreaId = LoginUser.AreaId;
            body.Id = 0;
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
        /// 新增外出务工信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("EditWorkInfo")]
        public async Task<bool> EditWorkInfo([FromServices] IWorkService service, SaveWorkBody body)
        {
            body.AreaId = LoginUser.AreaId;

            if (body.Id <= 0)
            {
                throw new ValidException("缺少Id");
            }

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
        /// 村民认证申请列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetUserAuthList")]
        public async Task<IPagedList<VillageUserAuthRecord>> GetUserAuthList([FromServices] IUserAuthRecordService service, PageUserAuthBody body)
        {
            body.AreaId = LoginUser.AreaId;
            var result = await service.GetUserAuthList(body);
            return result;
        }


        /// <summary>
        /// 村民认证申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUserAuthDetail")]
        public async Task<UserAuthRecordDto> GetUserAuthDetail([FromServices] IUserAuthRecordService service, int id)
        {
            var result = await service.GetUserAuthDetail(id);
            return result;
        }

        /// <summary>
        /// 村门牌号列表
        /// </summary>
        /// <param name="areaId">行政区域Id</param>
        /// <param name="houseName">门牌名</param>
        /// <returns></returns>
        [HttpGet("GetHouseNumberList")]
        public async Task<IEnumerable<HouseNumberDto>> GetHouseNumberList([FromServices] IHouseholdCodeService service,int houseNameId)
        {
            var data = await service.GetHouseNumberList(LoginUser.AreaId, houseNameId);
            return data;
        }



        /// <summary>
        /// 审核村民认证
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("AuditUserAuth")]
        public async Task<bool> AuditUserAuth([FromServices] IUserAuthRecordService service, AuditUserAuthDto body)
        {
            body.AreaId = LoginUser.AreaId;
            var auth = new VillageUserAuthRecord()
            {
                Id = body.Id,
                AreaId = body.AreaId,
                AuditStatus = body.AuditStatus,
                AuditRemark = body.AuditRemark,
                Auditor = this.LoginUser == null ? 0 : this.LoginUser.UserId,
            };
            var result = await service.AuditUserAuth(auth, body.HouseholdId);
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
        /// <param name="code">类型code</param>
        /// <returns></returns>
        [HttpGet("GetBasicDictionaryList")]
        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList([FromServices] IBasicDictionaryService service, int typeCode)
        {
            var data = await service.GetBasicDictionaryList(typeCode);
            return data;
        }

        /// <summary>
        /// 户主门牌信息
        /// </summary>
        /// <param name="householdId">户码Id</param>
        /// <returns></returns>
        [HttpGet("GethouseholderAndHouseNumber")]
        public async Task<HouseholderAndHouseNumberDto> GethouseholderAndHouseNumber([FromServices] IHouseholdCodeService service, int householdId)
        {
            var data = await service.GethouseholderAndHouseNumber(householdId);
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
            // areaId = LoginUser.AreaId;
            var data = await service.GetBasicAreaTree(areaId);
            return data;
        }

        /// <summary>
        /// 计算户数和人数
        /// </summary>
        /// <param name="areaId">区域行政Id</param>
        /// <returns></returns>
        [HttpGet("HouseholdStatistics")]
        public async Task<HouseholdStatisticsDto> HouseholdStatistics([FromServices] IHouseholdCodeService service)
        {
            var data = await service.HouseholdStatistics(LoginUser.AreaId);
            return data;
        }

        /// <summary>
        /// 获取相关状态用户数
        /// </summary>
        /// <param name="areaId">区域行政Id</param>
        /// <param name="auditStatus">审核状态 0未申请， 1待审核，2审核通过，3审核失败</param>
        /// <returns></returns>
        [HttpGet("GetUserAuthCount")]
        public async Task<int> GetUserAuthCount([FromServices] IBasicUserService service, UserAuthAuditStatusEnum auditStatus)
        {
            var data = await service.GetUserAuthCount(LoginUser.AreaId, auditStatus);
            return data;
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
            var result = await service.SaveHouseholdCode(post,LoginUser);

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
        /// 告警信息查询列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("GetWarningMessagePageList")]
        public async Task<IPagedList<WarningMessage>> GetWarningMessagePageList([FromBody] WarnigMessageQueryBody body) {
            return await this.warningMessageService.GetWarningMessagePageList(body);
        }


        /// <summary>
        /// 告警信息查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetWarningMessageDetail")]
        public async Task<WarningMessage> GetWarningMessageDetail(int id)
        {
            return await this.warningMessageService.GetAsync(id);
        }


        /// <summary>
        /// 解除告警信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("FinishWarningMessage/{id}")]
        public async Task<bool> FinishWarningMessage(int id)
        {
            var result = await this.warningMessageService.FinishWarningMessage(id, LoginUser.UserId, LoginUser.NickName);
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
        /// 告警信息操作日志
        /// </summary>
        /// <param name="warningMessageId">告警信息id</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetWarningOperationLogList")]
        public async Task<List<WarningOperationLog>> GetWarningOperationLogList(int warningMessageId)
        {
            var result = await this.warningMessageService.GetWarningOperationLogList(warningMessageId);
            return result;
        }

        /// <summary>
        /// 告警信息添加
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("AddWarnigMessage")]
        public async Task<int> AddWarnigMessage([FromBody] WarnigMessageCreateBody body)
        {

            if (string.IsNullOrWhiteSpace(body.Mobile))
            {
                throw new ValidException("请输入正确的手机号码");
            }
            if (string.IsNullOrWhiteSpace(body.Title))
            {
                throw new ValidException("请输入正确的标题");
            }
            if (string.IsNullOrWhiteSpace(body.Descrition))
            {
                body.Descrition = "";
            }
            string mobile = body.Mobile;
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
            var res = await this.warningMessageService.AddWarningMessage(data, "告警系统");
            if (res != null)
            {
                
                string m = BasicSO.Encrypt(mobile);
                var user = await this.basicUserService.GetAsync(a => a.Mobile == m && a.IsDeleted == 0 && a.Status == 1 && a.Type == 2);
                if (user == null) {
                    return res.Id;
                }
                this.sendSMSService.SendWarning(mobile, data.Title, data.Descrition, $"今日{DateTime.Now.ToString("HH:mm:ss")}发送。");

                var userLogin = await this.userLoginSerice.GetAsync(a => a.UserId == user.Id);
                if (userLogin == null)
                {
                    return res.Id;
                }
                List<string> aliasList = new List<string>();
                List<string> huaweiPushIds = new List<string>();
                if (userLogin.Manufacturer == "HUAWEI") {
                    huaweiPushIds.Add(userLogin.PushId);
                } else {
                    aliasList.Add(userLogin.PushId);
                }
                
                PushMessageDto message = new PushMessageDto()
                {
                    Title = "告警系统通知",
                    Content = $"监测到【{body.Title}】，请及时处理",
                    AliasList = aliasList,
                    HuaweiPushIds = huaweiPushIds,
                    Extras = new { url = $"/warningDetails?id={res.Id}&differentiate=0", msgType = 1 },
                };
                this.pushService.PushMessage(message);
                return res.Id;
            }
            else
            {
                throw new ValidException("操作失败");
            }
        }

        /// <summary>
        /// 获取或更新pushId
        /// </summary>
        /// <param name="userLoginPostDto"></param>
        /// <returns></returns>
        [HttpPost("UpdateUserPushId")]
        public async Task<string> UpdateUserPushId(UserLoginPostDto userLoginPostDto) {
            var userId = LoginUser.UserId;
            var pushId = await basicUserService.updateUserPushId(userId,userLoginPostDto.PushId,userLoginPostDto.Manufacturer);
            return pushId;
        }

        /// <summary>
        /// 返乡统计
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("StatisticEpidemic")]
        public async Task<IPagedList<StatisticsEpidemicDto>> GetEpidemicStatisticsList([FromServices] IEpidemicService service, PagePostBody body)
        {
            body.AreaId = LoginUser.AreaId;
            return await service.GetEpidemicStatisticsList(body);
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
        /// 返乡台账明细查询
        /// 1 户籍人口返乡记录，2 14天内异常人员，3 累计异常，4 按年月查询返乡记录
        /// </summary>
        /// <param name="service"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("GetEpidemicPopulationInfoList")]
        public async Task<IPagedList<EpidemicPopulationDto>> GetEpidemicPopulationInfoList([FromServices] IEpidemicService service, EpidemicInfoListBody body)
        {
            if (body.AreaId == 0)
            {
                body.AreaId = LoginUser.AreaId;
            }
            var data = await service.GetEpidemicPopulationInfoList(body);
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
            if (areaId <= 0) {
                areaId = LoginUser.AreaId;
            }
            var data = await this.populationService.GetPopulationDetail(0, idCard, isConvert, 0, householdId, areaId);
            return data;
        }
    }
}
