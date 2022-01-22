using DVS.Common.Services;
using DVS.Core.Domains.IPS;
using DVS.Models.Dtos.IPS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.IPS
{
    public interface IIPSCompanyService : IServiceBase<IpsCompany>
    {

        Task<IPSCompanyTreeDto> GetCompanyNodesTree(string companyId = "");
    }
}
