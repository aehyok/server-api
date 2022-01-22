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
	public interface IGISCustomService : IServiceBase<GISCustom>
	{
        /// <summary>
        /// 删除自定义
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteCustomAsync(int id, int userid);

        /// <summary>
        /// 获取自定义列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<GISCustomDto>> ListCustomAsync(GISListQueryModel model);

        /// <summary>
        /// 获取自定义详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GISCustomDto> DetailCustomAsync(int id);


        /// <summary>
        /// 保存自定义
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveCustomAsync(GISCustom model);

        /// <summary>
        /// 导入EXCEL
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="category"></param>
        /// <param name="userId"></param>
        /// <param name="areaId">区域id</param>
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
