using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    public class FFPHouseholdExtraService : ServiceBase<FFPHouseholdExtraInfo>, IFFPHouseholdExtraService
    {
        private readonly IHouseholdCodeService householdCodeService;
        private readonly IServiceBase<FFPHouseholdExtraLog> householdExtraLogService;
        private readonly IBasicUserService basicUserService;
        public FFPHouseholdExtraService(
            DbContext dbContext,
            IMapper mapper,
            IBasicUserService basicUserService,
            IServiceBase<FFPHouseholdExtraLog> householdExtraLogService,
            IHouseholdCodeService householdCodeService) : base(dbContext, mapper)
        {
            this.householdCodeService = householdCodeService;
            this.basicUserService = basicUserService;
            this.householdExtraLogService = householdExtraLogService;
        }

        public async Task<List<FFPHouseholdExtraLogDto>> GetHouseholdExtraLogs(int householdId)
        {
            List<FFPHouseholdExtraLog> logs = await householdExtraLogService.GetQueryable().Where(log => log.HouseholdId == householdId).OrderByDescending(log => log.CreatedAt).ToListAsync();
            List<int> userIds = logs.Select(log => log.CreatedBy.Value).ToList();
            IList<BasicUser> users = await basicUserService.GetListAsync(user => userIds.Contains(user.Id));
            List<FFPHouseholdExtraLogDto> logDtos = mapper.Map<List<FFPHouseholdExtraLogDto>>(logs);
            foreach (FFPHouseholdExtraLogDto log in logDtos)
            {
                BasicUser actionUser = users.FirstOrDefault(user => user.Id == log.CreatedBy);
                if (actionUser != null)
                {
                    log.UserName = actionUser.NickName;
                }
            }
            return logDtos;
        }

        public async Task<List<FFPHouseholdExtraInfo>> Info(int householdId, List<string> typeCodes)
        {
            var query = this.GetQueryable().Where(extra => extra.HouseholdId == householdId);
            if (typeCodes != null && typeCodes.Count > 0)
            {
                query = query.Where(extra => typeCodes.Contains(extra.Key));
            }
            List<FFPHouseholdExtraInfo> extraInfos = await query.ToListAsync();
            return extraInfos;
        }

        public async Task<bool> Save(HouseholdeExtraSaveReq extraInfoReq, LoginUser loginUser)
        {
            bool result = true;
            if (extraInfoReq == null || extraInfoReq.ExtraInfos == null || extraInfoReq.ExtraInfos.Count == 0)
            {
                return false;
            }
            if (extraInfoReq.HouseholdId <= 0) {
                throw new ValidException("无效的户id");
            }
            var householdInfo = await householdCodeService.GetHouseholdCodeDetail(extraInfoReq.HouseholdId);
            if (householdInfo == null) {
                throw new ValidException("户不存在");
            }
            foreach (FFPHouseholdExtraInfoDto extraInfo in extraInfoReq.ExtraInfos)
            {
                if (extraInfo.Key.IsNullOrWhiteSpace()) {
                    throw new ValidException("无效的key");
                }
                if (extraInfo.Value.IsNullOrWhiteSpace())
                {
                    throw new ValidException("无效的值");
                }
            }
            foreach (FFPHouseholdExtraInfoDto extraInfo in extraInfoReq.ExtraInfos)
            {
                var query = this.GetQueryable().Where(extra => extra.HouseholdId == extraInfoReq.HouseholdId && extra.Key == extraInfo.Key);
                FFPHouseholdExtraInfo extra = await query.FirstOrDefaultAsync();
                if (extra != null)
                {
                    int count = await query.UpdateFromQueryAsync(extra => new FFPHouseholdExtraInfo()
                    {
                        Value = extraInfo.Value,
                        Remark = extraInfo.Remark,
                        UpdatedBy = loginUser.UserId
                    });
                    result = count > 0;
                    if (!result)
                    {
                        return false;
                    }
                }
                else
                {
                    var newExtraInfo = new FFPHouseholdExtraInfo()
                    {
                        HouseholdId = extraInfoReq.HouseholdId,
                        Key = extraInfo.Key,
                        Value = extraInfo.Value,
                        Remark = extraInfo.Remark??"",
                        CreatedBy = loginUser.UserId
                    };
                    var res = await this.InsertAsync(newExtraInfo);
                    result = res.Id > 0;
                    if (!result)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
