using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village.Farmland;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public interface IGISFarmLandService : IServiceBase<VillageFarmland>
    {
        Task<VillageFarmlandDto> DetailFarmLandAsync(int id);

        Task<int> DeleteFarmLandAsync(int id, int userid);

        Task<IPagedList<VillageFarmlandDto>> ListFarmLandAsync(GISListQueryModel model);

        Task<int> SaveFarmLandAsync(VillageFarmland model, int userId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> BatchUpdateFarmLandAsync(GISBatchDto model);

        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="category"></param>
        /// <param name="userid"></param>
        /// <param name="usefor"></param>
        /// <param name="objectid">园区id</param>
        /// <returns></returns>
        Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int usefor, int userid, int areaid, int objectid = 0);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="category"></param>
        /// <param name="userfor"></param>
        /// <returns></returns>
        Task<byte[]> GetLandExcelData(int areaId, int typeId = 0, string keyword = "", string ids = "", int category = 0, int usefor = 0, int objectId = 0);
    }
}
