using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface ISunFileInfoService : IServiceBase<SunFileInfo>
    {
        /// <summary>
        /// 根据ids获取文件信息列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<SunFileInfoDto>> GetSunFileInfoList(string ids);
        /// <summary>
        /// 根据ids获取相对路径字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<string> GetSunFileRelativeUrls(string ids);

        /// <summary>
        /// 相对路径转化为绝对路径
        /// </summary>
        /// <param name="imageUrls"></param>
        /// <returns></returns>
        string ToAbsolutePath(string imageUrls);

        /// <summary>
        /// 获取文件前段路径
        /// </summary>
        /// <returns></returns>
        string GetFileAccessUrl();

        Task<SunFileInfoDto>  GetSunFileInfo(int id);

    }
}
