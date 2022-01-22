using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    public interface IFFPHouseholdExtraService
    {
        /// <summary>
        /// 户的全部的扩展信息
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        public Task<List<FFPHouseholdExtraInfo>> Info(int householdId, List<string> typeCodes);
        public Task<bool> Save(HouseholdeExtraSaveReq extraInfos, LoginUser loginUser);
        Task<List<FFPHouseholdExtraLogDto>> GetHouseholdExtraLogs(int householdId);
    }
}
