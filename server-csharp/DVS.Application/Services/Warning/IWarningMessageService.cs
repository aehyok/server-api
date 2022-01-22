using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Core.Domains.Warning;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Dtos.Warning;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Warning
{
   public interface IWarningMessageService : IServiceBase<WarningMessage>
    {
       Task<WarningMessage> AddWarningMessage(WarningMessage data, string userName);

        Task<IPagedList<WarningMessage>> GetWarningMessagePageList(WarnigMessageQueryBody body);

        Task<MessageResult<bool>> FinishWarningMessage(int id, int userId, string userName);

        Task<List<WarningOperationLog>> GetWarningOperationLogList(int warningMessageId);
    }
}
