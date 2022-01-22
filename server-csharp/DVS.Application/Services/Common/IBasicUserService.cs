using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface IBasicUserService : IServiceBase<BasicUser>
    {
        Task<UserAuthStatusDto> GetUserAuthStatus(int  userId);
        Task<VillagePopulationDto> GetPopulationByUserId(int id);

        /// <summary>
        /// 获取相关状态用户数
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        Task<int> GetUserAuthCount(int areaId, UserAuthAuditStatusEnum auditStatus = 0);
        Task<BasicUserDto> GetUserInfo(int id);

        Task<VillagePopulationDto> GetPopulationMasterByUserId(int userid);

        /// <summary>
        /// 修改用户的pushId
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pushId">推送Id</param>
        /// <param name="manufacturer">厂商</param>
        /// <returns>pushId</returns>
        Task<string> updateUserPushId(int userId, string pushId, string manufacturer);
    }
}
