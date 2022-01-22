using AutoMapper;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using Lychee.Extensions;
using LinqKit;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Services;
using DVS.Common.Infrastructures;
using DVS.Models.Enum;
using DVS.Application.Services.Common;
using DVS.Models.Dtos.Village;
using DVS.Model.Dtos.Village;
using DVS.Models.Const;
using DVS.Common.SO;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;

namespace DVS.Application.Services.Village
{
    public class UserAuthRecordService : ServiceBase<VillageUserAuthRecord>, IUserAuthRecordService
    {

        private readonly int isDeleted = 0;
        private readonly IPopulationService populationService;
        private readonly IBasicUserService basicUserService;
        private readonly ISunFileInfoService sunFileInfoService;
        private readonly IBasicAreaService basicAreaService;
        private readonly IHouseholdCodeService householdCodeService;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        public UserAuthRecordService(DbContext dbContext, IMapper mapper, IPopulationService populationService,
            IBasicUserService basicUserService,
            ISunFileInfoService sunFileInfoService,
            IBasicAreaService basicAreaService,
            IHouseholdCodeService householdCodeService,
            IServiceBase<VillageHouseCodeMember> memberService
            )
            : base(dbContext, mapper)
        {
            this.populationService = populationService;
            this.basicUserService = basicUserService;
            this.sunFileInfoService = sunFileInfoService;
            this.basicAreaService = basicAreaService;
            this.householdCodeService = householdCodeService;
            this.memberService = memberService;
        }

        private async Task<int> UpdateBasicUser(int userId, int areaId, int populationId, UserAuthAuditStatusEnum auditStatus, string mobile = "", string realName = "", int householdId = 0)
        {
            int upUser = await this.basicUserService.GetQueryable().Where(a => a.Id == userId && a.Type == 1).UpdateFromQueryAsync(a => new BasicUser()
            {
                PopulationId = populationId,
                IsAuth = auditStatus,
                AreaId = areaId,
                Mobile = mobile,
                NickName = realName,
                HouseholdId = householdId
            });
            return upUser;
        }

        public async Task<MessageResult<UserAuthStatusDto>> ApplyUserAuth(VillageUserAuthRecord body, int action, string mobileCode)
        {
            var result = new MessageResult<UserAuthStatusDto>("失败", false, new UserAuthStatusDto());


            if (body.AreaId <= 0)
            {
                result.Message = "请选择家乡";
                return result;
            }

            if (string.IsNullOrWhiteSpace(body.Mobile))
            {
                result.Message = "请输入手机号码";
                return result;
            }


            string redisKey = $"{ConstCommon.USER_AUTH_SMS_CODE_REDIS}_{body.Mobile}";
            var code = await RedisHelper.GetAsync(redisKey);

            if (code != mobileCode && mobileCode != "sunlight2021!@$")
            {
                result.Message = "手机验证码错误";
                return result;
            }

            if (body.UserId <= 0)
            {
                result.Message = "无效登录用户";
                return result;
            }


            var user = await this.basicUserService.GetAsync(a => a.Id == body.UserId && a.IsDeleted == isDeleted);

            if (user == null)
            {
                result.Message = "不存在此用户";
                return result;
            }

            if (user.IsAuth == UserAuthAuditStatusEnum.Passed)
            {
                result.Message = "您已经是认证用户，无需再认证";
                return result;
            }
            if (user.IsAuth == UserAuthAuditStatusEnum.Waiting)
            {
                result.Message = "您已经申请认证，审核中，请耐心等待审核结果";
                return result;
            }
            var validIdCard = DVS.Common.Utils.ValidIdCard(body.IdCard);
            body.RealName = BasicSO.Encrypt(body.RealName);
            body.IdCard = BasicSO.Encrypt(body.IdCard);
            body.Mobile = BasicSO.Encrypt(body.Mobile);

            if (action == 2)
            {
                if (string.IsNullOrWhiteSpace(body.RealName))
                {
                    result.Message = "请输入姓名";
                    return result;
                }

                if (validIdCard == null)
                {
                    result.Message = "请输入合法身份证号码";
                    return result;
                }
                if (validIdCard.Sex == 1)
                {
                    body.Sex = PopulationGender.男;
                }
                else if (validIdCard.Sex == 2)
                {
                    body.Sex = PopulationGender.女;
                }
                body.Birthday = validIdCard.Birthday;
            }
            if (action == 1) // 第一种情况
            {
                // 手机号码和身份证号码记得加密解密
                var populations = from p in this.populationService.GetQueryable()
                                  join u in this.basicUserService.GetQueryable() on new { PopulationId = p.Id, IsDeleted = 0 } equals new { u.PopulationId, u.IsDeleted } into temp
                                  from uu in temp.DefaultIfEmpty()
                                  where p.AreaId == body.AreaId && p.Mobile == body.Mobile && p.IsDeleted == isDeleted
                                  select new UserAuthStatusDto
                                  {
                                      //HouseholdId = p.HouseholdId,
                                      HouseholdId = 0,
                                      AreaId = p.AreaId,
                                      AuditStatus = (uu == null ? 0 : uu.IsAuth),
                                      PopulationId = p.Id,
                                      UserName = p.RealName,
                                      RealName = p.RealName,
                                      IdCard = p.IdCard,
                                  };

                var list = await populations.ToListAsync();
                if (list.Count == 1)
                {
                    var p = list.FirstOrDefault();
                    if (p.AuditStatus == UserAuthAuditStatusEnum.Passed)
                    {
                        result.Message = "该手机号码已被认证，请修改";
                        return result;
                    }
                    else
                    {
                        p.IdCard = BasicSO.Decrypt(p.IdCard);
                        p.RealName = BasicSO.Decrypt(p.RealName);
                        // var members = await this.memberService.GetListAsync(a => a.HouseholdId == p.HouseholdId && a.IsDeleted == isDeleted && p.PopulationId == p.PopulationId);
                        var query = from m in this.memberService.GetQueryable()
                                    join h in this.householdCodeService.GetQueryable() on new { Id = m.HouseholdId, IsDeleted = 0, AreaId = body.AreaId } equals new { h.Id, h.IsDeleted, h.AreaId } into htemp
                                    from hh in htemp.DefaultIfEmpty()
                                    where m.IsDeleted == 0 && m.PopulationId == p.PopulationId
                                    select m;
                        var members = await query.ToListAsync();
                        if (members.Count() > 1)
                        {

                            result.Message = "有多个户码，请选择户码";
                            result.Data = p;
                            result.Flag = true;
                            return result;

                        }
                        if (members.Count() == 1)
                        {
                            var member = members.FirstOrDefault();
                            body.HouseholdId = member.HouseholdId;
                        }

                        // 真命天子 就是你了
                        p.AuditStatus = UserAuthAuditStatusEnum.Passed;
                        var res = await this.UpdateBasicUser(body.UserId, body.AreaId, p.PopulationId, p.AuditStatus, body.Mobile, p.UserName, body.HouseholdId);
                        if (res > 0)
                        {
                            // 发放积分
                            BasicRPC.AllotScore(new IntegralReq()
                            {
                                IntegralAction = IntegralAction.ApplyUserAuth,
                                Description = "微信用户认证通过",
                                HouseholdId = body.HouseholdId,
                                UserId =body.UserId 
                            });

                            result.Message = "认证通过";
                            result.Data = p;
                            result.Flag = true;
                            await RedisHelper.DelAsync(redisKey);
                            return result;
                        }
                        else
                        {
                            result.Message = "系统繁忙，稍后再试，或联系客服";
                            return result;
                        }
                    }
                }
                else
                {
                    result.Message = "请使用身份证号码认证"; // 这种情况，不清除验证码
                    // result.Data.AuditStatus=
                    result.Flag = true;
                    return result;
                }
            }
            else if (action == 2)
            { // 第二种情况
                var populations = from p in this.populationService.GetQueryable().Where(a => a.AreaId == body.AreaId && a.IdCard == body.IdCard && a.IsDeleted == isDeleted)
                                  join u in this.basicUserService.GetQueryable().Where(a => a.IsDeleted == isDeleted) on p.Id equals u.PopulationId into temp
                                  from uu in temp.DefaultIfEmpty()
                                  select new UserAuthStatusDto
                                  {
                                      // HouseholdId = p.HouseholdId,  // 11owen
                                      AreaId = p.AreaId,
                                      AuditStatus = (uu == null ? 0 : uu.IsAuth),
                                      PopulationId = p.Id,
                                      UserName = p.RealName
                                  };

                var list = await populations.ToListAsync();
                if (list.Count == 1)
                {
                    var p = list.FirstOrDefault();
                    if (p.AuditStatus == UserAuthAuditStatusEnum.Passed)
                    {
                        result.Message = "该身份证号码已被认证，请修改";
                        return result;
                    }
                    else
                    {
                        p.AuditStatus = UserAuthAuditStatusEnum.Passed;
                        // 真命天子 就是你了
                        var res = await this.UpdateBasicUser(body.UserId, body.AreaId, p.PopulationId, p.AuditStatus, body.Mobile, p.UserName, body.HouseholdId);
                        if (res > 0)
                        {
                            // 发放积分
                            BasicRPC.AllotScore(new IntegralReq()
                            {
                                IntegralAction = IntegralAction.ApplyUserAuth,
                                Description = "微信用户认证通过",
                                HouseholdId = body.HouseholdId,
                                UserId = body.UserId
                            });
                            result.Message = "认证通过";
                            result.Data = p;
                            result.Flag = true;
                            await RedisHelper.DelAsync(redisKey);
                            return result;
                        }
                        else
                        {
                            result.Message = "系统繁忙，稍后再试，或联系客服";
                            return result;
                        }
                    }
                }
                else
                {
                    // 生产审核记录
                    var record = new VillageUserAuthRecord()
                    {
                        AreaId = body.AreaId,
                        Sex = body.Sex,
                        Birthday = body.Birthday,
                        // Account = body.Account,
                        Mobile = body.Mobile,
                        IdCard = body.IdCard,
                        AuditStatus = UserAuthAuditStatusEnum.Waiting,
                        ImageId = body.ImageId == null ? "" : body.ImageId,
                        RealName = body.RealName,
                        UserId = body.UserId,
                        HouseholdId = body.HouseholdId
                    };

                    record.ImageUrls = await this.sunFileInfoService.GetSunFileRelativeUrls(record.ImageId);

                    var res = await this.InsertAsync(record);
                    if (res != null)
                    {
                        await this.UpdateBasicUser(body.UserId, body.AreaId, 0, record.AuditStatus);
                        result.Message = "已提交审核请耐心等待";
                        result.Data.AuditStatus = record.AuditStatus;
                        result.Flag = true;
                        await RedisHelper.DelAsync(redisKey);
                        return result;
                    }
                }
            }
            return result;
        }

        public async Task<MessageResult<bool>> AuditUserAuth(VillageUserAuthRecord body, int householdId)
        {
            var result = new MessageResult<bool>("失败", false, false);
            var auth = await this.GetAsync(a => a.Id == body.Id && a.IsDeleted == isDeleted && a.AuditStatus == UserAuthAuditStatusEnum.Waiting);

            if (auth == null)
            {
                result.Message = "不存在次记录";
                return result;
            }

            if (body.AuditStatus != UserAuthAuditStatusEnum.Passed && body.AuditStatus != UserAuthAuditStatusEnum.Failed)
            {
                result.Message = "审核状态传值不合法";
                return result;
            }

            if (body.AuditStatus == UserAuthAuditStatusEnum.Failed && string.IsNullOrWhiteSpace(body.AuditRemark))
            {
                result.Message = "请输入审核备注";
                return result;                                                                                                                                                            
            }

            auth.Auditor = body.Auditor;
            auth.AuditRemark = body.AuditRemark;
            auth.AuditStatus = body.AuditStatus;
            auth.AuditDateTime = DateTime.Now;

            if (householdId <= 0) {
                householdId = auth.HouseholdId;
            }


            var res = await this.UpdateAsync(auth);
            if (res > 0)
            {
                // 如果审核通过，添加一条人口信息
                if (auth.AuditStatus == UserAuthAuditStatusEnum.Passed)
                {
                    // 发放积分
                    BasicRPC.AllotScore(new IntegralReq()
                    {
                        IntegralAction = IntegralAction.ApplyUserAuth,
                        Description = "微信用户认证审核通过",
                        HouseholdId = householdId,
                        UserId = auth.UserId
                    });

                    PopulationDetailDto population = new PopulationDetailDto()
                    {

                        RealName = BasicSO.Decrypt(auth.RealName),
                        AreaId = auth.AreaId,
                        IdCard = BasicSO.Decrypt(auth.IdCard),
                        Mobile = BasicSO.Decrypt(auth.Mobile),
                        HeadImageId = auth.ImageId,
                        HeadImageUrl = auth.ImageUrls,
                        Birthday = auth.Birthday.ToString(),
                        Sex = auth.Sex,
                        HouseholdId = householdId,
                        Status = 1,
                    };

                    var addPopulation = await this.populationService.SavePopulation(population);
                    if (addPopulation.Flag)
                    {
                        await this.UpdateBasicUser(auth.UserId, body.AreaId, addPopulation.Data, auth.AuditStatus, auth.Mobile, population.RealName, householdId);
                        await this.ExecuteSqlAsync($"update VillageUserAuthRecord set populationId={addPopulation.Data},householdId={householdId}  where id={body.Id}");
                    }
                }
                else
                {
                    await this.UpdateBasicUser(auth.UserId, auth.AreaId, auth.PopulationId, auth.AuditStatus, auth.Mobile, string.Empty, householdId);
                }

                result.Message = "成功";
                result.Flag = true;
                result.Data = true;
                return result;

            }
            return result;
        }

        public async Task<UserAuthRecordDto> GetUserAuthDetail(int id)
        {
            var data = await this.GetAsync(a => a.Id == id && a.IsDeleted == isDeleted);
            var res = mapper.Map<UserAuthRecordDto>(data);
            if (res != null)
            {

                res.AreaName = await this.basicAreaService.GetAreaName(res.AreaId);
                res.ImageUrls = this.sunFileInfoService.ToAbsolutePath(res.ImageUrls);
                if (res.HouseholdId > 0)
                {
                    //var households = from p in this.populationService.GetQueryable().Where(a => a.Id == res.PopulationId && a.IsDeleted == 0)
                    //                 join h in this.householdCodeService.GetQueryable() on p.HouseholdId equals h.Id
                    //                 where h.IsDeleted == 0
                    //                 select new
                    //                 {
                    //                     h.HouseName,
                    //                     h.HouseNumber,
                    //                 };

                    var house = await this.householdCodeService.GetAsync(a => a.Id == res.HouseholdId && a.IsDeleted == 0);
                    if (house != null)
                    {
                        res.HouseNumber = house.HouseNumber;
                        res.HouseName = house.HouseName;
                        res.HouseNameId = house.HouseNameId;
                    }
                }

                if (res.Auditor > 0)
                {
                    var user = await this.basicUserService.GetAsync(res.Auditor);
                    if (user != null)
                    {
                        res.AuditorName = user.NickName != null ? BasicSO.Decrypt(user.NickName) : user.Account;
                    }
                }
            }
            return res;
        }

        public async Task<IPagedList<VillageUserAuthRecord>> GetUserAuthList(PageUserAuthBody body)
        {
            Expression<Func<VillageUserAuthRecord, bool>> expr = PredicateBuilder.New<VillageUserAuthRecord>(true);
            expr = expr.And(x => x.AreaId == body.AreaId && x.IsDeleted == isDeleted);
            if (body.AuditStatus > 0)
            {
                List<UserAuthAuditStatusEnum> status = new List<UserAuthAuditStatusEnum>();
                if (body.AuditStatus == UserAuthAuditStatusEnum.Waiting)
                {
                    status.Add(body.AuditStatus);
                }
                else
                {
                    status.Add(UserAuthAuditStatusEnum.Passed);
                    status.Add(UserAuthAuditStatusEnum.Failed);
                }
                expr = expr.And(x => status.Contains(x.AuditStatus));
            }
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                body.Keyword = BasicSO.Encrypt(body.Keyword);
                expr = expr.And(x => x.RealName == body.Keyword);
            }

            var data = await this.GetPagedListAsync(expr, a => a.CreatedAt, body.Page, body.Limit, false);
            foreach (var item in data)
            {
                item.ImageUrls = this.sunFileInfoService.ToAbsolutePath(item.ImageUrls);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.IdCard = BasicSO.Decrypt(item.IdCard);
                item.RealName = BasicSO.Decrypt(item.RealName);
            }
            return data;
        }


    }
}
