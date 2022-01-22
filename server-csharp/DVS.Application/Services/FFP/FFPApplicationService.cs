using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.FFP;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using DVS.Models.Dtos.Village;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{

    public class FFPApplicationService : ServiceBase<FFPApplication>, IFFPApplicationService
    {
        private readonly IPopulationService populationService;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        private readonly IFFPHouseholdCodeService ffpHouseholdCodeService;
        private readonly IServiceBase<VillagePopulationAddress> populationAddressService;
        private readonly IServiceBase<FFPApplicationLog> applicationLogService;
        private readonly IBasicUserService basicUserService;
        public FFPApplicationService(DbContext dbContext, IMapper mapper,
            IHouseholdCodeService householdCodeService,
            IPopulationService populationService,
            IServiceBase<VillageHouseCodeMember> memberService,
            IFFPHouseholdCodeService ffpHouseholdCodeService,
            IServiceBase<VillagePopulationAddress> populationAddressService,
            IServiceBase<FFPApplicationLog> applicationLogService,
            IBasicUserService basicUserService
            ) : base(dbContext, mapper)
        {
            this.populationService = populationService;
            this.memberService = memberService;
            this.ffpHouseholdCodeService = ffpHouseholdCodeService;
            this.populationAddressService = populationAddressService;
            this.applicationLogService = applicationLogService;
            this.basicUserService = basicUserService;
        }

        public async Task<FFPApplicationDto> Detail(int workflowId)
        {
            FFPApplication application = await this.GetQueryable().Where(app => app.WorkflowId == workflowId).FirstOrDefaultAsync();
            if (application == null)
            {
                throw new ValidException("申请书未产生");
            }
            FFPApplicationDto applicationDto = mapper.Map<FFPApplication, FFPApplicationDto>(application);
            // 获取关系
            VillageHouseCodeMember membership = await memberService.GetQueryable().Where(member => member.HouseholdId == application.HouseholdId && member.IsHouseholder == 1).FirstOrDefaultAsync();
            if (membership != null)
            {
                // 获取人口信息
                VillagePopulation population = await populationService.GetQueryable().Where(population => population.Id == membership.PopulationId).FirstOrDefaultAsync();
                if (population != null)
                {
                    applicationDto.HouseholderName = Utils.Decrypt(population.RealName);
                }
            }
            // 获取户码信息
            FFPHouseholdCode ffpHouseholdCode = await ffpHouseholdCodeService.GetQueryable().Where(household => household.HouseholdId == application.HouseholdId).FirstOrDefaultAsync();
            if (ffpHouseholdCode != null)
            {
                applicationDto.IsWithoutPorverty = ffpHouseholdCode.IsWithoutPoverty;
            }
            //获取地址信息
            VillagePopulationAddress populationAddress = await populationAddressService.GetQueryable().Where(address => address.PopulationId == application.HouseholdId).FirstOrDefaultAsync();
            if (populationAddress != null)
            {
                applicationDto.LivingAddress = mapper.Map<PopulationAddressDto>(populationAddress);
            }


            return applicationDto;
        }

        private string getApplicationLog(FFPApplicationEditReq newApplication, FFPApplication oldApplication, FFPHouseholdCode oldHouseholdInfo, VillagePopulationAddress oldAddress)
        {
            StringBuilder message = new StringBuilder();
            Dictionary<string, object> modifyFields = new Dictionary<string, object>();
            List<string> addFields = new List<string>();
            if (newApplication.IsWithoutPorverty != oldHouseholdInfo.IsWithoutPoverty)
            {
                modifyFields["是否为已脱贫户"] = newApplication.IsWithoutPorverty == 1 ? "是" : "否";
            }
            //家庭成员
            if (newApplication.MemberCount != oldApplication.MemberCount)
            {
                if (oldApplication.MemberCount != -1)
                {
                    modifyFields["家庭成员"] = newApplication.MemberCount;
                }
                else
                {
                    addFields.Add("家庭成员");
                }
            }
            //劳动力
            if (newApplication.LabourCount != oldApplication.LabourCount)
            {
                if (oldApplication.LabourCount != -1)
                {
                    modifyFields["劳动力"] = newApplication.LabourCount;
                }
                else
                {
                    addFields.Add("劳动力");
                }
            }
            //在校生
            if (newApplication.StudentCount != oldApplication.StudentCount)
            {
                if (oldApplication.StudentCount != -1)
                {
                    modifyFields["在校生"] = newApplication.StudentCount;
                }
                else
                {
                    addFields.Add("在校生");
                }
            }


            return message.ToString();
        }

        public async Task<int> SaveApplication(FFPApplicationEditReq application, int loginUserId)
        {
            FFPApplication app = await this.GetQueryable().Where(app => app.WorkflowId == application.WorkflowId).FirstOrDefaultAsync();
            if (app == null)
            {
                FFPApplication applicationInfo = mapper.Map<FFPApplicationEditReq, FFPApplication>(application);
                applicationInfo.CreatedBy = loginUserId;
                FFPApplication insertedApplication = await this.InsertAsync(applicationInfo);
                await applicationLogService.InsertAsync(new FFPApplicationLog()
                {
                    CreatedBy = loginUserId,
                    ApplicationId = insertedApplication.Id,
                    Message = "创建了监测对象申请书"
                });
                return insertedApplication.Id;
            }
            else
            {
                var applicationQuery = this.GetQueryable().Where(app => app.WorkflowId == application.WorkflowId);
                await applicationQuery.UpdateFromQueryAsync(app => new FFPApplication()
                {
                    LabourCount = application.LabourCount,
                    StudentCount = application.StudentCount,
                    ChronicCount = application.ChronicCount,
                    MemberCount = application.MemberCount,
                    SeriousDiseaseCount = application.SeriousDiseaseCount,
                    DisabledPeopleCount = application.DisabledPeopleCount,
                    AllowanceCount = application.AllowanceCount,
                    YearIncome = application.YearIncome,
                    Difficulty = application.Difficulty,
                    DifficultyRemark = application.DifficultyRemark,
                    UpdatedBy = loginUserId
                });
                // 修改户地址
                var addressQuery = populationAddressService.GetQueryable().Where(address => address.PopulationId == application.HouseholdId);
                var livingAddress = await addressQuery.FirstOrDefaultAsync();
                if (application.LivingAddress != null)
                {
                    await addressQuery.UpdateFromQueryAsync(address => new VillagePopulationAddress()
                    {
                        Province = application.LivingAddress.Province,
                        City = application.LivingAddress.City,
                        District = application.LivingAddress.District,
                        Address = application.LivingAddress.Address,
                        MapCode = application.LivingAddress.MapCode,
                    });
                }
                //修改是否贫困户
                var householdQuery = ffpHouseholdCodeService.GetQueryable().Where(household => household.HouseholdId == application.HouseholdId);
                var householdInfo = await householdQuery.FirstOrDefaultAsync();
                await householdQuery.UpdateFromQueryAsync(household => new FFPHouseholdCode()
                {
                    IsWithoutPoverty = application.IsWithoutPorverty
                });
                // 修改日志
                string log = getApplicationLog(application, app, householdInfo, livingAddress);
                if (!log.IsNullOrWhiteSpace())
                {
                    await applicationLogService.InsertAsync(new FFPApplicationLog()
                    {
                        UpdatedBy = loginUserId,
                        ApplicationId = app.Id,
                        Message = "修改了监测对象申请书"
                    });
                }
                return app.Id;
            }
        }

        public async Task<List<FFPApplicationLogDto>> GetApplicationLogs(int applicationId)
        {
            List<FFPApplicationLog> logs = await applicationLogService.GetQueryable().Where(log => log.ApplicationId == applicationId).OrderByDescending(log => log.CreatedAt).ToListAsync();
            List<int> userIds = logs.Select(log => log.CreatedBy.Value).ToList();
            IList<BasicUser> users = await basicUserService.GetListAsync(user => userIds.Contains(user.Id));
            List<FFPApplicationLogDto> logDtos = mapper.Map<List<FFPApplicationLogDto>>(logs);
            foreach (FFPApplicationLogDto log in logDtos)
            {
                BasicUser actionUser = users.FirstOrDefault(user => user.Id == log.CreatedBy);
                if (actionUser != null)
                {
                    log.UserName = actionUser.NickName;
                }
            }
            return logDtos;
        }
    }
}
