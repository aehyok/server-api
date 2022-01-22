using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface IBasicAreaService : IServiceBase<BasicArea>
    {
        Task<string> GetAreaName(int areaId);
        List<BasicArea> FindParentAreas(int areaId);
        string FindParentAreaString(int areaId, bool includeMe = true);

        /// <summary>
        /// 获取区域树结构
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<BasicAreaTreeDto> GetBasicAreaTree(int areaId = 0);

        string GetParentAreaString(int areaId);

        Task<List<BasicArea>> FindChildrenAreas(int areaId);
        Task<List<int>> FindChildrenAreaIds(int areaId, bool includeMe = true, bool cache = true);
    }
}
