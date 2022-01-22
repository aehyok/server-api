using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village.Farmland;
using DVS.Models.Dtos.Village.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public interface IVillageFarmlandService : IServiceBase<VillageFarmland>
    {
        Task<bool> Remove(List<int> ids);
        Task<IPagedList<VillageFarmlandDto>> GetFarmlands(int areaId, int typeId, string keyword, int householdId = 0, int category = 1, int usefor = 1, int page = 1, int limit = 10, List<OrderBy> orders = null, string ids = "");
        Task<IPagedList<FarmlandAreaSummaryDto>> GetAreaFarmlands(int areaId, string keyword, int page = 1, int limit = 10, List<OrderBy> orders=null);
        Task<VillageFarmlandDto> GetDetail(int id);
        Task<int> Save(VillageFarmland villageFarmland,int userId);

        Task<byte[]> GetExcelData(List<int> ids,int areaId,string keyword="");

        Task<List<StatisticsFarmlandDto>> GetStatisticsFarmland(int householdId);

        Task<int> SaveImport(VillageFarmland villageFarmland, int userId);
    }
}
