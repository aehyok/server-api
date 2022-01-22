using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.FFP;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    public class FFPHouseholdCodeService : ServiceBase<FFPHouseholdCode>, IFFPHouseholdCodeService
    {
        private readonly IFFPPopulationService fFPPopulationService;
        private readonly IModuleDictionaryService moduleDictionaryService;
        private readonly IPopulationAddressService populationAddressService;
        private readonly IServiceBase<VillageHouseholdCode> householdCodeService;
        public FFPHouseholdCodeService(DbContext dbContext, IMapper mapper,
            IFFPPopulationService fFPPopulationService,
            IModuleDictionaryService moduleDictionaryService,
            IPopulationAddressService populationAddressService,
            IServiceBase<VillageHouseholdCode> householdCodeService
            )
        : base(dbContext, mapper)
        {
            this.fFPPopulationService = fFPPopulationService;
            this.moduleDictionaryService = moduleDictionaryService;
            this.populationAddressService = populationAddressService;
            this.householdCodeService = householdCodeService;
        }


        public async Task<FFPHouseholdCodeDetailDto> GetFFPHouseholdCodeDetail(int householdId, int isConvertDictionary = 0)
        {

            var result = new FFPHouseholdCodeDetailDto()
            {
                FFPHouseholdCodeInfo = new FFPHouseholdCodeDto(),
                FFPPopulationList = new List<FFPPopulationDto>(),
            };
            var householdCode = await this.GetAsync(a => a.IsDeleted == 0 && a.HouseholdId == householdId);
            if (householdCode != null)
            {
                result.FFPHouseholdCodeInfo = mapper.Map<FFPHouseholdCodeDto>(householdCode);
                result.FFPHouseholdCodeInfo.FamilyAddressInfo = await this.populationAddressService.GetAddressDetail(householdCode.Id, PopulationAddressTypeEnum.防返贫家庭地址);
                result.FFPHouseholdCodeInfo.PlaceAreaAddressInfo = await this.populationAddressService.GetAddressDetail(householdCode.Id, PopulationAddressTypeEnum.防返贫安置区);
                result.FFPHouseholdCodeInfo.Mobile = BasicSO.Decrypt(result.FFPHouseholdCodeInfo.Mobile);

                if (isConvertDictionary == 1)
                {
                    result.FFPHouseholdCodeInfo.HouseholdTypeDto = await this.moduleDictionaryService.GetModuleDictionaryByCode(householdCode.HouseholdType);
                }
            }
            else
            {
                // 没有数据的时候去户码表获取
                //var household = await this.householdCodeService.GetAsync(a => a.Id == householdId && a.IsDeleted == 0);
                //if (household != null) {             
                //}
            }
            result.FFPPopulationList = await this.fFPPopulationService.GetFFPPopulationMemebers(householdId, isConvertDictionary);
            return result;
        }


        public async Task<MessageResult<int>> SaveFFPHouseholdCodeInfo(FFPHouseholdCodeInfoPostDto body, int createdBy)
        {

            MessageResult<int> result = new MessageResult<int>("失败", false);
            if (body == null || body.FFPHouseholdCodeInfo == null)
            {

                result.Message = "缺少参数";
                return result;
            }
            var householdCodeInfo = mapper.Map<FFPHouseholdCode>(body.FFPHouseholdCodeInfo);


            if (householdCodeInfo.HouseholdId <= 0)
            {
                result.Message = "缺少户码信息";
                return result;
            }
            householdCodeInfo.UpdatedAt = DateTime.Now;
            householdCodeInfo.UpdatedBy = createdBy;
            if (!string.IsNullOrWhiteSpace(householdCodeInfo.Mobile) && householdCodeInfo.Mobile.Length == 11)
            {
                householdCodeInfo.MobileShort = householdCodeInfo.Mobile.Substring(0, 3) + householdCodeInfo.Mobile.Substring(7, 4);
            }
            householdCodeInfo.Mobile = BasicSO.Encrypt(householdCodeInfo.Mobile);

            if (householdCodeInfo.Id > 0)
            {
                var data = await this.GetAsync(a => a.HouseholdId == householdCodeInfo.HouseholdId && a.IsDeleted == 0);
                if (data == null || householdCodeInfo.Id != data.Id)
                {
                    result.Message = "不存在此记录";
                    return result;
                }
                data.HouseholdType = householdCodeInfo.HouseholdType;
                data.IsInPlaceArea = householdCodeInfo.IsInPlaceArea;
                data.IsWithoutPoverty = householdCodeInfo.IsWithoutPoverty;
                data.Mobile = householdCodeInfo.Mobile;
                data.MobileShort = householdCodeInfo.MobileShort;
                data.UpdatedAt = householdCodeInfo.UpdatedAt;
                data.UpdatedBy = householdCodeInfo.UpdatedBy;
                var res = await this.UpdateAsync(data);
                if (res > 0)
                {
                    await this.populationAddressService.SaveAddress(householdCodeInfo.Id, PopulationAddressTypeEnum.防返贫家庭地址, body.FFPHouseholdCodeInfo.FamilyAddressInfo);
                    await this.populationAddressService.SaveAddress(householdCodeInfo.Id, PopulationAddressTypeEnum.防返贫安置区, body.FFPHouseholdCodeInfo.PlaceAreaAddressInfo);

                }
            }
            else
            {
                householdCodeInfo.CreatedAt = DateTime.Now;
                householdCodeInfo.CreatedBy = createdBy;
                var res = await this.InsertAsync(householdCodeInfo);
                if (res != null)
                {
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.防返贫家庭地址, body.FFPHouseholdCodeInfo.FamilyAddressInfo);
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.防返贫安置区, body.FFPHouseholdCodeInfo.PlaceAreaAddressInfo);
                }
            }

            foreach (var item in body.FFPPopulationList)
            {
                item.HouseholdId = householdCodeInfo.HouseholdId;
            }

            var populationResult = await this.fFPPopulationService.SaveFFPPopulation(body.FFPPopulationList, createdBy);

            result.Message = "成功";
            result.Flag = true;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> UpdateFFPHouseholdCodeInfo(int householdId, string householdType, PopulationAddressDto familyAddressInfo, int updatedBy)
        {
            var data = await this.GetAsync(a => a.HouseholdId == householdId && a.IsDeleted == 0);
            if (data != null)
            {
                var res = await this.Context.Database.ExecuteSqlRawAsync(" UPDATE FFPHouseholdCode SET updatedBy={0},householdType={1} WHERE id={2} ", updatedBy, householdType, data.Id);
                await this.populationAddressService.SaveAddress(data.Id, PopulationAddressTypeEnum.防返贫家庭地址, familyAddressInfo);
                return res > 0;
            }
            return false;
        }
    }
}
