using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public interface IGISHouseHoldService : IServiceBase<VillageHouseholdCode>
    {
        Task<IPagedList<HouseholdCodeDto>> ListHouseholdCodeAsync(GISListQueryModel model);

        Task<HouseholderAndHouseNumberDto> DetailHouseholdCodeAsync(int id);
    }
}
