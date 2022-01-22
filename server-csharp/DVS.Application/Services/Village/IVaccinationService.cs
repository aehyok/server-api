using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Farmland;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Dtos.Village.Vaccination;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public interface IVaccinationService : IServiceBase<VillageVaccination>
    {
        Task<IPagedList<VaccinationHouseholdDto>> GetVaccinationList(PagePostBody body);

        Task<IPagedList<VaccinationDto>> GetVaccinationInfoList(int householdId, int year, int populationId = 0, int page = 1, int limit = 10);

        Task<MessageResult<bool>> SaveVaccinationInfo(VillageVaccination body);

        Task<MessageResult<bool>> DeleteVaccinationInfo(int id, int userid);

        Task<VaccinationDto> DetailVaccinationInfo(int id);

        Task<IPagedList<VaccinationDto>> GetVaccinationInfoList(VaccinationPagePostBody body);

        Task<IPagedList<VaccinationDto>> GetVaccinationHouseholdList(VaccinationPagePostBody body);

        Task<IPagedList<StatisticsVaccinationDto>> GetVaccinationStatisticsList(VaccinationPagePostBody body);

        Task<byte[]> GetVaccinationExcelData(int areaId, int year, string ids);

        Task<byte[]> GetVaccinationStatisticsExcelData(VaccinationPagePostBody body);

        Task<List<VaccinationDto>> VaccinationInfoList(int populationId, int householdId);

        Task<IPagedList<VaccinationInfoDto>> GetVaccinationStatisticsList(int householdId, int page = 1, int limit = 10);
    }
}
