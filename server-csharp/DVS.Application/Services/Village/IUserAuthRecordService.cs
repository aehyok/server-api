using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using DVS.Model.Dtos.Village;

namespace DVS.Application.Services.Village
{
    public interface IUserAuthRecordService : IServiceBase<VillageUserAuthRecord>
    {

        Task<IPagedList<VillageUserAuthRecord>> GetUserAuthList(PageUserAuthBody body);

        Task<UserAuthRecordDto> GetUserAuthDetail(int id);

        Task<MessageResult<bool>> AuditUserAuth(VillageUserAuthRecord body,int householdId);


        Task<MessageResult<UserAuthStatusDto>> ApplyUserAuth(VillageUserAuthRecord body,int action, string mobileCode);

    }






}
