using DVS.Application.Services.IPS;
using DVS.Common.Models;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.IPS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVS.IPS.Api.Controllers.App
{
    /// <summary>
    /// ips组织和门店
    /// </summary>
    [Route("/api/ips/app/company")]
    public class IPSCompanyController : DvsControllerBase
    {

        private readonly IIPSCompanyService _service;

        public IPSCompanyController(IIPSCompanyService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取该商户所有子节点
        /// </summary>
        /// <returns></returns>
        [HttpPost("getCompanyNodesTree")]
        public async Task<IPSCompanyTreeDto> GetCompanyNodesTreeAsync()
        {
            //LoginUser loginuser = this.LoginUser;
            return  await this._service.GetCompanyNodesTree("");
        }
       
    }
}
