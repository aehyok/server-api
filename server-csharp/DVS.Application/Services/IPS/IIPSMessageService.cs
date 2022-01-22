using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.IPS;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.IPS
{
    public interface IIPSMessageService : IServiceBase<IpsMessage>
    {
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteMessage(int id, int userid);

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<IPagedList<IPSMessageDto>> ListMessage(string keyword, int page,int limit, int areaId = 0);

        /// <summary>
        /// 获取消息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IPSMessageDto> DetailMessage(int id);


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<int> SaveMessage(IpsMessage message);

        Task<IPSStatisticDto> StatisticMessage(int areaId);
    }
}
