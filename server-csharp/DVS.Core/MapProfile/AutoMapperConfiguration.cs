using DVS.Core.Domains.Common;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using Lychee.Core.Infrastructures;
using Microsoft.Extensions.Configuration;
using System.IO;
using DVS.Model.Dtos.Village;
using DVS.Common.Models;
using DVS.Common.SO;
using DVS.Common.Models;
using DVS.Models.Dtos.GIS;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.Village.Household;
using DVS.Models.Dtos.Village.Vaccination;
using DVS.Models.Dtos.IPS;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.FFP;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;

namespace DVS.Core.MapProfile
{
    public class AutoMapperConfiguration : AutoMapper.Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<ConsInfoPublic, ConsInfoPublicDto>();

            CreateMap<CreatePhotoAnywhereModel, ConsPhotoAnywhere>();
            CreateMap<ConsPhotoAnywhere, ListPhotoAnywhereModel>();
            CreateMap<ConsPhotoAnywhere, DetailPhotoAnywhereModel>();

            CreateMap<CreateProduceSaleModel, ConsProduceSale>();
            CreateMap<ConsProduceSale, ListProduceSaleModel>();
            CreateMap<ConsProduceSale, DetailProduceSaleModel>();

            CreateMap<CreateServiceGuideModel, ConsServiceGuide>();
            CreateMap<ConsServiceGuide, ListServiceGuideModel>()
                .ForMember(a => a.CreatedAt, a => a.MapFrom(r => r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(a => a.UpdatedAt, a => a.MapFrom(r => r.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<ConsServiceGuide, DetailServiceGuideModel>();

            CreateMap<CreateServiceChannelModel, ConsServiceChannel>();
            CreateMap<ConsServiceChannel, ListServiceChannelModel>()
                  .ForMember(a => a.CreatedAt, a => a.MapFrom(r => r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))
                  .ForMember(a => a.UpdatedAt, a => a.MapFrom(r => r.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<ConsServiceChannel, DetailServiceChannelModel>();
            CreateMap<SunFileInfo, SunFileInfoDto>()
                .ForMember(a => a.Url, a => a.MapFrom(r => GetUrl(r.RelativePath)))
                .ForMember(a => a.ThumbnailUrl, a => a.MapFrom(r => GetUrl(r.ThumbnailRelativePath)));

            CreateMap<SaveIncomeBody, VillageIncome>();
            CreateMap<VillageHouseholdCode, VillageHouseholdCodeDto>();
            CreateMap<VillagePopulation, VillagePopulationDto>()
                .ForMember(a => a.RealName, a => a.MapFrom(r => BasicSO.Decrypt(r.RealName)))
                .ForMember(a => a.Mobile, a => a.MapFrom(r => BasicSO.Decrypt(r.Mobile)))
            ;
            CreateMap<VillagePopulationAddress, PopulationAddressDto>();
            CreateMap<VillagePopulation, PopulationDetailDto>()
                .ForMember(a => a.TagNames, a => a.Ignore())
                .ForMember(a => a.LiveAddressInfo, a => a.Ignore())
                .ForMember(a => a.RegisterAddressInfo, a => a.Ignore())
                .ForMember(a => a.NativePlaceInfo, a => a.Ignore())
                .ForMember(a => a.RealName, a => a.MapFrom(r => BasicSO.Decrypt(r.RealName)))
                .ForMember(a => a.IdCard, a => a.MapFrom(r => BasicSO.Decrypt(r.IdCard)))
                .ForMember(a => a.Mobile, a => a.MapFrom(r => BasicSO.Decrypt(r.Mobile)))
            ;





            CreateMap<PopulationDetailDto, VillagePopulation>()
                // .ForMember(a => a.RealName, a => a.MapFrom(r => BasicSO.Encrypt(r.RealName)))
                .ForMember(a => a.IdCard, a => a.MapFrom(r => BasicSO.Encrypt(r.IdCard)))
                .ForMember(a => a.Mobile, a => a.MapFrom(r => BasicSO.Encrypt(r.Mobile)))
                ;
            CreateMap<PopulationAddressDto, VillagePopulationAddress>();
            CreateMap<BasicDictionary, BasicDictionaryDto>();

            CreateMap<VillageUserAuthRecord, UserAuthRecordDto>()
                  .ForMember(a => a.RealName, a => a.MapFrom(r => BasicSO.Decrypt(r.RealName)))
                .ForMember(a => a.IdCard, a => a.MapFrom(r => BasicSO.Decrypt(r.IdCard)))
                .ForMember(a => a.Mobile, a => a.MapFrom(r => BasicSO.Decrypt(r.Mobile)))
                ;
            CreateMap<BasicCategory, BasicCategoryDto>();
            CreateMap<BasicCategory, BasicCategoryModel>();

            CreateMap<BasicArea, BasicAreaTreeDto>();
            CreateMap<BasicUser, BasicUserDto>();
            CreateMap<BasicUserDto, LoginUser>();

            CreateMap<SaveWorkBody, VillageWork>();

            CreateMap<GISPlotItem, GISPlotItemDto>();
            CreateMap<GISCamera, GISCameraDto>();
            CreateMap<GISCollectiveProperty, GISCollectivePropertyDto>();
            CreateMap<GISCustom, GISCustomDto>();
            CreateMap<GISGreenHouse, GISGreenHouseDto>();
            CreateMap<VillageHouseName, VillageHouseNameDto>();
            CreateMap<VillageHouseNameDto, VillageHouseName>();
            CreateMap<VillageHouseholdCodeTemplate, VillageHouseholdCodeTemplateDto>();
            CreateMap<VillageHouseholdCodeTemplateDto, VillageHouseholdCodeTemplate>();

            CreateMap<VillageVaccination, VaccinationDto>();

            CreateMap<VillageVaccination, VaccinationDto>();
            CreateMap<VillageVaccination, VaccinationDto>();

            CreateMap<IpsMessage, IPSMessageDto>();
            CreateMap<IpsSchedule, IPSScheduleDto>();
            CreateMap<IpsDevice, IPSDeviceDto>();
            CreateMap<IpsCompany, IPSCompanyDto>();

            CreateMap<VillageEpidemic, VillageEpidemic>();

            CreateMap<FFPMatrix, FFPMatrixDto>();
            CreateMap<FFPMatrixHousehold, FFPMatrixHouseholdDto>();

            CreateMap<FFPHouseholdCode, FFPHouseholdCodeDto>();
            CreateMap<FFPPopulation, FFPPopulationDto>();

            CreateMap<FFPMoPaiLog, FFPMoPaiLogDto>();
            CreateMap<FFPApplicationEditReq, FFPApplication>();
            CreateMap<FFPApplication, FFPApplicationDto>();

            CreateMap<FFPPublicityManage, PublicityManageDto>();
            CreateMap<FFPPublicityManage, ReviewReportManageDto>();
            CreateMap<FFPPublicityHousehold, PublicityListManageDto>();
            CreateMap<FFPPublicityHousehold, PublicityList>();

            CreateMap<ModuleDictionary, ModuleDictionaryDto>();


            CreateMap<FFPHouseholdCodePostDto, FFPHouseholdCode>();
            CreateMap<FFPPopulationPostDto, FFPPopulation>();
            CreateMap<FFPHouseholdExtraInfoDto, FFPHouseholdExtraInfo>();
        }

        private string GetUrl(string relativePath)
        {
            IConfiguration configuration = ServiceLocator.Current.GetService<IConfiguration>();
            string store = configuration["File:Store"];
            return $"{configuration[$"File:{store}:AccessUrl"]}{relativePath}";
        }
    }
}