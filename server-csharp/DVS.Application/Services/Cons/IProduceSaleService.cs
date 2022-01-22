using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Cons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.Common;
using DVS.Models.Enum;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Models;

namespace DVS.Application.Services.Cons
{
    public interface IProduceSaleService : IServiceBase<ConsProduceSale>
    {
        Task<IPagedList<ListProduceSaleModel>> GetDataList(ProduceSaleListQueryModel model);

        Task<IPagedList<ListProduceSaleModel>> GetProduceSaleList(ProduceSaleListQueryModel model);

        Task<DetailProduceSaleModel> GetDetail(int id, PlatFormCode platformcode = PlatFormCode.UNKNOWN,LoginUser loginUser=null);


        Task<int> Save(CreateProduceSaleModel model, PlatFormCode platformcode = PlatFormCode.UNKNOWN);

        Task<IPagedList<ListProduceSaleModel>> MyDataList(ProduceSaleListQueryModel model, PlatFormCode platformcode = PlatFormCode.UNKNOWN);

        Task<IPagedList<ListProduceSaleModel>> GetDataByAreaIdList(ProduceSaleListQueryModel model);

        Task<int> MyPublishCount(int userId, PlatFormCode platformcode = PlatFormCode.UNKNOWN);

        Task<List<PublisherModel>> GetPublisher(PostBody model);

        Task<BasicCategoryModel> GetCategoryIds(int id);
    }
}
