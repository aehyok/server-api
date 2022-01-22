using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    public interface IFFPPublicityManageService : IServiceBase<FFPPublicityManage>
    {
        /// <summary>
        /// 保存公示信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<int> SavePublicityManage(FFPPublicityManage data, PublicityManageAddDto model, ConsInfoPublic info );
    }
}
