using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village.Farmland;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
	public interface IGISGreenHouseService : IServiceBase<GISGreenHouse>
	{
        /// <summary>
        /// 删除大棚
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteGreenHouseAsync(int id, int userid);

        /// <summary>
        /// 获取大棚列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<GISGreenHouseDto>> ListGreenHouseAsync(GISListQueryModel model);

        /// <summary>
        /// 获取大棚详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GISGreenHouseDto> DetailGreenHouseAsync(int id);


        /// <summary>
        /// 保存大棚
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveGreenHouseAsync(GISGreenHouse model);

        /// <summary>
        /// 批量保存大棚
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> BatchUpdateGreenHouseAsync(GISBatchDto model);

        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="category"></param>
        /// <param name="userId"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int userId, int areaId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<byte[]> GetExcelData(int areaId, int typeId = 0, string keyword = "", string ids = "", int category = 0);
    }
}
