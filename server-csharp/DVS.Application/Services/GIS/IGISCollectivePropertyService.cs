using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
	public interface IGISCollectivePropertyService : IServiceBase<GISCollectiveProperty>
	{
        /// <summary>
        /// 删除公共设施
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> DeleteCollectivePropertyAsync(int id, int userid);

        /// <summary>
        /// 获取公共设施列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<GISCollectivePropertyDto>> ListCollectivePropertyAsync(GISListQueryModel model);

        /// <summary>
        /// 获取公共设施详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GISCollectivePropertyDto> DetailCollectivePropertyAsync(int id);


        /// <summary>
        /// 保存公共设施
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveCollectivePropertyAsync(GISCollectiveProperty model);

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> BatchUpdateCollectivePropertyAsync(GISBatchDto model);
        
        /// <summary>
        /// 导入EXCEL
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="category"></param>
        /// <param name="userId"></param>
        /// <param name="areaId"></param>
        /// <param name="objectId">区域id或园区id</param>
        /// <returns></returns>
        Task<GISImportRes> ImportExcelAsync(Stream fileStream, int category, int userId, int areaId, int objectId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<byte[]> GetExcelData(int areaId, int typeId = 0, string keyword = "", string ids = "", int category = 0, int objectId = 0);
    }
}
