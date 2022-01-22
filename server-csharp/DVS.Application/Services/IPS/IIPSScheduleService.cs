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
    public interface IIPSScheduleService : IServiceBase<IpsSchedule>
    {
        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> DeleteSchedule(int id, int userid);

        /// <summary>
        /// 获取日程列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<IPagedList<IPSScheduleDto>> ListSchedule(string keyword, int page,int limit, int areaId = 0);

        /// <summary>
        /// 获取日程详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IPSScheduleDto> DetailSchedule(int id);


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        Task<int> SaveSchedule(IpsSchedule schedule);
        
        Task<IPSStatisticDto> StatisticSchedule(int areaId);
    }
}
