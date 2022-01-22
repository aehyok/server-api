using AutoMapper;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using Lychee.Extensions;
using LinqKit;
using DVS.Common.Services;
using DVS.Common.Infrastructures;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using DVS.Models.Enum;

namespace DVS.Application.Services.Village
{
    public class PopulationAddressService : ServiceBase<VillagePopulationAddress>, IPopulationAddressService
    {

       
        public PopulationAddressService(DbContext dbContext, IMapper mapper
            )
            : base(dbContext, mapper)
        {
            
        }

        public async Task<PopulationAddressDto> GetAddressDetail(int populationId, PopulationAddressTypeEnum type)
        {
            var data = await this.GetAsync(a => a.PopulationId == populationId && a.Type == type && a.IsDeleted == 0);
            return mapper.Map<PopulationAddressDto>(data);
            
        }

        public async Task<bool> SaveAddress(int populationId,PopulationAddressTypeEnum type,PopulationAddressDto address)
        {
            if (address == null || address.Province == null || (address.Province == null && address.City == null)) {

                return false;
            }

            var data = await this.GetAsync(a => a.PopulationId == populationId && a.IsDeleted == 0 && a.Type == type);
            if (data != null)
            {
                // address.Id = data.Id;
                data.Address = address.Address;
                data.Province = address.Province;
                data.City = address.City;
                data.District = address.District;
                data.MapCode = address.MapCode;

                var res = await this.UpdateAsync(data);
                return res > 0;
            }
            else {
                var addData = mapper.Map<VillagePopulationAddress>(address);
                addData.PopulationId = populationId;
                addData.Type = type;
                addData.UpdatedAt = DateTime.Now;
                addData.CreatedAt = DateTime.Now;
                var res = await this.InsertAsync(addData);
                return res != null;
            }
        }
    }
}
