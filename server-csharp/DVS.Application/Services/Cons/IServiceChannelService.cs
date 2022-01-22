using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Cons
{
   public interface IServiceChannelService : IServiceBase<ConsServiceChannel>
    {
        Task<IPagedList<ListServiceChannelModel>> GetDataList(PagedListQueryModel model);


        Task<DetailServiceChannelModel> GetDetail(int id , PlatFormCode platformcode = PlatFormCode.UNKNOWN, LoginUser loginUser = null);


        Task<int> Save(CreateServiceChannelModel model);

        Task<int> Remove(int id);
    }
}
