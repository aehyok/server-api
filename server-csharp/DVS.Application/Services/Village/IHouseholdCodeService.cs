using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Household;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public interface IHouseholdCodeService : IServiceBase<VillageHouseholdCode>
    {
        Task<IPagedList<HouseholdCodeDto>> GetHouseholdCodeList(PostBody body, bool isGetTag = false);
        public Task<string> GetSingleQrCodeImage(int id);
        Task<Stream> SingleQrCode(int id);

        Task<int> GetHouseholdCodeGenStatus(int taskId);

        Task<VillageHouseholdCodeGrenTask> GetHouseholdGenTaskAsync();

        Task<Stream> SingleHouseholdCode(int id,int templateId);

        Task<string> GetHouseholdCodeImage(int id,int templageId);

        Task<VillageHouseholdCode> GetHouseholdCodeDetail(int id);

        Task<int> CreateHouseholdCodeTask(int areaCode, List<int> ids,int templateId,int taskType);

        Task<MessageResult<bool>> SaveHouseholdCode(VillageHouseholdCode villageHouseholdCode,LoginUser loginUser);

        /// <summary>
        /// 获取组别
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<List<HouseNameDto>> GetHouseNameList(int areaId);

        Task<MessageResult<bool>> SetHouseholdImage(SetHouseholdImageBody body);

        /// <summary>
        /// 获取乡村治理荣誉统计
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<IEnumerable<StatisticsCommonDto>> GetVillageHonor(int areaId);

        Task<int> HouseholeCodeGenCompleted(HouseholdCodeCompletedReq req);
        Task<Stream> DownloadHouseholdCodeZiFile(int taskId);
        Task<IEnumerable<HouseNumberDto>> GetHouseNumberList(int areaId, int houseNameId);
        Task<HouseholderAndHouseNumberDto> GethouseholderAndHouseNumber(int householdId);
        /// <summary>
        /// 计算户数和人口数
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<HouseholdStatisticsDto> HouseholdStatistics(int areaId);
        Task<bool> ChangeStatus(int id, int status);

        Task<MessageResult<ImportResultDto>> ImportHouseHoldCode(Stream fileStream, int areaId,LoginUser loginUser);
        Task<string> GetTemplateImage(VillageHouseholdCodeTemplate template);
        Task<List<Models.Dtos.Village.Export.ExportHouseHoldCodeDto>> ExportHouseHoldCode(string ids);


        /// <summary>
        /// 获取门牌名列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<IPagedList<VillageHouseName>> GetHouseNamePageLsit(PageHouseNameBody body);

        /// <summary>
        /// 添加编辑门牌名
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>

        Task<MessageResult<int>> SaveHouseName(VillageHouseName body);



        /// <summary>
        /// 获取门牌名详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VillageHouseNameDto> GetHouseNameDetail(int id);

        /// <summary>
        /// 删除门牌名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedBy">操作人Id</param>
        /// <returns></returns>

        Task<bool> DeleteHouseName(int id, int updatedBy = 0);

        Task<bool> RemoveTemplate(int id);

        Task<VillageHouseholdCodeTemplateDto> TemplateDetail(int id);
        Task<int> SaveTemplate(VillageHouseholdCodeTemplateDto template);
        Task<List<VillageHouseholdCodeTemplateDto>> TemplateList(int areaId);

        /// <summary>
        /// 获取门牌名列表，公众号村民认证专用
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="idCard"></param>
        /// <returns></returns>
        Task<List<HouseNumberDto>> GetHouseNumberListByIdCard(int areaId, string idCard);



        /// <summary>
        /// 获取户主信息
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        Task<HouseholderDto> GetHouseholderInfo(int householdId);



    }
}