using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
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
    public interface IPopulationService : IServiceBase<VillagePopulation>
    {
        /// <summary>
        /// 户籍人口列表
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<IPagedList<PopulationDto>> GetPopulationList(PopulationListBody body);

        /// <summary>
        /// 户籍人口列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<WechatMessagePopulationDto>> GetPopulationList(WechatMessagePopulationQuery query);


        /// <summary>
        /// 户籍人口详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PopulationDetailDto> GetPopulationDetail(int id, string idCard = "", int isConvert = 0, int isDeleted = 0, int householdId = 0, int areaId = 0);

        /// <summary>
        /// 添加编辑户籍人口
        /// </summary>
        /// <param name="villagePopulation"></param>
        /// <returns></returns>
        Task<MessageResult<int>> SavePopulation(PopulationDetailDto villagePopulation, int updatedBy = 0);

        /// <summary>
        /// 设置与户主管理
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<MessageResult<bool>> SetHouseholdRelationship(SetRelationshipBody body);

        /// <summary>
        /// 移除，加入户码
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<MessageResult<bool>> SetFromHousehold(SetFromHouseholdBody body);

        /// <summary>
        /// 乡村治理居民参与情况 男女比例
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<IEnumerable<StatisticsCommonDto>> GetVillagePartakeSex(int areaId);

        /// <summary>
        /// 乡村治理居民参与情况 年龄段
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<IEnumerable<StatisticsCommonDto>> GetVillagePartakeAge(int areaId);


        /// <summary>
        /// 给所有户籍人口数据加密
        /// </summary>
        /// <returns></returns>
        Task<int> EncryptAllPopulation();

        Task<MessageResult<ImportResultDto>> ImportPopulation(Stream fileStream, int areaId);

        Task<List<DVS.Models.Dtos.Village.Export.ExportPopulationDto>> ExportPopulation(string ids);

        Task<VillagePopulation> GetHouseholderInfo(int householdId);

        Task<string> ImportPopulationAndHouseName(Stream fileStream, int areaId);

        /// <summary>
        /// 获取户主信息
        /// </summary>
        /// <param name="householdId"></param>
        /// <param name="isConvert"></param>
        /// <returns></returns>
        Task<PopulationDetailDto> GetHouseholdManDetail(int householdId, int isConvert = 0);

        /// <summary>
        /// 获取简约版家庭成员信息
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        Task<List<KeyValueDto>> GetMembersSimple(int householdId);

        Task<List<VillageHouseholdCode>> GetHouseholdList(int populationId);

        Task<int> DecryptAllPopulation();
        /// <summary>
        /// 切换户码
        /// </summary>
        /// <param name="householdId"></param>
        /// <param name="loginUserId"></param>
        /// <returns></returns>
        Task<bool> SwitchHouseholdCodeAsync(int householdId, int loginUserId);
        /// <summary>
        /// 根据用户的id获取用户所属的户码列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserHouseholdListRes>> GetHouseholdByUserIdAsync(int userId);
    }
}