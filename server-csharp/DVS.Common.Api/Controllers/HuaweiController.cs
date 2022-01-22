using DVS.Application.Services.Common;
using DVS.Common.Infrastructures;
using DVS.Models.Dtos.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.Common.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [DvsResult]
    [DvsException]
    [Route("/api/common/hw/")]
    public class HuaweiController : ControllerBase
    {
        private readonly ILogger<HuaweiController> _logger;

        public HuaweiController(ILogger<HuaweiController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 扫描
        /// </summary>
        /// <param name="service"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("scanIdCard")]
        public async Task<HWOcrIDCardRes> ScanIdCardAsync([FromServices] IHWService service, IFormFile file) {
          var result=  await service.ScanIdCard(file);
            return result;
        }

    }
}
