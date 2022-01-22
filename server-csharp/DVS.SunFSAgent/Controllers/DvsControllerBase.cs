using AutoMapper;
using DVS.Common.Infrastructures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.SunFSAgent.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "sunfs")]
    [DvsResult]
    [DvsException]
    public class DvsControllerBase : ControllerBase
    {
        protected IMapper Mapper
        {
            get
            {
                return HttpContext.RequestServices.GetRequiredService<IMapper>();
            }
        }
    }
}
