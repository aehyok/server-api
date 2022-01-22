using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Enum;
using DVS.Common.Models;

namespace DVS.Application.Services.Cons
{
    public interface IPhotoAnywhereService : IServiceBase<ConsPhotoAnywhere>
    {
        Task<IPagedList<ListPhotoAnywhereModel>> GetDataList(PhotoAnywhereListQueryModel model);


        Task<DetailPhotoAnywhereModel> GetDetail(int id);


        Task<int> Save(CreatePhotoAnywhereModel model);

        Task<List<PhotoAnywhereTypeDto>> StatisticPhotoAnywhereByType(int id);

        Task<List<PhotoAnywhereTypeMonthDto>> StatisticPhotoAnywhereByTypeMonth(int id);
        List<PhotoAnywhereTypeDto> StatisticPhotoAnywhereByStatus(int AreaId);

        List<PhotoAnywhereTypeDto> StatisticPhotoAnywhereStatusByUser(int userId);

        Task<List<BasicDictionaryDto>> GetPhotoAnywhereType();

        Task<int> MyPhotoAnywhereCount(int userId);

        Task<int> MyReplyPhotoAnywhereCount(int userId);

        Task<IPagedList<ListPhotoAnywhereModel>> MyReplyDataList(PhotoAnywhereListQueryModel model);

        Task<int> Reply(ReplyPhotoAnywhereModel model, LoginUser loginuser);

        /// <summary>
        /// 删除随手拍
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<int> DeletePhotoAnywhereAsync(int id, int userid);
    }
}
