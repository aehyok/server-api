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
    public interface IGISCameraService : IServiceBase<GISCamera>
    {
        /// <summary>
        /// 删除摄像头
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteCameraAsync(int id, int userid);

        /// <summary>
        /// 获取摄像头列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IPagedList<GISCameraDto>> ListCameraAsync(GISListQueryModel model);

        /// <summary>
        /// 获取摄像头详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GISCameraDto> DetailCameraAsync(int id);


        /// <summary>
        /// 保存摄像头
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveCameraAsync(GISCamera model);

        /// <summary>
        /// 导入excel
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
