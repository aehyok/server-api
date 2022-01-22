using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using DVS.Models.Enum;

namespace DVS.Application.Services.Village
{
    public interface IPopulationAddressService : IServiceBase<VillagePopulationAddress>
    {
        Task<bool> SaveAddress(int populationId, PopulationAddressTypeEnum type, PopulationAddressDto address);
        Task<PopulationAddressDto> GetAddressDetail(int populationId, PopulationAddressTypeEnum type);

    }





  
}
