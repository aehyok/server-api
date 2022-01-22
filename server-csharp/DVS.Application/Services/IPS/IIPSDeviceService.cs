using DVS.Common.Services;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.IPS
{
    public interface IIPSDeviceService : IServiceBase<IpsDevice>
    {
        Task<IPSDeviceTreeDto> List(int areaId);
    }
}
