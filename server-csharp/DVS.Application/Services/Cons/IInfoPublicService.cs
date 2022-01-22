using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Cons;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Cons
{
    public interface IInfoPublicService : IServiceBase<ConsInfoPublic>
    {
        /// <summary>
        /// 删除公开信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteConsInfoPublic(int id, int userid);

        /// <summary>
        /// 获取公开信息列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<IPagedList<ConsInfoPublicDto>> ListConsInfoPublic(int type, string keyword, string startdate, string enddate, int userid, int page,int limit, int areaId = 0, PlatFormCode platformcode = PlatFormCode.UNKNOWN);

        /// <summary>
        /// 获取公开信息列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<IPagedList<ConsInfoPublicDto>> ListInfoPublic(List<int> types, string keyword, string startdate, string enddate, int userid, int page, int limit, int areaId = 0, PlatFormCode platformcode = PlatFormCode.UNKNOWN);

        /// <summary>
        /// 获取公开信息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ConsInfoPublicDto> DetailConsInfoPublic(int id, PlatFormCode platformcode = PlatFormCode.UNKNOWN, LoginUser loginUser = null);


        /// <summary>
        /// 保存公开信息
        /// </summary>
        /// <param name="consinfopublic"></param>
        /// <returns></returns>
        Task<int> SaveConsInfoPublic(ConsInfoPublic consinfopublic);
    }
}
