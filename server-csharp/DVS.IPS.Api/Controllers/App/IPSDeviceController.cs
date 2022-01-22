using DVS.Application.Services.IPS;
using DVS.Common.Models;
using DVS.Models.Dtos.IPS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVS.IPS.Api.Controllers.App
{
    /// <summary>
    /// ips设备
    /// </summary>
    [Route("/api/ips/app/device")]
    public class IPSDeviceController : DvsControllerBase
    {

        private readonly IIPSDeviceService _service;

        public IPSDeviceController(IIPSDeviceService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取区域对应门店下的设备
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPSDeviceTreeDto> ListAsync()
        {
            LoginUser loginuser = this.LoginUser;
            return  await this._service.List(loginuser.AreaId);
        }
       
    }
}
