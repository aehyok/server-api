using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public interface IFFPHouseholdCodeService : IServiceBase<FFPHouseholdCode>
    {
        Task<FFPHouseholdCodeDetailDto> GetFFPHouseholdCodeDetail(int householdId, int isConvertDictionary = 0);
        Task<MessageResult<int>> SaveFFPHouseholdCodeInfo(FFPHouseholdCodeInfoPostDto body, int createdBy);
        Task<bool> UpdateFFPHouseholdCodeInfo(int householdId, string householdType, PopulationAddressDto familyAddressInfo, int updatedBy);

    }
}
