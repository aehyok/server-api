using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.FFP;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.FFP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using DVS.Common.Infrastructures;
using DVS.Models.Enum;
using DVS.Application.Services.Village;
using DVS.Common;

namespace DVS.Application.Services.FFP
{
    public class FFPPopulationService : ServiceBase<FFPPopulation>, IFFPPopulationService
    {

        private readonly IModuleDictionaryService moduleDictionaryService;
        private readonly ISunFileInfoService sunFileService;
        private readonly IServiceBase<VillageHouseCodeMember> houseCodeMemberService;
        private readonly IServiceBase<VillagePopulation> populationService;
        private readonly IPopulationAddressService populationAddressService;
        public FFPPopulationService(DbContext dbContext, IMapper mapper, 
            IModuleDictionaryService moduleDictionaryService, 
            ISunFileInfoService sunFileService,
            IServiceBase<VillageHouseCodeMember>  houseCodeMemberService,
            IServiceBase<VillagePopulation> populationService,
            IPopulationAddressService populationAddressService
            )
        : base(dbContext, mapper)
        {
            this.moduleDictionaryService = moduleDictionaryService;
            this.sunFileService = sunFileService;
            this.houseCodeMemberService = houseCodeMemberService;
            this.populationService = populationService;
            this.populationAddressService = populationAddressService;
        }


        public async Task<List<FFPPopulationDto>> GetFFPPopulationMemebers(int householdId, int isConvertDictionary = 0)
        {
            var data = await this.GetListAsync(a => a.IsDeleted == 0 && a.HouseholdId == householdId);
            List<FFPPopulationDto> list = mapper.Map<List<FFPPopulationDto>>(data);

            if (list == null || list.Count <= 0)
            {
                var populations = from m in this.houseCodeMemberService.GetQueryable()
                                 join p in this.populationService.GetQueryable() on new { m.PopulationId } equals new { PopulationId = p.Id }
                                 where m.HouseholdId == householdId && m.IsDeleted == 0 && p.IsDeleted == 0
                                 select p;

                foreach (var item in populations)
                {
                    list.Add(new FFPPopulationDto()
                    {
                        PopulationId = item.Id,
                        RealName = item.RealName,
                        IdCard = item.IdCard,
                        IdCardType = 1,
                        Mobile = item.Mobile,
                        MobileShort = item.MobileShort,
                        HouseholdId= householdId,
                        Id = 0,
                        AreaId = item.AreaId,
                        Birthday= item.Birthday,
                        Sex= item.Sex,
                        Tags= item.Tags,
                        // Education = item.Education,
                        // Income = item.Income,
                        HeadImageId = item.HeadImageId,
                        HeadImageUrl = item.HeadImageUrl,
                        // Marital = item.Marital,
                        // Nation = item.Nation,
                        // Political = item.Political,
                        // Relationship = 0,
                        // Religion = item.Religion,
                    });
                }

            }

            List<string> dirs = new List<string>();
            foreach (var item in list)
            {

                dirs.Add(item.Education);
                dirs.Add(item.Income);
                dirs.Add(item.Health);
                dirs.Add(item.LaborSkill);
                dirs.Add(item.Marital);
                dirs.Add(item.MedicalInsuranceStatus);
                dirs.Add(item.MinLivingSecurityStatus);
                dirs.Add(item.MovePeopleStatus);
                dirs.Add(item.Nation);
                dirs.Add(item.Political);
                dirs.Add(item.PoorStatus);
                dirs.Add(item.Relationship);
                dirs.Add(item.Religion);
                dirs.Add(item.StudentStatus);
                // dirs.Add(item.WorkingTiems);

                item.WorkingAreaAddressInfo = await this.populationAddressService.GetAddressDetail(item.Id, PopulationAddressTypeEnum.务工地区);
            }

            var dirNames = await this.moduleDictionaryService.GetModuleDictionaryListAsync(dirs);

            foreach (var item in list)
            {
                item.EducationDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Education, dirNames);
                item.IncomeDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Income, dirNames);
                item.HealthDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Health, dirNames);
                item.LaborSkillDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.LaborSkill, dirNames);
                item.MaritalDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Marital, dirNames);
                item.MedicalInsuranceStatusDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.MedicalInsuranceStatus, dirNames);
                item.MinLivingSecurityStatusDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.MinLivingSecurityStatus, dirNames);
                item.MovePeopleStatusDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.MovePeopleStatus, dirNames);
                item.NationDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Nation, dirNames);
                item.PoliticalDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Political, dirNames);
                item.PoorStatusDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.PoorStatus, dirNames);
                item.RelationshipDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Relationship, dirNames);
                item.ReligionDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.Religion, dirNames);
                item.StudentStatusDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.StudentStatus, dirNames);
                // item.WorkingTiemsDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(item.WorkingTiems, dirNames);

                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.IdCard = BasicSO.Decrypt(item.IdCard);
                item.HeadImageUrl = this.sunFileService.ToAbsolutePath(item.HeadImageUrl);
            }
            return list;
        }


        public async Task<MessageResult<int>> SaveFFPPopulation(List<FFPPopulationPostDto> populationDtos, int createdBy) {

            MessageResult<int> result = new MessageResult<int>("成功", true);
            int count = 0;

           //  List<FFPPopulation> list = mapper.Map<List<FFPPopulation>>(populationDtos);
            foreach (var item in populationDtos)
            {
                FFPPopulation population = mapper.Map<FFPPopulation>(item);

                var idCardInfo = Utils.ValidIdCard(population.IdCard);
                if (idCardInfo != null) {
                    population.Birthday = idCardInfo.Birthday;
                    population.Sex = idCardInfo.Sex == 1 ? PopulationGender.男 : PopulationGender.女;
                }

                population.UpdatedAt = DateTime.Now;
                population.UpdatedBy = createdBy;
                population.RealName = BasicSO.Encrypt(population.RealName);
                population.IdCard = BasicSO.Encrypt(population.IdCard);
                if (population.Id > 0)
                {
                    var ffpp = await this.GetAsync(a => a.Id == population.Id && a.IsDeleted == 0);
                    // population.CreatedAt = DateTime.Now;
                    if (ffpp != null)
                    {
                        ffpp.AreaId = population.AreaId;
                        ffpp.HouseholdId = population.HouseholdId;
                        ffpp.PopulationId = population.PopulationId;
                        ffpp.RealName = population.RealName;
                        ffpp.Sex = population.Sex;
                        ffpp.Relationship = population.Relationship;
                        ffpp.IdCard = population.IdCard;
                        ffpp.IdCardType = population.IdCardType;
                        ffpp.Nation = population.Nation;
                        ffpp.Political = population.Political;
                        ffpp.Education = population.Education;
                        ffpp.HeadImageId = population.HeadImageId;
                        ffpp.HeadImageUrl = population.HeadImageUrl;
                        ffpp.Health = population.Health;
                        ffpp.StudentStatus = population.StudentStatus;
                        ffpp.LaborSkill = population.LaborSkill;
                        ffpp.WorkingTimes = population.WorkingTimes;
                        ffpp.MedicalInsuranceStatus = population.MedicalInsuranceStatus;
                        ffpp.SeriousIllnessInsuranceStatus = population.SeriousIllnessInsuranceStatus;
                        ffpp.EndowmentInsuranceStatus = population.EndowmentInsuranceStatus;
                        ffpp.MinLivingSecurityStatus = population.MinLivingSecurityStatus;
                        ffpp.PoorStatus = population.PoorStatus;
                        ffpp.MovePeopleStatus = population.MovePeopleStatus;
                        ffpp.UpdatedAt = population.UpdatedAt;
                        ffpp.UpdatedBy = population.UpdatedBy;
                        var res = await this.UpdateAsync(ffpp);
                        if (res > 0)
                        {
                            await this.populationAddressService.SaveAddress(population.Id, PopulationAddressTypeEnum.务工地区, item.WorkingAreaAddressInfo);
                            count += 1;
                        }
                    }
                }
                else {
                    population.CreatedAt = DateTime.Now;
                    population.CreatedBy = createdBy;
                    var res = await this.InsertAsync(population);
                    if (res !=null)
                    {
                        await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.务工地区, item.WorkingAreaAddressInfo);
                        count += 1;
                    }
                }
            }
            result.Data = count;
            return result;
        }

    }
}
