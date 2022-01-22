using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public interface IFFPApplicationService
    {
        public Task<int> SaveApplication(FFPApplicationEditReq application,int loginUserId);

        public Task<FFPApplicationDto> Detail(int workflowId);
        Task<List<FFPApplicationLogDto>> GetApplicationLogs(int applicationId);
    }
}
