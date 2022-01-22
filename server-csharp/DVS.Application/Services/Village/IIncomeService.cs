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

namespace DVS.Application.Services.Village
{
    public interface IIncomeService : IServiceBase<VillageIncome>
    {

        Task<IPagedList<VillageIncomeDto>> GetIncomeList(PagePostBody body,string ids="");
        Task<VillageIncome> GetIncomeDetail(int householdId, int year);

        Task<MessageResult<int>> SaveIncome(SaveIncomeBody body);

        Task<bool> DeleteIncome(int id, int householdId);
    }
}
